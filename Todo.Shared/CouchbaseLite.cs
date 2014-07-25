using System.Linq;
using Xamarin.Forms;
using Couchbase.Lite;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

[assembly: Dependency (typeof (Todo.Shared.CouchbaseLite))]
namespace Todo.Shared
{
	public class CouchbaseLite : ITodoItemDatabase
	{
		Database db;

		public CouchbaseLite ()
		{
			db = Manager.SharedInstance.GetDatabase("todo");
			SetupViews ();
			SetupReplication (new Uri("http://localhost:4984/todo"));
		}

		void SetupReplication(Uri server){
			var pull = db.CreatePullReplication (server);
			var push = db.CreatePushReplication (server);
			pull.Continuous = push.Continuous = true;
			pull.Start();
			push.Start();
		}

		void SetupViews ()
		{
			var view = db.GetView("NotDone");
			if (view.Map == null)
			{
				view.SetMap((doc, emit) => 
					{
						object isDone;
						doc.TryGetValue("done", out isDone);

						if ((bool)isDone) {
							return;
						}
						emit (doc["id"], doc);
					}, "1");
			}

			var viewAll = db.GetView("All");
			if (viewAll.Map == null)
			{
				viewAll.SetMap((doc, emit) => 
					{
						emit (doc["id"], doc);
					}, "1");
			}
		}

		TodoItem ToTodo(Document d){
			var props = d.Properties;
			return new TodoItem {
				ID = Convert.ToInt32(props ["id"]),
				Name = (string)props["name"],
				Notes = (string)props["notes"],
				Done = (bool)props["done"]
			};
		}

		static Dictionary<string, object> ToDictionary (TodoItem item)
		{
			return new Dictionary<string, object> () {
				{"id",item.ID},
				{"name",item.Name},
				{"notes",item.Notes},
				{"done",item.Done}
			};
		}

		Document GetById (int id)
		{
			var view = db.GetView ("All").CreateQuery ();
			view.StartKey = id;
			view.EndKey = id;
			return view.Run ().Single ().Document;
		}

		#region ITodoItemDatabase implementation

		public IEnumerable<TodoItem> GetItems ()
		{
			var query = db.GetView("All").CreateQuery().ToLiveQuery ();
			return new LiveQueryObservableCollection<TodoItem> (query, ToTodo);
		}

		public IEnumerable<TodoItem> GetItemsNotDone ()
		{
			var view = db.GetView("NotDone");
			return view.CreateQuery ().Run ().Select (x => ToTodo (x.Document));
		}

		public TodoItem GetItem (int id)
		{
			return ToTodo (GetById(id));
		}

		public int SaveItem (TodoItem item)
		{
			if (item.ID != 0) {
				var doc = GetById (item.ID);
				doc.Update(x=>{
					x.SetProperties (ToDictionary (item));
					return true;
				});
			}else{
				item.ID = db.DocumentCount + 1;
				var doc = db.CreateDocument();
				doc.PutProperties (ToDictionary (item));
			}
		
			return item.ID;
		}

		public void DeleteItem (int id)
		{
			var doc = GetById (id);
			doc.Delete ();
		}

		#endregion
	}


}