namespace Server.DTOs;
public class RegistrationCompleteDto {
    public long Id { get; set; }
    public string Username { get; set; } = "";
    public string Token { get; set; } = "";
}
