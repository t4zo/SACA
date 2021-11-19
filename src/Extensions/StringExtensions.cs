using System.Globalization;
using System.Text;

namespace SACA.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveAccent(this string text)
        {
            return new string(text
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
    }
}
