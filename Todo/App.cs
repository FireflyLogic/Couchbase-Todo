using System;
using Xamarin.Forms;

namespace Todo
{
	public static class App
	{
		static ITodoItemDatabase database;

		public static Page GetMainPage ()
		{
			var mainNav = new NavigationPage (new TodoListPage ());

			return mainNav;
		}

		public static ITodoItemDatabase Database {
			get { 
				if (database == null) {
					database = DependencyService.Get<ITodoItemDatabase> ();
				}
				return database; 
			}
		}
	}
}

