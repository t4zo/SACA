namespace SACA.Constants
{
    public static class AuthorizationConstants
    {

        public const string AllowedOrigins = "AllowedOrigins";

        public const string Remember = "rmb";

        public static class Roles
        {
            public const string AllName = "All";

            public const string Superuser = "Superuser";

            public const string User = "User";
        }

        public static class CustomClaimTypes
        {
            public const string Permissions = "Permissions";
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
    }
}
