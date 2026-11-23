using SwimTrack.Services.Common.Results;
using SwimTrack.Services.DTOs.SwimmingPool.Request;
using SwimTrack.Services.DTOs.SwimmingPool.Response;

namespace SwimTrack.Services.Contracts;

public interface ISwimmingPoolService
{
    Task<ServiceResult> CreateSwimmingPoolAsync(CreateSwimmingPoolRequestModel request, string currentUserId);

    Task<EditSwimmingPoolResponseModel?> GetSwimmingPoolForEditAsync(int id, string currentUserId);

    Task<ServiceResult> EditSwimmingPoolAsync(EditSwimmingPoolRequestModel request, string currentUserId);

    Task<ServiceResult> RemoveSwimmingPoolAsync(int id, string currentUserId);

    Task<SwimmingPoolDetailsResponseModel?> GetSwimmingPoolDetailsAsync(int id, string currentUserId);

    Task<IEnumerable<AllSwimmingPoolsResponseModel>> GetAllSwimmingPoolsAsync(string currentUserId);
}
