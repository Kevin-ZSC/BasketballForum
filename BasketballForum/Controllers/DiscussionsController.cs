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

namespace BasketballForum.Controllers
{
	public class DiscussionsController : Controller
	{
		private readonly BasketballForumContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public DiscussionsController(BasketballForumContext context, IWebHostEnvironment hostingEnvironment)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
		}

		// GET: Discussions
		public async Task<IActionResult> Index()
		{
			return View(await _context.Discussion.ToListAsync());
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
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiscussionId,Title,Content,ImageFile,CreateDate")] Discussion discussion)
        {

            if (ModelState.IsValid)
            {
                // rename the uploaded file to a guid (unique filename). Set before photo saved in database.
                discussion.ImageFilename = Guid.NewGuid().ToString() + Path.GetExtension(discussion.ImageFile?.FileName);

                if (discussion.ImageFile != null)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", discussion.ImageFilename);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await discussion.ImageFile.CopyToAsync(fileStream);
                    }
                }

                _context.Update(discussion);
                await _context.SaveChangesAsync();

                return RedirectToAction("GetDiscussion", "Discussions", new { id = discussion.DiscussionId });

            }

            return View(discussion);
        }


        // GET: Discussions/Edit/5
        public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var discussion = await _context.Discussion.FindAsync(id);
			if (discussion == null)
			{
				return NotFound();
			}
			return View(discussion);
		}

		// POST: Discussions/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost("Discussions/Edit/{id}")]  //add the route parameter to the post method in case of AmbiguousMatchException
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiscussionId,Title,Content,ImageFile,CreateDate,ImageFilename")] Discussion discussion)
        {
            if (id != discussion.DiscussionId)
            {
                return NotFound();
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

                return RedirectToAction("GetDiscussion", "Discussions", new { id = existingDiscussion.DiscussionId });
            }

            return View(discussion);
        }


        // GET: Discussions/Delete/5
        public async Task<IActionResult> Delete(int? id)
		{
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

			return View(discussion);
		}

		// POST: Discussions/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var discussion = await _context.Discussion.FindAsync(id);
			if (discussion != null)
			{
				_context.Discussion.Remove(discussion);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction("Index","Home");
		}
	}
}
