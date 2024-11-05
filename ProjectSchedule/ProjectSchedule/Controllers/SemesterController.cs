using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSchedule.Models;
using System.Linq;

[ApiController]
[Route("api/Semester")]
public class SemesterController : ControllerBase
{
    private readonly ProjectPRN231Context _context;

    public SemesterController(ProjectPRN231Context context)
    {
        _context = context;
    }

    [HttpGet("GetSemester")]
    public IActionResult GetSlot()
    {
        var semester = _context.Semesters.Select(s => new
        {
            SemesterId = s.SemesterId,
            SemesterName = s.SemesterName,

        }).ToList();
        return Ok(semester);
    }
}
