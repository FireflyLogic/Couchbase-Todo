using System;
using SQLite.Net.Attributes;

namespace Todo
{
	public class TodoItem
	{
		public TodoItem ()
		{
		}
			
		public string ID { get; set; }
		public string Name { get; set; }
		public string Notes { get; set; }
		public bool Done { get; set; }
	}
}

