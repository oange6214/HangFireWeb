using Hangfire;
using Microsoft.AspNetCore.Mvc;
using src.Models;
using src.Services;

namespace HangFireApp.Controllers;

[ApiController]
[Route("[controller]")]
public class DriversController : ControllerBase
{
    private static List<Driver> drivers = new List<Driver>();
    private readonly ILogger<DriversController> _logger;

    public DriversController(ILogger<DriversController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult AddDriver(Driver driver)
    {
        if (ModelState.IsValid)
        {
            drivers.Add(driver);

            // Enqueue a background job to send email
            var jobId = BackgroundJob.Enqueue<IServiceManagement>(x => x.SendEmail());

            return CreatedAtAction(nameof(GetDriver), new { driver.Id }, driver);
        }

        return BadRequest();
    }

    [HttpGet]
    public IActionResult GetDriver(Guid id)
    {
        var driver = drivers.FirstOrDefault(x => x.Id == id);

        if (driver is null)
            return NotFound();

        return Ok(driver);
    }

    [HttpDelete]
    public IActionResult DeleteDriver(Guid id)
    {
        var driver = drivers.FirstOrDefault(x => x.Id == id);

        if (driver is null)
            return NotFound();

        driver.Status = 0;

        // Add or update a recurring job to update database every minute
        RecurringJob.AddOrUpdate<IServiceManagement>(Guid.NewGuid().ToString(), x => x.UpdateDatabase(), Cron.Minutely);
        
        return NoContent();
    }
}
