using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Libs
{
    public static class MAExtensions
    {
        public static string ToHyperLinkTitle(this string title)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = title.Normalize(NormalizationForm.FormD);
            return regex
                .Replace(temp, String.Empty)
                .Replace('\u0111', 'd')
                .Replace('\u0110', 'D')
                .Trim()
                .Replace(' ', '-')
                .Replace('/', '-')
                .Replace('\\', '-')
                .Replace('.', '_');
        }
    }
}