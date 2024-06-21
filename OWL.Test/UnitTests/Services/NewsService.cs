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
    public class NewsServiceTests
    {

        [Fact]
        public void GetNewsById_ExistingId_ReturnsNews()
        {
            // Arrange
            int newsId = 1;
            var mockRepository = new Mock<INewsRepository>();
            mockRepository.Setup(repo => repo.GetNewsDtoById(newsId))
                          .Returns(new NewsDto { Id = newsId, Title = "Test News" });

            var newsService = new NewsService(mockRepository.Object);

            // Act
            var result = newsService.GetNewsById(newsId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newsId, result.Id);
        }

        [Fact]
        public void AddNews_ValidNews_AddsSuccessfully()
        {
            // Arrange
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var newsToAdd = new News { Title = "New Article", Date = currentDate };
            var mockRepository = new Mock<INewsRepository>();

            // Setup the CheckTitleExists method without using It.IsAny for optional arguments
            mockRepository.Setup(repo => repo.CheckTitleExists(It.IsAny<NewsDto>(), default(int)))
                          .Returns(false);

            var newsService = new NewsService(mockRepository.Object);

            // Act
            newsService.AddNews(newsToAdd);

            // Assert
            mockRepository.Verify(repo => repo.AddNewsDto(It.IsAny<NewsDto>()), Times.Once);
        }

        [Fact]
        public void UpdateNews_ValidNews_UpdatesSuccessfully()
        {
            // Arrange
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var newsToUpdate = new News { Id = 1, Title = "Updated Article", Date = currentDate };
            var mockRepository = new Mock<INewsRepository>();
            mockRepository.Setup(repo => repo.CheckTitleExists(It.IsAny<NewsDto>(), newsToUpdate.Id))
                          .Returns(false);
            var newsService = new NewsService(mockRepository.Object);

            // Act
            newsService.UpdateNews(newsToUpdate);

            // Assert
            mockRepository.Verify(repo => repo.UpdateNewsDto(It.IsAny<NewsDto>()), Times.Once);
        }

        [Fact]
        public void DeleteNews_ValidNews_DeletesSuccessfully()
        {
            // Arrange
            var newsToDelete = new News { Id = 1, Title = "Article to delete", Date = DateOnly.FromDateTime(DateTime.Today) };
            var mockRepository = new Mock<INewsRepository>();
            var newsService = new NewsService(mockRepository.Object);

            // Act
            newsService.DeleteNews(newsToDelete);

            // Assert
            mockRepository.Verify(repo => repo.DeleteNews(It.IsAny<NewsDto>()), Times.Once);
        }

        [Fact]
        public void AddNews_TitleExists_ThrowsNameExistsException()
        {
            // Arrange
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var newsToAdd = new News { Title = "Existing Article", Date = currentDate };
            var mockRepository = new Mock<INewsRepository>();

            // Setup the CheckTitleExists method with explicit parameters including any optional ones
            mockRepository.Setup(repo => repo.CheckTitleExists(It.IsAny<NewsDto>(), default))
                          .Returns(true); // Change to return true for existing title

            var newsService = new NewsService(mockRepository.Object);

            // Act and Assert
            var ex = Assert.Throws<NameExistsException>(() => newsService.AddNews(newsToAdd));
            Assert.Equal("An article with this title already exists.", ex.Message);
        }

        [Fact]
        public void UpdateNews_TitleExists_ThrowsNameExistsException()
        {
            // Arrange
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var newsToUpdate = new News { Id = 1, Title = "Existing Article", Date = currentDate };
            var mockRepository = new Mock<INewsRepository>();
            mockRepository.Setup(repo => repo.CheckTitleExists(It.IsAny<NewsDto>(), newsToUpdate.Id))
                          .Returns(true);
            var newsService = new NewsService(mockRepository.Object);

            // Act and Assert
            var ex = Assert.Throws<NameExistsException>(() => newsService.UpdateNews(newsToUpdate));
            Assert.Equal("An article with this title already exists.", ex.Message);
        }


        [Fact]
        public void UpdateNews_DateRequiredException()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var newsService = new NewsService(mockRepository.Object);

            var newsToUpdate = new News
            {
                Id = 1,
                Title = "Test Article",
                Date = default(DateOnly), // Set DateOnly to default value (01/01/0001)
                // Other required properties
            };

            // Act and Assert
            var ex = Assert.Throws<DateRequiredException>(() => newsService.UpdateNews(newsToUpdate));
            Assert.Equal("Date is required for the article.", ex.Message);
        }
    }
}
