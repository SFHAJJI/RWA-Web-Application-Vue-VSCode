namespace RWA.Web.Application.Models.Dtos
{
    public class AdditionalInformation
    {
        public AdditionalInformation()
        {
            AddtoBDDDto = new AddtoBDDDto();
        }
        public bool IsValeurMobiliere { get; set; }
        public AddtoBDDDto AddtoBDDDto { get; set; }
        public bool TethysRafStatus { get; set; }
        public string RafOrigin { get; set; }
    }

    public class AddtoBDDDto
    {
        public bool AddToBDD { get; set; }
        public bool IsMappedByIdUniqueRetenu { get; set; }
        public bool IsMappedByIdOrigine { get; set; }
        
    }
}
