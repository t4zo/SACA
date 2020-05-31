﻿using System.Collections.Generic;

namespace SACA.Configurations
{
    public class UserConfiguration
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
