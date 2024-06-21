using Microsoft.AspNetCore.Mvc.Diagnostics;
using Moq;
using OWL.Core.Interfaces;
using OWL.Core.DTO;
using OWL.Core.Services;
using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OWL.Core.CustomExceptions;
using Microsoft.AspNetCore.Http;

namespace OWL.Test.UnitTests.Services
{
    public class CharacterServiceTests
    {
        [Fact]
        public void GetAllCharacters_ShouldReturnList_When_Exists()
        {
            //Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();

            var dummyCharacterDtos = new List<CharacterDto> { new CharacterDto { Id = 1 }, new CharacterDto { Id = 2 } };
            mockCharacterRepository.Setup(repo => repo.GetAllCharactersWithFightstyle()).Returns(dummyCharacterDtos);

            var characterService = new CharacterService(mockCharacterRepository.Object);

            var result = characterService.GetAllCharactersWithFightstyle();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(dummyCharacterDtos.Count, result.Count);
        }

        [Fact]
        public void GetCharacterById_ShouldReturnCharacter_WhenIdExists()
        {
            // Arrange
            int existingCharId = 1; 
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterDto = new CharacterDto
            {
                Id = existingCharId,
                Name = "TestCharacter",
                Description = "Test description",
                Image = "example.com",
                NewlyAdded = true,
                Fightstyle = new Fightstyle { Id = 1, Name = "Kung Fu", Power = 4, Speed = 5 }
            };
            mockCharacterRepository.Setup(repo => repo.GetCharacterDtoById(existingCharId))
                                  .Returns(characterDto);

            var characterService = new CharacterService(mockCharacterRepository.Object);

            // Act
            var result = characterService.GetCharacterById(existingCharId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingCharId, result.Id);
            Assert.Equal(characterDto.Name, result.Name);
            Assert.Equal(characterDto.Description, result.Description);
            Assert.Equal(characterDto.Image, result.Image);
            Assert.Equal(characterDto.NewlyAdded, result.NewlyAdded);
            Assert.NotNull(result.FightStyle);
            Assert.Equal(characterDto.Fightstyle.Id, result.FightStyle.Id);
            Assert.Equal(characterDto.Fightstyle.Name, result.FightStyle.Name);
            Assert.Equal(characterDto.Fightstyle.Power, result.FightStyle.Power);
            Assert.Equal(characterDto.Fightstyle.Speed, result.FightStyle.Speed);

            mockCharacterRepository.Verify(repo => repo.GetCharacterDtoById(existingCharId), Times.Once);
        }

        [Fact]
        public void GetCharacterById_ShouldThrowIdNotFoundException_WhenIdNotFound()
        {
            // Arrange
            int nonExistingCharId = 100; // Assuming this ID does not exist in the database
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            mockCharacterRepository.Setup(repo => repo.GetCharacterDtoById(nonExistingCharId))
                                   .Returns((CharacterDto)null); // Simulate that character with ID does not exist

            var characterService = new CharacterService(mockCharacterRepository.Object);

            // Act & Assert
            var exception = Assert.Throws<IdNotFoundException>(() => characterService.GetCharacterById(nonExistingCharId));

            Assert.Equal($"Entity with ID '{nonExistingCharId}' not found.", exception.Message);
            Assert.Equal(nonExistingCharId, exception.Id);

            mockCharacterRepository.Verify(repo => repo.GetCharacterDtoById(nonExistingCharId), Times.Once);
        }

        [Fact]
        public void AddCharacter_ShouldCallAddCharacterDto_WhenCalled()
        {
            var mockCharacterRepository = new Mock<ICharacterRepository>();

            var characterService = new CharacterService(mockCharacterRepository.Object);

            characterService.AddCharacter(new Character(1,"James","Test dummy","example.com",true,new Fightstyle(1,"kung fu",4,5)));

            mockCharacterRepository.Verify(repo => repo.AddCharacterDto(It.IsAny<CharacterDto>()), Times.Once);
        }

        [Fact]
        public void AddCharacter_ShouldThrowNameRequiredException()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterService = new CharacterService(mockCharacterRepository.Object);
            var newCharacter = new Character(1, null, "Test dummy", "example.com", true, new Fightstyle(1, "kung fu", 4, 5));

            // Act & Assert
            var exception = Assert.Throws<NameRequiredException>(() => characterService.AddCharacter(newCharacter));
            Assert.Equal("Character name is required.", exception.Message);
        }

