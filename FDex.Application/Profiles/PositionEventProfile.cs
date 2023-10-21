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
            CreateMap<PositionDetail,PositionDetailDto>();
            CreateMap<Position, PositionDTOView>();

            CreateMap<PositionDetail, PositionDetailHistoryDto>();
            CreateMap<Position, PositionDTOViewHistory>();

            CreateMap<PositionDetail, PositionDetailViewOrderDto>();
            CreateMap<Position, PositionDTOViewOrder>();
        }
    }
}
