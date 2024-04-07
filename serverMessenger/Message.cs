class Message
{
  public required string id { get; set; }
  public required string author { get; set; }
  public required string value { get; set; }
  public required string type { get; set; }
  public string? action { get; set; }
}

enum MessageType
{
  Message,
  Action
}