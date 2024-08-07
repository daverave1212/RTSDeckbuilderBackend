﻿using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// Public API
public partial class StringServer {
	public void OnClientConnectedAsync(Action<StringServerClient> callback) {       // void(StringServerClient stringServerClient) ... 
		onClientConnectedListeners.Add(callback);
	}
	public void OnClientDisonnectedAsync(Action<StringServerClient> callback) {     // void(StringServerClient stringServerClient) ... 
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
				NetworkStream tcpStream = client.GetStream();

				try {

					HandleStreamBlocking(tcpStream, stringServerClient);

				} catch (System.IO.IOException e) {
					RemoveXServerClientAsync(stringServerClient);
					foreach (var callback in onClientDisconnectedListeners) {
						callback(stringServerClient);
					}
				}
			});
		}
	}
}

// Implementation
public partial class StringServer {

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



	protected virtual void HandleStreamBlocking(NetworkStream tcpStream, StringServerClient stringServerClient) {
		byte[] buffer = new byte[256];  // Every incoming message will be at most 256 characters long (string). Otherwise it will be split into multiple packets.
		int readTotal;

		do {
			readTotal = tcpStream.Read(buffer, 0, buffer.Length);
			string message = Encoding.UTF8.GetString(buffer, 0, readTotal);
			stringServerClient.onMessageReceived(message);
		} while (readTotal != 0);
	}

}

public partial class StringServer {

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