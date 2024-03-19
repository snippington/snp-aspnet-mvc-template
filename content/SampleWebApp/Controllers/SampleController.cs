
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SampleWebApp.Data;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers
{
    public class SampleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SampleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sample
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var sample = await _context.Sample
                .Include(p => p.SampleProgram)
                .FirstOrDefaultAsync(m => m.SampleId == id);
            if (sample == null)
            {
                return NotFound();
            }
            return View(sample);
        }
    
    
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SampleProgramId,Title,Content,IsActive")] Sample sample)
        {
            sample.CreatedOn = DateTime.Now;
            sample.LastModifiedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(sample);
                await _context.SaveChangesAsync();
            }

            
            return sample == null ? NotFound() : Ok(Json( new
                {
                    sampleId= sample.SampleId,
                    sampleProgramId = sample.SampleProgramId,
                    title = sample.Title,
                    content = sample.Content,
                }
            ));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SampleId, SampleProgramId,Title,Content, IsActive")] Sample sample)
        {

            if (id != sample.SampleId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    sample.LastModifiedOn = DateTime.Now;
                    _context.Update(sample);
                    await _context.SaveChangesAsync();
                    
                    var result = _context.Sample
                        .Include(p => p.SampleProgram)
                        .FirstOrDefaultAsync(m => m.SampleId == id);
                    return result == null ? NotFound() : Ok(result);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SampleExists(sample.SampleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }       
                }
            }else{
                return BadRequest();
            }
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sample = await _context.Sample.FindAsync(id);
            if (sample == null)
            {
                return NotFound();
            }
            _context.Sample.Remove(sample);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/sample/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Sample>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllSamples(int skip=0, int take=7)
        {
            var samples = _context.Sample
                .Include(p => p.SampleProgram)
                .OrderByDescending(i=> i.LastModifiedOn).Skip(skip)
                .Take(take);

            return samples == null ? NotFound() : Ok(samples.ToListAsync());
        }

        [HttpGet("/sample/search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Sample>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SearchAllSamples(string term)
        {
            var samples = _context.Sample.AsQueryable();
            if (!string.IsNullOrEmpty(term))
            {
                samples = _context.Sample
                .Include(p => p.SampleProgram)
                .Where(
                    s =>
                 EF.Functions.Like(s.Title, $"%{term}%")||
                 EF.Functions.Like(s.Content, $"%{term}%")
                ).Take(7);
            }
            return samples == null ? NotFound() : Ok(samples.ToListAsync());
        }

        [HttpGet("/sample/count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public IActionResult GetCount()
        {
            var count = _context.Sample
                .Include(p => p.SampleProgram)
                .Count();
            return Ok(count);
        }
        
        [HttpGet("/sample/programs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Program>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPrograms()
        {
            var programs = _context.SampleProgram;
            return programs == null ? NotFound() : Ok(programs.ToListAsync());
        }
        
        

        [HttpGet("/sample/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Sample))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sample = await _context.Sample
                .Include(p => p.SampleProgram)
                .FirstOrDefaultAsync(m => m.SampleId == id);
            if (sample == null)
            {
                return NotFound();
            }

            return sample == null ? NotFound() : Ok(sample);
        }


        private bool SampleExists(int id)
        {
            return _context.Sample.Any(e => e.SampleId == id);
        }
    }
}