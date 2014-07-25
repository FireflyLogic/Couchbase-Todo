using System.Linq;
using Xamarin.Forms;
using Couchbase.Lite;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

[assembly: Dependency (typeof (Todo.Shared.CouchbaseLite))]
namespace Todo.Shared
{

	public class LiveQueryObservableCollection<T> : ObservableCollection<T>,IDisposable
	{
		LiveQuery query;
		Func<Document, T> transform;

		public LiveQueryObservableCollection(LiveQuery query,Func<Document,T> transform)
			: base() {
			this.transform = transform;
			this.query = query;
			query.Changed += (sender, e) => {
				Items.Clear ();
				foreach (var row in e.Rows) {
					Items.Add (transform (row.Document));
				}
				OnCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add, Items.ToList ()));
				OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			};

			query.Start ();
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			query.Stop ();
		}
		#endregion
	}
}