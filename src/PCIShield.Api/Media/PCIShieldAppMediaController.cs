using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using BlazorMauiShared.Models.Merchant;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;

using PCIShield.Api.Auth;

using PCIShield.Domain.Entities;
using PCIShield.Domain.Specifications;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using Serilog.Parsing;
using NRedisStack.Search;
using PCIShieldCore.BlazorMauiShared.CustomModelsDto;

namespace PCIShield.Api.Media;

[ApiController]
public class PCIShieldAppMediaController : ControllerBase
{
    private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<ComplianceOfficer> _complianceOfficerDbRepository;
    private readonly IConfiguration _configuration;
    private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<Merchant> _merchantDbRepository;
    private readonly IMailKitEmailServicePlus _emailService;
    private readonly IGridFsBlobStorageService _gridFsBlobStorageService;
    private readonly IAppLoggerService<PCIShieldAppMediaController> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<ApplicationUser> _userDbRepository;
    private readonly UserManager<CustomPCIShieldUser> _userManager;

    public PCIShieldAppMediaController(
        UserManager<CustomPCIShieldUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IAppLoggerService<PCIShieldAppMediaController> logger,
        IMailKitEmailServicePlus emailService,
        PCIShieldLib.SharedKernel.Interfaces.IRepository<ApplicationUser> userDbRepository,
        PCIShieldLib.SharedKernel.Interfaces.IRepository<Merchant> merchantDbRepository,
        PCIShieldLib.SharedKernel.Interfaces.IRepository<ComplianceOfficer> complianceOfficerDbRepository,
        IGridFsBlobStorageService gridFsBlobStorageService
    )
    {
        _emailService = emailService;
        _merchantDbRepository = merchantDbRepository;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _roleManager = roleManager;
        _userDbRepository = userDbRepository;
        _complianceOfficerDbRepository = complianceOfficerDbRepository;
        _gridFsBlobStorageService = gridFsBlobStorageService;
    }

    [HttpGet("api/files/stream/{FileName}")]
    public async Task<IActionResult> StreamFile([FromRoute] string FileName)
    {
        _logger.LogInformation($"Attempting to stream file: {FileName}");
        GridFSFileInfo? fileInfo = await _gridFsBlobStorageService.GetFileInfoAsync(FileName);
        if (fileInfo == null)
        {
            _logger.LogWarning($"File not found: {FileName}");
            return NotFound($"File with name {FileName} not found.");
        }

        Stream? fileStream = await _gridFsBlobStorageService.GetBlobAsync(fileInfo.Filename);
        if (fileStream == null)
        {
            _logger.LogWarning($"Failed to obtain stream for file: {FileName}");
            return NotFound($"File with name {FileName} not found.");
        }

        string contentType = fileInfo.Metadata["ContentType"].AsString;
        _logger.LogInformation($"Streaming file {FileName} with content type {contentType}");
        return File(fileStream, contentType);
    }

