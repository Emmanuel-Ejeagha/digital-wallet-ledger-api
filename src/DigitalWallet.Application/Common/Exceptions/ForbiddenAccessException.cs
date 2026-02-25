namespace DigitalWallet.Application.Common.Exceptions;

public class ForbiddenAccessException : ApplicationException
{
    public ForbiddenAccessException() : base("You do not have permission to perform this action.")  {  }
}
