using FluentValidation;
using SACA.Models.Requests;

namespace SACA.Validators
{
    public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Usuário não informado");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email não informado")
                .EmailAddress().WithMessage("Endereço de email inválido");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Senha não informada")
                .MinimumLength(6).WithMessage("Senha menor que 6 caracteres");

            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Senhas não conferem");
        }
    }
}
