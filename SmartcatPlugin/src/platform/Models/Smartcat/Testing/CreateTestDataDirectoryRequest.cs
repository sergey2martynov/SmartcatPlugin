namespace SmartcatPlugin.Models.Smartcat.Testing
{
    public class CreateTestDataDirectoryRequest
    {
        public TestDirectory RootDirectory { get; set; }
    }

    public class TestDirectory
    {
        public string Title { get; set; }
        public TestPage[] Pages { get; set; }
        public TestDirectory[] Children { get; set; }
    }

    public class TestPage
    {
        public TestValue Title { get; set; }
        public TestValue Content { get; set; }
        public TestPage[] Children { get; set; }
    }

    public class TestValue
    {
        public string EnglishValue { get; set; }
        public string RussianValue { get; set; }
        public string SpanishValue { get; set; }
    }
}