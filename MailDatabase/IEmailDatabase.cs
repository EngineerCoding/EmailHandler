using System.Collections.Generic;

namespace MailDatabase
{
	public interface IEmailDatabase
	{
		IEmailEntry Find(string emailUser);

		void Insert(IEmailEntry emailEntry);

		void Update(IEmailEntry emailEntry);

		void Delete(IEmailEntry emailEntry);

		IEnumerable<IEmailEntry> All();

		int GetCount();

	}
}
