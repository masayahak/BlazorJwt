using BlazorJwt.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorJwt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WeatherController() : ControllerBase
    {
        [HttpPost("all")]
        public async Task<ActionResult<WeatherModel[]>> All([FromBody] int ListCount)
        {
            await Task.Delay(0);

            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            WeatherModel[] weathers = Enumerable.Range(1, ListCount).Select(index => new WeatherModel
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            }).ToArray();
            return Ok(weathers);
        }
    }
}
