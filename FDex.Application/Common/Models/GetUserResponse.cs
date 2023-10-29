using FDex.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDex.Application.Common.Models
{
    public class GetUserResponse
    {
        public int NumberOfPage { get; set; }

        public List<User> Users { get; set; }
    }
}
