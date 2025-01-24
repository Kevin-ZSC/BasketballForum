using System.Diagnostics;
using BasketballForum.Data;
using BasketballForum.Models;
using Microsoft.AspNetCore.Mvc;

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
            var discussions = GetDiscussion();
            return View(discussions);
        }

        public IEnumerable<Discussion> GetDiscussion()
        {
            return _context.Discussion.ToList();
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
