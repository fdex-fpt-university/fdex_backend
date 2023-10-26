using System;
using AutoMapper;
using FDex.Application.DTOs.User;
using FDex.Domain.Entities;

namespace FDex.Application.Profiles
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<User, UserDTOLeaderboardItemView>()
				.ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet));

			CreateMap<User, UserDto>();
		}
	}
}

