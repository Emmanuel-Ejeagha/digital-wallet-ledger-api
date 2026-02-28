namespace DigitalWallet.API.Controllers.v1;
/// <summary>
/// Adminmistrative endpoints for managing users, system accounts, and KYC reviews.
/// Only accessible by users with "Admin" role.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of all users.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of users per page (default: 20).</param>
    /// <returns>List of user DTOs.</returns>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetAllUsersQuery { Page = page, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all system accounts (reserve and fee accounts).
    /// </summary>
    /// <returns>List of account DTOs.</returns>
    [HttpGet("system-accounts")]
    [ProducesResponseType(typeof(List<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<AccountDto>>> GetSystemAccounts()
    {
        var query = new GetSystemAccountsQuery();
        var result = _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Performs a ledger reconciliation check, verifing that all transactions are balanced.
    /// </summary>
    /// <returns>Reconcilaition result with discrepancies if any.</returns>
    [HttpPost("reconcile")]
    [ProducesResponseType(typeof(ReconciliationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReconciliationResultDto>> Reconcile()
    {
        var command = new ReconcileLedgerCommand();
        var result = _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves KYC submissions with optional status filtering and pagination.
    /// </summary>
    /// <param name="status">Filter by KYC status (optional).</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 20).</param>
    /// <returns>List of KYC submission DTOs.</returns>
    [HttpGet("kyc-submissions")]
    [ProducesResponseType(typeof(List<KycSubmissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<KycSubmissionDto>>> GetKycSubmissions(
        [FromQuery] KycStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetKycSubmissionsQuery { Status = status, Page = page, PageSize = page };
        var result = _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Approves a KYC submission.
    /// </summary>
    /// <param name="submissionId">ID of the KYC submission to approve.</param>description
    [HttpPost("kyc-submissions/{submissionId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveKyc(Guid submissionId)
    {
        var command = new ApproveKycCommand { SubmissionId = submissionId };
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Rejects a KYC subnission with notes.
    /// </summary>
    /// <param name="submissionId">ID of the KYC submission to reject.</param>
    /// <param name="notes">Rejection reason.</param>
    [HttpPost("kyc-submissions/{subnissionId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectKyc(Guid submissionId, [FromBody] string notes)
    {
        var command = new RejectKycCommand { SubmissionId = submissionId, Notes = notes };
        await _mediator.Send(command);
        return NoContent();
    }
}
