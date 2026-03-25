using UnityEngine;
using System;

namespace Assets._Game.Scripts.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ConditionalDisplayAttribute : PropertyAttribute
    {
        public string ComparedPropertyName { get; private set; }
        public object ComparedValue { get; private set; }
        public ComparisonType Comparison { get; private set; }

        public ConditionalDisplayAttribute(string comparedPropertyName, object comparedValue = null, ComparisonType comparison = ComparisonType.Bool)
        {
            ComparedPropertyName = comparedPropertyName;
            ComparedValue = comparedValue;
            Comparison = comparison;
        }

        public enum ComparisonType
        {
            Bool = 1,
            Flag = 2,
        }
    }
}