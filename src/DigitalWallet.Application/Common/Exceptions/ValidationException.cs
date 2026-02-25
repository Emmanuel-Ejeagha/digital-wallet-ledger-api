namespace DigitalWallet.Application.Common.Exceptions;

public class ValidationException : ApplicationException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>());
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = new ReadOnlyDictionary<string, string[]>(
             failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray())
        );
    }
}
