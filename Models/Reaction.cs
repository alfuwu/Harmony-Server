namespace Server.Models;
public class Reaction {
    public long Id { get; set; }
    public long[]? Reactors { get; set; }
    public Emoji? Emoji { get; set; }
}
