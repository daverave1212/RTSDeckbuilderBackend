using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

// After client connected, the client can only send messages that are a Command.cs class object JSON
public partial class GameServer {

	public string hostname;
	public int port;

	List<ServerPlayer> playersConnected;
	Dictionary<string, LiveGame> liveGames;

	Action<ServerPlayer> onPlayerConnected;

	public GameServer(string hostname, int port) {
		this.hostname = hostname;
		this.port = port;
		playersConnected = new List<ServerPlayer>();
		liveGames = new Dictionary<string, LiveGame>();
	}

	public void StartServerAndWait() {
		StartServerAndDontWait();
		while (true) { }
	}
	public void StartServerAndDontWait() {
		var server = new JsonStringServer(hostname, port);

		server.OnClientConnectedAsync(stringServerClient => {

			var serverPlayer = new ServerPlayer(sendStringToClient: str => stringServerClient.SendMessage(str));
			playersConnected.Add(serverPlayer);

			stringServerClient.OnMessageReceived(jsonStr => {
				var commandDto = JsonSerializer.Deserialize<Command>(jsonStr);
				Console.WriteLine($"Successfully deserialized command: {commandDto.name}");
				serverPlayer.OnReceiveCommand(commandDto);
			});

			onPlayerConnected?.Invoke(serverPlayer);
		});

		server.OnClientDisonnectedAsync(stringServerClient => {
			Console.WriteLine("Client disconnected.");
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
	public void OnPlayerConnected(Action<ServerPlayer> callback) {
		onPlayerConnected = callback;
	}
}


public partial class GameServer {
	public void Error(string msg = "") {
		Console.WriteLine("ERROR: " + msg);
	}
}