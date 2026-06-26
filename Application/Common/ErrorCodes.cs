namespace DataAnalysis.Application.Common;

public static class ErrorCodes
{
    public static class Auth
    {
        public const string InvalidCredentials = "auth.invalid_credentials";
        public const string UserInactive = "auth.user_inactive";
        public const string AccountLocked = "auth.account_locked";
        public const string FirstLoginPasswordChangeRequired = "auth.first_login_password_change_required";
        public const string TwoFactorSetupRequired = "auth.two_factor_setup_required";
        public const string TwoFactorCodeInvalid = "auth.two_factor_code_invalid";
        public const string InvalidToken = "auth.invalid_token";
        public const string TokenExpired = "auth.token_expired";
        public const string TokenRevoked = "auth.token_revoked";
        public const string CurrentPasswordIncorrect = "auth.current_password_incorrect";
        public const string SamePassword = "auth.same_password";
        public const string MfaTokenAlreadyUsed = "auth.mfa_token_already_used";

    }

    public static class Users
    {
        public const string UserNotFound = "users.user_not_found";
        public const string EmailAlreadyExists = "users.email_already_exists";
        public const string UserNotLocked = "users.user_not_locked";
        public const string MfaNotConfigured = "users.mfa_not_configured";
        public const string UserInactive = "users.user_inactive";
    }
}