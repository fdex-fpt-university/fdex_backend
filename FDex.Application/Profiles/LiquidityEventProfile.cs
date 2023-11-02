using System;
using AutoMapper;
using FDex.Application.DTOs.Liquidity;
using FDex.Domain.Entities;

namespace FDex.Application.Profiles
{
	public class LiquidityEventProfile : Profile
	{
		public LiquidityEventProfile()
		{
			CreateMap<LiquidityDTOAdd, Liquidity>();
				
		}
	}
}

