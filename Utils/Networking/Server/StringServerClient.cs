using System;
using System.Net.Sockets;
using System.Text;

public class StringServerClient {

	TcpClient client;
	TcpListener tcpListener;

	public Action<string> onMessageReceived;

	public StringServerClient(TcpClient client, TcpListener tcpListener) {
		this.client = client;
		this.tcpListener = tcpListener;
	}

	public void OnMessageReceived(Action<string> onMessageReceived) {
		this.onMessageReceived = onMessageReceived;
	}

	public void SendMessage(string message) {
		var tcpStream = client.GetStream();
		var reply = Encoding.UTF8.GetBytes(message);
		tcpStream?.Write(reply, 0, reply.Length);
	}

}
