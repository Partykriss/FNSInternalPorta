using FNS.Main.Data;

namespace FNS.Main.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LastActionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LastActionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestActions()
        {
            var calls = _context.Calls
                                .Select(c => new
                                {
                                    Type = "Call",
                                    Date = c.CallDate,
                                    UserName = c.UserName,
                                    Message = string.Empty
                                });
            var feedbacks = _context.Feedback
                                    .Select(f => new
                                    {
                                        Type = "Feedback",
                                        Date = f.FeedDate,
                                        UserName = f.UserName,
                                        Message = f.Message
                                    });

            var latestActions = await calls.Concat(feedbacks)
                                           .OrderByDescending(a => a.Date)
                                           .Take(20)
                                           .ToListAsync();

            return Ok(latestActions);
        }
    }
}
