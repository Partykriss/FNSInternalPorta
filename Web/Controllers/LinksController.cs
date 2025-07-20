using FNS.Main.Data;
using FNS.Main.Models;

[ApiController]
[Route("api/[controller]")]
public class LinksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LinksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Link>>> GetLink() =>
        await _context.Links.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Link>> GetLinkById(int id) =>
    await _context.Links.FindAsync(id) is var link && link != null
        ? Ok(link)
        : NotFound();

    [HttpPost]
    public async Task<ActionResult<Link>> PostLink(Link link)
    {
        _context.Links.Add(link);
        var saveResult = await _context.SaveChangesAsync();
        if (saveResult > 0)
            return CreatedAtAction(nameof(GetLinkById), new { id = link.Id}, link);
        else 
            return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLink(int id, [FromBody] Link updatedLink)    {
        if (id != updatedLink.Id) 
            return BadRequest();

        var existingLink = await _context.Links.FindAsync(id);
        if (existingLink == null)
            return NotFound($"Link with ID {id} not found.");

        existingLink.DisplayOrder = updatedLink.DisplayOrder;

        try
        { 
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!_context.Links.Any(n => n.Id == id))
                return NotFound();
            else
            {
                Console.WriteLine($"Error updating lins with ID {id}: {ex.Message}");
                throw new UpdateException($"Failed to update links with ID {id}: {ex.Message}");
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLink(int id)
    {
        var link = await _context.Links.FindAsync(id);
        if (link == null)
            return NotFound();

        _context.Links.Remove(link);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}