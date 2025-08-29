using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using RWA.Web.Application.Models;
using System.Collections.Generic;

namespace RWA.Web.Application.Hubs
{
    public class WorkflowHub : Hub
    {
        public async Task SendWorkflowUpdate(IEnumerable<WorkflowStep> workflowSteps)
        {
            await Clients.All.SendAsync("ReceiveWorkflowUpdate", workflowSteps);
        }

        public async Task SendToast(string level, string message, string? actionLabel = null, string? actionToken = null)
        {
            await Clients.All.SendAsync("ReceiveToast", new { level, message, actionLabel, actionToken });
        }

        public async Task SendTethysUpdate(RWA.Web.Application.Models.Dtos.HecateTethysDto tethysDto)
        {
            await Clients.All.SendAsync("ReceiveTethysUpdate", tethysDto);
        }
    }
}
