namespace TagsManagement.DomainModels.Contents
{
    public class Video : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public ICollection<VideoTag> VideoTags { get; set; }
    }
}
