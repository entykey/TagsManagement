using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TagsManagement.DTOs
{
    public class PostAddModel
    {
        [Required]
        public string Title { get; set; } = "No Title";

        [Required]
        public string Content { get; set; } = "No Content";

        [JsonProperty("tags")] // case sensitive (eg. asp.net, ASP.NET)
        public List<string> Tags { get; set; } = new List<string>();
    }
}
