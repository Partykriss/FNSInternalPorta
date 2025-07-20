using FNS.Main.Data;
using FNS.Main.Models;
using System.Diagnostics;

namespace FNS.Main.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VotingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VotingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voting>>> GetVotings() => await _context.Votings
                                                                                           .Include(v => v.Answers)
                                                                                           .Include(v => v.Votes)
                                                                                           .ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Voting>> GetVotingById(int id)
        {
            var voting = await _context.Votings
                                       .Include(v => v.Answers)
                                       .Include(v => v.Votes)
                                       .FirstOrDefaultAsync(v => v.Id == id);

            if (voting == null)
                    return NotFound();

            return Ok(voting);
        }

        [HttpPost]
        public async Task<ActionResult<Voting>> PostVoting(Voting voting)
        {
            _context.Votings.Add(voting);

            if (voting.Answers != null)
                foreach (var answer in voting.Answers)
                {
                    answer.VotingId = voting.Id;
                    _context.Answers.Add(answer);
                }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVotingById), new {id = voting.Id}, voting);
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateVoting(int id)
        {
            var deactivatedVoting = await _context.Votings.FindAsync(id);
            if (deactivatedVoting == null)
                return NotFound();

            deactivatedVoting.IsActive = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                    Debug.WriteLine($"Error updating voting with ID {id}: {ex.Message}");
                    throw new UpdateException($"Failed to update voting with ID {id}: {ex.Message}");
            }

            return NoContent();
        }
    }

}
