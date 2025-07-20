using FNS.Main.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using FNS.Main.Data;
using Microsoft.Data.SqlClient;
using FNS.Main.Services;

namespace FNS.Main.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<CallsHub> _hubContext;
        private readonly IUserIdentityService _userIdentityService;

        public HomeController(ApplicationDbContext context, IHubContext<CallsHub> hubContext, IUserIdentityService userIdentityService)
        {
            _context = context;
            _hubContext = hubContext;
            _userIdentityService = userIdentityService;
        }

        public async Task<IActionResult> Index()
        {
            var currentDate = DateTime.Now;
            var userLogin = User.Identity.Name;
            int index = userLogin.IndexOf('/');
            if (index >= 0)
                userLogin = userLogin.Substring(index + 1);

            var userName = _userIdentityService.GetUserFullName(User.Identity.Name);
            ViewBag.UserFullName = userName;

            var news = await _context.News
                        .Where(n => n.PublishDate >= currentDate.AddDays(-30))
                        .OrderByDescending(n => n.PublishDate)
                        .ToListAsync();

            var links = await _context.Links.OrderBy(l => l.DisplayOrder).ToListAsync();

            var currentVoting = await _context.Votings
                                              .Include(v => v.Answers)
                                              .FirstOrDefaultAsync(v => v.IsActive);

            var hasVoted = currentVoting != null && _context.VotedUsers.Any(v => (v.VotingID == currentVoting.Id && v.UserLogin == userLogin));

            var recentVotings = await _context.Votings
                                            .Where(v => !v.IsActive && v.EndDate >= DateTime.Now.AddDays(-30))
                                            .Include(v => v.Answers)
                                            .OrderByDescending(v => v.EndDate)
                                            .ToListAsync();


            var viewModel = new HomeViewModel
            {
                News = news,
                Links = links,
                CurrentVoting = currentVoting,
                HasVoted = hasVoted,
                RecentVotings = recentVotings
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PostFeedback(string message)
        {
            var userName = $"{_userIdentityService.GetUserFullName(User.Identity.Name)} ({User.Identity.Name.Substring(User.Identity.Name.IndexOf("\\") + 1)})";
            if (message != null)
            {
                var feedback = new Feedback
                {
                    Message = message,
                    UserName = userName,
                    FeedDate = DateTime.Now
                };
                _context.Feedback.Add(feedback);
                await _context.SaveChangesAsync();

                ViewBag.UserFullName = _userIdentityService.GetUserFullName(User.Identity.Name);
                TempData["SuccessMessage"] = feedback.UserName + " спасибо за обратную связь!";
                return View("Message");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostCall()
        {
            var userName = $"{_userIdentityService.GetUserFullName(User.Identity.Name)}" +
                $" ({User.Identity.Name.Substring(User.Identity.Name.IndexOf("\\") + 1)})";

            var adminCall = new AdminCall
            {
                UserName = userName,
                CallDate = DateTime.Now
            };
            _context.Calls.Add(adminCall);
            await _context.SaveChangesAsync();

            // Отправка уведомления через SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveCall", adminCall.UserName);

            ViewBag.UserFullName = _userIdentityService.GetUserFullName(User.Identity.Name);
            TempData["SuccessMessage"] = "Вызов принят!";
            return View("Message");
        }

        [HttpPost]
        public async Task<IActionResult> PostVote(int selectedAnswerId)
        {
            var userName = User.Identity.Name;
            var votingId = _context.Answers.Where(a => a.Id == selectedAnswerId).Select(a => a.VotingId).FirstOrDefault();
            try
            {
                var commandText = "EXEC AddVote @VotingId, @AnswerId, @UserName";
                var votingIdParam = new SqlParameter("@VotingId", votingId);
                var answerIdParam = new SqlParameter("@AnswerId", selectedAnswerId);
                var userNameParam = new SqlParameter("@UserName", userName);
                await _context.Database.ExecuteSqlRawAsync(commandText, new[] { votingIdParam, answerIdParam, userNameParam });

                ViewBag.UserFullName = _userIdentityService.GetUserFullName(User.Identity.Name);
                TempData["SuccessMessage"] = "Ваш голос учтен!";
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex, "Ошибка при попытке голосования");
                TempData["ErrorMessage"] = "Вы уже проголосовали!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Неизвестная ошибка при голосовании");
                TempData["ErrorMessage"] = "Произошла ошибка при голосовании. Пожалуйста, попробуйте позже.";
            }

            return View("Message");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
