using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSchedule.Dto;
using ProjectSchedule.Models;
using System.Data;
using System.Linq;

[ApiController]
[Route("api/Subject")]
public class SubjectController : ControllerBase
{
    private readonly ProjectPRN231Context _context;

    public SubjectController(ProjectPRN231Context context)
    {
        _context = context;
    }
    [HttpGet("GetSubjectCode")]
    public IActionResult GetSubjectCode()
    {
        var subject = _context.Subjects.Select(s => new
        {
            s.SubjectId,
            s.SubjectCode
        }).ToList();

        return Ok(subject);
    }
    [HttpGet("GetSubjectBySemesters")]
    public IActionResult GetSubjectBySemester()
    {
        var subjects = _context.Subjects
                                .Include(s => s.Semester)
                                .Select(s => new GetSubjectDTO
                                {
                                    SubjectName = s.SubjectName,
                                    SubjectCode = s.SubjectCode,
                                    SemesterId =s.SemesterId,
                                    SemesterName = s.Semester.SemesterName,
                                }).ToList(); ;

        return Ok(subjects);
    }

    [HttpGet("GetSubjectByClass/{classId}")]
    public IActionResult GetSubjectByClass(int classId)
    {
        var subjects = _context.Subjects
            .Where(s => s.SubjectClasses.Any(sc => sc.ClassId == classId))
            .Select(s => new SubjectDTO
            {
                SubjectId = s.SubjectId,
                SubjectName = s.SubjectName,
                SubjectCode = s.SubjectCode,
                SubjectClassIds = s.SubjectClasses.Select(sc => sc.SubjectClassId).ToList(),

            })
            .ToList();

        return Ok(subjects);
    }

    [HttpGet("GetSubjectClassId/{subjectId}/{classId}")]
    public IActionResult GetSubjectClassId(int subjectId, int classId)
    {
        var subjectClass = _context.SubjectClasses.FirstOrDefault(sc => sc.ClassId == classId && sc.SubjectId == subjectId);

        if (subjectClass == null)
        {
            return NotFound("Subject class not found");
        }

        return Ok(new { SubjectClassId = subjectClass.SubjectClassId });
    }
    [HttpPost("CreateSubjectInClass/{subjectId}/{classId}")]
    public IActionResult CreateSubjectInClass(int subjectId, int classId)
    {
        try
        {
            var subjectClass = new SubjectClass
            {
                SubjectId = subjectId,
                ClassId = classId
            };

            _context.SubjectClasses.Add(subjectClass);

            _context.SaveChanges();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while creating subject in class.");
        }
    }
    [HttpPost("CreateSubject")]
    public IActionResult CreateSubject([FromBody] CreateSubjectDTO sub)
    {
        try
        {
            var subject = new Subject
            {
                SubjectName = sub.SubjectName,
                SubjectCode = sub.SubjectCode,
                SemesterId = sub.SemesterId,
            };

            _context.Subjects.Add(subject);

            _context.SaveChanges();

            return Ok(sub);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while creating subject in class.");
        }
    }
    [HttpPut("UpdateSubject/{id}")]
    public async Task<IActionResult> UpdateSubject(int id, Subject subject)
    {
        if (id != subject.SubjectId)
        {
            return BadRequest();
        }

        _context.Entry(subject).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SubjectExists(id))
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
    [HttpDelete("DeleteSubject/{id}")]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
        {
            return NotFound();
        }

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SubjectExists(int id)
    {
        return _context.Subjects.Any(e => e.SubjectId == id);
    }
}

