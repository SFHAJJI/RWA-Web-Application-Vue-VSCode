using System.Text.Json.Serialization;

namespace RWA.Web.Application.Models.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BddMatchStatus
    {
        Ok,
        Ko,
        NotApplicable
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BddMatchType
    {
        IdentifiantOrigineBDD,
        UniqueRetenuBDD,
        NotMatched,
        NotApplicable
    }

    public class AuditInventoryDto
    {
        public int NumLigne { get; set; }
        public BddMatchStatus MatchedBDD { get; set; }
        public BddMatchType BDDMatch { get; set; }
    }
}
