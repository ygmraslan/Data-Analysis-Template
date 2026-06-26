const errorMessages = {
  // Auth
  'auth.invalid_credentials': 'E-posta adresi veya şifre hatalı.',
  'auth.user_inactive': 'Hesabınız aktif değil. Lütfen yöneticinizle iletişime geçin.',
  'auth.account_locked': 'Hesabınız kilitlenmiştir. Lütfen daha sonra tekrar deneyin.',
  'auth.first_login_password_change_required': 'İlk girişte şifrenizi değiştirmeniz gerekmektedir.',
  'auth.two_factor_setup_required': 'İki faktörlü doğrulama kurulumu gereklidir.',
  'auth.two_factor_code_invalid': 'Doğrulama kodu hatalı veya süresi dolmuş.',
  'auth.invalid_token': 'Geçersiz token. Lütfen tekrar giriş yapın.',
  'auth.token_expired': 'Oturumunuzun süresi dolmuştur. Lütfen tekrar giriş yapın.',
  'auth.token_revoked': 'Oturumunuz sonlandırılmıştır. Lütfen tekrar giriş yapın.',
  'auth.current_password_incorrect': 'Mevcut şifre hatalı.',
  'auth.same_password': 'Yeni şifre mevcut şifreyle aynı olamaz.',
  'auth.mfa_token_already_used': 'MFA oturumunuz zaten kullanıldı veya süresi doldu. Lütfen yeniden giriş yapın.',

  // Users
  'users.user_not_found': 'Kullanıcı bulunamadı.',
  'users.email_already_exists': 'Bu e-posta adresi zaten kullanımda.',
  'users.user_not_locked': 'Kullanıcı kilitli değil.',
  'users.mfa_not_configured': 'Kullanıcıda aktif MFA bulunamadı.',
  'users.user_inactive': 'Pasif kullanıcıya şifre gönderilemez. Önce hesabı aktive edin.',

  // Default
  'default': 'Bir hata oluştu. Lütfen tekrar deneyin.',
}

export const getErrorMessage = (errorCode) => {
  return errorMessages[errorCode] || errorMessages['default']
}

export default errorMessages