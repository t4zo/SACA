using System.Collections.Generic;

namespace SACA.Utilities
{
    public static class Constants
    {
        public const string All = "all";

        public const string Administrador = "administrador";

        public const string Usuario = "usuario";

        public const string RememberUser = "rmb";

        public static int DatabaseIdStartValue = 100;

        public static IEnumerable<string> AllRoles = new List<string> { Administrador, Usuario };

        public const string users = "users";

        public const string Development = "Development";
        public const string SACA_Development = "SACA_Development";
        public const string SACA = "SACA";
    }
}
