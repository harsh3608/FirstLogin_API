using FirstLogin.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstLogin.Core.Identity
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string? PersonName { get; set; }
        public string? Gender { get; set; }
    }
}
