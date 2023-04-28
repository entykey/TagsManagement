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

        // inject service(bussiness logics):
        private readonly ITagService _tagService;

        public TagsController(IUnitOfWork unitOfWork, ITagService tagService)
        {
            _unitOfWork = unitOfWork;
            _tagService = tagService;
        }

        [HttpGet("AsList")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAllTagsAsList()
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();    // async
            // NOTE: nhược điểm của approach này là :
            // ta gọi unitofwork trong controller thay vì service, nên data trả về trực tiếp
            // là data system, ko đc map qua DTO hay view model, bussiness logics
            // => less secure & khó thêm bussiness logics ( chỉ có thể get all )

            return Ok(tags);
        }

        // using service for better bussiness logics
        [HttpGet("AsListByService")]
        public async Task<ActionResult<IEnumerable<TagViewModel>>> GetAllTagsAsListByService()
        {
            var tags = await _tagService.GetAllTagsAsList();
            return Ok(tags);
        }

        [HttpGet("AsQueryableByService")]
        public async Task<ActionResult<IEnumerable<TagViewModel>>> GetAllTagsAsQueryableByService()
        {
            var tags = await _tagService.GetAllTagsAsQueryable();
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
