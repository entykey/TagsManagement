using Microsoft.AspNetCore.Mvc;
using TagsManagement.DomainModels;
using TagsManagement.DTOs;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TagsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("AsList")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAllTagsAsList()
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();    // async

            return Ok(tags);
        }
        [HttpGet("AsQueryable")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAllTagsAsQueryable()
        {
            var tags = _unitOfWork.Tags.GetAll();   // synchronous

            return Ok(tags);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(string id)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }

        [HttpPost]
        public async Task<ActionResult<Tag>> CreateTag(TagViewModel tagvm)
        {
            try
            {
                //using var _unitOfWork = new UnitOfWork(_dbContext);
                Tag tag = new Tag { Name = tagvm.Name, Id = Guid.NewGuid().ToString() };
                await _unitOfWork.TagRepository.AddAsync(tag);
                await _unitOfWork.SaveEntitiesAsync();   // CommitAsync

                return CreatedAtAction(nameof(GetTag), new { id = Guid.NewGuid().ToString() }, tag);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(string id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }

            var tagToBeUpdated =  _unitOfWork.TagRepository.GetByIdAsync(id);

            if (tagToBeUpdated == null)
            {
                return NotFound("Tag Not Found!");
            }

            _unitOfWork.TagRepository.Update(tag);

            await _unitOfWork.SaveEntitiesAsync();

            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(string id)
        {
            Tag tag = await _unitOfWork.TagRepository.GetByIdAsync(id);

            _unitOfWork.TagRepository.Delete(tag);  // cannot await void!

            await _unitOfWork.SaveEntitiesAsync();

            return NoContent();
        }
    }
}
