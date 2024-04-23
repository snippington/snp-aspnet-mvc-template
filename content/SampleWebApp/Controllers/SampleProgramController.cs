
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SampleWebApp.Data;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers
{
    public class SampleProgramController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SampleProgramController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/sampleprogram")]   
        [HttpGet("/sampleprogram/index")]
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        [HttpGet("/sampleprogram/details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var sampleProgram = await _context.SampleProgram.FirstOrDefaultAsync(m => m.SampleProgramId == id);
            if (sampleProgram == null)
            {
                return NotFound();
            }
            return View(sampleProgram);
        }
    
        [HttpPost("/sampleprogram/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsActive")] SampleProgram sampleProgram)
        {
            sampleProgram.CreatedOn = DateTime.Now;
            sampleProgram.LastModifiedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(sampleProgram);
                await _context.SaveChangesAsync();
            }

            
            return sampleProgram == null ? NotFound() : Ok(Json( new
                {
                    sampleProgramId= sampleProgram.SampleProgramId,
                    name = sampleProgram.Name,
                }
            ));
        }


        [HttpPost("/sampleprogram/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SampleProgramId, Name, IsActive")] SampleProgram sampleProgram)
        {

            if (id != sampleProgram.SampleProgramId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    sampleProgram.LastModifiedOn = DateTime.Now;
                    _context.Update(sampleProgram);
                    await _context.SaveChangesAsync();
                    
                    var result = _context.SampleProgram.FirstOrDefaultAsync(m => m.SampleProgramId == id);
                    return result == null ? NotFound() : Ok(result);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SampleProgramExists(sampleProgram.SampleProgramId))
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

        [HttpPost("/sampleprogram/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sampleProgram = await _context.SampleProgram.FindAsync(id);
            if (sampleProgram == null)
            {
                return NotFound();
            }
            _context.SampleProgram.Remove(sampleProgram);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/sampleprogram/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SampleProgram>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllSamplePrograms(int skip=0, int take=7)
        {
            var samplePrograms = _context.SampleProgram.OrderByDescending(i=> i.LastModifiedOn).Skip(skip)
                .Take(take);

            return samplePrograms == null ? NotFound() : Ok(samplePrograms.ToListAsync());
        }

        [HttpGet("/sampleprogram/search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SampleProgram>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SearchAllSamplePrograms(string term)
        {
            var samplePrograms = _context.SampleProgram.AsQueryable();
            if (!string.IsNullOrEmpty(term))
            {
                samplePrograms = _context.SampleProgram.Where(
                    s => EF.Functions.Like(s.Name, $"%{term}%")
                ).Take(7);
            }
            return samplePrograms == null ? NotFound() : Ok(samplePrograms.ToListAsync());
        }

        [HttpGet("/sampleprogram/count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public IActionResult GetCount()
        {
            var count = _context.SampleProgram.Count();
            return Ok(count);
        }
        

        [HttpGet("/sampleprogram/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SampleProgram))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sampleProgram = await _context.SampleProgram.FirstOrDefaultAsync(m => m.SampleProgramId == id);
            if (sampleProgram == null)
            {
                return NotFound();
            }

            return sampleProgram == null ? NotFound() : Ok(sampleProgram);
        }


        private bool SampleProgramExists(int id)
        {
            return _context.SampleProgram.Any(e => e.SampleProgramId == id);
        }
    }
}