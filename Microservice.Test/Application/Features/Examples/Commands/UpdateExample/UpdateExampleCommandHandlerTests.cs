using FluentAssertions;
using Moq;
using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.Features.Examples.Commands.UpdateExample;
using Microservice.Domain.Entities;

namespace Microservice.Test.Application.Features.Examples.Commands.UpdateExample
{
    /// <summary>
    /// Unit tests for UpdateExampleCommandHandler
    /// Tests entity update operations and persistence
    /// </summary>
    public class UpdateExampleCommandHandlerTests
    {
        private readonly Mock<IReadRepository<Example>> _mockReadRepository;
        private readonly Mock<IWriteRepository<Example>> _mockWriteRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateExampleCommandHandler _handler;

        public UpdateExampleCommandHandlerTests()
        {
            _mockReadRepository = new Mock<IReadRepository<Example>>();
            _mockWriteRepository = new Mock<IWriteRepository<Example>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new UpdateExampleCommandHandler(
                _mockReadRepository.Object,
                _mockWriteRepository.Object,
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithExistingId_ShouldUpdateAndReturnSuccess()
        {
            // Arrange
            var command = new UpdateExampleCommand(1, "Updated Name", "Updated Description");
            var example = new Example("Test", "Description");
            example.Id = 1;

            _mockReadRepository
                .Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(example);

            _mockWriteRepository
                .Setup(r => r.Update(example))
                .Verifiable();

            _mockUnitOfWork
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(1);
            _mockWriteRepository.Verify(r => r.Update(example), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentId_ShouldReturnFailure()
        {
            // Arrange
            var command = new UpdateExampleCommand(999, null, null);

            _mockReadRepository
                .Setup(r => r.FindAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Example)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].Code.Should().Be("NotFound");
            _mockWriteRepository.Verify(r => r.Update(It.IsAny<Example>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallFindAsyncWithCorrectId()
        {
            // Arrange
            var command = new UpdateExampleCommand(5, null, null);
            var example = new Example("Test", "Description");
            example.Id = 5;

            _mockReadRepository
                .Setup(r => r.FindAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(example);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockReadRepository.Verify(r => r.FindAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallUpdateWithCorrectExample()
        {
            // Arrange
            var command = new UpdateExampleCommand(1, "New Name", null);
            var example = new Example("Test", "Description");
            example.Id = 1;

            _mockReadRepository
                .Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(example);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockWriteRepository.Verify(r => r.Update(example), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSaveChangesAfterUpdate()
        {
            // Arrange
            var command = new UpdateExampleCommand(1, null, "New Description");
            var example = new Example("Test", "Description");
            example.Id = 1;

            _mockReadRepository
                .Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(example);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockUnitOfWork.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public async Task Handle_WithDifferentIds_ShouldReturnCorrectId(int id)
        {
            // Arrange
            var command = new UpdateExampleCommand(id, null, null);
            var example = new Example("Test", "Description");
            example.Id = id;

            _mockReadRepository
                .Setup(r => r.FindAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(example);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(id);
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var command = new UpdateExampleCommand(1, null, null);
            var example = new Example("Test", "Description");
            example.Id = 1;
            var cancellationToken = new CancellationToken(canceled: false);

            _mockReadRepository
                .Setup(r => r.FindAsync(1, cancellationToken))
                .ReturnsAsync(example);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockUnitOfWork.Verify(
                u => u.SaveChangesAsync(cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            var command = new UpdateExampleCommand(1, null, null);

            _mockReadRepository
                .Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenSaveChangesThrows_ShouldPropagateException()
        {
            // Arrange
            var command = new UpdateExampleCommand(1, null, null);
            var example = new Example("Test", "Description");
            example.Id = 1;

            _mockReadRepository
                .Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(example);

            _mockUnitOfWork
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Concurrency error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithNonExistentId_ShouldNotUpdateEntity()
        {
            // Arrange
            var command = new UpdateExampleCommand(999, null, null);

            _mockReadRepository
                .Setup(r => r.FindAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Example)null!);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockWriteRepository.Verify(r => r.Update(It.IsAny<Example>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnErrorMessage_WhenNotFound()
        {
            // Arrange
            var command = new UpdateExampleCommand(999, null, null);

            _mockReadRepository
                .Setup(r => r.FindAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Example)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Errors[0].Message.Should().Contain("999");
            result.Errors[0].Message.Should().Contain("no encontrado");
        }
    }
}
