namespace DigitalWallet.API.Controllers.v1;
/// <summary>
/// Handles authentication related endpoints (login, token exchange, etc.)
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
    /// Returns Auth0 configuration for the frontend (domain, clientId, audience).
    /// </summary>
    [HttpGet("config")]
    [AllowAnonymous]
    public IActionResult GetConfig()
    {
        return Ok(new
        {
            domain = _auth0Settings.Domain,
            clientId = _auth0Settings.ClientId,
            audience = _auth0Settings.Audience
        });
    }

    /// <summary>
    /// Optional endpoint to exchange an authorization code for token (if not using Auth0's built-in).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginCommand command)
    {
        return BadRequest("Please use Auth0's Universal Login directly.");
    }
}

public class Auth0Settings
{
    public string Domain { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}