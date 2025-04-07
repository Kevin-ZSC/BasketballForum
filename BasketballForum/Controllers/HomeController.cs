 using System.Diagnostics;
using BasketballForum.Data;
using BasketballForum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasketballForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly BasketballForumContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(BasketballForumContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var totalDiscussions = _context.Discussion.Count();
            var discussions = _context.Discussion
                .Include(d => d.ApplicationUser)
                .Include(d => d.Comments)  
                .OrderByDescending(d => d.CreateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PaginatedList<Discussion>(discussions, totalDiscussions, page, pageSize);
            return View(model);
        }


        public async Task<IActionResult> Profile(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(); 
            }

            var discussions = _context.Discussion
                                      .Where(d => d.ApplicationUserId == user.Id)
                                      .Include(d => d.ApplicationUser)
                                      .OrderByDescending(d => d.CreateDate)
                                      .ToList();

          
            ViewData["Discussions"] = discussions; ;

            return View(user); 
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
                                     .Include(d => d.Comments)
                                     .ThenInclude(c => c.ApplicationUser)
                                     .FirstOrDefault(d => d.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound(); 
            }
            discussion.Comments = discussion.Comments.OrderByDescending(c => c.CreateDate).ToList();

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
