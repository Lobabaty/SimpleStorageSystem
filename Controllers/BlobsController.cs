using FileStorageSys.Data;
using FileStorageSys.Helper;
using FileStorageSys.Models;
using FileStorageSys.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections;

// Lobaba Eltayeb Author 12/8/2024  (Simple Drive Project) 

namespace FileStorageSys.Controllers
{

    [Route("/v1/blobs")]
    [ApiController]
    [Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    public class BlobsController : Controller
    {
        private readonly StorageService RekazStorageService ;
        private string StorageServiceType = "LocalFile";//DataBase Or //LocalFile ;
        public BlobsController(StorageService storageSer)
        {
            this.RekazStorageService = storageSer;
        }
        /// <summary>
        /// Downloads a file by its unique identifier in GUID format
        /// </summary>
        /// <param name="Id">The unique identifier (GUID) of the file to be downloaded</param>
        /// <returns>An IActionResult that contains the file details </returns>
        [HttpGet("{Id}")]
        public async Task<IActionResult> download(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                {
                    return BadRequest("Invalid File ID");
                }
                if (!FileHelper.IsGuid(Id))
                {
                    return BadRequest("Invalid File ID (GUID FORMAT)");

                }
                //Determine Storage Type here 
                var ser = RekazStorageService.CreateStorageService(StorageServiceType);
              
                var Guid = new Guid(Id);
                var result = await ser.Download(Guid);
                var response = new FileResponseModel();
                if (result != null)
                {
                    response = new FileResponseModel
                    {
                        Id = result.Id,
                        Data = Convert.ToBase64String(result.FileData),
                        Created_at = result.Created_at,
                        Size = result.FileData.Length,

                    };
                    return Ok(response);

                }
                else
                    return BadRequest("File Not Found");

            }
            catch(Exception e)
            {
                return BadRequest("File Download Failed");

            }
        }
        /// <summary>
        /// Uploads a file using the provided GUID and Base64 encoded binary data
        /// </summary>
        /// <param name="fileModel">The model containing the file data in Base64 encoded format and the unique identifier (GUID) of the file</param>
        /// <returns>An IActionResult that indicates the result of the upload operation, including a message with the file ID if successful</returns>
        [HttpPost]
        public async Task<IActionResult> upload(FileRequestModel fileModel)
        {
            if (fileModel == null || string.IsNullOrEmpty(fileModel.Id) || string.IsNullOrEmpty(fileModel.Data))
            { 
                return BadRequest("Invalid file data."); 
            }
            if (!FileHelper.IsGuid(fileModel.Id))
            {
                return BadRequest("Invalid File ID (GUID FORMAT)");

            }
            try
            {
                //Determine Storage Type here 
                var ser = RekazStorageService.CreateStorageService(StorageServiceType);

                Guid idValue = new Guid(fileModel.Id);
                var result =await ser.Upload(idValue, fileModel.Data);
                if (result)
                    return Ok("File successfully uploaded with Id: " + idValue);
                else
                    return NotFound("File Upload Failed");

            }
            catch (Exception ex)
            {

                return BadRequest("File Upload Failed");
            }
        }
      

    }
}
