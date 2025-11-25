namespace Server.Models;
public class Reaction {
    public List<long> Reactors { get; set; } = [];
    public Emoji? Emoji { get; set; }
}
