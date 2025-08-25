using FluentValidation;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Validation
{
    public class ObligationValidator : AbstractValidator<HecateInventaireNormalise>
    {
        public ObligationValidator()
        {
            RuleFor(x => x.DateMaturite).NotNull().WithMessage("DateMaturite is required for OBL items.");
            RuleFor(x => x.TauxObligation).NotNull().WithMessage("TauxObligation is required for OBL items.");
        }
    }
}
