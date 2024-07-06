using SmartcatPlugin.Models;
using System;
using System.Linq;

namespace SmartcatPlugin.Tools
{
    public class IdFieldExtractor
    {
        public static ItemIdFiledName ExtractIdAndField(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));

            string[] parts = input.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                throw new ArgumentException("Invalid input format.", nameof(input));

            parts = parts.Reverse().ToArray();

            return new ItemIdFiledName
            { 
                Id = parts[1],
                Field = parts[0]
            };
        }
    }
}