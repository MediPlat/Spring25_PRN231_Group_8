using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.RequestObject.Auth;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using System.Security.Claims;

namespace MediPlat.Service.Services
{
    public class ProfileService : IProfileService
    {
        static Guid guid;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProfileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ProfileResponse?> Create(ProfileRequest ProfileModel, ClaimsPrincipal claims)
        {
            Guid guid = Guid.NewGuid();
            var profile = _mapper.Map<Model.Model.Profile>(ProfileModel);
            profile.Id = guid;
            profile.PatientId = new Guid(claims.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            _unitOfWork.Profiles.Add(profile);
            await _unitOfWork.SaveChangesAsync();
            return _mapper?.Map<ProfileResponse?>(await _unitOfWork.Profiles.GetAsync(p => p.Id.Equals(guid)));
        }

        public async Task<ProfileResponse?> DeleteById(ClaimsPrincipal claims)
        {
            var id = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ProfileId = new Guid(id);
            var Profile = await _unitOfWork.Profiles.GetAsync(p => p.Id == ProfileId);

            if (Profile == null)
            {
                throw new KeyNotFoundException("Incorrect jwt token or Profile deleted");
            }
            _unitOfWork.Profiles.Remove(Profile);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProfileResponse>(Profile);
        }

        public async Task<List<ProfileResponse>> GetAll(ClaimsPrincipal claims)
        {
            var Profiles = await _unitOfWork.Profiles.GetAllAsync();
            return _mapper.Map<List<ProfileResponse>>(Profiles);
        }

        public async Task<ProfileResponse?> GetById(string code)
        {
            bool isValid = Guid.TryParse(code, out guid);
            if (!isValid)
            {
                throw new ArgumentException("Incorrect GUID format.");
            }

            var Profile = await _unitOfWork.Profiles.GetAsync(p => p.Id == guid);
            if (Profile == null)
            {
                throw new KeyNotFoundException("Profile not found.");
            }
            return _mapper.Map<ProfileResponse>(Profile);
        }

        public async Task<ProfileResponse?> Update(ProfileRequest ProfileModel, ClaimsPrincipal claims)
        {
            var id = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ProfileId = new Guid(id);
            var Profile = await _unitOfWork.Profiles.GetAsync(p => p.Id == ProfileId);

            if (Profile == null)
            {
                throw new KeyNotFoundException("Incorrect jwt token or Profile deleted");
            }
            Profile = _mapper.Map<Model.Model.Profile>(ProfileModel);

            _unitOfWork.Profiles.Update(Profile);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProfileResponse>(Profile);
        }
    }
}
