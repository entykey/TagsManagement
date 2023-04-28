namespace TagsManagement.Repositories
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using DomainModels;
    using DomainModels.Contents;

    public class EFAppDbContext : IdentityDbContext
    {
        public EFAppDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // must install Microsoft.EntityFrameworkCore.SqlServer
            optionsBuilder.UseSqlServer("Server=MACBOOKPRO; Database=TagManage; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // must have this line first, else you will end up getting the error (The entity type 'IdentityUserLogin<string>' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating')
            base.OnModelCreating(modelBuilder);

            #region configure many-many relations:

            // Post - Tag joining entity:
            modelBuilder.Entity<PostTag>()
                .HasKey(sc => new { sc.PostId, sc.TagId });

            modelBuilder.Entity<PostTag>()
                .HasOne<Tag>(sc => sc.Tag)
                .WithMany(s => s.PostTags)   // PostTags is a ICollection<PostTag> field in Tag model
                .HasForeignKey(sc => sc.TagId);       // a field (foreignkey) in PostTag

            modelBuilder.Entity<PostTag>()
                .HasOne<Post>(sc => sc.Post)
                .WithMany(s => s.PostTags)   // PostTags is a ICollection<PostTag> field in Post model
                .HasForeignKey(sc => sc.PostId);       // a field (foreignkey) in PostTag


            // Video - Tag joining enity:
            modelBuilder.Entity<VideoTag>()
                .HasKey(sc => new { sc.VideoId, sc.TagId });

            
            modelBuilder.Entity<VideoTag>()
                .HasOne<Tag>(sc => sc.Tag)
                .WithMany(s => s.VideoTags)   // VideoTags is a ICollection field in Tag model
                .HasForeignKey(sc => sc.TagId);       // a field (foreignkey) in VideoTag

            modelBuilder.Entity<VideoTag>()
                .HasOne<Video>(sc => sc.Video)
                .WithMany(s => s.VideoTags)   // VideoTags is a ICollection field in Video model
                .HasForeignKey(sc => sc.VideoId);       // a field (foreignkey) in VideoTag


            #endregion

            #region remove "AspNet" postfix:
            // Bỏ tiền tố AspNet của các table: mặc định các table trong IdentityDbContext có
            // tên với tiền tố AspNet như: AspNetUserRoles, AspNetUser ...
            // Đoạn mã sau chạy khi khởi tạo DbContext, tạo database sẽ loại bỏ tiền tố đó
            // eg.      AspNetUsers -> Users,  AspNetRoles -> Roles
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6)); // remove first 6 character
                }
            }
            #endregion
        }



        // Users , it can be ApplicationUser in your project!
        //public DbSet<ApplicationUser> Users { get; set; } // "ApplicationUsers" will be the name of SQL table


        // Tag entity:
        public DbSet<Tag> Tags { get; set; }

        // PostTag joining entity:
        public DbSet<PostTag> PostTags { get; set; }

        // VideoTag joining entity:
        public DbSet<VideoTag> VideoTags { get; set; }

        // content entities:
        public DbSet<Video> Videos { get; set; }
        public DbSet<Post> Posts { get; set; }


        /*@Note:  chatgpt
            Tags matches the Tag entity.
            PostTags matches the PostTag entity, which represents the many-to-many relationship between posts and tags.
            VideoTags matches the VideoTag entity, which represents the many-to-many relationship between videos and tags.
            Videos matches the Video entity.
            Posts matches the Post entity.
        */

    }
}
