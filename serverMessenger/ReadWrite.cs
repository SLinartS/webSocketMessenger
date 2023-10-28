using System.Text.Json;
using System.Text.Json.Serialization;

static class ReadWrite
{
  public static async Task AddNewMessage(string value)
  {
    var localData = await ReadFile();

    Message? message = JsonSerializer.Deserialize<Message>(value);

    if (message is not null)
    {
      localData.Messages.Add(message);
    }

    WriteFile(localData);
  }

  public static async Task<string> GetAllMessages()
  {
    var localData = await ReadFile();

    string? message = JsonSerializer.Serialize(localData.Messages);

    return message;
  }


  public async static Task<LocalData> ReadFile()
  {
    FileStream fs = new("local.json", FileMode.Open);
    LocalData? localData = await JsonSerializer.DeserializeAsync<LocalData>(fs);
    fs.Close();
    if (localData is not null)
    {
      return localData;
    }
    throw new Exception("File reading error");
  }
  public async static void WriteFile(LocalData localData)
  {
    FileStream fs = new("local.json", FileMode.Create);
    await JsonSerializer.SerializeAsync(fs, localData);
    fs.Close();
  }
}