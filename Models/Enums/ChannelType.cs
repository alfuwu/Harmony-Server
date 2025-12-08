namespace Server.Models.Enums;
public enum ChannelType : ushort {
    Category = 0,       // category channel, used for organizing other channels
    Text = 1,           // default text channel
    Voice = 2,          // default voice channel
    Announcement = 3,   // announcement channel
    Rules = 4,          // rules channel
    Thread = 5,         // thread, parented to some other channel
    Forum = 6,          // forum, no normal text stuff but allows creating threads and displays those as posts
    Calendar = 7,       // calendar channel, used for scheduling & showing events
    Document = 8,       // document channel, used for collaborative document editing
    DM = 9,             // direct message channel between two users
    GroupDM = 10,       // group direct message channel
    Unknown = 11        // unknown channel type
}
public static class ChannelTypeMethods {
    public static bool IsServerChannel(this ChannelType type) =>
        type != ChannelType.DM && type != ChannelType.GroupDM && type < ChannelType.Unknown;
    public static bool CanBeNamedWhatever(this ChannelType type) =>
        type == ChannelType.Category || type == ChannelType.Voice || type == ChannelType.Thread;
    public static bool CanHaveThread(this ChannelType type) =>
        type == ChannelType.Text || type == ChannelType.Announcement || type == ChannelType.Rules ||
        type == ChannelType.Forum || type == ChannelType.DM || type == ChannelType.GroupDM;
}