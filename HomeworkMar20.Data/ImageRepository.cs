using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkMar20.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;
        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Image> GetAllImages()
        {
            using var context = new ImagesDataContext(_connectionString);
            return context.Images.ToList();
        }
        public void Add(Image image)
        {
            using var context = new ImagesDataContext(_connectionString);
            image.UploadDate = DateTime.Now;
            context.Images.Add(image);
            context.SaveChanges();
        }
        public Image GetImageById(int id)
        {
            using var context = new ImagesDataContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }
        public int AddLike(int id)
        {
            using var context = new ImagesDataContext(_connectionString);
            var image = GetImageById(id);
            image.Likes++;
            context.Entry(image).State = EntityState.Modified;
            context.SaveChanges();
            return image.Likes;
        }
    }
}
