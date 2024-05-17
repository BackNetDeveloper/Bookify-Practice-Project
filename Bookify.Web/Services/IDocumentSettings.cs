using Bookify.Web.Helper;
using CloudinaryDotNet;

namespace Bookify.Web.Services
{
    public interface IDocumentSettings
    {
        string UploadFileToLocalServer(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderPath);
        void UploadFileThumbToLocalServer(IFormFile file, string fileName, IWebHostEnvironment webHostEnvironment, string folderPath);
        void DeleteFileFromLocalServer(string FileName, IWebHostEnvironment webHostEnvironment);
        void DeleteFileThumbFromLocalServer(string FileName, IWebHostEnvironment webHostEnvironment);
        Task<CustomCloudinaryResults> UploadFileToCloudinary(IFormFile file, Cloudinary cloudinary);
        string GetThumbnailUrlFromCloudinary(string url);
    }
}
