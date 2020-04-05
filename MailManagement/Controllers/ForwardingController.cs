using MailDatabase;
using MailManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MailManagement.Controllers
{
	[ApiController]
	[Route("api/mail/[controller]")]
	public class ForwardingController : ControllerBase
	{
		private readonly IEmailDatabase _emailDatabase;

		public ForwardingController(IEmailDatabase emailDatabase)
		{
			_emailDatabase = emailDatabase;
		}

		[HttpGet("All")]
		public PaginatedResult<IEmailEntry> All(int page = 0, int pageCount = 25)
		{
			return new PaginatedResult<IEmailEntry>()
			{
				Page = page,
				ItemCount = PaginatedEmailEntries(page, pageCount).Count(),
				TotalItemCount = _emailDatabase.GetCount(),
				Items = PaginatedEmailEntries(page, pageCount),
			};
		}

		private IEnumerable<IEmailEntry> PaginatedEmailEntries(int page, int pageCount)
		{
			page = Math.Max(0, page);
			pageCount = Math.Max(0, pageCount);
			int skip = page * pageCount;

			return _emailDatabase.All().Skip(skip).Take(pageCount);
		}

		[HttpPost]
		public IActionResult Create([FromBody] IEmailEntry emailEntry)
		{
			IEmailEntry existingItem = _emailDatabase.Find(emailEntry.EmailUser);
			if (existingItem == null)
			{
				_emailDatabase.Insert(emailEntry);
				return new ObjectResult(emailEntry)
				{
					StatusCode = StatusCodes.Status201Created
				};
			}
			else
			{
				return Conflict();
			}
		}

		[HttpGet("{emailUser}")]
		public IActionResult Get(string emailUser)
		{
			IEmailEntry emailEntry = _emailDatabase.Find(emailUser);
			if (emailEntry == null)
			{
				return NotFound();
			}
			else
			{
				return new ObjectResult(emailEntry)
				{
					StatusCode = StatusCodes.Status200OK,
				};
			}
		}

		[HttpPut("{emailUser}")]
		public IActionResult Update(string emailUser, [FromBody] IEmailEntry emailEntry)
		{
			IEmailEntry existingItem = _emailDatabase.Find(emailUser);
			if (existingItem == null)
			{
				return NotFound();
			}

			bool updated = false;
			foreach (PropertyInfo info in emailEntry.GetType().GetProperties())
			{
				object newValue = info.GetValue(emailEntry);
				if (newValue != null)
				{
					info.SetValue(existingItem, newValue);
					updated = true;
				}
			}

			if (updated)
			{
				_emailDatabase.Update(existingItem);
			}
			
			return new ObjectResult(existingItem)
			{
				StatusCode = StatusCodes.Status200OK,
			};
		}

		[HttpDelete("{emailUser}")]
		public IActionResult Delete(string emailUser)
		{
			IEmailEntry emailEntry = _emailDatabase.Find(emailUser);
			if (emailEntry == null)
			{
				return NoContent();
			}
			else
			{
				_emailDatabase.Delete(emailEntry);
				return new ObjectResult(emailEntry)
				{
					StatusCode = StatusCodes.Status200OK,
				};
			}
		}
	}
}
