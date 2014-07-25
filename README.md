To get running, download the sync server and start it up with the following config.json.


```
{
	"log": ["CRUD+", "REST+", "Changes+", "Attach+"],
	"databases": {
		"todo": {
			"server": "walrus:data",
			"sync": `
function(doc){
	channel(doc.channels);
}`,
			"users": {
			    "GUEST": {"disabled": false, "admin_channels": ["*"]}
			}

		}

	}
}

```

You'll need to manually add a URL for your sync server in [Todo.Shared/CouchbaseLite.cs](Todo.Shared/CouchbaseLite.cs)
