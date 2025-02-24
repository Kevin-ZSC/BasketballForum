using System.Diagnostics;
using BasketballForum.Data;
using BasketballForum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasketballForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly BasketballForumContext _context;

        public HomeController(BasketballForumContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var discussions = GetDiscussions().OrderByDescending(c => c.CreateDate).ToList();
            return View(discussions);
        }

        public IEnumerable<Discussion> GetDiscussions()
        {
            return _context.Discussion
                .Include(d => d.ApplicationUser)
                .Include(d => d.Comments)
                .ToList();
        }

        public IActionResult GetDiscussion(int id)
        {
            var discussion = _context.Discussion
                                     .Include(d => d.ApplicationUser)
                                     .Include(d => d.Comments.OrderByDescending(c => c.CreateDate))
                                     .FirstOrDefault(d => d.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound(); // Handle the case where the discussion is not found
            }

            return View("~/Views/Home/GetDiscussion.cshtml", discussion);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
