using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public class StringClient {

	public bool IsConnected { get; set; } = false;

	public string hostname;
	public int port;
	public Action<string> logToConsole;

	protected TcpClient client;
	Action<string> onMessageReceived;

	public StringClient(Action<string> logToConsole) {
		this.logToConsole = logToConsole;
	}

	public async void ConnectAndDontWaitAsync(string hostname, int port, Action<string> onMessageReceived) {
		if (IsConnected) {
			throw new Exception($"ERROR: Client already connected to {hostname}:{port}. Disconnect before connecting again.");
		}

		this.hostname = hostname;
		this.port = port;

		try {
			using (client = new TcpClient()) {
				logToConsole("Connecting to server...");
				await client.ConnectAsync(hostname, port);
				IsConnected = true;
				logToConsole("Connected!");
				StartListeningBlocking(onMessageReceived);
			}
		} catch (Exception e) {
			logToConsole("StringClient error:");
			logToConsole(e.ToString());
		}
	}

	protected virtual void StartListeningBlocking(Action<string> onMessageReceived) {
		NetworkStream stream = client.GetStream();
		byte[] buffer = new byte[256];
		int readTotal;

		do {
			readTotal = stream.Read(buffer, 0, buffer.Length);
			string message = Encoding.UTF8.GetString(buffer, 0, readTotal);
			onMessageReceived?.Invoke(message);
			//await Task.Delay(100);  // Prevents too much CPU usage (?)
		} while (readTotal != 0);
	}

	public void Disconnect() {
		client.Close();
		IsConnected = false;
	}

	public void OnMessageReceived(Action<string> onMessageReceived) {
		this.onMessageReceived = onMessageReceived;
	}

	public async void SendMessageAsync(string message) {
		NetworkStream stream = client.GetStream();
		byte[] dataToSend = Encoding.UTF8.GetBytes(message);
		await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
	}





}
