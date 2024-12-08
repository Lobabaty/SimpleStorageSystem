using System.Security.Cryptography;
using System.Text;

namespace FileStorageSys.Helper
{
    public static class FileHelper
    {
        public static async Task<string> ConvertToBase64Async(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
        }
      
        public static async Task<IFormFile> ConvertFromBase64String(string base64String, string fileName = null)
        {
            var fileBytes = Convert.FromBase64String(base64String);
            var stream = new MemoryStream(fileBytes);
            fileName ??= $"file_{Guid.NewGuid()}.txt";
            // Generate a unique file name if not provided
            return new FormFile(stream, 0, stream.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }
        public static bool IsGuid(string input) 
        { 
            return Guid.TryParse(input, out _);
        }
    }
}
