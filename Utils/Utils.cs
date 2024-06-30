using System;
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

	public static (string[], string, int) SplitMergedJsonsStringByBraces(string str, int nBracketsLeftOpen=0) { // "{nr:1337}{age:26}"	->	["{nr:1337}", "{age:26}"]
		List<string> strings = new List<string>();
		int nBracketsOpen = nBracketsLeftOpen;
		int jsonStartI = 0;

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
					var length = i - jsonStartI + 1;
					strings.Add(str.Substring(jsonStartI, length));
				}
			}
		}

		var remainderLength = str.Length - 1 - jsonStartI + 1;

		var fullJsonStringParts = strings.ToArray();
		var nBracketsLeftUnclosed = nBracketsOpen;
		var incompleteJsonRemainder = str.Substring(jsonStartI, remainderLength);

		return (fullJsonStringParts, incompleteJsonRemainder, nBracketsLeftUnclosed);
	}
	public static bool IsStringJsonNotYetEnded(string str) {
		var trimmedStr = str.TrimEnd();
		return str[str.Length - 1] != '}';
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
