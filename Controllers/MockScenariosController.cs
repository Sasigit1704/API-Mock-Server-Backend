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
        public async Task<IActionResult> Create(CreateMockScenarioDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Mock Scenario created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateMockScenarioDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Mock Scenario updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return Ok("Mock Scenario deleted successfully.");
        }
    }
}