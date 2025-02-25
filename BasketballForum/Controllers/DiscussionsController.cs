using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BasketballForum.Data;
using BasketballForum.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BasketballForum.Controllers
{
    [Authorize]
	public class DiscussionsController : Controller
	{
		private readonly BasketballForumContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public DiscussionsController(BasketballForumContext context, IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

		// GET: Discussions
		public async Task<IActionResult> Index()
		{
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var discussions = await _context.Discussion
            .Where(d => d.ApplicationUserId == user.Id)
            .OrderByDescending(d => d.CreateDate) 
            .ToListAsync();
            return View(discussions);
		}

		public async Task<IActionResult> GetDiscussion(int id)
		{
			var discussion = await _context.Discussion.Include(d => d.Comments).FirstOrDefaultAsync(d => d.DiscussionId == id);

			if (discussion == null)
			{
				return NotFound();
			}
            discussion.Comments = discussion.Comments.OrderByDescending(c => c.CreateDate).ToList();

            return View(discussion);
		}

		// GET: Discussions/Create
		public IActionResult Create()
		{
            return View();
		}

		// POST: Discussions/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiscussionId,Title,Content,ImageFile,CreateDate")] Discussion discussion)
        {

            if (ModelState.IsValid)
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(); 
                }

                discussion.ApplicationUserId = user.Id;
                
                discussion.ImageFilename = Guid.NewGuid().ToString() + Path.GetExtension(discussion.ImageFile?.FileName);

                if (discussion.ImageFile != null)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", discussion.ImageFilename);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await discussion.ImageFile.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    discussion.ImageFilename = " "; 
                }

                _context.Add(discussion);
                await _context.SaveChangesAsync();

                return RedirectToAction("GetDiscussion", "Home", new { id = discussion.DiscussionId });

            }

            return View(discussion);
        }


        // GET: Discussions/Edit/5
        public async Task<IActionResult> Edit(int? id)
		{

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }


            if (id == null)
			{
				return NotFound();
			}

			var discussion = await _context.Discussion.FindAsync(id);
			if (discussion == null)
			{
				return NotFound();
			}

            if (discussion.ApplicationUserId != user.Id)
            {
                return Forbid(); 
            }
            return View(discussion);
		}

		// POST: Discussions/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost("Discussions/Edit/{id}")]  //add the route parameter to the post method in case of AmbiguousMatchException
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Content,ImageFile,CreateDate,ImageFilename")] Discussion discussion)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            } 

            if (ModelState.IsValid)
            {
                // Retrieve the existing discussion from the database
                var existingDiscussion = await _context.Discussion
                    .Include(d => d.Comments) 
                    .FirstOrDefaultAsync(d => d.DiscussionId == id);

                if (existingDiscussion == null)
                {
                    return NotFound();
                }

                if (existingDiscussion.ApplicationUserId != user.Id)
                {
                    return Forbid();
                }

                // Update editable properties
                existingDiscussion.Title = discussion.Title;
                existingDiscussion.Content = discussion.Content;
                
                // Handle image upload logic
                if (discussion.ImageFile != null)
                {
                    // Generate a new filename and save the file
                    existingDiscussion.ImageFilename = Guid.NewGuid().ToString() + Path.GetExtension(discussion.ImageFile.FileName);

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", existingDiscussion.ImageFilename);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await discussion.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // Save changes to the database
                _context.Update(existingDiscussion);
                await _context.SaveChangesAsync();

                return RedirectToAction("GetDiscussion", "Home", new { id = existingDiscussion.DiscussionId });
            }

            return View(discussion);
        }

      
        //GET: Discussions/Detail/5

        public async Task<IActionResult> Detail(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }
            if (id == null)
            {
                return NotFound();
            }
            var discussion = await _context.Discussion
                .FirstOrDefaultAsync(m => m.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }

            if (discussion.ApplicationUserId != user.Id)
            {
                return Forbid();
            }
            return View(discussion);
        }

        //POST: Discussions/Detail/5
        [HttpPost]
        public async Task<IActionResult> Detail(int id, [Bind("DiscussionId,Title,Content,ImageFilename,CreateDate")] Discussion discussion)
        {
            if (id != discussion.DiscussionId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionExists(discussion.DiscussionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(discussion);
        }

        // GET: Discussions/Delete/5
        public async Task<IActionResult> Delete(int? id)
		{
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
			{
				return NotFound();
			}

			var discussion = await _context.Discussion
				.FirstOrDefaultAsync(m => m.DiscussionId == id);
			if (discussion == null)
			{
				return NotFound();
			}

            if (discussion.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            return View(discussion);
		}

		// POST: Discussions/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var discussion = await _context.Discussion.FindAsync(id);

            if (discussion == null)
            {
                return NotFound();
            }

            //check if the user is the owner of the discussion
            if (discussion.ApplicationUserId != user.Id)
            {
                return Forbid(); 
            }
            _context.Discussion.Remove(discussion);
            await _context.SaveChangesAsync();
			return RedirectToAction("Index","Home");
		}

        private bool DiscussionExists(int discussionId)
        {
            throw new NotImplementedException();
        }
    }
}
