using Microsoft.AspNetCore.Http;
using Moq;
using OWL.Core.CustomExceptions;
using OWL.Core.DTO;
using OWL.Core.Interfaces;
using OWL.Core.Models;
using OWL.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Test.UnitTests.Services
{
    public class MoveServiceTests
    {
        private readonly Mock<IMoveRepository> _mockMoveRepo;
        private readonly MoveService _moveService;

        public MoveServiceTests()
        {
            _mockMoveRepo = new Mock<IMoveRepository>();
            _moveService = new MoveService(_mockMoveRepo.Object);
        }

        [Fact]
        public void GetAllUniversalMoves_ShouldReturnMoves()
        {
            // Arrange
            var moveDtos = new List<MoveDto> { new MoveDto { Id = 1, Name = "Move1" } };
            _mockMoveRepo.Setup(repo => repo.GetAllUniversalMoves()).Returns(moveDtos);

            // Act
            var result = _moveService.GetAllUniversalMoves();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Move1", result.First().Name);
        }

        [Fact]
        public void AddMoveImage_ShouldSaveImage()
        {
            // Arrange
            var move = new Move();
            var imageFileMock = new Mock<IFormFile>();
            var content = "Fake Image Content";
            var fileName = "image.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            imageFileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            imageFileMock.Setup(_ => _.FileName).Returns(fileName);
            imageFileMock.Setup(_ => _.Length).Returns(ms.Length);
            imageFileMock.Setup(_ => _.ContentType).Returns("image/jpeg");

            // Act
            _moveService.AddMoveImage(move, imageFileMock.Object);

            // Assert
            var expectedPrefix = "/Images/Move/image_";
            Assert.StartsWith(expectedPrefix, move.Image);

            // Additional regex pattern to match the entire structure
            var regexPattern = @"^/Images/Move/image_\d+\.jpg$";
            Assert.Matches(regexPattern, move.Image);
        }

        [Fact]
        public void AddMoveImage_ShouldThrowInvalidImageExtensionException()
        {
            // Arrange
            var move = new Move();
            var imageFileMock = new Mock<IFormFile>();
            imageFileMock.Setup(_ => _.FileName).Returns("file.txt");
            imageFileMock.Setup(_ => _.Length).Returns(100); // Ensure the file length is greater than 0

            // Act & Assert
            var exception = Assert.Throws<InvalidImageExtensionException>(() => _moveService.AddMoveImage(move, imageFileMock.Object));

            // Additional Assert to verify the exception message if needed
            Assert.Equal("Invalid image file type.", exception.Message);
        }

        [Fact]
        public void AddMoveImage_ShouldThrowInvalidImageTypeException()
        {
            // Arrange
            var move = new Move();
            var imageFileMock = new Mock<IFormFile>();
            var content = "Fake Image Content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            imageFileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            imageFileMock.Setup(_ => _.FileName).Returns("file.jpg");
            imageFileMock.Setup(_ => _.Length).Returns(ms.Length);
            imageFileMock.Setup(_ => _.ContentType).Returns("text/plain"); // Invalid MIME type

            // Act & Assert
            var exception = Assert.Throws<InvalidImageTypeException>(() => _moveService.AddMoveImage(move, imageFileMock.Object));

            // Additional Assert to verify the exception message if needed
            Assert.Equal("Invalid image MIME type.", exception.Message);
        }

        [Fact]
        public void RemoveMoveImage_ShouldDeleteImage()
        {
            // Arrange
            var move = new Move { Image = "/Images/Move/test.jpg" };
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Move", "test.jpg");
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            // Act
            _moveService.RemoveMoveImage(move);

            // Assert
            Assert.False(File.Exists(path));
        }

        [Fact]
        public void AddMove_ShouldThrowNameRequiredException()
        {
            // Arrange
            var move = new Move { Name = "" };

            // Act & Assert
            Assert.Throws<NameRequiredException>(() => _moveService.AddMove(move, 1));
        }

        [Fact]
        public void AddMove_ShouldThrowNameExistsException()
        {
            // Arrange
            var move = new Move { Name = "Move1", Id = 1 };
            _mockMoveRepo.Setup(repo => repo.CheckNameExists(It.IsAny<MoveDto>(), It.IsAny<int?>())).Returns(true);

            // Act & Assert
            Assert.Throws<NameExistsException>(() => _moveService.AddMove(move, 1));
        }

        [Fact]
        public void AddMove_ShouldCallAddMoveDto()
        {
            // Arrange
            var move = new Move { Name = "Move1" };
            _mockMoveRepo.Setup(repo => repo.CheckNameExists(It.IsAny<MoveDto>(), It.IsAny<int?>())).Returns(false);

            // Act
            _moveService.AddMove(move, 1);

            // Assert
            _mockMoveRepo.Verify(repo => repo.AddMoveDto(It.IsAny<MoveDto>(), 1), Times.Once);
        }
    }
}
