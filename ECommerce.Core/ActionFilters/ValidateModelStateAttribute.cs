using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ECommerce.Core.ActionFilters
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                StringBuilder errors = new StringBuilder();
                foreach (var value in context.ModelState.Values)
                {
                    var errorKey = value.GetType().GetProperty("SubKey").GetValue(value).ToString();
                    errors.AppendLine(errorKey + " " + value.Errors[0].ErrorMessage);
                }
                context.Result = new UnprocessableEntityObjectResult(errors.ToString());
            }
        }
    }
}
