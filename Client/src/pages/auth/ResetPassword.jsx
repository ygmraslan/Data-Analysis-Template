import { useState } from 'react'
import { useNavigate, useSearchParams, Link } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { HiArrowRight } from 'react-icons/hi'
import authApi from '../../api/authApi'
import AuthCard from '../../components/auth/AuthCard'
import AuthTitle from '../../components/auth/AuthTitle'
import { newPasswordSchema } from '../../utils/passwordSchema'
import PasswordInput from '../../components/ui/PasswordInput'
import AlertMessage from '../../components/ui/AlertMessage'
import FormButton from '../../components/ui/FormButton'
import FooterText from '../../components/ui/FooterText'

const schema = z.object({
  password: newPasswordSchema,
  confirmPassword: z.string().min(1, 'Şifre tekrarı zorunludur.'),
}).refine(data => data.password === data.confirmPassword, {
  message: 'Şifreler uyuşmuyor.',
  path: ['confirmPassword'],
})

export default function ResetPassword() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState(null)
  const [isSuccess, setIsSuccess] = useState(false)

  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
  })

  const onSubmit = async (data) => {
    const token = searchParams.get('token')
    if (!token) {
      setError('Geçersiz şifre sıfırlama bağlantısı.')
      navigate('/login')
      return
    }

    setError(null)
    setIsLoading(true)
    try {
      await authApi.resetPassword({ token, newPassword: data.password, confirmPassword: data.confirmPassword })
      setIsSuccess(true)
      setTimeout(() => navigate('/login'), 2000)
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AuthCard error={error}>
      <AuthTitle subtitle="Yeni şifrenizi belirleyin.">
        Şifre Sıfırla
      </AuthTitle>

      <AlertMessage type="success" message={isSuccess ? 'Şifreniz başarıyla sıfırlandı. Giriş sayfasına yönlendiriliyorsunuz...' : ''} />

      <form onSubmit={handleSubmit(onSubmit)} noValidate className="mt-4 space-y-6">
        <PasswordInput
          register={register}
          name="password"
          label="Yeni Şifre"
          size="lg"
          error={errors.password}
        />
        <PasswordInput
          register={register}
          name="confirmPassword"
          label="Şifre Tekrar"
          size="lg"
          error={errors.confirmPassword}
        />

        <FormButton
          loading={isLoading}
          disabled={isSuccess}
          loadingText="Kaydediliyor..."
          className="w-full h-16 rounded-2xl bg-slate-900 hover:bg-slate-800 text-white text-base hover:-translate-y-0.5 hover:shadow-xl"
        >
          Şifreyi Sıfırla
          <HiArrowRight size={20} />
        </FormButton>

        <div className="flex justify-center">
          <Link to="/login" className="text-sm font-semibold text-emerald-400 hover:text-emerald-600 transition-colors">
            Giriş sayfasına dön
          </Link>
        </div>
      </form>
      <FooterText className="mt-6" />
    </AuthCard>
  )
}