using AutoMapper;
using FDex.Application.DTOs.TradingPosition;
using FDex.Domain.Entities;
using FDex.Domain.ValueObjects;
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
            CreateMap<Position, PositionDTOView>()
                .ForMember(dest => dest.CollateralToken, opt => opt.MapFrom(src => src.CollateralToken))
                .ForMember(dest => dest.IndexToken, opt => opt.MapFrom(src => src.IndexToken))
                .ForMember(dest => dest.Side, opt => opt.MapFrom(src => src.Side))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.EntryPrice, opt => opt.MapFrom(src => src.PositionDetails.FirstOrDefault().EntryPrice));

            CreateMap<Position, PositionDTOViewHistory>()
                .ForMember(dest => dest.CollateralToken, opt => opt.MapFrom(src => src.CollateralToken))
                .ForMember(dest => dest.IndexToken, opt => opt.MapFrom(src => src.IndexToken))
                .ForMember(dest => dest.Side, opt => opt.MapFrom(src => src.Side))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.EntryPrice, opt => opt.MapFrom(src => src.PositionDetails.FirstOrDefault().EntryPrice))
                .ForMember(dest => dest.Pnl, opt => opt.MapFrom(src => new SignedInt()));

            CreateMap<Position, PositionDTOViewOrder>()
                .ForMember(dest => dest.CollateralToken, opt => opt.MapFrom(src => src.CollateralToken))
                .ForMember(dest => dest.IndexToken, opt => opt.MapFrom(src => src.IndexToken))
                .ForMember(dest => dest.Side, opt => opt.MapFrom(src => src.Side))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.EntryPrice, opt => opt.MapFrom(src => src.PositionDetails.FirstOrDefault().EntryPrice));
        }
    }
}
