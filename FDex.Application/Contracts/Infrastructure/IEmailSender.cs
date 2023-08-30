using System;
using FDex.Application.Models.Infrastructure;

namespace FDex.Application.Contracts.Infrastructure
{
	public interface IEmailSender
	{
        Task SendEmail(Email email);
    }
}

