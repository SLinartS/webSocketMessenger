using System.Text.Json;

static class ReadWrite
{
  public static async Task AddNewMessage(Message message)
  {
    var localData = await ReadFile();

    if (message is not null)
    {
      message.id = Guid.NewGuid().ToString();
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
    Console.WriteLine("File reading error");
    return new LocalData(new List<Message>());
  }

  public async static void WriteFile(LocalData localData)
  {
    FileStream fs = new("local.json", FileMode.Create);
    await JsonSerializer.SerializeAsync(fs, localData);
    fs.Close();
  }

  public async static void ClearMessages()
  {
    var localData = new LocalData(new List<Message>());
    FileStream fs = new("local.json", FileMode.Create);
    await JsonSerializer.SerializeAsync(fs, localData);
    fs.Close();
  }
}