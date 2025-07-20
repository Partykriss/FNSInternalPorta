using FNS.Main.Data;
using FNS.Main.Models;

[ApiController]
[Route("api/[controller]")]

public class NewsController: ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NewsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<News>>> GetNews() => 
        await _context.News.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<News>> GetNewsById(int id) =>
    await _context.News.FindAsync(id) is var news && news != null
        ? Ok(news)
        : NotFound();

    [HttpPost]
    public async Task<ActionResult<News>> PostNews(News news)
    {
        _context.News.Add(news);
        var saveResult = await _context.SaveChangesAsync();
        if (saveResult > 0)
            return CreatedAtAction(nameof(GetNewsById), new { id = news.Id, news });
        else 
            return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutNews(int id, [FromBody] News news)
    {
        if (id != news.Id)
            return BadRequest();

        _context.Entry(news).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!_context.News.Any(n => n.Id == id))
                return NotFound();
            else
            {
                Console.WriteLine($"Error updating news with ID {id}: {ex.Message}");
                throw new UpdateException($"Failed to update news with ID {id}: {ex.Message}");
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNews(int id)
    {
        var news = await _context.News.FindAsync(id);
        var test = news;

        if (news == null)
            return NotFound();

        _context.News.Remove(news);
        await _context.SaveChangesAsync();


        return NoContent();
    }
}