using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OWL.Core.DTO
{
    public class NewsDto
    {
        public int Id { get;  set; }

        public string Title { get; set; }

        public string Description { get;  set; }

        public string Image { get;  set; }

        public DateOnly Date {  get; set; }

        public Category Category { get; set; }

        public NewsDto() { }

        public NewsDto(News news)
        {
            Id = news.Id;
            Title = news.Title;
            Description = news.Description;
            Image = news.Image;
            Date = news.Date;
            Category = news.Category;
        }
    }
}
