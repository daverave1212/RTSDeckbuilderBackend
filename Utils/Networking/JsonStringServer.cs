using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class JsonStringServer : StringServer {
	public JsonStringServer(string hostname, int port) : base(hostname, port) { }

	public override void HandleStreamBlocking(NetworkStream tcpStream, StringServerClient stringServerClient) {
		byte[] buffer = new byte[256];  // Every incoming message will be at most 256 characters long (string). Otherwise it will be split into multiple packets.
		int readTotal;

		string lastIncompleteJsonPart = "";
		int nIncompleteBraces = 0;

		do {
			readTotal = tcpStream.Read(buffer, 0, buffer.Length);
			string message = Encoding.UTF8.GetString(buffer, 0, readTotal);

			// Read as many Jsons as you can and save whatever remains trailing in that string
			// It's possible that multiple requests are needed for one json
			(string[] fullJsons, string remainder, int nBracketsLeftUnclosed) = Utils.SplitMergedJsonsStringByBraces(message, nIncompleteBraces);

			foreach (var jsonString in fullJsons) {
				stringServerClient.onMessageReceived(jsonString);
			}

			if (fullJsons.Length == 0) {
				lastIncompleteJsonPart += remainder;
				nIncompleteBraces += nBracketsLeftUnclosed;
			} else {
				lastIncompleteJsonPart = remainder;
				nIncompleteBraces = nBracketsLeftUnclosed;
			}
		} while (readTotal != 0);
	}
}