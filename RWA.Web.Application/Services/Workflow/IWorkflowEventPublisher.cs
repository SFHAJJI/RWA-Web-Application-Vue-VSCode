using System.Threading.Tasks;

namespace RWA.Web.Application.Services.Workflow
{
    public interface IWorkflowEventPublisher
    {
        Task PublishWorkflowUpdateAsync(object payload);
        Task PublishTransitionAsync(Dtos.TransitionDto transition);
    }
}
