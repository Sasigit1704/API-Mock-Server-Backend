using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionsController : ControllerBase
    {
        private readonly ICollectionService _service;

        public CollectionsController(ICollectionService service)
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
            var collection = await _service.GetByIdAsync(id);

            if (collection == null)
                return NotFound();

            return Ok(collection);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCollectionDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Collection created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateCollectionDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Collection updated successfully.");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, PatchCollectionDTO dto)
        {
            try
            {
                var updated = await _service.PatchAsync(id, dto);

                if (!updated)
                    return NotFound();

                return Ok("Collection patched successfully.");
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
            return Ok("Collection deleted successfully.");
        }
    }
}