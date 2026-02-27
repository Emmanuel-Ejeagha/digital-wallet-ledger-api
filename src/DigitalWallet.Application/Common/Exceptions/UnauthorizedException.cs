namespace DigitalWallet.Application.Common.Exceptions;

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(): base("You are not authenticated.") {}
}
