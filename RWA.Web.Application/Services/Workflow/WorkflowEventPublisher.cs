using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RWA.Web.Application.Hubs;

namespace RWA.Web.Application.Services.Workflow
{
    public class WorkflowEventPublisher : IWorkflowEventPublisher
    {
        private readonly IHubContext<WorkflowHub> _hubContext;

        public WorkflowEventPublisher(IHubContext<WorkflowHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishWorkflowUpdateAsync(object payload)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveWorkflowUpdate", payload);
        }

        public async Task PublishTransitionAsync(Dtos.TransitionDto transition)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveTransition", transition);
        }
    }
}
