namespace DigitalWallet.API.Controllers.v1;
/// <summary>
/// Endpoints for financial transactions: transfer, deposit, withdraw, and history
/// </summary>
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Transfers money between two accounts.
    /// </summary>
    /// <param name="command">Transfer details including idempotency key.</param>
    [HttpPost("transfer")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TransactionDto>> Transfer([FromBody] TransferCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deposits money into an account (from system reserve).
    /// </summary>
    /// <param name="command">Deposit details including idempotency key</param>
    [HttpPost("deposit")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TransactionDto>> Deposit([FromBody] DepositCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Withdraws money from an account (to system payout)
    /// </summary>
    /// <param name="command">Withdrawal details including idempotency key.</param>
    [HttpPost("withdraw")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TransactionDto>> Withdraw([FromBody] WithdrawCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Gets paginated transactions history for an account.
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <param name="from">Start date (optional).</param>
    /// <param name="to">End date (optional)</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 20)</param>
    [HttpGet("history/{accountId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetHistory(
        Guid accountId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetTransactionHistoryQuery
        {
            AccountId = accountId,
            From = from,
            To = to,
            Page = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

