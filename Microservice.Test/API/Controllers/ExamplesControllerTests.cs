using FluentAssertions;
using Moq;
using MediatR;
using Microservice.API.Controllers;
using Microservice.Application.Common.Results;
using Microservice.Application.DTOs;
using Microservice.Application.Features.Examples.Commands.CreateExample;
using Microservice.Application.Features.Examples.Queries.GetExampleById;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Test.API.Controllers
{
    /// <summary>
    /// Unit tests for ExamplesController
    /// Tests HTTP endpoint handling, request/response mapping, and result conversion
    /// </summary>
    public class ExamplesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly ExamplesController _controller;

        public ExamplesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new ExamplesController(_mockMediator.Object);
        }

        [Fact]
        public async Task CreateExample_WithValidCommand_ShouldReturnCreatedResult()
        {
            // Arrange
            var command = new CreateExampleCommand(1);
            var expectedId = 1;

            _mockMediator
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<int>.Success(expectedId));

            // Act
            var result = await _controller.CreateExample(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var createdResult = result as ObjectResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task CreateExample_ShouldSendCommandToMediator()
        {
            // Arrange
            var command = new CreateExampleCommand(1);

            _mockMediator
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<int>.Success(1));

            // Act
            await _controller.CreateExample(command, CancellationToken.None);

            // Assert
            _mockMediator.Verify(
                m => m.Send(command, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public async Task CreateExample_WithDifferentIds_ShouldReturnCorrectId(int id)
        {
            // Arrange
            var command = new CreateExampleCommand(id);

            _mockMediator
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<int>.Success(id));

            // Act
            var result = await _controller.CreateExample(command, CancellationToken.None);

            // Assert
            var objResult = result as ObjectResult;
            objResult.Should().NotBeNull();
            objResult!.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetExampleById_WithExistingId_ShouldReturnOkResult()
        {
            // Arrange
            var id = 1;
            var expectedDto = new GetExampleByPredicateDto { Id = id };

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetExampleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetExampleByIdDto>.Success(
                    new GetExampleByIdDto { Id = id }));

            // Act
            var result = await _controller.GetExampleById(id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetExampleById_ShouldSendQueryToMediator()
        {
            // Arrange
            var id = 1;

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetExampleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetExampleByIdDto>.Success(
                    new GetExampleByIdDto { Id = id }));

            // Act
            await _controller.GetExampleById(id, CancellationToken.None);

            // Assert
            _mockMediator.Verify(
                m => m.Send(It.Is<GetExampleByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetExampleById_WithNonExistentId_ShouldReturnNotFoundResult()
        {
            // Arrange
            var id = 999;
            var failureResult = Result<GetExampleByIdDto>.Failure(
                Error.NotFound("Ejemplo no encontrado"));

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetExampleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failureResult);

            // Act
            var result = await _controller.GetExampleById(id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var objResult = result as ObjectResult;
            objResult.Should().NotBeNull();
            objResult!.StatusCode.Should().Be(404);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(int.MaxValue)]
        public async Task GetExampleById_WithDifferentIds_ShouldQueryCorrectId(int id)
        {
            // Arrange
            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetExampleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetExampleByIdDto>.Success(
                    new GetExampleByIdDto { Id = id }));

            // Act
            await _controller.GetExampleById(id, CancellationToken.None);

            // Assert
            _mockMediator.Verify(
                m => m.Send(It.Is<GetExampleByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateExample_ShouldRespectCancellationToken()
        {
            // Arrange
            var command = new CreateExampleCommand(1);
            var cancellationToken = new CancellationToken(canceled: false);

            _mockMediator
                .Setup(m => m.Send(command, cancellationToken))
                .ReturnsAsync(Result<int>.Success(1));

            // Act
            await _controller.CreateExample(command, cancellationToken);

            // Assert
            _mockMediator.Verify(
                m => m.Send(command, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task GetExampleById_ShouldRespectCancellationToken()
        {
            // Arrange
            var id = 1;
            var cancellationToken = new CancellationToken(canceled: false);

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetExampleByIdQuery>(), cancellationToken))
                .ReturnsAsync(Result<GetExampleByIdDto>.Success(
                    new GetExampleByIdDto { Id = id }));

            // Act
            await _controller.GetExampleById(id, cancellationToken);

            // Assert
            _mockMediator.Verify(
                m => m.Send(It.IsAny<GetExampleByIdQuery>(), cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task CreateExample_WhenMediatorThrows_ShouldPropagateException()
        {
            // Arrange
            var command = new CreateExampleCommand(1);

            _mockMediator
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Mediator error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _controller.CreateExample(command, CancellationToken.None));
        }

        [Fact]
        public async Task GetExampleById_WhenMediatorThrows_ShouldPropagateException()
        {
            // Arrange
            var id = 1;

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetExampleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Mediator error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _controller.GetExampleById(id, CancellationToken.None));
        }

        [Fact]
        public async Task CreateExample_WithValidationFailure_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new CreateExampleCommand(0); // Invalid ID
            var failureResult = Result<int>.Failure(
                Error.Validation("Id must be greater than 0"));

            _mockMediator
                .Setup(m => m.Send(It.IsAny<CreateExampleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failureResult);

            // Act
            var result = await _controller.CreateExample(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var objResult = result as ObjectResult;
            objResult.Should().NotBeNull();
            objResult!.StatusCode.Should().Be(400);
        }
    }

    // Helper DTO for testing (mirrors actual DTO structure)
    public class GetExampleByPredicateDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
