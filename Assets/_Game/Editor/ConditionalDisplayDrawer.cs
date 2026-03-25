#if UNITY_EDITOR
using Assets._Game.Scripts.Shared.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalDisplayAttribute))]
public class ConditionalDisplayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalDisplayAttribute conditionalAttribute = (ConditionalDisplayAttribute)attribute;

        SerializedProperty comparedProperty = GetConditionProperty(property, conditionalAttribute.ComparedPropertyName);

        if (comparedProperty != null && CheckCondition(comparedProperty, conditionalAttribute.ComparedValue, conditionalAttribute.Comparison))
        {
            // If the condition is met, draw the field as usual
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalDisplayAttribute conditionalAttribute = (ConditionalDisplayAttribute)attribute;
        SerializedProperty comparedProperty = GetConditionProperty(property, conditionalAttribute.ComparedPropertyName);

        if (comparedProperty != null && CheckCondition(comparedProperty, conditionalAttribute.ComparedValue, conditionalAttribute.Comparison))
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            // If the condition is not met, return 0 height to hide it completely
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool CheckCondition(SerializedProperty comparedProperty, object comparedValue, ConditionalDisplayAttribute.ComparisonType comparisonType)
    {
        if (comparisonType == ConditionalDisplayAttribute.ComparisonType.Bool)
        {
            var boolValue = comparedProperty.boolValue;
            var expectedValue = comparedValue is not bool || (bool)comparedValue; // Default to true if not specified
            return boolValue == expectedValue;
        }

        if (comparisonType == ConditionalDisplayAttribute.ComparisonType.Flag)
        {
            var flagValue = comparedProperty.enumValueFlag;
            if (comparedValue is not int expectedValue)
                expectedValue = default;
            return flagValue == expectedValue;
        }

        return true; // Default to showing if type not supported
    }

    private SerializedProperty GetConditionProperty(SerializedProperty property, string conditionName)
    {
        string path = property.propertyPath;

        int lastDot = path.LastIndexOf('.');

        if (lastDot != -1)
        {
            string conditionPath = path.Substring(0, lastDot + 1) + conditionName;
            return property.serializedObject.FindProperty(conditionPath);
        }

        return property.serializedObject.FindProperty(conditionName);
    }
}
#endif

