using System;
using Mono.TextTemplating;

namespace RWA.Web.Application.Services.Workflow.Dtos
{
    public class TransitionDto
    {
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Trigger { get; set; } = string.Empty;
        public DateTime OccurredAtUtc { get; set; }
    }
}
