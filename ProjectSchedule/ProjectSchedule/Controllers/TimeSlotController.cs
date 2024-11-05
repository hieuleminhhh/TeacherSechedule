using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSchedule.Models;
using System.Linq;

[ApiController]
[Route("api/TimeSlot")]
public class TimeSLotController : ControllerBase
{
    private readonly ProjectPRN231Context _context;

    public TimeSLotController(ProjectPRN231Context context)
    {
        _context = context;
    }

    [HttpGet("GetSlot")]
    public IActionResult GetSlot()
    {
        var slot = _context.TimeSlots.Select(s => new 
        {
            SlotId = s.SlotId,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();
        return Ok(slot);
    }
}
