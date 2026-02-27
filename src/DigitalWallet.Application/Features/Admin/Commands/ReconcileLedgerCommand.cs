using System;

namespace DigitalWallet.Application.Features.Admin.Commands;

public class ReconcileLedgerCommand : IRequest<ReconciliationResultDto> { }

public class ReconcileLedgerCommandHandler : IRequestHandler<ReconcileLedgerCommand, ReconciliationResultDto>
{
    private readonly ITransactionRepository _transactionRepository;

    public ReconcileLedgerCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<ReconciliationResultDto> Handle(ReconcileLedgerCommand request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);

        var discripancies = new List<string>();
        decimal totalDebits = 0, totalCredits = 0;

        foreach (var t in transactions)
        {
            var debitSum = t.Entries
                .Where(e => e.Type == EntryType.Debit)
                .Sum(e => e.Amount.Amount);
            var creditSum = t.Entries
                .Where(e => e.Type == EntryType.Credit)
                .Sum(e => e.Amount.Amount);

            if (debitSum != creditSum)
            {
                discripancies.Add(
                    $"Transaction {t.Reference}: Debits {debitSum} != Credits {creditSum}");
            }
            totalDebits += debitSum;
            totalCredits += creditSum;
        }

        return new ReconciliationResultDto
        {
            IsBalanced = discripancies.Count == 0,
            TotalDebits = totalDebits,
            TotalCredits = totalCredits,
            Difference = totalDebits - totalCredits,
            Discripancies = discripancies
        };
    }
}