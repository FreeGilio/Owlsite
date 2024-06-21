using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OWL.Core.CustomExceptions;
using OWL.Core.Models;
using OWL.Core.Services;

namespace OWL.MVC.Controllers
{

    public class NewsController : Controller
    {

        private readonly NewsService newsService;
        private readonly CategoryService categoryService;

        public NewsController(NewsService newsService, CategoryService categoryService) 
        {
            this.newsService = newsService;
            this.categoryService = categoryService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult NewsInfo(int id)
        {
            News newsModel = newsService.GetNewsById(id);
            return View(newsModel);
        }

        [HttpGet]
        public IActionResult AddArticle()
        {
            var categories = categoryService.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(new News());
        }

        [HttpPost]
        public IActionResult AddArticle(News newsToBeAdded, int selectedCategoryId, IFormFile imageFile)
        {
            try
            {
                newsService.AddArticleImage(newsToBeAdded, imageFile);
                newsToBeAdded.Category = categoryService.GetCategoryById(selectedCategoryId);
                newsService.AddNews(newsToBeAdded);
                return RedirectToAction("Index", "Home");
            }
         
            catch (NameExistsException ex)
            {
                ModelState.AddModelError("Title", ex.Message);
            }

            var categories = categoryService.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(newsToBeAdded);
        }

        [HttpGet]
        public IActionResult EditArticle(int id)
        {
            var newsModel = newsService.GetNewsById(id);
            ViewBag.Categories = new SelectList(categoryService.GetAllCategories(), "Id", "Name", newsModel.Category.Id);

            return View(newsModel);
        }

        [HttpPost]
        public IActionResult EditArticle(News newsToBeUpdated, int selectedCategoryId, IFormFile imageFile)
        {
            try
            {
                // Retrieve current news details to get the old image path
                var currentNews = newsService.GetNewsById(newsToBeUpdated.Id);

                if (imageFile != null && imageFile.Length > 0)
                {
                    newsService.RemoveArticleImage(currentNews);
                    newsService.AddArticleImage(newsToBeUpdated, imageFile);
                }
                else
                {
                    // Keep the current image if no new image is uploaded
                    newsToBeUpdated.Image = currentNews.Image;
                }

                newsToBeUpdated.Category = categoryService.GetCategoryById(selectedCategoryId);
                newsService.UpdateNews(newsToBeUpdated);
                return RedirectToAction("Index", "Home");
            }           
            catch (NameExistsException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
            }

            var categories = categoryService.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(newsToBeUpdated);
        }

        [HttpPost]
        public IActionResult DeleteArticle(int id)
        {
            // Retrieve news details
            var news = newsService.GetNewsById(id);
            if (news == null)
            {
                return NotFound();
            }

            newsService.RemoveArticleImage(news);

            // Delete news from the database
            newsService.DeleteNews(news);

            return RedirectToAction("Index", "Home");
        }
    }
}
