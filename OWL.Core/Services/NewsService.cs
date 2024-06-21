using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWL.Core.Interfaces;
using OWL.Core.Models;
using OWL.Core.DTO;
using OWL.Core.Logger;
using System.Threading.Tasks;
using OWL.Core.CustomExceptions;
using Microsoft.AspNetCore.Http;

namespace OWL.Core.Services
{

    public class NewsService
    {
        private readonly INewsRepository _newsRepo;

        public NewsService(INewsRepository newsRepository)
        {
            this._newsRepo = newsRepository;
        }

        public News GetNewsById(int? newsId)
        {
            if (!newsId.HasValue)
            {
                throw new IdNotFoundException("News ID has not been provided.");
            }

            NewsDto newsDto = _newsRepo.GetNewsDtoById(newsId.Value);
            if (newsDto == null)
            {
                throw new IdNotFoundException(newsId.Value);
            }

            return new News(newsDto);
        }

        public List<News> GetAllNewsWithCategories()
        {
            try
            {
                
                List<NewsDto> newsDtos = _newsRepo.GetAllNewsWithCategories();             
                return News.MapToNews(newsDtos);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError("Error getting all news with categories", ex);
                throw; // Optionally rethrow the exception or return an empty list
            }
        }

        public void AddArticleImage(News news, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Validate file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    throw new InvalidImageExtensionException("Invalid image file type.");
                }

                // Validate MIME type
                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                if (!allowedMimeTypes.Contains(imageFile.ContentType))
                {
                    throw new InvalidImageTypeException("Invalid image MIME type.");
                }

                // Generate a unique file name
                var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";

                // Set the path to store the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/News", uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                // Save the path in the model
                news.Image = $"/Images/News/{uniqueFileName}";
            }
            else
            {
                news.Image = $"/Images/News/Question.png";
            }
        }

        public void RemoveArticleImage(News news)
        {
            // Delete associated image file if it exists
            if (!string.IsNullOrEmpty(news.Image))
            {
                if (news.Image != "/Images/News/Question.png")
                {
                    try
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", news.Image.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    catch (ImageNotDeletedException ex)
                    {
                        Console.WriteLine($"Error deleting image file: {ex.Message}");
                    }
                }

            }
        }

        public void AddNews(News newsToAdd)
        {

            if (string.IsNullOrEmpty(newsToAdd.Title))
            {
                throw new NameRequiredException("Article title is required.");
            }

            if (_newsRepo.CheckTitleExists(new NewsDto
            {
                Title = newsToAdd.Title,
                Id = newsToAdd.Id
            }))
            {
                throw new NameExistsException("An article with this title already exists.", newsToAdd.Title);
            }

            NewsDto newsDto = new NewsDto(newsToAdd);
            _newsRepo.AddNewsDto(newsDto);
        }

        public void UpdateNews(News newsToUpdate)
        {

            if (string.IsNullOrEmpty(newsToUpdate.Title))
            {
                throw new NameRequiredException("Article title is required.");
            }

            if (newsToUpdate.Date == null || newsToUpdate.Date == default)
            {
                throw new DateRequiredException();
            }

            if (_newsRepo.CheckTitleExists(new NewsDto
            {
                Title = newsToUpdate.Title,
                Id = newsToUpdate.Id
            }, newsToUpdate.Id))
            {
                throw new NameExistsException("An article with this title already exists.", newsToUpdate.Title);
            }

            NewsDto newsDto = new NewsDto(newsToUpdate);
            _newsRepo.UpdateNewsDto(newsDto);

        }
        public void DeleteNews(News newsToRemove)
        {
            try
            {
                NewsDto newsDto = new NewsDto(newsToRemove) { };
                _newsRepo.DeleteNews(newsDto);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError($"Error deleting article {newsToRemove.Title}", ex);
                throw; // Optionally rethrow the exception
            }
        }
    }
}
