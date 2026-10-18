using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class TMPTextExtensions
    {
        public static GameObject TryCreateFromFormattedTemplate(this TMP_Text template, RectTransform container, params string[] arguments)
        {
            if (arguments == null || arguments.Length == 0) return null;
            var text = template.text.FormatSafe(arguments);
            return CreateFromTemplate(template, container, text);
        }

        public static GameObject TryCreateFromTemplate(this TMP_Text template, RectTransform container, string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            return CreateFromTemplate(template, container, text);
        }

        public static GameObject CreateFromTemplate(this TMP_Text template, RectTransform container, string text)
        {
            var textTemplateInstance = Object.Instantiate(template, container);
            textTemplateInstance.text = text;
            textTemplateInstance.gameObject.SetActive(true);

            return textTemplateInstance.gameObject;
        }
    }
}
