using System;
using System.Text;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string FormatSafe(this string format, object[] args, string absentArgumentPlaceholder = "?")
        {
            if (string.IsNullOrEmpty(format))
                return string.Empty;

            var result = new StringBuilder(format.Length);

            for (int i = 0; i < format.Length; i++)
            {
                if (format[i] != '{')
                {
                    result.Append(format[i]);
                    continue;
                }

                int end = format.IndexOf('}', i + 1);

                // If there's no closing brace, we treat the opening brace as a literal character and append it to the result
                if (end == -1)
                {
                    result.Append('{');
                    continue;
                }

                if (int.TryParse(
                    format.AsSpan(i + 1, end - i - 1),
                    out int index))
                {
                    if (index >= 0 && index < args.Length)
                        result.Append(args[index]);
                    else
                        result.Append(absentArgumentPlaceholder);
                }
                else
                {
                    // If the content inside the braces is not a valid integer, we treat it as a literal string and append it as is
                    result.Append(format, i, end - i + 1);
                }

                i = end;
            }

            return result.ToString();
        }
    }
}
