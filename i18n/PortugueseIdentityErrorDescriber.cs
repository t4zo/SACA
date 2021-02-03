using Microsoft.AspNetCore.Identity;

namespace SACA.i18n
{
    public class PortugueseIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new() {Code = nameof(DefaultError), Description = "Um erro desconhecido ocorreu."};
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new()
            {
                Code = nameof(ConcurrencyFailure),
                Description = "Falha de concorrência otimista, o objeto foi modificado."
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new() {Code = nameof(PasswordMismatch), Description = "Senha incorreta."};
        }

        public override IdentityError InvalidToken()
        {
            return new() {Code = nameof(InvalidToken), Description = "Token inválido."};
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new() {Code = nameof(LoginAlreadyAssociated), Description = "Já existe um usuário com este login."};
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new()
            {
                Code = nameof(InvalidUserName),
                Description = $"Login '{userName}' é inválido, pode conter apenas letras ou dígitos."
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new() {Code = nameof(InvalidEmail), Description = $"Email '{email}' é inválido."};
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new() {Code = nameof(DuplicateUserName), Description = $"Login '{userName}' já está sendo utilizado."};
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new() {Code = nameof(DuplicateEmail), Description = $"Email '{email}' já está sendo utilizado."};
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new() {Code = nameof(InvalidRoleName), Description = $"A permissão '{role}' é inválida."};
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new() {Code = nameof(DuplicateRoleName), Description = $"A permissão '{role}' já está sendo utilizada."};
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new() {Code = nameof(UserAlreadyHasPassword), Description = "Usuário já possui uma senha definida."};
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new() {Code = nameof(UserLockoutNotEnabled), Description = "Lockout não está habilitado para este usuário."};
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new() {Code = nameof(UserAlreadyInRole), Description = $"Usuário já possui a permissão '{role}'."};
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new() {Code = nameof(UserNotInRole), Description = $"Usuário não tem a permissão '{role}'."};
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new() {Code = nameof(PasswordTooShort), Description = $"Senhas devem conter ao menos {length} caracteres."};
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new()
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Senhas devem conter ao menos um caracter não alfanumérico."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new()
            {
                Code = nameof(PasswordRequiresDigit), Description = "Senhas devem conter ao menos um digito ('0'-'9')."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new()
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Senhas devem conter ao menos um caracter em caixa baixa ('a'-'z')."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new()
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "Senhas devem conter ao menos um caracter em caixa alta ('A'-'Z')."
            };
        }
    }
}