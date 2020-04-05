using System.Collections.Generic;

namespace MailManagement.Models
{
	public class PaginatedResult<T>
	{
		public int Page { get; set; }

		public int ItemCount { get; set; }

		public int TotalItemCount { get; set; }

		public IEnumerable<T> Items { get; set; }

	}
}
