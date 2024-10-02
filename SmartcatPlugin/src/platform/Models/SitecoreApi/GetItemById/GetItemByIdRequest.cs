using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetItemById
{
    public class GetItemByIdRequest
    {
        public List<ExternalObjectId> ItemIds { get; set; } = new List<ExternalObjectId>();
    }
}