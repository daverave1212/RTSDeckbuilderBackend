using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// Accepts any string that's formatted like a JSON
class JsonStringServer : StringServer {
	public JsonStringServer(string hostname, int port) : base(hostname, port) { }

	protected override void HandleStreamBlocking(NetworkStream tcpStream, StringServerClient stringServerClient) {
		byte[] buffer = new byte[32];  // Every incoming message will be at most 256 characters long (string). Otherwise it will be split into multiple packets.
		int readTotal;

		string lastIncompleteJsonPart = "";
		int nIncompleteBraces = 0;

		do {
			readTotal = tcpStream.Read(buffer, 0, buffer.Length);
			string message = Encoding.UTF8.GetString(buffer, 0, readTotal);

			// Read as many Jsons as you can and save whatever remains trailing in that string
			// It's possible that multiple requests are needed for one json
			(lastIncompleteJsonPart, nIncompleteBraces) = Utils.HandleJsonStringPartReceived(message, lastIncompleteJsonPart, nIncompleteBraces, fullJson => {
				stringServerClient.onMessageReceived(fullJson);
			});

		} while (readTotal != 0);
	}
}