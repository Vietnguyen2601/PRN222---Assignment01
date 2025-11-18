using Microsoft.AspNetCore.Hosting;

namespace EleVehicleDealer.Presentation.Helpers
{
    public static class ImageHelper
    {
        private static List<string>? _imageFiles;
        private static readonly object _lock = new object();

        public static List<string> GetAllImages(IWebHostEnvironment webHostEnvironment)
        {
            if (_imageFiles == null)
            {
                lock (_lock)
                {
                    if (_imageFiles == null)
                    {
                        var imagesPath = Path.Combine(webHostEnvironment.WebRootPath, "images");
                        if (Directory.Exists(imagesPath))
                        {
                            _imageFiles = Directory.GetFiles(imagesPath, "*.jpg")
                                .Select(Path.GetFileName)
                                .Where(f => f != null)
                                .Select(f => f!)
                                .ToList();
                        }
                        else
                        {
                            _imageFiles = new List<string>();
                        }
                    }
                }
            }
            return _imageFiles;
        }

        public static string GetRandomImage(IWebHostEnvironment webHostEnvironment)
        {
            var images = GetAllImages(webHostEnvironment);
            if (images.Count == 0)
            {
                return "default.jpg";
            }
            var random = new Random();
            return images[random.Next(images.Count)];
        }
    }
}
