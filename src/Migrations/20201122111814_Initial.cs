using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SACA.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AspNetRoles",
                table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>("text", nullable: true)
                },
                constraints: table => { table.PrimaryKey("pk_roles", x => x.id); });

            migrationBuilder.CreateTable(
                "AspNetUsers",
                table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name =
                        table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>("boolean", nullable: false),
                    password_hash = table.Column<string>("text", nullable: true),
                    security_stamp = table.Column<string>("text", nullable: true),
                    concurrency_stamp = table.Column<string>("text", nullable: true),
                    phone_number = table.Column<string>("text", nullable: true),
                    phone_number_confirmed = table.Column<bool>("boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>("boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>("boolean", nullable: false),
                    access_failed_count = table.Column<int>("integer", nullable: false)
                },
                constraints: table => { table.PrimaryKey("pk_users", x => x.id); });

            migrationBuilder.CreateTable(
                "categories",
                table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>("text", nullable: true),
                    icon_name = table.Column<string>("text", nullable: true)
                },
                constraints: table => { table.PrimaryKey("pk_categories", x => x.id); });

            migrationBuilder.CreateTable(
                "AspNetRoleClaims",
                table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>("integer", nullable: false),
                    claim_type = table.Column<string>("text", nullable: true),
                    claim_value = table.Column<string>("text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claims", x => x.id);
                    table.ForeignKey(
                        "fk_role_claims_asp_net_roles_application_role_id",
                        x => x.role_id,
                        "AspNetRoles",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserClaims",
                table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>("integer", nullable: false),
                    claim_type = table.Column<string>("text", nullable: true),
                    claim_value = table.Column<string>("text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                    table.ForeignKey(
                        "fk_user_claims_asp_net_users_application_user_id",
                        x => x.user_id,
                        "AspNetUsers",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserLogins",
                table => new
                {
                    login_provider = table.Column<string>("text", nullable: false),
                    provider_key = table.Column<string>("text", nullable: false),
                    provider_display_name = table.Column<string>("text", nullable: true),
                    user_id = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        "fk_user_logins_asp_net_users_application_user_id",
                        x => x.user_id,
                        "AspNetUsers",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserRoles",
                table => new
                {
                    user_id = table.Column<int>("integer", nullable: false),
                    role_id = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        "fk_user_roles_asp_net_roles_application_role_id",
                        x => x.role_id,
                        "AspNetRoles",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_user_roles_asp_net_users_application_user_id",
                        x => x.user_id,
                        "AspNetUsers",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserTokens",
                table => new
                {
                    user_id = table.Column<int>("integer", nullable: false),
                    login_provider = table.Column<string>("text", nullable: false),
                    name = table.Column<string>("text", nullable: false),
                    value = table.Column<string>("text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        "fk_user_tokens_asp_net_users_application_user_id",
                        x => x.user_id,
                        "AspNetUsers",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "application_user_category",
                table => new
                {
                    application_users_id = table.Column<int>("integer", nullable: false),
                    categories_id = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_category",
                        x => new { x.application_users_id, x.categories_id });
                    table.ForeignKey(
                        "fk_application_user_category_categories_categories_id",
                        x => x.categories_id,
                        "categories",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_application_user_category_users_application_users_id",
                        x => x.application_users_id,
                        "AspNetUsers",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "images",
                table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_id = table.Column<int>("integer", nullable: false),
                    user_id = table.Column<int>("integer", nullable: true),
                    name = table.Column<string>("text", nullable: true),
                    url = table.Column<string>("text", nullable: true),
                    fully_qualified_public_url = table.Column<string>("text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_images", x => x.id);
                    table.ForeignKey(
                        "fk_images_categories_category_id",
                        x => x.category_id,
                        "categories",
                        "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_images_users_user_id",
                        x => x.user_id,
                        "AspNetUsers",
                        "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "ix_application_user_category_categories_id",
                "application_user_category",
                "categories_id");

            migrationBuilder.CreateIndex(
                "ix_role_claims_role_id",
                "AspNetRoleClaims",
                "role_id");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                "ix_user_claims_user_id",
                "AspNetUserClaims",
                "user_id");

            migrationBuilder.CreateIndex(
                "ix_user_logins_user_id",
                "AspNetUserLogins",
                "user_id");

            migrationBuilder.CreateIndex(
                "ix_user_roles_role_id",
                "AspNetUserRoles",
                "role_id");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                "AspNetUsers",
                "normalized_email");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "AspNetUsers",
                "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                "ix_images_category_id",
                "images",
                "category_id");

            migrationBuilder.CreateIndex(
                "ix_images_user_id",
                "images",
                "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "application_user_category");

            migrationBuilder.DropTable(
                "AspNetRoleClaims");

            migrationBuilder.DropTable(
                "AspNetUserClaims");

            migrationBuilder.DropTable(
                "AspNetUserLogins");

            migrationBuilder.DropTable(
                "AspNetUserRoles");

            migrationBuilder.DropTable(
                "AspNetUserTokens");

            migrationBuilder.DropTable(
                "images");

            migrationBuilder.DropTable(
                "AspNetRoles");

            migrationBuilder.DropTable(
                "categories");

            migrationBuilder.DropTable(
                "AspNetUsers");
        }
    }
}