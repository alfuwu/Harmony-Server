namespace Server.Models;
public class Emoji {
    /// <summary>
    /// If the reaction is a custom emoji, this field contains the emoji ID.
    /// </summary>
    public long? Id { get; set; }
    /// <summary>
    /// If the reaction is a unicode emoji, this field contains the unicode character(s).
    /// </summary>
    public string Name { get; set; } = "";
}
