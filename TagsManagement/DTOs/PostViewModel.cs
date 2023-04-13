using TagsManagement.DomainModels;

namespace TagsManagement.DTOs
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //public string Description { get; set; }
        public string Content { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set;}

    }
}
