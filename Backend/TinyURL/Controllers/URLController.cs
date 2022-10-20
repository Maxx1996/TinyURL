using Microsoft.AspNetCore.Mvc;
using TinyURL.Filters;
using TinyURL.Models;
using TinyURL.Services;

namespace TinyURL.Controllers;

[ApiController]
public class URLController : ControllerBase
{ 
    private readonly IURLService uRLService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public URLController(IURLService uRLService, IHttpContextAccessor httpContextAccessor)
    {
        this.uRLService = uRLService;
        this.httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("tinyUrl/create")]
    [ValidateInputUri]
    public async Task<IActionResult> Create([FromBody]URLInputModel model)
    {
        return new OkObjectResult(
            httpContextAccessor?.HttpContext?.Request.Headers["Origin"].ToString() +
            "/" +
            await uRLService.GetTinyUrl(model));
    }

    [HttpGet("{codedUri}")]
    public async Task<IActionResult> Get([FromRoute]string codedUri)
    {
        string longUri = await uRLService.GetLongUrl(codedUri);
        return string.IsNullOrEmpty(longUri) ? NotFound() : Redirect(longUri);
    }
}

