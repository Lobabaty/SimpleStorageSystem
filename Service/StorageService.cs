using FileStorageSys.Data;
using Microsoft.EntityFrameworkCore;

namespace FileStorageSys.Service
{
    public class StorageService
    {
        public StorageService() { }
        private readonly RekazDbContext dbcontext;
        public StorageService(RekazDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        public IStorageType CreateStorageService(string storagType)
        {
          
            return storagType switch
            {
                "DataBase" => new DbStorageService(dbcontext),
                "LocalFile" => new LocalFileStorageService(dbcontext),
       
                _ => throw new ArgumentException("")
            };
        }
    }
}
