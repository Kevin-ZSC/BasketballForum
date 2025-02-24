using BasketballForum.Data;

namespace BasketballForum.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int DiscussionId { get; set; }

        // Foreign key (AspNetUsers table)
        public string ApplicationUserId { get; set; } = string.Empty;
        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
