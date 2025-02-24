using System;
using System.ComponentModel.DataAnnotations.Schema;
using BasketballForum.Data;

namespace BasketballForum.Models
{
    public class Discussion
    {
        public int DiscussionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageFilename {  get; set; } = string.Empty;
        
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public int CommentsCount { get; set; }
        public ICollection<Comment>? Comments { get; set; }

        // Foreign key (AspNetUsers table)
        public string ApplicationUserId { get; set; } = string.Empty;
        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
