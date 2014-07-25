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
		LiveQueryObservableCollection<TodoItem> _items;

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
			var viewAll = db.GetView("All");

			viewAll.SetMap((doc, emit) => {
				emit (doc["_id"], doc);
			}, "1.1");

			var query = viewAll.CreateQuery().ToLiveQuery();
			_items = new LiveQueryObservableCollection<TodoItem> (query, ToTodo);
		}

		TodoItem ToTodo(Document d){
			var props = d.Properties;
			return new TodoItem {
				ID = props ["_id"].ToString(),
				Name = (string)props["name"],
				Notes = (string)props["notes"],
				Done = (bool)props["done"]
			};
		}

		static Dictionary<string, object> ToDictionary (TodoItem item)
		{
			return new Dictionary<string, object> () {
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

		public ObservableCollection<TodoItem> Items {
			get {
				return _items;
			}
		}

		public TodoItem GetItem (string id)
		{
			return ToTodo (db.GetExistingDocument (id));
		}

		public string SaveItem (TodoItem item)
		{
			if (!String.IsNullOrEmpty(item.ID)) {
				var doc = db.GetDocument (item.ID);				
				doc.Update(x=>{
					x.SetProperties (ToDictionary (item));
					return true;
				});
			} else {
				var doc = db.CreateDocument();
				doc.PutProperties (ToDictionary (item));
				item.ID = doc.GetProperty("_id").ToString();
			}
		
			return item.ID;
		}

		public void DeleteItem (string id)
		{
			var doc = db.GetDocument(id);	
			doc.Delete ();
		}

		#endregion
	}


}