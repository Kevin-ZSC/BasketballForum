using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BasketballForum.Data;
using BasketballForum.Models;

namespace BasketballForum.Controllers
{
    public class CommentsController : Controller
    {
        private readonly BasketballForumContext _context;

        public CommentsController(BasketballForumContext context)
        {
            _context = context;
        }
        // GET: Comments/Create
        public IActionResult Create(int DiscussionId)
        {
            var discussion = _context.Discussion.FirstOrDefault(d => d.DiscussionId == DiscussionId);

            if (discussion == null) { 
                return NotFound();
            }
            ViewData["DiscussionId"] = DiscussionId;

            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Content,CreateDate,DiscussionId")] Comment comment)
        {
            if (ModelState.IsValid)
            {   
                comment.CreateDate = DateTime.Now;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetDiscussion", "Discussions", new {id = comment.DiscussionId} );
            }
            return View(comment);
        }


    }
}
