using FluentAssertions;
using Microservice.Domain.Entities;

namespace Microservice.Test.Domain.Entities
{
    /// <summary>
    /// Unit tests for the Example domain entity
    /// Tests business logic and validation rules of the Example class
    /// </summary>
    public class ExampleTests
    {
        [Fact]
        public void Constructor_WithValidId_ShouldCreateInstance()
        {
            // Arrange
            var id = 1;

            // Act
            var example = new Example(id);

            // Assert
            example.Should().NotBeNull();
            example.Id.Should().Be(id);
            example.CreatedAt.Should().NotBeAfter(DateTimeOffset.UtcNow.AddSeconds(1));
            example.UpdatedAt.Should().NotBeAfter(DateTimeOffset.UtcNow.AddSeconds(1));
        }

        [Fact]
        public void Constructor_WithValidId_ShouldSetCreatedAndUpdatedAtToCurrentTime()
        {
            // Arrange
            var id = 100;
            var beforeCreation = DateTimeOffset.UtcNow.AddSeconds(-1);

            // Act
            var example = new Example(id);
            var afterCreation = DateTimeOffset.UtcNow.AddSeconds(1);

            // Assert
            example.CreatedAt.Should().BeOnOrAfter(beforeCreation);
            example.CreatedAt.Should().BeOnOrBefore(afterCreation);
            example.UpdatedAt.Should().BeOnOrAfter(beforeCreation);
            example.UpdatedAt.Should().BeOnOrBefore(afterCreation);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_WithInvalidId_ShouldThrowArgumentException(int invalidId)
        {
            // Act & Assert
            var action = () => new Example(invalidId);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Id must be greater than 0.*");
        }

        [Fact]
        public void Constructor_WithZero_ShouldThrowArgumentExceptionWithCorrectParameterName()
        {
            // Act & Assert
            var action = () => new Example(0);
            action.Should().Throw<ArgumentException>()
                .WithParameterName("Id");
        }

        [Fact]
        public void Constructor_WithNegativeId_ShouldThrowArgumentException()
        {
            // Act & Assert
            var action = () => new Example(-1);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Id must be greater than 0.*");
        }

        [Fact]
        public void CreatedAtAndUpdatedAt_ShouldBeEqual_WhenJustCreated()
        {
            // Arrange & Act
            var example = new Example(1);

            // Assert
            example.CreatedAt.Should().Be(example.UpdatedAt);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        [InlineData(999)]
        public void Constructor_WithDifferentValidIds_ShouldHaveCorrectId(int id)
        {
            // Act
            var example = new Example(id);

            // Assert
            example.Id.Should().Be(id);
        }
    }
}
