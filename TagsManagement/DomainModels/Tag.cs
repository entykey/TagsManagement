namespace TagsManagement.DomainModels
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<PostTag>? PostTags { get; set; } =  new List<PostTag>();
        public ICollection<VideoTag>? VideoTags { get; set; } = new List<VideoTag>();
    }

}
