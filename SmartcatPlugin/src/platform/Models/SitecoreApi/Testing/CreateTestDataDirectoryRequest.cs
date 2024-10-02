using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.Testing
{
    public class CreateTestDataDirectoryRequest
    {
        public TestDirectory RootDirectory { get; set; }
    }

    public class TestDirectory
    {
        public string Title { get; set; }
        public List<TestPage> Pages { get; set; }
        public List<TestDirectory> Children { get; set; } = new List<TestDirectory>();
    }

    public class TestPage
    {
        public TestValue Title { get; set; }
        public TestValue Content { get; set; }
        public List<TestPage> Children { get; set; } = new List<TestPage>();
    }

    public class TestValue
    {
        public string EnglishValue { get; set; }
        public string RussianValue { get; set; }
        public string SpanishValue { get; set; }
    }
}