using System.Text.RegularExpressions;
using System.Web;

namespace TeamBuilder.Data.Common.Security
{
    public static class InputSanitizer
    {
        private static readonly Regex HtmlTagRegex = new(@"<[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ScriptRegex = new(@"<script[^>]*>.*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex DangerousAttributesRegex = new(@"\b(on\w+|javascript:)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string SanitizeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove script tags
            input = ScriptRegex.Replace(input, "");
            
            // Remove dangerous attributes
            input = DangerousAttributesRegex.Replace(input, "");
            
            // HTML encode the result
            return HttpUtility.HtmlEncode(input);
        }

        public static string SanitizeText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove HTML tags
            input = HtmlTagRegex.Replace(input, "");
            
            // HTML encode the result
            return HttpUtility.HtmlEncode(input);
        }

        public static string SanitizeForDatabase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove potential SQL injection patterns
            var sqlInjectionPatterns = new[]
            {
                @"(\b(union|select|insert|update|delete|drop|create|alter|exec|execute)\b)",
                @"(--|/\*|\*/)",
                @"(\b(and|or)\b\s+\d+\s*[=<>])",
                @"(\b(and|or)\b\s+['""]\w+['""]\s*[=<>])"
            };

            foreach (var pattern in sqlInjectionPatterns)
            {
                input = Regex.Replace(input, pattern, "", RegexOptions.IgnoreCase);
            }

            return input.Trim();
        }

        public static bool ContainsDangerousContent(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var dangerousPatterns = new[]
            {
                @"<script",
                @"javascript:",
                @"on\w+\s*=",
                @"<iframe",
                @"<object",
                @"<embed",
                @"union\s+select",
                @"--",
                @"/\*",
                @"\*/"
            };

            return dangerousPatterns.Any(pattern => 
                Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
        }
    }
}
