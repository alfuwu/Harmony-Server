namespace Server.Models.Enums;
public enum MessageType : byte {
    Normal = 0,
    Welcome = 1, // welcome message for new members
    Forward = 2, // treat the message's "Reference" field as a forwarded message ID
    Unknown = 3
}
