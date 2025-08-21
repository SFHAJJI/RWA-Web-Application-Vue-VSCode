using System;

namespace RWA.Web.Application.Services.Validation
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class SupportedWorkflowStepAttribute : Attribute
	{
		public string StepName { get; }

		public SupportedWorkflowStepAttribute(string stepName)
		{
			StepName = stepName;
		}
	}
}
