using System;
using System.Numerics;
using AutoMapper;
using FDex.Application.DTOs.Swap;
using FDex.Domain.Entities;

namespace FDex.Application.Profiles
{
	public class SwapEventProfile : Profile
	{
		public SwapEventProfile()
		{
			//CreateMap<SwapDTO, Swap>()
			//	.ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
			//	.ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTime.Now))
			//	.ReverseMap();

            CreateMap<SwapDTOBase, Swap>()
            .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
            .ForMember(dest => dest.TokenIn, opt => opt.MapFrom(src => src.TokenIn))
            .ForMember(dest => dest.TokenOut, opt => opt.MapFrom(src => src.TokenOut))
            .ForMember(dest => dest.AmountIn, opt => opt.MapFrom(src => src.AmountIn.ToString()))
            .ForMember(dest => dest.AmountOut, opt => opt.MapFrom(src => src.AmountOut.ToString()))
            .ForMember(dest => dest.Fee, opt => opt.MapFrom(src => src.Fee.ToString()))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTime.Now))
            .ReverseMap()
            .ForMember(dest => dest.AmountIn, opt => opt.MapFrom(src => BigInteger.Parse(src.AmountIn)))
            .ForMember(dest => dest.AmountOut, opt => opt.MapFrom(src => BigInteger.Parse(src.AmountOut)))
            .ForMember(dest => dest.Fee, opt => opt.MapFrom(src => BigInteger.Parse(src.Fee)));
        }
	}
}

