using System.Collections.Generic;

namespace SACA.Configurations
{
    public class AppOptions
    {
        public TokenOptions Token { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<UserOptions> Users { get; set; }
    }
}
