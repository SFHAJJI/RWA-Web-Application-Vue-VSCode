using FluentValidation;
using RWA.Web.Application.Models;
using System;
using System.Globalization;

namespace RWA.Web.Application.Services.Validation
{
    public class ObligationValidator : AbstractValidator<HecateInventaireNormalise>
    {
        public ObligationValidator()
        {
            RuleFor(x => x.TauxObligation)
                .NotNull()
                .WithMessage("TauxObligation must not be null.");

            RuleFor(x => x.DateMaturite)
                .Must((item, dateMaturite) => BeOnOrAfterPeriodeCloture(dateMaturite, item.PeriodeCloture))
                .When(x => x.DateMaturite.HasValue)
                .WithMessage("DateMaturite must be on or after the last day of the PeriodeCloture month.");
        }

        private bool BeOnOrAfterPeriodeCloture(DateOnly? dateMaturite, string periodeCloture)
        {
            if (!dateMaturite.HasValue || string.IsNullOrEmpty(periodeCloture) || periodeCloture.Length != 6)
            {
                return true;
            }

            try
            {
                int month = int.Parse(periodeCloture.Substring(0, 2));
                int year = int.Parse(periodeCloture.Substring(2, 4));
                var lastDayOfMonth = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

                return dateMaturite.Value >= lastDayOfMonth;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
