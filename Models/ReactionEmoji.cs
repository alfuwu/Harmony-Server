namespace Server.Models;
public class ReactionEmoji {
    /// <summary>
    /// If the reaction is a custom emoji, this field contains the emoji ID.
    /// </summary>
    public long? Id { get; set; }
    /// <summary>
    /// If the reaction is a unicode emoji, this field contains the unicode character(s). Otherwise, it contains the custom emoji's name.
    /// </summary>
    public string Name { get; set; } = "";
    public bool Animated { get; set; }
}
