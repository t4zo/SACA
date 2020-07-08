using System.Collections.Generic;

namespace SACA.Constants
{
    public static class AuthorizationConstants
    {
        public static class Roles
        {
            public const string AllName = "All";

            public const string Superuser = "Superuser";

            public const string User = "User";

            public static IEnumerable<string> All = new List<string> { Superuser, User };
        }

        public static class CustomClaimTypes
        {
            public const string Permission = "Permission";
        }

        public static class Permissions
        {
            public static class Users
            {
                public const string View = "Permissions.Users.View";
                public const string Delete = "Permissions.Users.Delete";
            }

            public static class Categories
            {
                public const string View = "Permissions.Categorias.View";
                public const string Create = "Permissions.Categorias.Create";
                public const string Update = "Permissions.Categorias.Update";
                public const string Delete = "Permissions.Categorias.Delete";
            }

            public static class Images
            {
                public const string View = "Permissions.Images.View";
                public const string Create = "Permissions.Images.Create";
                public const string Update = "Permissions.Images.Update";
                public const string Delete = "Permissions.Images.Delete";
            }
        }

        public static class Database
        {
            public static int StartValueId = 100;

            public const string DefaultPassword = "123qwe";
        }

        public const string DefaultCorsPolicyName = "localhost";

        public const string Remember = "rmb";

        public const string users = "users";
        public const string Development = "Development";
        public const string SACA_Development = "SACA_Development";
        public const string SACA = "SACA";
    }
}
