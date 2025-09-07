using System.Security.Cryptography;

namespace Api.Services;

public interface IOAuthStateService
{
    string GenerateState();
    bool ValidateState(string state);
    void CleanupExpiredStates();
}
