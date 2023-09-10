using System;
using AutoMapper;
using FDex.Application.DTOs.Transaction;
using FDex.Domain.Entities;

namespace FDex.Application.Profiles
{
	public class TransactionProfile : Profile
	{
		public TransactionProfile()
		{
			CreateMap<Transaction, TransactionDTO>().ReverseMap();
			CreateMap<Transaction, AddTransactionDTO>().ReverseMap();
		}
	}
}

