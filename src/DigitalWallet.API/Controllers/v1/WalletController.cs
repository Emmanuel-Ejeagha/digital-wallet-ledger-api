namespace DigitalWallet.API.Controllers.v1;
/// <summary>
/// Manages user wallet accounts.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class WalletController : ControllerBase
{
    private readonly IMediator _mediator;

    public WalletController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new wallet account for the authenticated user.
    /// </summary>
    /// <param name="command">Account creation details including idempotency key.</param>
    [HttpPost("accounts")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAccountBalance), new { accountId = result.Id }, result);
    }

    /// <summary>
    /// Gets the balance and details of a specific account.
    /// </summary>
    [HttpGet("accounts/{accountId:guid}/balance")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> GetAccountBalance(Guid accountid)
    {
        var query = new GetAccountBalanceQuery { AccountId = accountid };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Lists all accounts belonging to the current user.
    /// </summary>
    [HttpGet("accounts")]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetMyAccounts()
    {
        var query = new GetUserAccountQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
