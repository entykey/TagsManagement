namespace TagsManagement.DomainModels.Contents
{
    public class Post : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ICollection<PostTag> PostTags { get; set; }
    }
}
