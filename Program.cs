
using System.Net.Sockets;
using System.Text.Json;

public class Program {

	static string HOSTNAME = "127.0.0.1";
	static int PORT = 5000;


	static void Main(string[] args) {
		MainServer();
		//MainClient();
	}





	static void MainServer() {
		Console.WriteLine("MAIN");

		var gameServer = new CommandServer<ServerPlayer>(HOSTNAME, PORT);
		gameServer.StartServerAndDontWait();

		gameServer.OnPlayerConnected(player => {
			player.SendCommand(new Command("Say", "You just connected hun"));
		});

		while (true) { }
	}

	static async void MainClient() {
		var stringClient = new JsonStringClient(Console.WriteLine);
		stringClient.ConnectAndDontWaitAsync(HOSTNAME, PORT, message => {
			Console.WriteLine($"Received from server: {message}");
		});

		while (true) { }
	}

}