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
        public void AddCharacter_ShouldCallAddCharacterDto_WhenCalled()
        {
            var mockCharacterRepository = new Mock<ICharacterRepository>();

            var characterService = new CharacterService(mockCharacterRepository.Object);

            characterService.AddCharacter(new Character());

            mockCharacterRepository.Verify(repo => repo.AddCharacterDto(It.IsAny<CharacterDto>()), Times.Once);
        }
    }
}
