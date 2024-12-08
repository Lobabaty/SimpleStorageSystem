using FileStorageSys.Data;
using FileStorageSys.Entities;
using FileStorageSys.Helper;
using FileStorageSys.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageSys.Service
{
    public class DbStorageService : IStorageType
    {
        private readonly RekazDbContext dbcontext;

        public DbStorageService(RekazDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public async Task<Boolean> Upload(Guid idValue, string data)
        {
            var file = await FileHelper.ConvertFromBase64String(data);

            var fileIfExsit = await dbcontext.FilesDB.FindAsync(idValue);
            if (fileIfExsit != null)
                return false;

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);

            var fileModel = new FileDB
            {
                Id = idValue,
            };
            using (var dataStream = new MemoryStream())
            {
                await file.CopyToAsync(dataStream);
                fileModel.data = dataStream.ToArray();
            }


            var fileMetaData = new FileMetaData
            {
                FileId = idValue,
                FileType = file.ContentType,
                Extension = extension,
                Name = fileName,
                Description = "Description" + fileName,

            };
            dbcontext.FilesDB.Add(fileModel);
            dbcontext.FilesMetaData.Add(fileMetaData);

            dbcontext.SaveChanges();

            return true;
        }


        public async Task<FileDataViewModel> Download(Guid id)
        {
            var file = await dbcontext.FilesDB.Where(x => x.Id == id).FirstOrDefaultAsync();
            var fileMetaData = await dbcontext.FilesMetaData.Where(x => x.FileId == id).FirstOrDefaultAsync();

            if (file == null)
                return null;
            return new FileDataViewModel(file.Id.ToString(), file.data, fileMetaData.FileType, fileMetaData.Name + fileMetaData.Extension, file.Created_at);
        }

    }
}
