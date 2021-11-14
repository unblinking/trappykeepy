using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrappyKeepy.Api.Models;

namespace TrappyKeepy.Api.Controllers
{
    [Route("api/keepers")]
    [ApiController]
    public class KeepersController : ControllerBase
    {
        private readonly KeepyContext _context;

        public KeepersController(KeepyContext context)
        {
            _context = context;
        }

        // GET: api/Keepers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Keeper>>> GetKeepers()
        {
            return await _context.Keepers.ToListAsync();
        }

        // GET: api/Keepers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Keeper>> GetKeeper(Guid id)
        {
            var keeper = await _context.Keepers.FindAsync(id);

            if (keeper == null)
            {
                return NotFound();
            }

            return keeper;
        }

        // PUT: api/Keepers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKeeper(Guid id, Keeper keeper)
        {
            if (id != keeper.Id)
            {
                return BadRequest();
            }

            _context.Entry(keeper).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeeperExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Keepers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Keeper>> PostKeeper(Keeper keeper)
        {
            _context.Keepers.Add(keeper);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKeeper", new { id = keeper.Id }, keeper);
        }

        // DELETE: api/Keepers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKeeper(Guid id)
        {
            var keeper = await _context.Keepers.FindAsync(id);
            if (keeper == null)
            {
                return NotFound();
            }

            _context.Keepers.Remove(keeper);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KeeperExists(Guid id)
        {
            return _context.Keepers.Any(e => e.Id == id);
        }
    }
}
