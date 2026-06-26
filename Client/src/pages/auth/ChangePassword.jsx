import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import axiosInstance from '../../api/axiosInstance'
import { newPasswordSchema } from '../../utils/passwordSchema'
import PasswordInput from '../../components/ui/PasswordInput'
import PasswordRules from '../../components/ui/PasswordRules'
import AlertMessage from '../../components/ui/AlertMessage'
import FormButton from '../../components/ui/FormButton'
import Label from '../../components/ui/Label'
import AuthLayout from '../../components/auth/AuthLayout'
import AuthTitle from '../../components/auth/AuthTitle'
import FooterText from '../../components/ui/FooterText'


const schema = z.object({
  currentPassword: z.string().min(1, 'Mevcut şifre zorunludur.'),
  newPassword: newPasswordSchema,
  confirmPassword: z.string().min(1, 'Şifre tekrarı zorunludur.'),
}).refine(data => data.newPassword === data.confirmPassword, {
  message: 'Şifreler uyuşmuyor.',
  path: ['confirmPassword'],
})

export default function ChangePassword() {
  const navigate = useNavigate()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const { register, handleSubmit, watch, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
  })

  const newPasswordValue = watch('newPassword', '')

  const onSubmit = async (data) => {
    setLoading(true)
    setError('')
    try {
      const passwordChangeToken = sessionStorage.getItem('passwordChangeToken')
      const endpoint = passwordChangeToken
        ? '/auth/change-password-with-token'
        : '/auth/change-password'
      await axiosInstance.post(endpoint, {
        currentPassword: data.currentPassword,
        newPassword: data.newPassword,
        confirmPassword: data.confirmPassword,
        ...(passwordChangeToken ? { passwordChangeToken } : {}),
      })
      sessionStorage.removeItem('passwordChangeToken')
      navigate('/login', {
        state: { successMessage: 'Şifreniz başarıyla değiştirildi. Lütfen yeni şifrenizle giriş yapın.' }
      })
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <AuthLayout maxWidth="max-w-3xl">
      <AuthTitle subtitle="Güvenliğiniz için ilk girişte şifrenizi değiştirmeniz gerekmektedir.">
        Şifrenizi Değiştirin
      </AuthTitle>

      <AlertMessage type="error" message={error} />

      <form onSubmit={handleSubmit(onSubmit)} noValidate className="mt-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">

          {/* Sol — Input'lar */}
          <div className="space-y-5">
            <div>
              <Label>Geçici Şifre</Label>
              <PasswordInput
                register={register}
                name="currentPassword"
                size="lg"
                error={errors.currentPassword}
              />
            </div>
            <div>
              <Label>Yeni Şifre</Label>
              <PasswordInput
                register={register}
                name="newPassword"
                size="lg"
                error={errors.newPassword}
              />
            </div>
            <div>
              <Label>Yeni Şifre Tekrar</Label>
              <PasswordInput
                register={register}
                name="confirmPassword"
                size="lg"
                error={errors.confirmPassword}
              />
            </div>

            <FormButton
              loading={loading}
              loadingText="Kaydediliyor..."
              className="w-full h-14 rounded-2xl bg-slate-900 hover:bg-slate-800 text-white text-base hover:-translate-y-0.5 hover:shadow-xl mt-2"
            >
              Şifremi Değiştir ve Devam Et
            </FormButton>
          </div>

          {/* Sağ — Şifre Kuralları */}
          <div className="md:pt-7">
            <PasswordRules value={newPasswordValue} />
          </div>         
        </div>
      </form>
       <FooterText className="mt-6" />
    </AuthLayout>
  )
}