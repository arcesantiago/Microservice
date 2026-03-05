using FluentAssertions;
using Microservice.Application.DTOs;

namespace Microservice.Test.Application.DTOs
{
    /// <summary>
    /// Unit tests for Data Transfer Objects (DTOs)
    /// Tests DTO property initialization and data integrity
    /// </summary>
    public class GetExampleByIdDtoTests
    {
        [Fact]
        public void DTO_ShouldInitializeProperties()
        {
            // Arrange
            var id = 1;
            var createdAt = DateTimeOffset.UtcNow;
            var updatedAt = DateTimeOffset.UtcNow;

            // Act
            var dto = new GetExampleByIdDto
            {
                Id = id,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            dto.Id.Should().Be(id);
            dto.CreatedAt.Should().Be(createdAt);
            dto.UpdatedAt.Should().Be(updatedAt);
        }

        [Fact]
        public void DTO_ShouldInitializeNameAndDescription()
        {
            // Act
            var dto = new GetExampleByIdDto
            {
                Id = 1,
                Name = "Test",
                Description = "Description",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            // Assert
            dto.Name.Should().Be("Test");
            dto.Description.Should().Be("Description");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void DTO_ShouldHandleDifferentIds(int id)
        {
            // Act
            var dto = new GetExampleByIdDto { Id = id };

            // Assert
            dto.Id.Should().Be(id);
        }

        [Fact]
        public void DTO_ShouldPreserveTimestamps()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow;

            // Act
            var dto = new GetExampleByIdDto
            {
                CreatedAt = now,
                UpdatedAt = now.AddHours(1)
            };

            // Assert
            dto.CreatedAt.Should().Be(now);
            dto.UpdatedAt.Should().Be(now.AddHours(1));
        }

        [Fact]
        public void DTO_ShouldAllowCreatedBeforeUpdatedAt()
        {
            // Arrange
            var createdAt = DateTimeOffset.UtcNow.AddDays(-1);
            var updatedAt = DateTimeOffset.UtcNow;

            // Act
            var dto = new GetExampleByIdDto
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            dto.CreatedAt.Should().BeBefore(dto.UpdatedAt);
        }
    }

    public class GetExampleByPredicateDtoTests
    {
        [Fact]
        public void DTO_ShouldInitializeProperties()
        {
            // Arrange
            var id = 1;
            var createdAt = DateTimeOffset.UtcNow;
            var updatedAt = DateTimeOffset.UtcNow;

            // Act
            var dto = new GetExampleByPredicateDto
            {
                Id = id,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            dto.Id.Should().Be(id);
            dto.CreatedAt.Should().Be(createdAt);
            dto.UpdatedAt.Should().Be(updatedAt);
        }

        [Fact]
        public void DTO_ShouldInitializeNameAndDescription()
        {
            // Act
            var dto = new GetExampleByPredicateDto
            {
                Id = 1,
                Name = "Test",
                Description = "Description",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            // Assert
            dto.Name.Should().Be("Test");
            dto.Description.Should().Be("Description");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(int.MaxValue)]
        public void DTO_WithDifferentIds_ShouldStoreCorrectValue(int id)
        {
            // Act
            var dto = new GetExampleByPredicateDto { Id = id };

            // Assert
            dto.Id.Should().Be(id);
        }
    }

    public class DTOGeneralTests
    {
        [Fact]
        public void MultipleObjects_ShouldMaintainSeparateState()
        {
            // Arrange
            var dto1 = new GetExampleByIdDto { Id = 1 };
            var dto2 = new GetExampleByIdDto { Id = 2 };

            // Act & Assert
            dto1.Id.Should().Be(1);
            dto2.Id.Should().Be(2);
            dto1.Should().NotBe(dto2);
        }

        [Fact]
        public void DTO_ShouldBeReusable()
        {
            // Arrange
            var createdAt = DateTimeOffset.UtcNow;

            // Act
            var dto = new GetExampleByIdDto
            {
                Id = 1,
                CreatedAt = createdAt
            };

            var updatedDto = dto;
            updatedDto.Id = 2;

            // Assert
            updatedDto.Id.Should().Be(2);
        }
    }
}
