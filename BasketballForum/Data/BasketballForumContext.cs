using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BasketballForum.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BasketballForum.Data
{
    public class BasketballForumContext : IdentityDbContext<ApplicationUser>
    {
        public BasketballForumContext (DbContextOptions<BasketballForumContext> options)
            : base(options)
        {
        }

        public DbSet<BasketballForum.Models.Discussion> Discussion { get; set; } = default!;
        public DbSet<BasketballForum.Models.Comment> Comment { get; set; } = default!;

    }
}
