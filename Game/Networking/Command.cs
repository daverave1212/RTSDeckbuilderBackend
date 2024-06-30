using System;
using System.Collections.Generic;
using System.Text.Json;

public class Command {
	public string name { get; set; } = "";
	public Dictionary<string, string>[] data { get; set; }

	// Multiple constructors for easy data providing
	public Command() { }
	public Command(string name, Dictionary<string, string>[] data) {
		this.name = name;
		this.data = data;
	}
	public Command(string name, Dictionary<string, string> dataDict) {
		this.name = name;
		this.data = new Dictionary<string, string>[] { dataDict };
	}
	public Command(string name, string dataString) {
		this.name = name;
		var dict = new Dictionary<string, string>();
		dict["data"] = dataString;
		this.data = new Dictionary<string, string>[] { dict };
	}

	public Dictionary<string, string> GetDict() {
		if (data == null || data.Length == 0)
			return null;
		return data[0];
	}
	public string GetString() {
		if (data == null || data.Length == 0)
			return null;
		return data[0]["data"];
	}

	public override string ToString() {
		string str = $"name: {name}, data: [\n";
		str += string.Join(",\n", data?.Select(dict => Utils.StringDictToString(dict)));
		str += "]";
		return str;
	}
}
