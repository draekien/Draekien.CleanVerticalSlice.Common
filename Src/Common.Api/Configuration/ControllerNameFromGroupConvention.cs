using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Common.Api.Configuration
{
    public class ControllerNameFromGroupConvention : IControllerModelConvention
    {
        /// <inheritdoc />
        public void Apply(ControllerModel controller)
        {
            if (string.IsNullOrWhiteSpace(controller.ApiExplorer.GroupName)) return;

            controller.ControllerName = controller.ApiExplorer.GroupName;
        }
    }
}
