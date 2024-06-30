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
		var server = new StringServer("127.0.0.1", 8080);

		server.OnClientConnectedAsync(xServerClient => {

			var serverPlayer = new ServerPlayer(str => {
				xServerClient.SendMessage(str);
			});

			xServerClient.OnMessageReceived(str => {
				if (Utils.LooksLikeJson(str) == false) {
					Error($"Message received does not look like JSON: {str}");
					return;
				}
				var commandDto = JsonSerializer.Deserialize<CommandDto>(str);
				serverPlayer.OnReceiveCommand(commandDto);
			});
		});

		server.OnClientDisonnectedAsync(xServerClient => {
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