using System;
using System.Collections.Generic;
using System.Text.Json;

public class CommandDto {
	public string name { get; set; } = "";
	public Dictionary<string, string>[] data { get; set; }

	// Multiple constructors for easy data providing
	public CommandDto() { }
	public CommandDto(string name, Dictionary<string, string>[] data) {
		this.name = name;
		this.data = data;
	}
	public CommandDto(string name, Dictionary<string, string> dataDict) {
		this.name = name;
		this.data = new Dictionary<string, string>[] { dataDict };
	}
	public CommandDto(string name, string dataString) {
		this.name = name;
		var dict = new Dictionary<string, string>();
		dict["data"] = dataString;
		this.data = new Dictionary<string, string>[] { dict };
	}


	public override string ToString() {
		string str = $"name: {name}, data: [\n";
		str += string.Join(",\n", data.Select(dict => Utils.StringDictToString(dict)));
		str += "]";
		return str;
	}
}
