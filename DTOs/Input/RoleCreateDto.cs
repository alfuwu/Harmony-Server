namespace Server.DTOs.Input;
public class RoleCreateDto {
    public long ServerId { get; set; }
    public string Name { get; set; } = "new role";
    public string? Description { get; set; }
}
