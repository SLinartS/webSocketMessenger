
using System.Net.WebSockets;
using System.Text;

class Program
{
  // private static WebSocket webSocket;

  private static List<WebSocket> clients = new();

  static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();

    app.MapGet("/", () => "Hello World!");

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

  private static async Task SendAllMessages()
  {
    var allMessages = await ReadWrite.GetAllMessages();
    var buffer = Encoding.UTF8.GetBytes(allMessages);

    foreach (var client in clients)
    {
      if (!client.CloseStatus.HasValue)
      {
        try
        {
          await client.SendAsync(
               buffer,
               WebSocketMessageType.Text,
               true,
               CancellationToken.None);
        }
        catch (System.Exception e)
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
      Console.WriteLine("get message: " + Encoding.UTF8.GetString(buffer));

      var encodeString = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
      await ReadWrite.AddNewMessage(encodeString);

      await SendAllMessages();

      buffer = new byte[1024 * 15];
      receiveResult = await webSocket.ReceiveAsync(
          buffer, CancellationToken.None);
    }

    await webSocket.CloseAsync(
        receiveResult.CloseStatus.Value,
        receiveResult.CloseStatusDescription,
        CancellationToken.None);
    // clients.Remove(client);
    Console.WriteLine("WebSocket connection closed");

  }
}

