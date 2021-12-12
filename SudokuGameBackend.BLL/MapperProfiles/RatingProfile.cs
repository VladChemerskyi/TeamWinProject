using AutoMapper;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.MapperProfiles
{
    class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMap<DuelRating, RatingDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.UserId))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(source => source.User.Name))
                .ForMember(dest => dest.CountryCode, opts => opts.MapFrom(source => source.User.CountryCode))
                .ForMember(dest => dest.Value, opts => opts.MapFrom(source => source.Rating));

            CreateMap<SolvingRating, RatingDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.UserId))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(source => source.User.Name))
                .ForMember(dest => dest.CountryCode, opts => opts.MapFrom(source => source.User.CountryCode))
                .ForMember(dest => dest.Value, opts => opts.MapFrom(source => source.Time));
        }
    }
}
