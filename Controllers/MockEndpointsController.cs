using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockEndpointsController : ControllerBase
    {
        private readonly IMockEndpointService _service;

        public MockEndpointsController(IMockEndpointService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var endpoints = await _service.GetAllAsync();
            return Ok(endpoints);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var endpoint = await _service.GetByIdAsync(id);

            if (endpoint == null)
                return NotFound();

            return Ok(endpoint);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMockEndpointDto dto)
        {
            try 
            {
                await _service.CreateAsync(dto);
                return Ok("Mock endpoint created successfully.");
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateMockEndpointDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok("Mock endpoint updated successfully.");
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
            try
            {
                await _service.DeleteAsync(id);
                return Ok("Mock endpoint deleted successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}