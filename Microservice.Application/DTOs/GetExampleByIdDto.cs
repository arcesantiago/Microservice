namespace Microservice.Application.DTOs
{
    public class GetExampleByIdDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