    [HttpGet("api/videos/stream/{videoFileName}")]
    public async Task<IActionResult> StreamVideo([FromRoute] string videoFileName)
    {
        if (string.IsNullOrWhiteSpace(videoFileName))
        {
            return BadRequest("Video file name is required.");
        }
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        string finalUrl = $"{baseUrl}/api/videos/stream/{videoFileName}";
        _logger.LogInformation($"Final URL: {finalUrl}");

        GridFSFileInfo fileInfo = await _gridFsBlobStorageService.GetFileInfoAsync(videoFileName);
        if (fileInfo == null)
        {
            _logger.LogWarning($"File not found: {videoFileName}");
            return NotFound($"Video with name {videoFileName} not found.");
        }

        string contentType = fileInfo.Metadata["ContentType"].AsString;
        long fileSize = fileInfo.Length;

        var range = Request.Headers["Range"].ToString();
        long start = 0, end = fileSize - 1;

        if (!string.IsNullOrEmpty(range))
        {
            var match = System.Text.RegularExpressions.Regex.Match(range, @"bytes=(\d+)-(\d*)");
            if (match.Success)
            {
                start = long.Parse(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                    end = Math.Min(long.Parse(match.Groups[2].Value), fileSize - 1);
            }
        }

        long length = end - start + 1;
        Response.StatusCode = 206;
        Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileSize}");
        Response.Headers.Add("Accept-Ranges", "bytes");
        Response.ContentType = contentType;
        Response.ContentLength = length;

        using (var stream = await _gridFsBlobStorageService.GetBlobAsync(videoFileName))
        {
            if (start > 0)
            {
                await stream.ReadAsync(new byte[start], 0, (int)start);
            }

            var buffer = new byte[64 * 1024];
            long totalBytesRead = 0;

            while (totalBytesRead < length)
            {
                int bytesToRead = (int)Math.Min(buffer.Length, length - totalBytesRead);
                int bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead);
                if (bytesRead == 0) break;

                await Response.Body.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
            }
        }

