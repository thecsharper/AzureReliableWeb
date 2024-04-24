using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relecloud.Web.CallCenter.Infrastructure;
using Relecloud.Web.CallCenter.ViewModels;
using Relecloud.Web.Models.ConcertContext;
using Relecloud.Web.Models.Services;

namespace Relecloud.Web.CallCenter.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        #region Fields

        private readonly ILogger<TicketController> _logger;
        private readonly IConcertContextService _concertService;

        #endregion

        #region Constructors

        public TicketController(IConcertContextService concertService, ILogger<TicketController> logger)
        {
            _concertService = concertService;
            _logger = logger;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(int currentPage)
        {
            try
            {
                var userId = User.GetUniqueId();
                var pagedResultModel = await _concertService.GetAllTicketsAsync(userId, currentPage * TicketViewModel.DefaultPageSize, TicketViewModel.DefaultPageSize);

                return View(new TicketViewModel
                {
                    CurrentPage = currentPage,
                    TotalCount = pagedResultModel?.TotalCount ?? 0,
                    Tickets = pagedResultModel?.PageOfData ?? new List<Ticket>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to retrieve upcoming concerts");
                return View();
            }
        }

        #endregion
    }
}