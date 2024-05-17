using Bookify.Web.Helper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace Bookify.Web.Services
{
    public class DocumentSettings:IDocumentSettings
    {
        public string UploadFileToLocalServer(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderPath)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine($"{webHostEnvironment.WebRootPath}{folderPath}", fileName);
            using var stream = File.Create(filePath);
            file.CopyToAsync(stream);
            stream.Dispose();
            return fileName;
        }
        public void DeleteFileFromLocalServer(string FileName, IWebHostEnvironment webHostEnvironment)
        {
            var OldImagePath = $"{webHostEnvironment.WebRootPath}{FileName}";
            if (File.Exists(OldImagePath))
                File.Delete(OldImagePath);
        }
        public void UploadFileThumbToLocalServer(IFormFile file, string fileName, IWebHostEnvironment webHostEnvironment, string folderPath)
        {
            var filePath = Path.Combine($"{webHostEnvironment.WebRootPath}{folderPath}", fileName);
            // Use Package ImageSharp.Web
            using var image = Image.Load(file.OpenReadStream());
            var ratio = (float)image.Width / 200;
            var height = image.Height / ratio;
            image.Mutate(image => image.Resize(width: 200, height: (int)height));
            image.Save(filePath);
        }
        public void DeleteFileThumbFromLocalServer(string FileName, IWebHostEnvironment webHostEnvironment)
        {
            var OldImageThumbPath = $"{webHostEnvironment.WebRootPath}{FileName}";
            if (File.Exists(OldImageThumbPath))
                File.Delete(OldImageThumbPath);
        }
        public async Task<CustomCloudinaryResults> UploadFileToCloudinary(IFormFile file, Cloudinary cloudinary)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            using var stream = file.OpenReadStream();
            var imageParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                UseFilename = true
            };
            var result = await cloudinary.UploadAsync(imageParams);
            var CoudinaryResult = new CustomCloudinaryResults()
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId
            };
            return CoudinaryResult;
        }
        public string GetThumbnailUrlFromCloudinary(string url)
        {
            var separator = "image/upload/";
            var urlParts = url.Split(separator);
            var thumbnailUrl = $"{urlParts[0]}{separator}c_thumb,w_200,g_face/{urlParts[1]}";
            return thumbnailUrl;
        }
    }
}
