using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TinyURL.Models;

namespace TinyURL.Filters
{
    public class ValidateInputUriAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.ActionArguments.TryGetValue("model", out var model);
            URLInputModel? uRLModel = model as URLInputModel;
            if (model == null || Uri.IsWellFormedUriString(uRLModel?.Uri ?? "", UriKind.Absolute) == false)
                context.Result = new BadRequestObjectResult(model);
        }
    }
}

