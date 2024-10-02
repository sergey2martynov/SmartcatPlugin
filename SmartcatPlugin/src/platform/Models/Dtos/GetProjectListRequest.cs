namespace SmartcatPlugin.Models.Dtos
{
    public class GetProjectListRequest
    {
        public int Offset { get; set; }
        public string WorkspaceId { get; set; }
    }
}