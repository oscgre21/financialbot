using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FinancialBot.BL.DTOs;
using FinancialBot.Domain.Entities;

namespace FinancialBot.BL.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region Globales
            CreateMap<UserRegistrationDto, Users>();
            #endregion


        }
    }
}
