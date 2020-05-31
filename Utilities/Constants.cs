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
    }
}
