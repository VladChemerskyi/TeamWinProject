using SudokuGameBackend.BLL.InputModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IUserService
    {
        Task AddUser(AddUserInput input);
    }
}
