
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SampleWebApp.Data;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers
{
    public class SocialPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SocialPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/socialpost")]   
        [HttpGet("/socialpost/index")]
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        [HttpGet("/socialpost/details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var socialpost = await _context.SocialPost
                .Include(p => p.SampleProgram)
                .FirstOrDefaultAsync(m => m.SocialPostId == id);
            if (socialpost == null)
            {
                return NotFound();
            }
            return View(socialpost);
        }
    
        [HttpPost("/socialpost/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SampleProgramId,Title,Content,IsActive")] SocialPost socialpost)
        {
            socialpost.CreatedOn = DateTime.Now;
            socialpost.LastModifiedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(socialpost);
                await _context.SaveChangesAsync();
            }

            
            return socialpost == null ? NotFound() : Ok(Json( new
                {
                    socialpostId= socialpost.SocialPostId,
                    sampleProgramId = socialpost.SampleProgramId,
                    title = socialpost.Title,
                    content = socialpost.Content,
                }
            ));
        }


        [HttpPost("/socialpost/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SocialPostId, SampleProgramId,Title,Content, IsActive")] SocialPost socialpost)
        {

            if (id != socialpost.SocialPostId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    socialpost.LastModifiedOn = DateTime.Now;
                    _context.Update(socialpost);
                    await _context.SaveChangesAsync();
                    
                    var result = _context.SocialPost
                        .Include(p => p.SampleProgram)
                        .FirstOrDefaultAsync(m => m.SocialPostId == id);
                    return result == null ? NotFound() : Ok(result);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SocialPostExists(socialpost.SocialPostId))
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


        [HttpPost("/socialpost/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var socialpost = await _context.SocialPost.FindAsync(id);
            if (socialpost == null)
            {
                return NotFound();
            }
            _context.SocialPost.Remove(socialpost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/socialpost/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SocialPost>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllSocialPosts(int skip=0, int take=7)
        {
            var socialposts =_context.SocialPost
                .Include(p => p.SampleProgram)
                .OrderByDescending(i=> i.LastModifiedOn).Skip(skip)
                .Take(take);

            return socialposts == null ? NotFound() : Ok(socialposts.ToListAsync());
        }

        [HttpGet("/socialpost/search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SocialPost>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SearchAllSocialPosts(string term)
        {
            var socialposts =_context.SocialPost.AsQueryable();
            if (!string.IsNullOrEmpty(term))
            {
                socialposts =_context.SocialPost
                .Include(p => p.SampleProgram)
                .Where(
                    s =>
                 EF.Functions.Like(s.Title, $"%{term}%")||
                 EF.Functions.Like(s.Content, $"%{term}%")
                ).Take(7);
            }
            return socialposts == null ? NotFound() : Ok(socialposts.ToListAsync());
        }

        [HttpGet("/socialpost/count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public IActionResult GetCount()
        {
            var count = _context.SocialPost
                .Include(p => p.SampleProgram)
                .Count();
            return Ok(count);
        }
        
        [HttpGet("/socialpost/programs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Program>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPrograms()
        {
            var programs = _context.SampleProgram;
            return programs == null ? NotFound() : Ok(programs.ToListAsync());
        }
        
    
        [HttpGet("/socialpost/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SocialPost))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socialpost = await _context.SocialPost
                .Include(p => p.SampleProgram)
                .FirstOrDefaultAsync(m => m.SocialPostId == id);
            if (socialpost == null)
            {
                return NotFound();
            }

            return socialpost == null ? NotFound() : Ok(socialpost);
        }


        private bool SocialPostExists(int id)
        {
            return _context.SocialPost.Any(e => e.SocialPostId == id);
        }
    }
}