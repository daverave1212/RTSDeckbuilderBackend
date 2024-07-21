using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class JsonStringClient : StringClient {

	public JsonStringClient(Action<string> logFunc) : base(logFunc) { }

	protected override void StartListeningBlocking(Action<string> onMessageReceived) {
		NetworkStream stream = client.GetStream();
		byte[] buffer = new byte[256];
		int readTotal;

		string lastIncompleteJsonPart = "";
		int nIncompleteBraces = 0;

		do {
			readTotal = stream.Read(buffer, 0, buffer.Length);
			string message = Encoding.UTF8.GetString(buffer, 0, readTotal);

			// Read as many Jsons as you can and save whatever remains trailing in that string
			// It's possible that multiple requests are needed for one json

			(string[] fullJsons, string remainder, int nBracketsLeftUnclosed) = Utils.SplitMergedJsonsStringByBraces(message, lastIncompleteJsonPart, nIncompleteBraces);

			foreach (var jsonString in fullJsons) {
				onMessageReceived?.Invoke(jsonString);
			}

			nIncompleteBraces = nBracketsLeftUnclosed;

			if (fullJsons.Length == 0) {
				lastIncompleteJsonPart += remainder;
			} else {
				lastIncompleteJsonPart = remainder;
			}

			//await Task.Delay(100);  // Prevents too much CPU usage (?)
		} while (readTotal != 0);
	}

}
