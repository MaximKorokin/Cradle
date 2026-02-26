namespace Assets._Game.Editor
{
    using Assets._Game.Scripts.Entities.Control;
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ControlProviderData), true)]
    public class ControlProviderDataDrawer : PropertyDrawer
    {
        private static Type[] _types;

        static ControlProviderDataDrawer()
        {
            _types = TypeCache.GetTypesDerivedFrom<ControlProviderData>()
                .Where(t => !t.IsAbstract)
                .ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            string currentName = property.managedReferenceValue == null
                ? "None"
                : property.managedReferenceValue.GetType().Name;

            if (GUI.Button(headerRect, currentName, EditorStyles.popup))
            {
                var menu = new GenericMenu();

                foreach (var type in _types)
                {
                    menu.AddItem(
                        new GUIContent(type.Name),
                        false,
                        () =>
                        {
                            property.managedReferenceValue = Activator.CreateInstance(type);
                            property.serializedObject.ApplyModifiedProperties();
                        });
                }

                menu.ShowAsContext();
            }

            if (property.managedReferenceValue != null)
            {
                var bodyRect = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 2,
                    position.width,
                    EditorGUI.GetPropertyHeight(property, true));

                EditorGUI.PropertyField(bodyRect, property, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight +
                   EditorGUI.GetPropertyHeight(property, true) + 4;
        }
    }
}
