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
    public class FightstyleServiceTests
    {
        [Fact]
        public void GetFightstyleById_ShouldReturnFightstyle_WhenIdExists()
        {
            // Arrange
            int existingStyleId = 1;
            var mockStyleRepository = new Mock<IFightstyleRepository>();
            var fightstyleDto = new FightstyleDto
            {
                Id = existingStyleId,
                Name = "TestCharacter",
                Power = 4,
                Speed = 5
            };
            mockStyleRepository.Setup(repo => repo.GetFightstyleDtoById(existingStyleId))
                                  .Returns(fightstyleDto);

            var fightstyleService = new FightstyleService(mockStyleRepository.Object);

            // Act
            var result = fightstyleService.GetFightstyleById(existingStyleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingStyleId, result.Id);
            Assert.Equal(fightstyleDto.Name, result.Name);
            Assert.Equal(fightstyleDto.Power, result.Power);
            Assert.Equal(fightstyleDto.Speed, result.Speed);
           

            mockStyleRepository.Verify(repo => repo.GetFightstyleDtoById(existingStyleId), Times.Once);
        }

        [Fact]
        public void GetFightstyleById_ShouldThrowIdNotFoundException_WhenIdNotFound()
        {
            // Arrange
            int nonExistingStyleId = 200; 
            var mockStyleRepository = new Mock<IFightstyleRepository>();
            mockStyleRepository.Setup(repo => repo.GetFightstyleDtoById(nonExistingStyleId))
                                   .Returns((FightstyleDto)null); 

            var fightstyleService = new FightstyleService(mockStyleRepository.Object);

            // Act & Assert
            var exception = Assert.Throws<IdNotFoundException>(() => fightstyleService.GetFightstyleById(nonExistingStyleId));

            Assert.Equal($"Entity with ID '{nonExistingStyleId}' not found.", exception.Message);
            Assert.Equal(nonExistingStyleId, exception.Id);

            mockStyleRepository.Verify(repo => repo.GetFightstyleDtoById(nonExistingStyleId), Times.Once);
        }

    }
}
