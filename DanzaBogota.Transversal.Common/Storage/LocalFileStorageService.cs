using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DanzaBogota.Transversal.Common.Storage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly string _baseUrl;

        public LocalFileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _basePath = Path.Combine(env.WebRootPath, "uploads");
            var request = httpContextAccessor.HttpContext!.Request;
            _baseUrl = $"{request.Scheme}://{request.Host}";
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder)
        {
            var folderPath = Path.Combine(_basePath, folder);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(stream);

            return $"{_baseUrl}/uploads/{folder}/{uniqueFileName}";
        }

        public Task<bool> DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return Task.FromResult(false);

            var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
            var folder = new Uri(fileUrl).Segments[^2].TrimEnd('/');
            var filePath = Path.Combine(_basePath, folder, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
