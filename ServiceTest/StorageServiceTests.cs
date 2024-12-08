using FileStorageSys.Service;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageSys.ServiceTest
{
    public class StorageServiceTests
    {

        private readonly StorageService RekazStorageService;
       
        [Fact]
        public async void UploadTest()
        {

            // Arrange var
            var storageService = new StorageService();
            string data = "SGVsbG8gaW4gTG9iYWJhIFNpbXBsZSBEcml2ZSBQcm9qZWN0Lg==";
            Guid id = new Guid("365bfd62-a304-49aa-97fa-4e6210fc3118");

            //Act
            var ss = storageService.CreateStorageService("LocalFile");
            var result = await ss.Upload(id, data);
            // Assert
            Assert.True(result);
        }
        public async void DownloadTest()
        {
            // Arrange var
            var storageService = new StorageService();
            string fileId = "365bfd62-a304-49aa-97fa-4e6210fc3118";
            Guid id = new Guid(fileId);

            //Act
            var ss = storageService.CreateStorageService("LocalFile");
            var result = await ss.Download(id);
            // Assert

            Assert.Equal(fileId, result.Id);
        }
        
    }
}