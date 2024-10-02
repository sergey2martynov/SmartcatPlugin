namespace SmartcatPlugin.Models.SmartcatApi
{
    public class SmartcatDocument
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string Status { get; set; }
        public int WordsCount { get; set; }
    }
}