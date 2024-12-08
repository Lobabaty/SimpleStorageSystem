using FileStorageSys.Data;
using FileStorageSys.Entities;
using FileStorageSys.Helper;
using FileStorageSys.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageSys.Service
{
    public class LocalFileStorageService : IStorageType
    {
        private readonly RekazDbContext dbcontext;

        public LocalFileStorageService(RekazDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public async Task<Boolean> Upload(Guid idValue, string data)
        {
            var file = await FileHelper.ConvertFromBase64String(data);

            var fileIfExsit = await dbcontext.FilesLocalFile.FindAsync(idValue);
            if (fileIfExsit != null)
                return false;

            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + (Guid.NewGuid().ToString());
            var filePath = Path.Combine(basePath, file.FileName);
            var extension = Path.GetExtension(file.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var fileModel = new FileLocalFile
                {
                    Id = idValue,
                    Created_at = DateTime.UtcNow,
                    FilePath = filePath
                };

                var fileMetaData = new FileMetaData
                {
                    FileId = idValue,
                    FileType = file.ContentType,
                    Extension = extension,
                    Name = fileName,
                    Description = "Description" + fileName,

                };

                var result = dbcontext.FilesLocalFile.Add(fileModel);
                dbcontext.FilesMetaData.Add(fileMetaData);

                dbcontext.SaveChanges();
                return true;
            }
            else
                return false;


        }

        public async Task<FileDataViewModel> Download(Guid id)
        {
            var file = await dbcontext.FilesLocalFile.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null)
                return null;
            var memory = new MemoryStream();
            using (var stream = new FileStream(file.FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var fileMetaData = await dbcontext.FilesMetaData.Where(x => x.FileId == id).FirstOrDefaultAsync();
            return new FileDataViewModel(file.Id.ToString(), memory.ToArray(), fileMetaData.FileType, fileMetaData.Name + fileMetaData.Extension, file.Created_at);
        }
    }
}
