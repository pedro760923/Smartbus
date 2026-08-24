namespace SmartBus.Application.Abstractions;

/// <summary>
/// Abstrai o algoritmo de hash de senha (BCrypt hoje) para que a
/// Application não dependa diretamente da lib concreta.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verify(string senha, string hash);
}
