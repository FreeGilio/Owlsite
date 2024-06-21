using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Interfaces
{
    public interface INewsRepository
    {
        NewsDto GetNewsDtoById(int newsId);

        bool CheckTitleExists(NewsDto news, int? currentNewsId = null);

        void AddNewsDto(NewsDto newsToAdd);

        void UpdateNewsDto(NewsDto newsToUpdate);
        void DeleteNews(NewsDto news);
        List<NewsDto> GetAllNewsWithCategories();
    }
}
