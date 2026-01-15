using Microservice.Domain.Common;

namespace Microservice.Domain.Entities
{
    public class Example : BaseDomainModel
    {

        private Example() { }

        public Example(int id)
        {

            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0.", nameof(Id));

            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
