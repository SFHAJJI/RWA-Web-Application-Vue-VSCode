namespace RWA.Web.Application.Models.Dtos
{
    public sealed class TethysStatusCounts
    {
        public int Total { get; set; }
        public int LookedUp { get; set; }
        public int PendingLookup { get; set; }
        public int Failed { get; set; }
        public int Successful { get; set; }
    }
}

