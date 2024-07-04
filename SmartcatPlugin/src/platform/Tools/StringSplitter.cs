using System;
using System.Collections.Generic;

namespace SmartcatPlugin.Tools
{
    public class StringSplitter
    {
        public static List<string> SplitString(string source, int maxLineLength = 50)
        {
            var result = new List<string>();
            var words = source.Split(new[] { ' ' }, StringSplitOptions.None);
            var currentLine = string.Empty;

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 > maxLineLength)
                {
                    result.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    if (currentLine.Length > 0)
                    {
                        currentLine += " ";
                    }
                    currentLine += word;
                }
            }

            if (currentLine.Length > 0)
            {
                result.Add(currentLine);
            }

            return result;
        }

        public static List<string> SplitStringWithNewlines(string source, int maxLineLength = 50)
        {
            var result = new List<string>();
            var lines = source.Split(new[] { '\n' }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                var splitLines = SplitString(line, maxLineLength);
                result.AddRange(splitLines);
            }

            return result;
        }
    }
}