namespace TagsManagement.DomainModels
{
    using System.ComponentModel.DataAnnotations;

    public abstract class BaseEntity
    {
        public string Id { get; set; }
        public string? Description { get; set; }
        public int Level { get; set; } = 0;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}