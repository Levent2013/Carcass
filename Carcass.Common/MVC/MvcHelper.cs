using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Carcass.Common.MVC
{
    public static class MvcHelper
    {
        public static string LimitText(string content, int symbolsCount = 512)
        {
            if (String.IsNullOrEmpty(content))
                return content;
            
            content = content.Trim();

            if (content.Length > symbolsCount)
            {
                content = content.Substring(0, symbolsCount - 3).Trim();
                var pos = content.LastIndexOfAny("., ".ToCharArray());
                if(pos != -1)
                    content = content.Substring(0, pos).Trim();

                pos = content.LastIndexOf("<");
                if (pos != -1)
                {
                    var end = content.IndexOf(">", pos);
                    if (end == -1)
                        content = content.Substring(0, pos - 1);
                }

                content += "...";
            }

            return content;
        }

        // TODO: Add UT for this method
        public static string GetHtmlPreview(string content, int symbolsCount = 512)
        {
            if (String.IsNullOrEmpty(content))
                return content;
            
            var regex = new Regex(
                @"<(/?)(\w+)((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)(/?)>", 
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            
            var matcher = new ReplaceHtmlForPreview(false);
            content = regex.Replace(content, matcher.Replace).Trim();
            content = LimitText(content, symbolsCount);
            

            var stack = new Stack<KeyValuePair<string, bool>>();
            var ndx = 0;
            var validHtml = true;
            Match m;
            
            // insert closing tags if they are trimmed
            while ((m = regex.Match(content, ndx)).Success)
            {
                var pair = new KeyValuePair<string, bool>(m.Groups[2].Value, m.Groups[1].Length > 0);
                
                // check closing tag
                if (pair.Value)
                {
                    if (stack.Count == 0 || !stack.Peek().Key.Equals(pair.Key))
                    {
                        validHtml = false;
                        break;
                    }

                    stack.Pop();
                }
                else if(m.Groups[6].Length == 0) // skip self-closed tags
                {
                    stack.Push(pair);
                }

                ndx = m.Index + m.Length;
            }

            if (validHtml)
            {
                if(stack.Count > 0)
                {
                    content = LimitText(content, symbolsCount - stack.Count * 5);
                }
                while (stack.Count > 0)
                {
                    var pair = stack.Pop();
                    content += String.Format("</{0}>", pair.Key);
                }
            }
            
            if (!validHtml || content.Length > symbolsCount) {
                matcher = new ReplaceHtmlForPreview(true);
                content = regex.Replace(content, matcher.Replace).Trim();
            }
            
            return content;
        }

        #region Privats

        private class ReplaceHtmlForPreview
        {
            private static string[] AllowedPreviewTags = new[]
            {
                "p", "i", "b", "a", "h1", "h2", "h3", "h4", "h5", "h6", "br"
            };

            private bool _fullReplace;

            public ReplaceHtmlForPreview(bool fullReplace)
            {
                _fullReplace = fullReplace;
            }

            /// <summary>
            /// Pass only AllowedPreviewTags tags
            /// </summary>
            /// <param name="m">Match to test</param>
            /// <returns></returns>
            public string Replace(Match m)
            {
                if (!_fullReplace && AllowedPreviewTags.Contains(m.Groups[2].Value))
                {
                    if (m.Groups[2].Value == "br")
                        return "<br/>"; // make valid

                    return m.Value; // return String.Format("<{0}{1}>", m.Groups[1].Value, m.Groups[2].Value);
                }

                return String.Empty;
            }
        }

        #endregion
    }
}
