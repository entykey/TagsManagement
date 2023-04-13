using TagsManagement.DomainModels.Contents;

namespace TagsManagement.DomainModels
{
    public class VideoTag
    {
        public string VideoId { get; set; } // Video's Id (foreign key)
        public Video? Video { get; set; }   // navigation properties
        public string TagId { get; set; }   // Tag's Id (foreign key)
        public Tag? Tag { get; set; }       // navigation properties
    }
}
