namespace Microservice.Application.DTOs
{
    public class GetExamplesWithProjectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
