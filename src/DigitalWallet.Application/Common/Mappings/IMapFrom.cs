namespace DigitalWallet.Application.Common.Mappings;
/// <summary>
/// Interface for entities/DTOs to define AutoMapper mapping.
/// </summary>
public interface IMapFrom<T>
{
    /// <summary>
    /// Configure AutoMapper mapping from T → this type
    /// Override for custom mappings.
    /// </summary>
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
