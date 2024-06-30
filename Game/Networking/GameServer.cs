using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

public partial class GameServer {

	List<ServerPlayer> playersConnected;
	Dictionary<string, LiveGame> liveGames;

	public GameServer() {
		playersConnected = new List<ServerPlayer>();
		liveGames = new Dictionary<string, LiveGame>();
	}

	public void StartServerAndWait() {
		StartServerAndDontWait();
		while (true) { }
	}
	public void StartServerAndDontWait() {
		var server = new JsonStringServer("127.0.0.1", 8080);

		server.OnClientConnectedAsync(stringServerClient => {

			var serverPlayer = new ServerPlayer(sendStringToClient: str => stringServerClient.SendMessage(str));
			playersConnected.Add(serverPlayer);

			stringServerClient.OnMessageReceived(jsonStr => {
				var commandDto = JsonSerializer.Deserialize<Command>(jsonStr);
				serverPlayer.OnReceiveCommand(commandDto);
			});
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
}


public partial class GameServer {
	public void Error(string msg = "") {
		Console.WriteLine("ERROR: " + msg);
	}
}