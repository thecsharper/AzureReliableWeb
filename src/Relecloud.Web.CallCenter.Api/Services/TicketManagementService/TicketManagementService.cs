using Microsoft.EntityFrameworkCore;
using Relecloud.Web.Api.Services.SqlDatabaseConcertRepository;
using Relecloud.Web.Models.ConcertContext;
using Relecloud.Web.Models.TicketManagement;

namespace Relecloud.Web.Api.Services.TicketManagementService
{
    public class TicketManagementService : ITicketManagementService
    {
        private readonly ConcertDataContext _database;
        private readonly ITicketRenderingService _ticketRenderingService;
        private readonly ILogger<TicketManagementService> _logger;

        public TicketManagementService(ConcertDataContext database, ITicketRenderingService ticketRenderingService, ILogger<TicketManagementService> logger)
        {
            _database = database;
            _ticketRenderingService = ticketRenderingService;
            _logger = logger;
        }

        public Task<CountAvailableTicketsResult> CountAvailableTicketsAsync(int concertId)
        {
            // in the future tickets will be limited due to seating
            // for current scope ticket sales are unlimited
            return Task.FromResult(new CountAvailableTicketsResult
            {
                CountOfAvailableTickets = 100
            });
        }

        public async Task<HaveTicketsBeenSoldResult> HaveTicketsBeenSoldAsync(int concertId)
        {
            // in the future tickets will be limited due to seating
            // as part of that plan we need to check to see if tickets have been sold
            // a business rule will be added:
            // if tickets have been sold then we cannot change the visibility of a concert from visible to hidden
            var soldTicketCount = await _database.Tickets.AsNoTracking()
                .Where(ticket => ticket.ConcertId == concertId)
                .CountAsync();

            _logger.LogInformation($"Sold ticket count {soldTicketCount}");

            return new HaveTicketsBeenSoldResult
            {
                HaveTicketsBeenSold = soldTicketCount > 0
            };
        }

        public async Task<ReserveTicketsResult> ReserveTicketsAsync(int concertId, string userId, int numberOfTickets, int customerId)
        {
            var newTickets = new List<Ticket>();
            for (int i = 0; i < numberOfTickets; i++)
            {
                var newTicket = new Ticket
                {
                    ConcertId = concertId,
                    UserId = userId,
                    CustomerId = customerId
                    //TicketNumber = not used. planned for use when tickets become limited due to seating
                };
                _database.Tickets.Add(newTicket);
                newTickets.Add(newTicket);
            }

            await _database.SaveChangesAsync();

            foreach (var ticket in newTickets)
            {
                await _ticketRenderingService.CreateTicketImageAsync(ticket.Id);
            }

            return new ReserveTicketsResult
            {
                Status = ReserveTicketsResultStatus.Success,
                // TicketNumbers = not used. planned for use when tickets become limited due to seating
            };
        }
    }
}