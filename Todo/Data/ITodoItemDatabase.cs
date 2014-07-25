using System;
using SQLite.Net;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Todo
{
	public interface ITodoItemDatabase{
		ObservableCollection<TodoItem> Items{ get; }
		TodoItem GetItem (string id);
		string SaveItem (TodoItem item);
		void DeleteItem (string id);
	}
	
}
