﻿using System;

namespace BasketballForum.Models
{
    public class Discussion
    {
        public int DiscussionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageFilename { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public int CommentsCount { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
