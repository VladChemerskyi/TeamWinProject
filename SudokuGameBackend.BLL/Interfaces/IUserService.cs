using SudokuGameBackend.BLL.InputModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IUserService
    {
        void AddUser(AddUserInput input);
    }
}
