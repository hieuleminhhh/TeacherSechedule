using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSchedule.Dto;
using ProjectSchedule.Models;
using System.Linq;

[ApiController]
[Route("api/Class")]
public class ClassController : ControllerBase
{
    private readonly ProjectPRN231Context _context;

    public ClassController(ProjectPRN231Context context)
    {
        _context = context;
    }
    [HttpGet("GetClass")]
    public IActionResult GetClass()
    {

        var classes = _context.Classes
            .Select(c => new { c.ClassId, c.ClassName })
            .ToList();

        return Ok(classes);
    }
    [HttpGet("GetClassById/{id}")]
    public IActionResult GetClassById(int id)
    {

        var a = _context.Classes.Where(t => t.ClassId == id).FirstOrDefault();
        if (a == null)
        {
            return NotFound("Not found user");
        }
        return Ok(a);
    }
    [HttpPost("CreateClass")]
    public IActionResult CreateClass([FromBody] ClassDTO classData)
    {
        var classes = new Class { ClassName = classData.ClassName };
        _context.Classes.Add(classes);

        _context.SaveChanges();
        return Ok(classData);
    }
    [HttpPut("UpdateClass/{id}")]
    public async Task<IActionResult> UpdateClass(int id, Class classItem)
    {
        if (id != classItem.ClassId)
        {
            return BadRequest();
        }

        _context.Entry(classItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClassExists(id))
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

    [HttpDelete("DeleteClass/{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var classItem = await _context.Classes.FindAsync(id);
        if (classItem == null)
        {
            return NotFound();
        }

        _context.Classes.Remove(classItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClassExists(int id)
    {
        return _context.Classes.Any(e => e.ClassId == id);
    }
}

