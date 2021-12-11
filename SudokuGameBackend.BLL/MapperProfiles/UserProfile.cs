using AutoMapper;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.InputModels;
using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.MapperProfiles
{
    class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<AddUserInput, User>();
        }
    }
}
