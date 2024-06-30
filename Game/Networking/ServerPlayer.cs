using System;
using System.Text.Json;

public partial class ServerPlayer {

	public string username;
	public Action<string> sendStringToClient;

	public ServerPlayer(Action<string> sendStringToClient) {
		this.sendStringToClient = sendStringToClient;
	}

	public void OnReceiveCommand(Command command) {
		Utils.InterpretCommandForObject<ServerPlayer>(this, command);
	}

	public void SendCommand(Command command) {
		sendStringToClient(JsonSerializer.Serialize(command));
	}

}


public partial class ServerPlayer {
	public void SetUsername(Command command) {
		username = command.GetString();
	}
	public void Say(Command command) {
		Console.WriteLine(username + ": " + command.GetString());
	}
}