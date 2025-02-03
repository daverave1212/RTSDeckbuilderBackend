using System;
using System.Text.Json;

// All available commands are in the other partial class, underneath this one
public partial class CommandServerClient {
	
	public Action<string> sendStringToClient { set; protected get; }

	public CommandServerClient() { }

	public void SendCommand(Command command) {
		sendStringToClient(JsonSerializer.Serialize(command));
	}
}