using System;
using DigitalWallet.Application.Common.Mappings;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Application.DTOs;
/// <summary>Data tansfer object for Currency value object.</summary>
public class CurrencyDto : IMapFrom<Currency>
{
    public string Code { get; set; } = string.Empty;
    public int DecimalPlaces { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