        [Fact]
        public void AddCharacter_ShouldThrowNameExistsException_WhenNameExists()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            mockCharacterRepository.Setup(repo => repo.CheckNameExists(It.IsAny<CharacterDto>(), null)).Returns(true);

            var characterService = new CharacterService(mockCharacterRepository.Object);
            var newCharacter = new Character(1, "ExistingName", "Test dummy", "example.com", true, new Fightstyle(1, "kung fu", 4, 5));

            // Act & Assert
            var exception = Assert.Throws<NameExistsException>(() => characterService.AddCharacter(newCharacter));

            Assert.Equal("A character with this name already exists.", exception.Message);
            mockCharacterRepository.Verify(repo => repo.CheckNameExists(It.IsAny<CharacterDto>(), null), Times.Once);
        }

        [Fact]
        public void AddCharacter_ShouldThrowFightstyleTiedToCharacterException_WhenTied()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            mockCharacterRepository.Setup(repo => repo.CheckFightstyleTiedToCharacter(It.IsAny<CharacterDto>(), null)).Returns(true);

            var characterService = new CharacterService(mockCharacterRepository.Object);
            var newCharacter = new Character(1, "NewCharacter", "Test dummy", "example.com", true, new Fightstyle(1, "ExistingStyle", 4, 5));

            // Act & Assert
            var exception = Assert.Throws<FightstyleTiedToCharacterException>(() => characterService.AddCharacter(newCharacter));

            Assert.Equal("A character is already tied with this fightstyle.", exception.Message);
            mockCharacterRepository.Verify(repo => repo.CheckFightstyleTiedToCharacter(It.IsAny<CharacterDto>(), null), Times.Once);
        }

        [Fact]
        public void UpdateCharacter_ShouldThrowNameExistsException_WhenNameExists()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            mockCharacterRepository.Setup(repo => repo.CheckNameExists(It.IsAny<CharacterDto>(), It.IsAny<int?>()))
                .Returns(true); // Simulate that the name exists in the database

            var characterService = new CharacterService(mockCharacterRepository.Object);
            var characterToUpdate = new Character(1, "ExistingName", "Test description", "example.com", true, new Fightstyle(1, "kung fu", 4, 5));

            // Act & Assert
            var exception = Assert.Throws<NameExistsException>(() => characterService.UpdateCharacter(characterToUpdate));

            // Optionally, you can assert the message of the exception
            Assert.Equal("A character with this name already exists.", exception.Message);
        }

        [Fact]
        public void UpdateCharacter_ShouldThrowFightstyleTiedToCharacterException_WhenTied()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            mockCharacterRepository.Setup(repo => repo.CheckFightstyleTiedToCharacter(It.IsAny<CharacterDto>(), It.IsAny<int?>()))
                .Returns(true); // Simulate that the fightstyle is already tied to another character

            var characterService = new CharacterService(mockCharacterRepository.Object);
            var characterToUpdate = new Character(1, "NewCharacter", "Test description", "example.com", true, new Fightstyle(1, "ExistingStyle", 4, 5));

            // Act & Assert
            var exception = Assert.Throws<FightstyleTiedToCharacterException>(() => characterService.UpdateCharacter(characterToUpdate));

            // Optionally, you can assert the message of the exception
            Assert.Equal("A character is already tied with this fightstyle.", exception.Message);
        }

        [Fact]
        public void AddCharacterImage_ShouldSetCorrectImagePath_WhenImageFileProvided()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterService = new CharacterService(mockCharacterRepository.Object);
            var characterToBeAdded = new Character();

            var imageFileName = "test-image.png";
            var imageFileMock = new Mock<IFormFile>();

            // Setup IFormFile mock
            imageFileMock.Setup(f => f.FileName).Returns(imageFileName);
            imageFileMock.Setup(f => f.Length).Returns(1024); // File length in bytes, adjust as needed
            imageFileMock.Setup(f => f.ContentType).Returns("image/png"); // Set valid MIME type

            // Act
            characterService.AddCharacterImage(characterToBeAdded, imageFileMock.Object);

            // Assert
            Assert.NotNull(characterToBeAdded.Image);
            Assert.Contains("/Images/Characters/", characterToBeAdded.Image);
            Assert.EndsWith(".png", characterToBeAdded.Image);

            // Verify repository method was not called (this test focuses on setting image path)
            mockCharacterRepository.Verify(repo => repo.AddCharacterDto(It.IsAny<CharacterDto>()), Times.Never);
        }

        [Fact]
        public void AddCharacterImage_ShouldSetDefaultImagePath_WhenImageFileNotProvided()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterService = new CharacterService(mockCharacterRepository.Object);
            var characterToBeAdded = new Character();
            IFormFile imageFile = null; // Simulate no image file provided

            // Act
            characterService.AddCharacterImage(characterToBeAdded, imageFile);

            // Assert
            Assert.NotNull(characterToBeAdded.Image);
            Assert.Equal("/Images/Characters/Unknown.png", characterToBeAdded.Image);

            // Verify repository method was not called (this test focuses on setting default image path)
            mockCharacterRepository.Verify(repo => repo.AddCharacterDto(It.IsAny<CharacterDto>()), Times.Never);
        }

        [Theory]
        [InlineData(".txt", "text/plain")]
        [InlineData(".exe", "application/octet-stream")]
        [InlineData(".pdf", "application/pdf")]
        public void AddCharacterImage_ShouldThrowInvalidImageExtensionException_WhenInvalidExtension(string extension, string mimeType)
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterService = new CharacterService(mockCharacterRepository.Object);
            var characterToBeAdded = new Character();
            var invalidImageFileMock = new Mock<IFormFile>();

            invalidImageFileMock.Setup(f => f.FileName).Returns($"invalid-file{extension}");
            invalidImageFileMock.Setup(f => f.ContentType).Returns(mimeType);
            invalidImageFileMock.Setup(f => f.Length).Returns(1024); // File length in bytes, adjust as needed

            // Act & Assert
            var ex = Assert.Throws<InvalidImageExtensionException>(() => characterService.AddCharacterImage(characterToBeAdded, invalidImageFileMock.Object));
            Assert.Equal("Invalid image file type.", ex.Message);
        }

        [Theory]
        [InlineData("image/jpeg", "application/octet-stream")]
        [InlineData("image/png", "application/pdf")]
        [InlineData("image/gif", "text/plain")]
        public void AddCharacterImage_ShouldThrowInvalidImageTypeException_WhenInvalidMimeType(string validMimeType, string invalidMimeType)
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterService = new CharacterService(mockCharacterRepository.Object);
            var characterToBeAdded = new Character();
            var invalidImageFileMock = new Mock<IFormFile>();

            invalidImageFileMock.Setup(f => f.FileName).Returns("test-image.jpg");
            invalidImageFileMock.Setup(f => f.ContentType).Returns(invalidMimeType);
            invalidImageFileMock.Setup(f => f.Length).Returns(1024); // File length in bytes, adjust as needed

            // Act & Assert
            var ex = Assert.Throws<InvalidImageTypeException>(() => characterService.AddCharacterImage(characterToBeAdded, invalidImageFileMock.Object));
            Assert.Equal("Invalid image MIME type.", ex.Message);
        }

        [Fact]
        public void RemoveCharacterImage_ShouldDeleteExistingImageFile()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterService = new CharacterService(mockCharacterRepository.Object);
            var character = new Character
            {
                Image = "/Images/Characters/test-image.png"
            };

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", character.Image.TrimStart('/'));

            // Create a dummy file to test deletion
            Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
            File.WriteAllText(imagePath, "dummy content");

            // Act
            characterService.RemoveCharacterImage(character);

            // Assert
            Assert.False(File.Exists(imagePath));
        }

        [Fact]
        public void RemoveCharacterImage_ShouldNotThrowException_WhenImageFileDoesNotExist()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterService = new CharacterService(mockCharacterRepository.Object);
            var character = new Character
            {
                Image = "/Images/Characters/non-existing-image.png"
            };

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", character.Image.TrimStart('/'));

            // Ensure the file does not exist
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            // Act & Assert
            var exception = Record.Exception(() => characterService.RemoveCharacterImage(character));
            Assert.Null(exception);
        }

        [Fact]
        public void RemoveCharacterImage_ShouldNotThrowException_WhenImageIsNullOrEmpty()
        {
            // Arrange
            var mockCharacterRepository = new Mock<ICharacterRepository>();
            var characterService = new CharacterService(mockCharacterRepository.Object);
            var characterWithNullImage = new Character
            {
                Image = null
            };

            var characterWithEmptyImage = new Character
            {
                Image = ""
            };

            // Act & Assert
            var exceptionForNullImage = Record.Exception(() => characterService.RemoveCharacterImage(characterWithNullImage));
            var exceptionForEmptyImage = Record.Exception(() => characterService.RemoveCharacterImage(characterWithEmptyImage));

            Assert.Null(exceptionForNullImage);
            Assert.Null(exceptionForEmptyImage);
        }
    }
}
