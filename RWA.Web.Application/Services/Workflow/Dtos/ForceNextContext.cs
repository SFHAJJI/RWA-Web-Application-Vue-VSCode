using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Workflow.Dtos
{
    /// <summary>
    /// Context for force next operations in the state machine
    /// </summary>
    public class ForceNextContext
    {
        /// <summary>
        /// Whether the state machine can fire the Next trigger
        /// </summary>
        public bool CanFireNext { get; set; }

        /// <summary>
        /// Whether to use fallback DB-driven transition
        /// </summary>
        public bool ShouldUseFallback => !CanFireNext;
    }
}
