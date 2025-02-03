using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class Utils {

	// Array
	public static void InsertInOrderedQueue<T>(List<T> queue, T elem, Func<T, float> getValueFunc) {
		if (queue.Count == 0) {
			queue.Add(elem);
			return;
		}
		var firstElem = queue[0];
		if (getValueFunc(elem) <= getValueFunc(firstElem)) {
			queue.Insert(0, elem);
			return;
		}
		var lastElem = queue[queue.Count - 1];
		if (getValueFunc(elem) >= getValueFunc(lastElem)) {
			queue.Add(elem);
			return;
		}

		for (int i = 0; i < queue.Count; i++) {
			var thisElem = queue[i];
			if (getValueFunc(elem) <= getValueFunc(thisElem)) {
				queue.Insert(i, elem);
				return;
			}
		}
	}




	// Async
	public static async Task DoEveryAsync(int milliseconds, Action action, CancellationToken cancellationToken = default) {
		var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
		using PeriodicTimer timer = new PeriodicTimer(timeSpan);
		while (true) {
			action();
			await timer.WaitForNextTickAsync();
		}
	}
	public static async void DoAfterAsync(int milliseconds, Action action) {
		await Task.Delay(milliseconds);
		action();
	}


	// String
	public static bool LooksLikeJson(string str) {
		if (str == null)
			return false;
		var trimmedStr = str.Trim();
		if (trimmedStr.Length < 2) {
			return false;
		}
		if (trimmedStr[0] == '{' && trimmedStr[trimmedStr.Length - 1] == '}') {
			return true;
		}
		return false;
	}
	public static string StringDictToString(Dictionary<string, string> dict) {
		return "{\n" + string.Join(",\n", dict.Select(kvp => $"  {kvp.Key}: {kvp.Value}")) + "\n}";
	}

	// Example usage: SplitMergedJsonsStringByBraces("}]}", "{\"name\":\"Say\",\"data\":[{\"data\":\"Alo salut\"", 2);
	public static (string[], string, int) SplitMergedJsonsStringByBraces(string str, string previousRemainder="", int nBracketsLeftOpen=0) { // "{nr:1337}{age:26}"	->	["{nr:1337}", "{age:26}"]
		if (str.StartsWith("}]}")) {
			Console.WriteLine("Here.");
		}
		List<string> strings = new List<string>();
		int nBracketsOpen = nBracketsLeftOpen;
		int jsonStartI = 0;
		int jsonEndI = -1;

		for (int i = 0; i < str.Length; i++) {
			if (str[i] == '{') {
				if (nBracketsOpen == 0) {
					jsonStartI = i;
				}
				nBracketsOpen++;
				continue;
			}
			if (str[i] == '}') {
				nBracketsOpen--;
				if (nBracketsOpen == 0) {
					jsonEndI = i;
					var length = jsonEndI - jsonStartI + 1;
					var thisSubstring = str.Substring(jsonStartI, length);
					if (strings.Count == 0) {
						strings.Add(previousRemainder + thisSubstring);
					} else {
						strings.Add(previousRemainder);
					}
				}
			}
		}

		var hasRemainder = jsonEndI < str.Length - 1;
		var remainderLength = str.Length - 1 - jsonEndI;

		var fullJsonStringParts = strings.ToArray();
		var nBracketsLeftUnclosed = nBracketsOpen;
		var incompleteJsonRemainder = hasRemainder == false? "" : str.Substring(jsonEndI + 1, remainderLength);

		return (fullJsonStringParts, incompleteJsonRemainder, nBracketsLeftUnclosed);
	}
	public static bool IsStringJsonNotYetEnded(string str) {
		var trimmedStr = str.TrimEnd();
		return str[str.Length - 1] != '}';
	}
	public static (string, int) HandleJsonStringPartReceived(string jsonStringPartReceived, string lastIncompleteJsonPart, int nUnclosedBrackets, Action<string> handleJson) {
		(string[] fullJsons, string remainder, int nBracketsLeftUnclosed) = Utils.SplitMergedJsonsStringByBraces(jsonStringPartReceived, lastIncompleteJsonPart, nUnclosedBrackets);

		foreach (var completeJsonFound in fullJsons) {
			handleJson(completeJsonFound);
		}

		var newUnclosedBrackets = nBracketsLeftUnclosed;

		string newRemainder = "";
		if (fullJsons.Length == 0) {
			newRemainder = lastIncompleteJsonPart + remainder;
		} else {
			newRemainder = remainder;
		}

		return (newRemainder, newUnclosedBrackets);
	}

	// Other
	public static void InterpretCommandForObject<T>(T obj, Command command) {
		var methodName = command.name;
		Command[] arguments = { command };
		MethodInfo method = typeof(T).GetMethod(methodName);
		if (method == null) {
			Console.WriteLine($"ERROR: Method {methodName} with not found on object of type {typeof(T)} with arguments: {command.ToString()}");
			return;
		}
		method.Invoke(obj, arguments);
	}
}
