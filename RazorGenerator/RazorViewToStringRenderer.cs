using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Temp
{
    public class RazorViewToStringRenderer
    {
        private readonly IRazorViewEngine RazorViewEngine;
        private readonly ITempDataProvider TempDataProvider;
        private readonly IServiceProvider ServiceProvider;
        
        public RazorViewToStringRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            this.RazorViewEngine = razorViewEngine;
            this.TempDataProvider = tempDataProvider;
            this.ServiceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToString(string name, EmailViewModel model)
        {
            var actionContext = GetActionContext();

            var viewEngineResult = this.RazorViewEngine.FindView(actionContext, name, false);

            if(viewEngineResult.Success == false)
            {
                throw new Exception("Not found!");
            }

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<EmailViewModel>(
                        //metadataProvider: new EmptyModelMetadataProvider(),
                        null,
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        this.TempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = this.ServiceProvider
            };

            return new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        }
    }
}