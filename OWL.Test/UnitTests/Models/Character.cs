using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Test.UnitTests.Models
{
    public class CharacterTests
    {
        [Fact]
        public void Set_Character_Properties()
        {
            //Arrange
            var character = new Character
            {
                Id = 1,
                Name = "John Doe",
                Description = "Test subject",
                Image = "https://example.com/johndoe.jpg",
                NewlyAdded = true
            };

            //Assert
            Assert.Equal(1, character.Id);
            Assert.Equal("John Doe",character.Name);
            Assert.Equal("Test subject", character.Description);
            Assert.Equal("https://example.com/johndoe.jpg", character.Image);
            Assert.Equal(true, character.NewlyAdded);
        }
    }
}
