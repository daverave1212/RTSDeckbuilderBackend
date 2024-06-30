using System;
using System.Text.Json;

public class ServerPlayer {

	public Action<string> sendStringToClient;

	public ServerPlayer(Action<string> sendStringToClient) {
		this.sendStringToClient = sendStringToClient;
	}

	public void OnReceiveCommand(CommandDto command) {
		Console.WriteLine($"Received command: {command.ToString()}");
	}

	public void SendCommand(CommandDto command) {
		sendStringToClient(JsonSerializer.Serialize(command));
	}

}
