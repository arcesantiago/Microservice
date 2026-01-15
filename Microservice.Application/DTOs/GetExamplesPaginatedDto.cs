namespace Microservice.Application.DTOs
{
    public class GetExamplesPaginatedDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
