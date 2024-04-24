using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Relecloud.Web.CallCenter.Services;

namespace Relecloud.Web.CallCenter.Controllers;

[Route("webapi/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ITicketImageService _ticketImageService;
    private readonly ILogger<ImageController> _logger;

    public ImageController(ITicketImageService ticketImageService, ILogger<ImageController> logger)
    {
        _ticketImageService = ticketImageService;
        _logger = logger;
    }

    [HttpGet("{imageName}")]
    [Authorize]
    public async Task<IActionResult> GetTicketImage(string imageName)
    {
        try
        {
            var imageStream = await _ticketImageService.GetTicketImagesAsync(imageName);

            return File(imageStream, "application/octet-stream");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unable to retrive image: {imageName}");
            return Problem("Unable to get the image");
        }
    }
}