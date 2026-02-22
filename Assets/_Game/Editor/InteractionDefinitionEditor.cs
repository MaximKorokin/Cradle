namespace Assets._Game.Editor
{
    using Assets._Game.Scripts.Entities.Interaction;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    // Работает с InteractionDefinitionSO из твоего примера:
    // [SerializeReference] public StepData Root;
    // SequenceData/ParallelData имеют: [SerializeReference] public List<StepData> Steps;
    [CustomEditor(typeof(InteractionDefinition))]
    public sealed class InteractionDefinitionSOEditor : Editor
    {
        private SerializedProperty _root;

        private static Type[] _stepTypes;

        // UI state
        private readonly Dictionary<string, bool> _foldouts = new();
        private readonly Dictionary<string, ReorderableList> _listsByPath = new();

        // Layout constants
        private const float HeaderH = 18f;
        private const float BoxPadY = 4f;
        private const float RowGap = 2f;
        private const float AfterHeaderGap = 4f;
        private const float StepsLabelGap = 2f;
        private const float StepsBottomGap = 6f;
        private const float DragHandlePadX = 12f; // чтобы не перекрывать ручку перетаскивания

        private void OnEnable()
        {
            _root = serializedObject.FindProperty("Root");

            _stepTypes ??= TypeCache.GetTypesDerivedFrom<InteractionDefinition.StepData>()
                .Where(t => !t.IsAbstract)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawToolbar();

            EditorGUILayout.Space(8);

            if (_root.managedReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Root step is not set.", MessageType.Info);
                if (GUILayout.Button("Create Root Step", GUILayout.Height(28)))
                    ShowTypeMenuForProperty(_root);
            }
            else
            {
                var h = CalcStepHeight(_root, isRoot: true);
                var r = GUILayoutUtility.GetRect(0f, h, GUILayout.ExpandWidth(true));
                DrawStepRect(_root, r, isRoot: true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("Interaction", EditorStyles.toolbarButton);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Expand All", EditorStyles.toolbarButton))
                    SetAllFoldouts(true);

                if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton))
                    SetAllFoldouts(false);

                if (GUILayout.Button("Clear Root", EditorStyles.toolbarButton))
                    _root.managedReferenceValue = null;
            }
        }

        // ---------------------------
        // Step drawing (RECT-based)
        // ---------------------------

        private void DrawStepRect(SerializedProperty stepProp, Rect rect, bool isRoot)
        {
            // outer box
            GUI.Box(rect, GUIContent.none);

            // inner padding
            var inner = new Rect(rect.x + 6, rect.y + BoxPadY, rect.width - 12, rect.height - BoxPadY * 2);

            // header
            var header = new Rect(inner.x, inner.y, inner.width, HeaderH);

            var label = stepProp.managedReferenceValue == null
                ? "Null Step"
                : NicifyType(stepProp.managedReferenceValue.GetType());

            bool expanded = GetFoldout(stepProp.propertyPath, defaultValue: true);

            expanded = EditorGUI.Foldout(
                new Rect(header.x, header.y, header.width - 86, header.height),
                expanded,
                label,
                true);

            SetFoldout(stepProp.propertyPath, expanded);

            // Type button
            if (GUI.Button(new Rect(header.xMax - 82, header.y, 54, header.height), "Type"))
                ShowTypeMenuForProperty(stepProp);

            // Remove button (not for root)
            if (!isRoot && GUI.Button(new Rect(header.xMax - 26, header.y, 26, header.height), "X"))
            {
                stepProp.managedReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (!expanded || stepProp.managedReferenceValue == null)
                return;

            // body
            var y = header.yMax + AfterHeaderGap;
            var bodyRect = new Rect(inner.x, y, inner.width, inner.yMax - y);

            DrawStepBodyRect(stepProp, bodyRect);
        }

        private void DrawStepBodyRect(SerializedProperty stepProp, Rect rect)
        {
            var it = stepProp.Copy();
            int startDepth = it.depth;

            if (!it.NextVisible(true)) return;

            float y = rect.y;

            do
            {
                if (it.depth <= startDepth) break;

                if (IsSerializeReferenceStepsList(it))
                {
                    // label
                    var labelRect = new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(labelRect, it.displayName, EditorStyles.boldLabel);
                    y += labelRect.height + StepsLabelGap;

                    // list
                    var list = GetOrCreateStepsList(it.propertyPath);
                    var listH = list.GetHeight();
                    var listRect = new Rect(rect.x, y, rect.width, listH);
                    list.DoList(listRect);
                    y += listH + StepsBottomGap;
                }
                else
                {
                    var h = EditorGUI.GetPropertyHeight(it, includeChildren: true);
                    var r = new Rect(rect.x, y, rect.width, h);
                    EditorGUI.PropertyField(r, it, includeChildren: true);
                    y += h + RowGap;
                }

            } while (it.NextVisible(false));
        }

        private float CalcStepHeight(SerializedProperty stepProp, bool isRoot)
        {
            // box + header + (optional body)
            float h = BoxPadY * 2 + HeaderH + 6; // small padding bottom

            if (stepProp.managedReferenceValue == null) return h;

            if (!GetFoldout(stepProp.propertyPath, true)) return h;

            h += AfterHeaderGap;
            h += CalcStepBodyHeight(stepProp);
            return h;
        }

        private float CalcStepBodyHeight(SerializedProperty stepProp)
        {
            float h = 0f;

            var it = stepProp.Copy();
            int startDepth = it.depth;

            if (!it.NextVisible(true)) return 0f;

            do
            {
                if (it.depth <= startDepth) break;

                if (IsSerializeReferenceStepsList(it))
                {
                    // label
                    h += EditorGUIUtility.singleLineHeight + StepsLabelGap;
                    // list
                    var list = GetOrCreateStepsList(it.propertyPath);
                    h += list.GetHeight() + StepsBottomGap;
                }
                else
                {
                    h += EditorGUI.GetPropertyHeight(it, includeChildren: true) + RowGap;
                }

            } while (it.NextVisible(false));

            return h;
        }

        // ---------------------------
        // ReorderableList (Steps)
        // ---------------------------

        private ReorderableList GetOrCreateStepsList(string listPropertyPath)
        {
            if (_listsByPath.TryGetValue(listPropertyPath, out var cached))
            {
                // важно: у ReorderableList есть serializedProperty, но мы всегда работаем через FindProperty внутри callbacks
                return cached;
            }

            SerializedProperty GetProp() => serializedObject.FindProperty(listPropertyPath);

            var list = new ReorderableList(serializedObject, GetProp(), draggable: true, displayHeader: false, displayAddButton: true, displayRemoveButton: true);

            list.elementHeightCallback = index =>
            {
                var lp = GetProp();
                if (lp == null) return HeaderH + 10;

                if (index < 0 || index >= lp.arraySize) return HeaderH + 10;

                var el = lp.GetArrayElementAtIndex(index); // managedReference element
                float h = HeaderH + 10;

                if (el.managedReferenceValue == null) return h;

                if (!GetFoldout(el.propertyPath, true)) return h;

                h += AfterHeaderGap;
                h += CalcStepBodyHeight(el);
                h += 6;
                return h;
            };

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var lp = GetProp();
                if (lp == null) return;

                if (index < 0 || index >= lp.arraySize) return;

                var el = lp.GetArrayElementAtIndex(index);

                rect.y += 2;
                rect.height -= 2;

                // не перекрываем drag-handle слева
                var content = new Rect(rect.x + DragHandlePadX, rect.y, rect.width - DragHandlePadX, rect.height);

                DrawStepRect(el, content, isRoot: false);
            };

            list.onAddDropdownCallback = (buttonRect, rl) =>
            {
                ShowTypeMenuForList(listPropertyPath);
            };

            list.onRemoveCallback = rl =>
            {
                var lp = GetProp();
                if (lp == null) return;

                if (rl.index < 0 || rl.index >= lp.arraySize) return;

                lp.DeleteArrayElementAtIndex(rl.index);
                serializedObject.ApplyModifiedProperties();
            };

            _listsByPath[listPropertyPath] = list;
            return list;
        }

        // ---------------------------
        // Type dropdowns
        // ---------------------------

        private void ShowTypeMenuForProperty(SerializedProperty target)
        {
            var menu = new GenericMenu();

            for (int i = 0; i < _stepTypes.Length; i++)
            {
                var t = _stepTypes[i];
                var label = NicifyType(t);

                menu.AddItem(new GUIContent(label), false, () =>
                {
                    target.managedReferenceValue = Activator.CreateInstance(t);
                    SetFoldout(target.propertyPath, true);
                    serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        private void ShowTypeMenuForList(string listPropertyPath)
        {
            var menu = new GenericMenu();

            for (int i = 0; i < _stepTypes.Length; i++)
            {
                var t = _stepTypes[i];
                var label = NicifyType(t);

                menu.AddItem(new GUIContent(label), false, () =>
                {
                    var lp = serializedObject.FindProperty(listPropertyPath);
                    if (lp == null) return;

                    lp.arraySize++;
                    var el = lp.GetArrayElementAtIndex(lp.arraySize - 1);
                    el.managedReferenceValue = Activator.CreateInstance(t);

                    SetFoldout(el.propertyPath, true);
                    serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        // ---------------------------
        // Helpers
        // ---------------------------

        private static bool IsSerializeReferenceStepsList(SerializedProperty prop)
        {
            // List<StepData> с [SerializeReference]
            // Надёжно: это Generic array, элементы — ManagedReference (если есть хотя бы один)
            if (!prop.isArray) return false;
            if (prop.propertyType != SerializedPropertyType.Generic) return false;

            // Пустой список тоже считаем Steps-списком (в твоих группах Steps всегда List<StepData>)
            if (prop.arraySize == 0) return true;

            var el0 = prop.GetArrayElementAtIndex(0);
            return el0.propertyType == SerializedPropertyType.ManagedReference;
        }

        private bool GetFoldout(string key, bool defaultValue)
            => _foldouts.TryGetValue(key, out var v) ? v : defaultValue;

        private void SetFoldout(string key, bool value)
            => _foldouts[key] = value;

        private void SetAllFoldouts(bool value)
        {
            var keys = _foldouts.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
                _foldouts[keys[i]] = value;
        }

        private static string NicifyType(Type t)
        {
            var name = t.Name;
            if (name.EndsWith("Data", StringComparison.Ordinal))
                name = name[..^4];
            return ObjectNames.NicifyVariableName(name);
        }
    }
}
