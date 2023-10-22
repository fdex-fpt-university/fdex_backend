using AutoMapper;
using FDex.Application.DTOs.TradingPosition;
using FDex.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDex.Application.Profiles
{
    public class PositionEventProfile : Profile
    {
        public PositionEventProfile()
        {
            CreateMap<Position, PositionDTOView>();

            CreateMap<Position, PositionDTOViewHistory>();

            CreateMap<Position, PositionDTOViewOrder>();
        }
    }
}
