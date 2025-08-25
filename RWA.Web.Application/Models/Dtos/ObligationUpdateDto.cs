using System;

namespace RWA.Web.Application.Models.Dtos
{
    public class ObligationUpdateDto
    {
        public int NumLigne { get; set; }
        public DateOnly DateMaturite { get; set; }
        public decimal TauxObligation { get; set; }
    }
}
