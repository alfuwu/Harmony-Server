namespace Server.Models.Enums;
public enum ChannelType : ushort {
    Category = 0,       // category channel, used for organizing other channels
    Text = 1,           // default text channel
    Voice = 2,          // default voice channel
    Announcement = 3,   // announcement channel
    Thread = 4,         // thread, parented to some other channel
    Forum = 5,          // forum, no normal text stuff but allows creating threads and displays those as posts
    Calendar = 6,       // calendar channel, used for scheduling & showing events
    Document = 7,       // document channel, used for collaborative document editing
    DM = 8,             // direct message channel between two users
    GroupDM = 9,        // group direct message channel
    Unknown = 65535     // unknown channel type
}
