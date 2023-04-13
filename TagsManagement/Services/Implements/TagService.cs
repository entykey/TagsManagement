using TagsManagement.DomainModels;
using TagsManagement.DTOs;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Services.Implements
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IMapper _mapper;

        public TagService(IUnitOfWork unitOfWork
            //IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            //_mapper = mapper;
        }

        public async Task<TagViewModel> GetTagByIdAsync(string id)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            //return _mapper.Map<TagViewModel>(tag);

            TagViewModel tagvm = new TagViewModel
            {
                Name = tag.Name
            };
            return tagvm;
        }

        public async Task<IEnumerable<TagViewModel>> GetAllTagsAsync()
        {
            var tags = await _unitOfWork.TagRepository.GetAllAsync();
            //return _mapper.Map<IEnumerable<TagViewModel>>(tags);

            List<TagViewModel> tagvms = new List<TagViewModel>();
            foreach(var tag in tags)
            {
                tagvms.Add(new TagViewModel { Name = tag.Name });
            }
            return tagvms;
        }

        public async Task AddTagAsync(TagViewModel tagvm)
        {
            //var tag = _mapper.Map<Tag>(tagvm);
            Tag tag = new Tag { Name = tagvm.Name, Id = Guid.NewGuid().ToString() };
            await _unitOfWork.TagRepository.AddAsync(tag);
            await _unitOfWork.SaveEntitiesAsync();
        }

        public async Task UpdateTagAsync(string id, TagViewModel tagvm)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                throw new ArgumentException($"Tag with id {id} not found.");
            }

            //_mapper.Map(tagvm, tag);

            _unitOfWork.TagRepository.Update(tag);  // void
            await _unitOfWork.SaveEntitiesAsync();
        }

        public async Task DeleteTagAsync(string id)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                throw new ArgumentException($"Tag with id {id} not found.");
            }

            _unitOfWork.TagRepository.Delete(tag);  // void
            await _unitOfWork.SaveEntitiesAsync();
        }
    }
}
