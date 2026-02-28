namespace DigitalWallet.API.Controllers.v1;
/// <summary>
/// Handles KYC (Know your Customer) operations for authenticated users.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class KycController : ControllerBase
{
    private readonly IMediator _mediator;

    public KycController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submits KYC information and identity document.
    /// </summary>
    /// <remarks>
    /// Expects multipart/form-data with document file.
    /// Allowed file types: PDF, JPEG, PNG. Max size: 10 MB
    /// </remarks>
    /// <param name="command">KYC submission data</param>
    [HttpPost("submit")]
    [Consumes("multipart/data-form")]
    [ProducesResponseType(typeof(KycStatusDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<KycStatusDto>> SubmitKyc([FromForm] SubmitKycCommand command)
    {
        var result = await _mediator.Send(command);
        return Accepted(result);
    }

    /// <summary>
    /// Gets the current users's KYC status.
    /// </summary>
    /// <returns>KYC status and submission details if any.</returns>
    [HttpGet("status")]
    [ProducesResponseType(typeof(KycStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<KycStatusDto>> GetKycStatus()
    {
        var query = new GetKycStatusQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
