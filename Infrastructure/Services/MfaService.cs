using DataAnalysis.Application.Common.Interfaces;
using OtpNet;
using QRCoder;

namespace DataAnalysis.Infrastructure.Services;

public class MfaService : IMfaService
{
    public string GenerateMfaSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string email, string secret)
    {
        return $"otpauth://totp/DataAnalysis:{email}?secret={secret}&issuer=DataAnalysis&digits=6&period=30";
    }

    public string GenerateQrCodeBase64(string uri)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(10);
        return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
    }

    public bool VerifyMfaCode(string secret, string code)
    {
        var secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes);
        return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}