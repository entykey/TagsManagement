namespace TagsManagement.DomainModels
{
    using DomainModels.Contents;


    public class PostTag
    {
        public string? PostId { get; set; }  // Post's Id (foreign key), in this case, not match convention of origional field name in model Post
        public Post? Post { get; set; }     // navigation properties

        public string? TagId { get; set; }   // Tag's Id (foreign key)
        public Tag? Tag { get; set; }       // navigation properties
    }
}
