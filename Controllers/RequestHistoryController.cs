using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestHistoryController : ControllerBase
    {
        private readonly IRequestHistoryService _service;

        public RequestHistoryController(
            IRequestHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _service.GetAllAsync();
            return Ok(logs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var log = await _service.GetByIdAsync(id);

            if (log == null)
                return NotFound();

            return Ok(log);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _service.DeleteAllAsync();
            return NoContent();
        }
    }
}