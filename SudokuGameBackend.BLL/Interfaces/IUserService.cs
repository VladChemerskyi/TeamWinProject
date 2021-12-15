using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.InputModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IUserService
    {
        Task AddUser(AddUserDto input);
        Task<UserDto> GetUser(string id);
        Task UpdateUser(string id, UpdateUserInput updateUserInput);
        Task DeleteUser(string id);
        Task<bool> IsUserNameCanBeUpdated(string userName, string userId);
        Task<bool> IsUserNameAvailable(string userName);
        Task<bool> DoesUserExist(string userId);
        string GetNameFromEmail(string email);
        Task<Dictionary<int, UserStatsItemDto>> GetUserStats(string id);
    }
}
