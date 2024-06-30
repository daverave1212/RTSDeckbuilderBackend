using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;



public class StringServer {

	public string hostname;
	public int port;
	public TcpListener tcpListener;

	public readonly SemaphoreSlim connectedXClientsSemaphore = new SemaphoreSlim(1, 1);
	public List<StringServerClient> connectedXClients = new List<StringServerClient>();
	List<Action<StringServerClient>> onClientConnectedListeners = new List<Action<StringServerClient>>();
	List<Action<StringServerClient>> onClientDisconnectedListeners = new List<Action<StringServerClient>>();

	public StringServer(string hostname, int port) {
		this.hostname = hostname;
		this.port = port;
	}

	public void OnClientConnectedAsync(Action<StringServerClient> callback) {		// void(StringServerClient stringServerClient) ... 
		onClientConnectedListeners.Add(callback);
	}

	public void OnClientDisonnectedAsync(Action<StringServerClient> callback) {		// void(StringServerClient stringServerClient) ... 
		onClientDisconnectedListeners.Add(callback);
	}

	public async void StartAsync() {
		var localhostIP = IPAddress.Parse(hostname); // or IPAddress.Any
		tcpListener = new TcpListener(IPAddress.Any, port);
		tcpListener.Start();

		while (true) {
			Console.WriteLine("Waiting for a client to connect...");
			TcpClient client = await tcpListener.AcceptTcpClientAsync();
			Console.WriteLine("Client connected!");
			StringServerClient stringServerClient = new StringServerClient(client, tcpListener);
			RememberXServerClientAsync(stringServerClient);

			foreach (var callback in onClientConnectedListeners) {
				callback(stringServerClient);
			}

			_ = Task.Run(() => {
				byte[] buffer = new byte[256];  // Every incoming message will be at most 256 characters long (string). Otherwise it will be split into multiple packets.

				var tcpStream = client.GetStream();
				int readTotal;

				try {
					do {
						readTotal = tcpStream.Read(buffer, 0, buffer.Length);
						string message = Encoding.UTF8.GetString(buffer, 0, readTotal);
						stringServerClient.onMessageReceived(message);
					} while (readTotal != 0);
				} catch (System.IO.IOException e) {
					RemoveXServerClientAsync(stringServerClient);
					foreach (var callback in onClientDisconnectedListeners) {
						callback(stringServerClient);
					}
				}
			});
		}
	}

	async void RememberXServerClientAsync(StringServerClient stringServerClient) {
		await connectedXClientsSemaphore.WaitAsync();
		connectedXClients.Add(stringServerClient);
		connectedXClientsSemaphore.Release();
	}

	async void RemoveXServerClientAsync(StringServerClient stringServerClient) {
		await connectedXClientsSemaphore.WaitAsync();
		connectedXClients.Remove(stringServerClient);
		connectedXClientsSemaphore.Release();
	}

}