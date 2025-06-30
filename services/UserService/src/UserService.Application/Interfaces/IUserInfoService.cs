using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTO;

namespace UserService.Application.Interfaces;

public interface IUserInfoService {
    public Task<IEnumerable<UserShortDTO>> GetUsersByRatingAsync (int pageNumber, int pageSize);
    public Task<IEnumerable<UserShortDTO>> GetUsersByDateAsync (int pageNumber, int pageSize);

    public Task<bool> CreateUserInfoAsync (UserInfoCreateDTO userDto);
    public Task<bool> UpdateUserInfoAsync (UserInfoUpdateDTO userDto);

    public Task<bool> CreateUserStatisticAsync (UserStatisticUpdateDto userDto);
    public Task<bool> UpdateUserStatisticAsync (UserStatisticUpdateDto userDto);

    public Task<bool> DeleteUserInfoAsync (Guid userId);
}
