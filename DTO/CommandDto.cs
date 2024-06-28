using System;
using System.Text.Json;

public class CommandDto
{
	public string token = "";
	public object data;

	public CommandDto() {}

	public string ToJson() {
		return JsonSerializer.Serialize(this);
	}

	public static CommandDto FromJson(string json) {
		return JsonSerializer.Deserialize<CommandDto>(json);
	}
}
