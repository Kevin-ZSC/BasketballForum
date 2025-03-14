﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BasketballForum.Data;
using BasketballForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BasketballForum.Controllers
{
    [Authorize]
	public class CommentsController : Controller
	{
		private readonly BasketballForumContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(BasketballForumContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
            _userManager = userManager;
		}
        // GET: Comments/Create
        public IActionResult Create(int DiscussionId)
        {
          
            var discussion = _context.Discussion.FirstOrDefault(d => d.DiscussionId == DiscussionId);

            if (discussion == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                DiscussionId = DiscussionId
            };

            return View(comment);
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
                var user = await _userManager.GetUserAsync(User);
                if (user == null) {
                    return Unauthorized();
                }

                comment.ApplicationUserId = user.Id;

                comment.CreateDate = DateTime.Now;
				_context.Add(comment);
                var discussion = await _context.Discussion.FindAsync(comment.DiscussionId);
                if (discussion != null)
                {
                    discussion.CommentsCount += 1;
                    _context.Discussion.Update(discussion);
                }
                await _context.SaveChangesAsync();
				return RedirectToAction("GetDiscussion", "Home", new { id = comment.DiscussionId });
			}
			return View(comment);
		}


	}
}