        return new EmptyResult();
    }

    [HttpPost("upload_file_from_maui")]
    public async Task<UploadFileResponse> UploadFileFromMaui([FromForm] UploadFileFromMauiRequest model)
    {
        UploadFileResponse? response = new();
        if (model == null || model.FileStream == null || string.IsNullOrWhiteSpace(model.MediaFileName))
        {
            _logger.LogError("Invalid UploadFile request.");
            response.ErrorMessage = "Invalid UploadFile request ";
            response.IsSuccess = false;
            return response;
        }

        try
        {
            string? fileFileName = model.MediaFileName;
            Stream? fileStream = model.FileStream.OpenReadStream();
            string? con = model.FileStream.ContentType;
            string? fileContentType = string.IsNullOrWhiteSpace(model.FileStream.ContentType)
                ? GetContentType(fileFileName)
                : model.FileStream.ContentType;
            ObjectId objectId =
                await _gridFsBlobStorageService.SaveBlobAsync(fileFileName, fileStream, fileContentType);
            if (objectId != null)
            {
                response.ObjectIdStr = objectId.ToString()!;

                response.IsSuccess = true;

                return response;
            }
            else
            {
                response.ErrorMessage = "Error uploading the file. ";
                response.IsSuccess = false;
                return response;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file: {ex.Message}");
            response.ErrorMessage = "Error uploading the file. ";
            response.IsSuccess = false;
            return response;
        }
    }

    [HttpPost("upload_file_from_maui_in_chunks")]
    public async Task<UploadFileResponse> UploadFileFromMaui([FromForm] IFormFile FileStream, [FromForm] string MediaFileName, [FromForm] int ChunkIndex, [FromForm] int TotalChunks)
    {
        UploadFileResponse response = new UploadFileResponse();
        response.IsVerified = false;
        if (FileStream == null || string.IsNullOrWhiteSpace(MediaFileName))
        {
            _logger.LogError("Invalid UploadFile request.");
            response.ErrorMessage = "Invalid UploadFile request";
            response.IsSuccess = false;
            return response;
        }

        try
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), MediaFileName + ".part" + ChunkIndex);
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await FileStream.CopyToAsync(stream);
            }
            if (ChunkIndex == TotalChunks - 1)
            {
                string finalFilePath = Path.Combine(Path.GetTempPath(), MediaFileName);
                using (var finalStream = new FileStream(finalFilePath, FileMode.Create))
                {
                    for (int i = 0; i < TotalChunks; i++)
                    {
                        string partPath = Path.Combine(Path.GetTempPath(), MediaFileName + ".part" + i);
                        using (var partStream = new FileStream(partPath, FileMode.Open))
                        {
                            await partStream.CopyToAsync(finalStream);
                        }
                        System.IO.File.Delete(partPath);
                    }
                }
                using (var fileStream = new FileStream(finalFilePath, FileMode.Open))
                {
                    ObjectId objectId = await _gridFsBlobStorageService.SaveBlobAsync(MediaFileName, fileStream, FileStream.ContentType);
                    if (objectId != null)
                    {
                        response.ObjectIdStr = objectId.ToString();
                        response.IsSuccess = true;
                        bool isVerified = await VerifyUploadedFile(objectId, finalFilePath);
                        response.IsVerified = isVerified;

                        if (!isVerified)
                        {
                            response.ErrorMessage = "File verification failed.";
                            response.IsSuccess = false;
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "Error uploading the file.";
                        response.IsSuccess = false;
                    }
                    System.IO.File.Delete(finalFilePath);
                }
            }
            else
            {
                response.IsSuccess = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file: {ex.Message}");
            response.ErrorMessage = "Error uploading the file.";
            response.IsSuccess = false;
        }

        return response;
    }

    private async Task<bool> VerifyUploadedFile(ObjectId objectId, string localFilePath)
    {
        try
        {
            var stream = await _gridFsBlobStorageService.GetBlobByIdAsync(objectId);
            if (stream == null)
            {
                return false;
            }
            using (var localFileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
            {
                return localFileStream.Length == stream.Length && await CompareStreams(localFileStream, stream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error verifying file: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> CompareStreams(Stream stream1, Stream stream2)
    {
        const int bufferSize = 8192;
        byte[] buffer1 = new byte[bufferSize];
        byte[] buffer2 = new byte[bufferSize];

        while (true)
        {
            int count1 = await stream1.ReadAsync(buffer1, 0, bufferSize);
            int count2 = await stream2.ReadAsync(buffer2, 0, bufferSize);

            if (count1 != count2)
            {
                return false;
            }

            if (count1 == 0)
            {
                return true;
            }

            if (!buffer1.SequenceEqual(buffer2))
            {
                return false;
            }
        }
    }

    [HttpPost("api/videos/stream/upload_video_from_api")]
    public async Task<IActionResult> UploadVideoFromApi([FromBody] UploadVideoFromAPiRequest? model)
    {
        if (ModelState.IsValid && model != null)
        {
            try
            {

                string? videoFileName = model.videoFileName;
                string contentRootPath = Directory.GetCurrentDirectory();
                string videoFolderPath = Path.Combine(contentRootPath, "videos\\touploadonly");
                string videoFilePath = Path.Combine(videoFolderPath, videoFileName);
                using (FileStream? videoStream = new(videoFilePath, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        await _gridFsBlobStorageService.SaveBlobAsync(videoFileName, videoStream, "video/mp4");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Invalid UploadVideo request. {e.Message}");
                        return BadRequest("Invalid UploadVideo request");
                    }
                }

                return Ok(new { });
            }
            catch (Exception a)
            {
                _logger.LogError($"Error uploading the video file. {a.Message}");
                return BadRequest($"Error uploading the video file. {a.Message}");
            }
        }
        else
        {
            _logger.LogError("Invalid UploadVideo request.");
            return BadRequest("Invalid UploadVideo request");
        }
    }
    [HttpDelete("api/media/clear-all")]
    public async Task<IActionResult> ClearAllMedia()
    {
        try
        {
            var files = await _gridFsBlobStorageService.GetAllFileInfosAsync();

            foreach (var file in files)
            {
                await _gridFsBlobStorageService.DeleteBlobByIdAsync(file.Id);
            }

            return Ok("All media files have been deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting files: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting files.");
        }
    }

    private string GetContentType(string filePath)
    {
        return Path.GetExtension(filePath).ToLower() switch
        {
            ".mp4" => "video/mp4",
            ".mp3" => "audio/mp3",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}

public class UploadFileFromMauiRequest
{
    public IFormFile FileStream { get; set; }
    public string MediaFileName { get; set; }
}

public class UploadVideoFromAPiRequest
{
    public string videoFileName { get; set; }
}