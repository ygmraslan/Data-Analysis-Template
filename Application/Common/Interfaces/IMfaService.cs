namespace DataAnalysis.Application.Common.Interfaces;

public interface IMfaService
{
    string GenerateMfaSecret();
    string GenerateQrCodeUri(string email, string secret);
    string GenerateQrCodeBase64(string uri);
    bool VerifyMfaCode(string secret, string code);
}