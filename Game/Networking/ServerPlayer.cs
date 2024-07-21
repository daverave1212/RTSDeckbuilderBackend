using System;
using System.Text.Json;

// All available commands are in the other partial class, underneath this one
public partial class ServerPlayer {

	public string username;
	public Action<string> sendStringToClient;

	public ServerPlayer(Action<string> sendStringToClient) {
		this.sendStringToClient = sendStringToClient;
	}

	public void OnReceiveCommand(Command command) {
		Utils.InterpretCommandForObject<ServerPlayer>(this, command);
	}

}


public partial class ServerPlayer {

	public void SendCommand(Command command) {
		sendStringToClient(JsonSerializer.Serialize(command));
	}
	public void SetUsername(Command command) {
		username = command.GetString();
		Console.WriteLine($"Set username to {username}");
	}
	public void Say(Command command) {
		Console.WriteLine(username + ": " + command.GetString());
	}

}