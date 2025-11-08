namespace orders_service.Src.DTOs
{
    public class UserDataDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
    }
}