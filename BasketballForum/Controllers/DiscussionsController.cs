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

			return View(discussion);
		}

		// GET: Discussions/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var discussion = await _context.Discussion.Include(d => d.Comments)
                .FirstOrDefaultAsync(m => m.DiscussionId == id);
			if (discussion == null)
			{
				return NotFound();
			}

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
		public async Task<IActionResult> Create(IFormFile ImageFile, [Bind("DiscussionId,Title,Content,ImageFilename,CreateDate")] Discussion discussion)
		{
			if (ModelState.IsValid)
			{
				if(ImageFile != null && ImageFile.Length > 0 )
				{
					var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

					var filePath = Path.Combine(_hostingEnvironment.WebRootPath,"images",uniqueFileName);

					if(!Directory.Exists(Path.GetDirectoryName(filePath)))
					{
                        _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    discussion.ImageFilename = uniqueFileName;
                }
				_context.Add(discussion);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
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
		public async Task<IActionResult> Edit(int id,IFormFile ImageFile, [Bind("DiscussionId,Title,Content,ImageFilename,CreateDate")] Discussion discussion)
		{
			if (id != discussion.DiscussionId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // check existing image and delete it
                        var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", discussion.ImageFilename);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath); 
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", uniqueFileName);

                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        discussion.ImageFilename = uniqueFileName;
                    }

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
			return RedirectToAction(nameof(Index));
		}

		private bool DiscussionExists(int id)
		{
			return _context.Discussion.Any(e => e.DiscussionId == id);
		}
	}
}
