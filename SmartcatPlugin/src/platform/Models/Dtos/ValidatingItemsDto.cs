using System.Collections.Generic;

namespace SmartcatPlugin.Models.Dtos
{
    public class ValidatingInfoDto
    {
        public string InvalidItemNames { get; set; }
        public int ValidItemCount { get; set; }
        public int InvalidItemCount { get; set; }
    }
}