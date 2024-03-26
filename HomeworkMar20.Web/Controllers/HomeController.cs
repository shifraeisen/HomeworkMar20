using HomeworkMar20.Data;
using HomeworkMar20.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace HomeworkMar20.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var repo = new ImageRepository(_connectionString);
            return View(new IndexViewModel
            {
                Images = repo.GetAllImages().OrderByDescending(i => i.UploadDate).ToList()
            });
        }
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Submit(IFormFile image, string title)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
            image.CopyTo(fs);

            var repo = new ImageRepository(_connectionString);
            repo.Add(new Image
            {
                Title = title,
                ImagePath = fileName
            });

            return Redirect("/");
        }
        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            var image = repo.GetImageById(id);
            var likedIDs = HttpContext.Session.Get<List<int>>("Liked IDs") ?? new List<int>();
            if (image == null)
            {
                return Redirect("/");
            }
            return View(new ViewImageViewModel
            {
                Image = image,
                Liked = likedIDs.Contains(image.Id)
            });
        }
        [HttpPost]
        public int AddLike(int id)
        {
            var ids = HttpContext.Session.Get<List<int>>("Liked IDs") ?? new List<int>();
            if (ids.Contains(id))
            {
                return -1;
            }
            var repo = new ImageRepository(_connectionString);
            ids.Add(id);
            HttpContext.Session.Set("Liked IDs", ids);
            return repo.AddLike(id);
        }
        public int GetLikesForImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            return repo.GetImageById(id).Likes;
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}