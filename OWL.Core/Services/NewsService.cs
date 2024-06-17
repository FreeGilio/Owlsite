using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWL.Core.Interfaces;
using OWL.Core.Models;
using OWL.Core.DTO;
using OWL.Core.Logger;
using System.Threading.Tasks;

namespace OWL.Core.Services
{

    public class NewsService
    {
        private readonly INewsRepository _newsRepo;

        public NewsService(INewsRepository newsRepository)
        {
            this._newsRepo = newsRepository;
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
    }
}
