using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ServerPlayer : CommandServerClient {

	public string username;

	public void Say(Command command) {
		Console.WriteLine(username + ": " + command.GetString());
	}

	public void SetUsername(Command command) {
		username = command.GetString();
		Console.WriteLine($"Set username to {username}");
	}

}

