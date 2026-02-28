using DigitalWallet.API.Filters;

namespace DigitalWallet.API.Controllers.v1;
/// <summary>
/// Handles incoming webhooks from external services (Auth0, payment providers)
/// Endpoints are secured with an API key
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/webhooks")]
[ApiVersion("1.0")]
public class WebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(IMediator mediator, ILogger<WebhookController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Auth0 webhook for user signup events.
    /// </summary>
    /// <param name="payload">Raw JSON payload from Auth0.</param>
    [HttpPost("auth0")]
    [AllowAnonymous]
    [ApiKeyAuth]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Auth0Webhook([FromBody] object payload)
    {
        _logger.LogInformation("Received Auth0 webhook");
        return Ok();
    }

    /// <summary>
    /// Webhook for external payment providers (e.g., deposit confirmations)
    /// </summary>
    /// <param name="payload">Raw JSON payload from the provide.</param>
    [HttpPost("payment-provider")]
    [AllowAnonymous]
    [ApiKeyAuth]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PaymentProviderWebhook([FromBody] object payload)
    {
        _logger.LogInformation("Received payment webhook");
        return Ok();
    }
}
