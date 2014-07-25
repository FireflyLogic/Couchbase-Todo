using System;
using SQLite.Net;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Todo
{
	public interface ITodoItemDatabase{
		IEnumerable<TodoItem> GetItems ();
		IEnumerable<TodoItem> GetItemsNotDone ();
		TodoItem GetItem (int id);
		int SaveItem (TodoItem item);
		void DeleteItem (int id);
	}
	
}
