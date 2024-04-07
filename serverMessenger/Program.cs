
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

class Program
{
  private static List<WebSocket> clients = new List<WebSocket>();

  static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();

    app.UseWebSockets();

    app.Use(async (context, next) =>
    {
      if (context.Request.Path == "/wsconnect")
      {
        if (context.WebSockets.IsWebSocketRequest)
        {
          var webSocket = await context.WebSockets.AcceptWebSocketAsync();
          Console.WriteLine("WebSocket connection established" + DateTime.Now.ToString());
          await GetMessages(webSocket);
        }
        else
        {
          context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
      }
      else
      {
        await next(context);
      }
    });
    app.Run();
  }

  private static void ClearDisconectedClients()
  {
    foreach (var client in clients.ToList())
    {
      if (client.CloseStatus.HasValue)
      {
        clients.Remove(client);
      }
    }
  }

  private static async Task SendAllMessages()
  {
    var allMessages = await ReadWrite.GetAllMessages();
    var buffer = Encoding.Default.GetBytes(allMessages);

    Console.WriteLine(clients.Count);
    foreach (var client in clients)
    {
      Console.WriteLine("check1");
      if (!client.CloseStatus.HasValue)
      {
        Console.WriteLine("check2");
        try
        {
          await client.SendAsync(
              buffer,
              WebSocketMessageType.Text,
              true,
              CancellationToken.None);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        }
      }
    }

  }
  private static async Task GetMessages(WebSocket webSocket)
  {
    clients.Add(webSocket);

    await SendAllMessages();

    var buffer = new byte[1024 * 15];

    var receiveResult = await webSocket.ReceiveAsync(
    buffer, CancellationToken.None);

    while (!receiveResult.CloseStatus.HasValue)
    {
      ClearDisconectedClients();
      var message = JsonSerializer.Deserialize<Message>(Encoding.Default.GetString(buffer).TrimEnd('\0'))!;

      Console.WriteLine("get message: " + message.value);
      Console.WriteLine($"type: {message.type}");
      Console.WriteLine($"action: {message.action}");

      if (message.type == "action" && message.action == "clearChat")
      {
        ReadWrite.ClearMessages();
      }
      else if (message.type == "message")
      {
        await ReadWrite.AddNewMessage(message);
      }
      await SendAllMessages();

      buffer = new byte[1024 * 15];
      receiveResult = await webSocket.ReceiveAsync(
          buffer, CancellationToken.None);
    }

    await webSocket.CloseAsync(
        receiveResult.CloseStatus.Value,
        receiveResult.CloseStatusDescription,
        CancellationToken.None);
    Console.WriteLine("WebSocket connection closed");

  }
}

