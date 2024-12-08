using FileStorageSys.Helper;
using FileStorageSys.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
// Lobaba Eltayeb Author 12/8/2024  (Simple Drive Project) 

namespace FileStorageSys.Controllers
{
    public class FileController : Controller
    {
        private readonly IConfiguration _configuration;
        public FileController( IConfiguration configuration)
        {     
            _configuration = configuration;
          
        }
        public IActionResult Index()
        {

            var token = Request.Cookies["AuthToken"]; 
            if (string.IsNullOrEmpty(token)) 
            {
                var newtoken = GetToken();
                var tokenString = new JwtSecurityTokenHandler().WriteToken(newtoken);
                // Store the token in a cookie
                Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }
                
            return View();
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            var base64String = await FileHelper.ConvertToBase64Async(file);

            // Send the Base64 string to blobs controller upload action
            var result = await SendToBlobsApi(base64String);
            
            return View("Index");
        }
        private async Task<string> SendToBlobsApi(string base64String)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Token not found";
                TempData["Message"] = null;
                return "Token not found";


            }

            var model = new FileRequestModel
            {
                Data = base64String,
                Id = Guid.NewGuid().ToString()
            };

         
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.PostAsync("https://localhost:7264/v1/blobs", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if(response.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    TempData["Message"] = responseString;
                    TempData["Error"] = null;
                }
                else
                {
                    TempData["Error"] = responseString;
                    TempData["Message"] = null;
                }
                return responseString;
            }
        }
        [HttpPost("GetFileData")]
        public async Task<IActionResult> GetFileData(string FileId)
        {
            if (FileId.IsNullOrEmpty())
            {
                return BadRequest("No file uploaded");
            }
            if(!FileHelper.IsGuid(FileId))
            {
                TempData["Message"] = null;
                TempData["Error"] = "Invalid File ID (GUID FORMAT)";
                return View("Index");

            }
            // Send the Base64 string to blobs controller upload action
            var result = await SendToBlobsGetDataApi(FileId);
            if (result != null)
            {


                if (result.Id == null)
                {
                    TempData["Message"] = null;
                    TempData["Error"] = "Failed to Retrieve File";

                }
                else
                {
                    TempData["Message"] = "File successfully retrieved";
                    TempData["Error"] = null;
                }
            }


            return View("Index", result);


        }
        private async Task<FileResponseModel> SendToBlobsGetDataApi(string Id)
        {

            var token = Request.Cookies["AuthToken"];
           if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Token not found";
                TempData["Message"] = null;
                return null;
            }
            var model = new FileRequestData
            {
                Id = Id,
            };
            FileResponseModel fileResponse;
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("https://localhost:7264/v1/blobs/" + Id);
                var responseString = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                     fileResponse = JsonConvert.DeserializeObject<FileResponseModel>(responseString); 
                }
                else
                {
                     fileResponse = new FileResponseModel(); 
                  
                }

                return fileResponse;
            }
        }
        private JwtSecurityToken GetToken()
        {
            var authClaims = new List<Claim>
                {

                    new Claim(ClaimTypes.NameIdentifier, "TestUser"),
                    new Claim("Name","TestUser"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddHours(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey,
                SecurityAlgorithms.HmacSha256)
                );
            return token;

        }
        public IActionResult Error() 
        {
            return View();
        }
    }
}
