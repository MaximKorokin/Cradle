using Assets._Game.Scripts.Entities.Stats;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Stats.Editor
{
    [CustomPropertyDrawer(typeof(StatModifier))]
    public sealed class StatModifierDrawer : PropertyDrawer
    {
        private static readonly Dictionary<StatStage, StatOperation[]> AllowedOpsByStage = new()
        {
            { StatStage.PreAdd,         new[] { StatOperation.Add } },
            { StatStage.Add,            new[] { StatOperation.Add } },
            { StatStage.Multiply,       new[] { StatOperation.Multiply } },
            { StatStage.PostMultiply,   new[] { StatOperation.Multiply } },
            { StatStage.Override,       new[] { StatOperation.Override } },
            { StatStage.Clamp,          new[] { StatOperation.ClampMin, StatOperation.ClampMax } },
        };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var scope = new EditorGUI.PropertyScope(position, label, property);

            var statProp = FindAny(property, "Stat", "stat");
            var stageProp = FindAny(property, "Stage", "stage");
            var opProp = FindAny(property, "Op", "op", "Operation", "operation");
            var valueProp = FindAny(property, "Value", "value");
            var prioProp = FindAny(property, "Priority", "priority", "Order", "order");

            if (statProp == null || stageProp == null || opProp == null || valueProp == null || prioProp == null)
            {
                EditorGUI.LabelField(position, label.text,
                    $"Missing fields. Found: Stat={statProp != null}, Stage={stageProp != null}, Op={opProp != null}, Value={valueProp != null}, Priority={prioProp != null}");
                return;
            }

            // Important: ReorderableList draws with indentation. Use indented rect and draw with indentLevel=0.
            var content = EditorGUI.IndentedRect(position);
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            try
            {
                var line1 = new Rect(content.x, content.y, content.width, EditorGUIUtility.singleLineHeight);
                var line2 = new Rect(content.x, line1.yMax + EditorGUIUtility.standardVerticalSpacing, content.width, EditorGUIUtility.singleLineHeight);

                // --- Line 1: Stat | Stage | Op
                EditorGUI.BeginChangeCheck();

                const float Gap = 6f;

                float w = line1.width;
                float statW = Mathf.Clamp(w * 0.42f, 120f, 220f);
                float stageW = Mathf.Clamp(w * 0.33f, 110f, 180f);
                float opW = Mathf.Max(80f, w - statW - stageW - Gap * 2f);

                var statRect = new Rect(line1.x, line1.y, statW, line1.height);
                var stageRect = new Rect(statRect.xMax + Gap, line1.y, stageW, line1.height);
                var opRect = new Rect(stageRect.xMax + Gap, line1.y, opW, line1.height);

                EditorGUI.PropertyField(statRect, statProp, GUIContent.none);

                var stageEnum = GetEnum<StatStage>(stageProp);
                stageEnum = (StatStage)EditorGUI.EnumPopup(stageRect, stageEnum);
                SetEnum(stageProp, stageEnum);

                var allowedOps = GetAllowedOps(stageEnum);
                EnsureValidOp(opProp, allowedOps);

                var opEnum = GetEnum<StatOperation>(opProp);
                int currentOpIndex = IndexOfOp(opEnum, allowedOps);
                if (currentOpIndex < 0) currentOpIndex = 0;

                int newOpIndex = EditorGUI.Popup(opRect, currentOpIndex, GetOpNames(allowedOps));
                newOpIndex = Mathf.Clamp(newOpIndex, 0, allowedOps.Length - 1);
                SetEnum(opProp, allowedOps[newOpIndex]);

                bool changed = EditorGUI.EndChangeCheck();

                // --- Line 2: Value | Priority
                opEnum = GetEnum<StatOperation>(opProp);

                // Fixed widths so Priority is always fully visible
                bool showPrio = ShouldShowPriority(stageEnum, opEnum);

                // "Priority" label + int field
                const float prioLabelW = 54f;   // "Priority"
                const float prioFieldW = 60f;   // int field

                float prioTotalW = showPrio ? (prioLabelW + prioFieldW + Gap) : 0f;

                // Value label + value field (draw value field WITHOUT label to keep it left)
                const float valueLabelW = 38f;  // "Value"
                float valueFieldW = Mathf.Max(60f, line2.width - prioTotalW - valueLabelW - Gap);

                var valueLabelRect = new Rect(line2.x, line2.y, valueLabelW, line2.height);
                var valueFieldRect = new Rect(valueLabelRect.xMax + Gap, line2.y, valueFieldW, line2.height);

                EditorGUI.LabelField(valueLabelRect, "Value");
                EditorGUI.PropertyField(valueFieldRect, valueProp, GUIContent.none);

                if (showPrio)
                {
                    var prioLabelRect = new Rect(valueFieldRect.xMax + Gap, line2.y, prioLabelW, line2.height);
                    var prioFieldRect = new Rect(prioLabelRect.xMax + Gap, line2.y, prioFieldW, line2.height);

                    EditorGUI.LabelField(prioLabelRect, new GUIContent("Priority", "Higher priority wins within the same stage."));
                    prioProp.intValue = EditorGUI.IntField(prioFieldRect, prioProp.intValue);
                }


                if (changed)
                    property.serializedObject.ApplyModifiedProperties();
            }
            finally
            {
                EditorGUI.indentLevel = oldIndent;
            }
        }

        // --------- robust property finding ---------

        private static SerializedProperty FindAny(SerializedProperty root, string pascal, string camelAlt, params string[] extraNames)
        {
            // Try a bunch of common patterns:
            // - Field: Stat / Stage / Op / Value / Priority
            // - Private: _stat, m_Stat, stat
            // - Auto-property: <Stat>k__BackingField
            // - Variants passed via extraNames
            var candidates = new List<string>(16)
            {
                pascal,
                camelAlt,
                "_" + camelAlt,
                "_" + pascal,
                "m_" + camelAlt,
                "m_" + pascal,
                "<" + pascal + ">k__BackingField",
            };

            if (extraNames != null)
            {
                foreach (var n in extraNames)
                {
                    if (string.IsNullOrWhiteSpace(n)) continue;
                    var c = char.ToLowerInvariant(n[0]) + n.Substring(1);
                    candidates.Add(n);
                    candidates.Add(c);
                    candidates.Add("_" + c);
                    candidates.Add("_" + n);
                    candidates.Add("m_" + c);
                    candidates.Add("m_" + n);
                    candidates.Add("<" + n + ">k__BackingField");
                }
            }

            for (int i = 0; i < candidates.Count; i++)
            {
                var p = root.FindPropertyRelative(candidates[i]);
                if (p != null) return p;
            }

            return null;
        }

        private static TEnum GetEnum<TEnum>(SerializedProperty p) where TEnum : struct, Enum
        {
            if (p.propertyType != SerializedPropertyType.Enum)
                return default;

            int idx = Mathf.Clamp(p.enumValueIndex, 0, Math.Max(0, p.enumNames.Length - 1));
            string name = p.enumNames.Length > 0 ? p.enumNames[idx] : string.Empty;

            if (!string.IsNullOrEmpty(name) && Enum.TryParse(name, out TEnum result))
                return result;

            return default;
        }

        private static void SetEnum<TEnum>(SerializedProperty p, TEnum value) where TEnum : struct, Enum
        {
            if (p.propertyType != SerializedPropertyType.Enum)
                return;

            string targetName = Enum.GetName(typeof(TEnum), value);
            if (string.IsNullOrEmpty(targetName))
                return;

            // Find matching enum name in SerializedProperty (Unity uses names, not numeric values)
            int idx = Array.IndexOf(p.enumNames, targetName);
            if (idx >= 0)
                p.enumValueIndex = idx;
        }

        // --------- ops filtering ---------

        private static StatOperation[] GetAllowedOps(StatStage stage)
            => AllowedOpsByStage.TryGetValue(stage, out var ops) ? ops : new[] { StatOperation.Add };

        private static void EnsureValidOp(SerializedProperty opProp, StatOperation[] allowedOps)
        {
            var current = GetEnum<StatOperation>(opProp);

            for (int i = 0; i < allowedOps.Length; i++)
                if (allowedOps[i].Equals(current))
                    return;

            SetEnum(opProp, allowedOps[0]);
        }

        private static int IndexOfOp(StatOperation op, StatOperation[] ops)
        {
            for (int i = 0; i < ops.Length; i++)
                if (ops[i] == op) return i;
            return -1;
        }

        private static string[] GetOpNames(StatOperation[] ops)
        {
            var names = new string[ops.Length];
            for (int i = 0; i < ops.Length; i++)
                names[i] = ops[i].ToString();
            return names;
        }

        private static bool ShouldShowPriority(StatStage stage, StatOperation op)
            => stage == StatStage.Override || stage == StatStage.Clamp;

        private static string GetValueTooltip(StatStage stage, StatOperation op)
        {
            return op switch
            {
                StatOperation.Add => "Flat value. Example: 10 means +10.",
                StatOperation.Multiply => "Multiplier delta. Example: 0.15 means +15%, -0.25 means -25%.",
                StatOperation.Override => "Final value set after all stages (unless another Override with higher priority wins).",
                StatOperation.ClampMin => "Minimum allowed value after all calculations.",
                StatOperation.ClampMax => "Maximum allowed value after all calculations.",
                _ => "Value"
            };
        }
    }
}
