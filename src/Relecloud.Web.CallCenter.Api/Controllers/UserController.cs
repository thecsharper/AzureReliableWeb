using Microsoft.AspNetCore.Mvc;
using Relecloud.Web.Api.Services;
using Relecloud.Web.Models.ConcertContext;
using System.Net.Mime;

namespace Relecloud.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IConcertRepository _concertRepository;

        public UserController(ILogger<UserController> logger, IConcertRepository concertRepository)
        {
            _logger = logger;
            _concertRepository = concertRepository;
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(string id)
        {
            try
            {
                var user = await _concertRepository.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception from UserController.GetAsync");
                return Problem("Unable to GetAsync this user");
            }
        }

        [HttpPatch("", Name = "CreateOrUpdateUser")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateOrUpdateUserAsync(User model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                await _concertRepository.CreateOrUpdateUserAsync(model);

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception from UserController.CreateOrUpdateUserAsync");
                return Problem("Unable to CreateOrUpdateUserAsync the user");
            }
        }
    }
}
