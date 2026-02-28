namespace DigitalWallet.API.Controllers.v1;
/// <summary>
/// Provides authentication-related configuration to the frontend.
/// Actual authentication is handled by Auth0's Universal Login.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly Auth0Settings _auth0Settings;

    public AuthenticationController(IMediator mediator, IOptions<Auth0Settings> auth0Settings)
    {
        _mediator = mediator;
        _auth0Settings = auth0Settings.Value;
    }

    /// <summary>
    /// Returns the Auth0 configuration (domain, clientId, audience) for the frontend.
    /// </summary>
    [HttpGet("config")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetConfig()
    {
        return Ok(new
        {
            domain = _auth0Settings.Domain,
            clientId = _auth0Settings.ClientId,
            audience = _auth0Settings.Audience
        });
    }
}

/// <summary>
/// Auth0 settings from configuration.
/// </summary>
public class Auth0Settings
{
    public string Domain { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}