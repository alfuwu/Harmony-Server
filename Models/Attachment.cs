namespace Server.Models;
public class Attachment : Identifiable {
    public string FileName { get; set; } = "";
    public string FilePath { get; set; } = "";
    public long? MessageId { get; set; }
    public Message? Message { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
