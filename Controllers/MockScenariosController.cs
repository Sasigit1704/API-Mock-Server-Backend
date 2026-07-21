using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockScenariosController : ControllerBase
    {
        private readonly IMockScenarioService _service;

        public MockScenariosController(IMockScenarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var scenarios = await _service.GetAllAsync();
            return Ok(scenarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var scenario = await _service.GetByIdAsync(id);

            if (scenario == null)
                return NotFound();

            return Ok(scenario);
        }

        [HttpGet("endpoint/{mockEndpointId}")]
        public async Task<IActionResult> GetByMockEndpointId(string mockEndpointId)
        {
            var scenarios = await _service.GetByMockEndpointIdAsync(mockEndpointId);
            return Ok(scenarios);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMockScenarioDTO dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Mock Scenario created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateMockScenarioDTO dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Mock Scenario updated successfully.");
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, PatchMockScenarioDTO dto)
        {
            try
            {
                var updated = await _service.PatchAsync(id, dto);

                if (!updated)
                    return NotFound();

                return Ok("Mock scenario patched successfully.");
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
            return Ok("Mock Scenario deleted successfully.");
        }
    }
}