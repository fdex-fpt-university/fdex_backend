using System;
using AutoMapper;
using FDex.Application.DTOs.Swap;
using FDex.Domain.Entities;

namespace FDex.Application.Profiles
{
	public class SwapEventProfile : Profile
	{
		public SwapEventProfile()
		{
			CreateMap<Swap, SwapDTO>();
			CreateMap<SwapDTO, Swap>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTime.Now));
		}
	}
}

