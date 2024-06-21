using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Models
{
    public class News
    {
        public int Id { get;  set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get;  set; }

        public DateOnly Date { get; set; }

        public Category Category { get; set; }

        public News(int id, string title, string description, string image, DateOnly date, Category category) 
        { 
            Id = id;
            Title = title;
            Description = description;
            Image = image;
            Date = date;
            Category = category;
        }

        public News(NewsDto newsDto)
        {
            Id = newsDto.Id;
            Title = newsDto.Title;
            Description = newsDto.Description;
            Image = newsDto.Image;
            Date = newsDto.Date;
            Category = newsDto.Category;
        }

        public News() { }

        public static List<News> MapToNews(List<NewsDto> newsDtos)
        {

            List<News> news = new List<News>();

            try
            {
                foreach (NewsDto newsDto in newsDtos)
                {
                    news.Add(new News(newsDto));
                }
            }
            catch (Exception ex)
            {

            }


            return news;
        }
    }
}
