using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

public class GameServer {

	List<ServerPlayer> playersConnected;
	List<LiveGame> liveGames;

	public GameServer() {
		playersConnected = new List<ServerPlayer>();
		liveGames = new List<LiveGame>();

		liveGames.Add(new LiveGame("asdf1234"));
	}

	public void StartServer() {
		var server = new StringServer("127.0.0.1", 8080);

		server.OnClientConnectedAsync(xServerClient => {

			var serverPlayer = new ServerPlayer(); 

			xServerClient.OnMessageReceived(str => {
				if (Utils.LooksLikeJson(str) == false) {
					Error();
					return;
				}
				using (JsonDocument jsonDocument = JsonDocument.Parse(str)) {

				}

			});
		});

		server.OnClientDisonnectedAsync(xServerClient => {
			Console.WriteLine("Client disconnected.");
		});

		server.StartAsync();

		while (true) { }
	}

	public void Error(string msg="") {
		Console.WriteLine("ERROR: " + msg);
	}

}


