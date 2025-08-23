using System.Collections.Generic;
using System.Linq;

namespace RWA.Web.Application.Services.Workflow.Dtos
{
    public class AggregateUploadResultContext
    {
        public List<UploadResultContext> Results { get; set; } = new List<UploadResultContext>();
        public bool IsSuccess => Results.All(r => r.IsSuccess);
    }
}
