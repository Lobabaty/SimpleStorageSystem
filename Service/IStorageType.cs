using FileStorageSys.Models;

namespace FileStorageSys.Service
{
    public interface IStorageType
    {
        Task<Boolean> Upload(Guid id,string file);
        Task<FileDataViewModel> Download(Guid id);
    }
}
