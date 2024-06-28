using System;

public class Utils {


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

	public static async Task DoEveryAsync(int milliseconds, Action action, CancellationToken cancellationToken = default) {
		var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
		using PeriodicTimer timer = new PeriodicTimer(timeSpan);
		while (true) {
			action();
			await timer.WaitForNextTickAsync();
		}
	}

	public static bool LooksLikeJson(string str) {
		if (str == null)
			return false;
		var trimmedStr = str.Trim();
		if (str.Length < 2) {
			return false;
		}
		if (trimmedStr[0] == '{' && trimmedStr[trimmedStr.Length - 1] == '}') {
			return true;
		}
		return false;
	}
}
