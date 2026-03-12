namespace Microservice.Application.DTOs
{
    public class GetExampleWithProjectionDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
