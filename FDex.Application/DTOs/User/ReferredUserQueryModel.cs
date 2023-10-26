using FDex.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDex.Application.DTOs.User
{
    public class ReferredUserQueryModel
    {
        public int NumberOfPage { get; set; }

        public List<UserDTO> Users { get; set; }
    }

    public class UserDTO
    {
        public string Wallet { get; set; }
        public DateTime? ReferredUserDate { get; set; }
        public int? ReferralPoint { get; set; }

    }
}
