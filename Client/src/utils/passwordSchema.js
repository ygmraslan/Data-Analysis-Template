import { z } from 'zod'

export const passwordRules = [
  { label: 'En az 8 karakter', test: (v) => v.length >= 8 },
  { label: 'En az bir büyük harf', test: (v) => /[A-Z]/.test(v) },
  { label: 'En az bir küçük harf', test: (v) => /[a-z]/.test(v) },
  { label: 'En az bir rakam', test: (v) => /[0-9]/.test(v) },
  { label: 'En az bir özel karakter', test: (v) => /[^a-zA-Z0-9]/.test(v) },
]

export const newPasswordSchema = z.string()
  .min(8, 'En az 8 karakter olmalıdır.')
  .regex(/[A-Z]/, 'En az bir büyük harf içermelidir.')
  .regex(/[a-z]/, 'En az bir küçük harf içermelidir.')
  .regex(/[0-9]/, 'En az bir rakam içermelidir.')
  .regex(/[^a-zA-Z0-9]/, 'En az bir özel karakter içermelidir.')

export const confirmPasswordSchema = (passwordField = 'newPassword') =>
  z.object({
    confirmPassword: z.string().min(1, 'Şifre tekrarı zorunludur.'),
  }).refine((data) => data[passwordField] === data.confirmPassword, {
    message: 'Şifreler uyuşmuyor.',
    path: ['confirmPassword'],
  })