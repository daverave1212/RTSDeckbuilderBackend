using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

// After client connected, the client can only send messages that are a Command.cs class object JSON
/* Use like:

var commandServer = new CommandServer<TCommandServerClient>("127.0.0.1", 5000);
commandServer.OnPlayerConnected(serverClient => {
	Console.WriteLine("Player connected. serverPlayer can now receive commands.");
});
gameServer.StartServerAndWait();

// Players can now start connecting to 127.0.0.1:5000
// Send a JSON 

*/
public partial class CommandServer<TCommandServerClient> where TCommandServerClient : CommandServerClient, new() {

	public string hostname;
	public int port;

	List<CommandServerClient> playersConnected;
	Dictionary<string, LiveGame> liveGames;

	Action<CommandServerClient> onPlayerConnected;

	public CommandServer(string hostname, int port) {
		this.hostname = hostname;
		this.port = port;
		playersConnected = new List<CommandServerClient>();
		liveGames = new Dictionary<string, LiveGame>();
	}

	public void StartServerAndWait() {
		StartServerAndDontWait();
		while (true) { }
	}
	public void StartServerAndDontWait() {
		var server = new JsonStringServer(hostname, port);

		server.OnClientConnectedAsync(stringServerClient => {

			var serverPlayer = new TCommandServerClient();
			serverPlayer.sendStringToClient = str => stringServerClient.SendMessage(str);
			playersConnected.Add(serverPlayer);


			stringServerClient.OnMessageReceived(jsonStr => {
				var commandDto = JsonSerializer.Deserialize<Command>(jsonStr);
				Console.WriteLine($"Successfully deserialized command: {commandDto.name}");
				Utils.InterpretCommandForObject<TCommandServerClient>(serverPlayer, commandDto);
			});

			onPlayerConnected?.Invoke(serverPlayer);
		});

		server.OnClientDisonnectedAsync(stringServerClient => {
			Console.WriteLine("Client disconnected.");
			// TODO: Remove player from list
		});

		server.StartAsync();
	}

	public string CreateLiveGame(string id="") {
		if (id == "") {
			id = Guid.NewGuid().ToString("N");
		}
		var newLiveGame = new LiveGame(id);
		liveGames[id] = newLiveGame;
		Console.WriteLine($"Created new LiveGame with id={id}");
		return id;
	}
	public void OnPlayerConnected(Action<CommandServerClient> callback) {
		onPlayerConnected = callback;
	}
}


public partial class CommandServer {
	public void Error(string msg = "") {
		Console.WriteLine("ERROR: " + msg);
	}
}
