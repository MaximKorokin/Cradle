using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ProjectGuide",
    menuName = "Project/Guide")]
public class ProjectGuide : ScriptableObject
{
    public List<ColorEntry> Colors = new();

    [Serializable]
    public class ColorEntry
    {
        public string Name;
        public Color Value = Color.white;

        public string Hex => $"#{ColorUtility.ToHtmlStringRGB(Value)}";
    }
}