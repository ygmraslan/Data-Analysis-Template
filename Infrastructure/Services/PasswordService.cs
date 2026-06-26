using DataAnalysis.Application.Common.Interfaces;

namespace DataAnalysis.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private static readonly HashSet<string> CommonPasswords = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "123456", "12345678", "qwerty", "abc123", "password1",
        "password123", "admin", "letmein", "welcome", "monkey", "dragon",
        "master", "login", "princess", "starwars", "passw0rd", "shadow",
        "sunshine", "michael", "football", "superman", "iloveyou", "trustno1",
        "000000", "111111", "123123", "654321", "666666", "121212", "123456789",
        "qwerty123", "qwertyuiop", "1q2w3e4r", "1234567890", "987654321"
    };

    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12); 

    public bool VerifyPassword(string password, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(password, passwordHash);

    public bool ValidatePasswordStrength(string password)
    {
        var result = ValidatePasswordWithDetails(password);
        return result.IsValid;
    }
    public PasswordValidationResult ValidatePasswordWithDetails(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required.");
            return new PasswordValidationResult(false, errors);
        }

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long.");

        if (password.Length > 128)
            errors.Add("Password must not exceed 128 characters.");

        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter.");

        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter.");

        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit.");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            errors.Add("Password must contain at least one special character.");

        if (HasRepeatingCharacters(password, 3))
            errors.Add("Password must not contain 3 or more repeating characters.");

        if (IsCommonPassword(password))
            errors.Add("Password is too common. Please choose a stronger password.");

        if (HasKeyboardPattern(password))
            errors.Add("Password must not contain keyboard patterns.");

        return new PasswordValidationResult(errors.Count == 0, errors);
    }

    public string GenerateTemporaryPassword()
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";
        const string allChars = uppercase + lowercase + digits + special;

        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var password = new char[10];

        password[0] = uppercase[GetRandomIndex(rng, uppercase.Length)];
        password[1] = lowercase[GetRandomIndex(rng, lowercase.Length)];
        password[2] = digits[GetRandomIndex(rng, digits.Length)];
        password[3] = special[GetRandomIndex(rng, special.Length)];

        for (int i = 4; i < 10; i++)
            password[i] = allChars[GetRandomIndex(rng, allChars.Length)];

        return new string(password.OrderBy(_ => GetRandomIndex(rng, 100)).ToArray());
    }

    private static int GetRandomIndex(System.Security.Cryptography.RandomNumberGenerator rng, int max)
    {
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        return (int)(BitConverter.ToUInt32(bytes, 0) % (uint)max);
    }

    private static bool HasRepeatingCharacters(string password, int maxRepeat)
    {
        for (int i = 0; i <= password.Length - maxRepeat; i++)
        {
            if (password.Skip(i).Take(maxRepeat).Distinct().Count() == 1)
                return true;
        }
        return false;
    }

    private static bool IsCommonPassword(string password)
    {
        if (CommonPasswords.Contains(password))
            return true;

        var basePassword = new string(password.Where(char.IsLetter).ToArray());
        if (CommonPasswords.Contains(basePassword))
            return true;

        return false;
    }

    private static bool HasKeyboardPattern(string password)
    {
        var patterns = new[]
        {
            "qwerty", "asdfgh", "zxcvbn", "qazwsx", "123456", "654321",
            "abcdef", "fedcba", "qweasd", "asdqwe"
        };

        var lowerPassword = password.ToLower();
        return patterns.Any(p => lowerPassword.Contains(p));
    }
}

public record PasswordValidationResult(bool IsValid, List<string> Errors)
{
    public string ErrorMessage => string.Join(" ", Errors);
}