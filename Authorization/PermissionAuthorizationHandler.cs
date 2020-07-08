﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SACA.Models;
using System.Linq;
using System.Threading.Tasks;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public PermissionAuthorizationHandler(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Modificar completamente para validar pelo Token
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);

            // Se não estiver logado retorna não autorizado
            if (user is null)
            {
                return;
            }

            // Autoriza se for Superusuário
            if (await _userManager.IsInRoleAsync(user, Roles.Superuser))
            {
                context.Succeed(requirement);
                return;
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userPermissions = userClaims.Where(x =>
                    x.Type == CustomClaimTypes.Permission &&
                    x.Value == requirement.Permission &&
                    x.Issuer == "LOCAL AUTHORITY"
                ).Select(x => x.Value);

            // Autoriza se o usuário possuir a permissão requerida
            if (userPermissions.Any())
            {
                context.Succeed(requirement);
                return;
            }

            var userRoleNames = await _userManager.GetRolesAsync(user);
            var userRoles = _roleManager.Roles.Where(x => userRoleNames.Contains(x.Name));

            foreach (var role in userRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                var rolePermissions = roleClaims.Where(x =>
                    x.Type == CustomClaimTypes.Permission &&
                    x.Value == requirement.Permission &&
                    x.Issuer == "LOCAL AUTHORITY"
                ).Select(x => x.Value);

                // Autoriza se alguma role do usuário possir a permissão requerida
                if (rolePermissions.Any())
                {
                    context.Succeed(requirement);
                    return;
                }
            }
        }
    }
}
