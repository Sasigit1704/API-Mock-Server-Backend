using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly IEnvironmentService _service;

        public EnvironmentController(IEnvironmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var environment = await _service.GetByIdAsync(id);

            if (environment == null)
                return NotFound();

            return Ok(environment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEnvironmentDTO dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Environment created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateEnvironmentDTO dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Environment updated successfully.");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, PatchEnvironmentDTO dto)
        {
            try
            {
                var updated = await _service.PatchAsync(id, dto);

                if (!updated)
                    return NotFound();

                return Ok("Mock environment patched successfully.");
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return Conflict(new
                    {
                        message = ex.Message
                    });
                }

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return Ok("Environment deleted successfully.");
        }
    }
}