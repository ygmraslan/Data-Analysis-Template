import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { HiArrowRight } from 'react-icons/hi'
import authApi from '../../api/authApi'
import useAuthStore from '../../store/authStore'
import AuthCard from '../../components/auth/AuthCard'
import AuthTitle from '../../components/auth/AuthTitle'
import FormButton from '../../components/ui/FormButton'
import Label from '../../components/ui/Label'
import FooterText from '../../components/ui/FooterText'

const schema = z.object({
  code: z.string().length(6, 'Doğrulama kodu 6 haneli olmalıdır.'),
})

export default function Mfa() {
  const navigate = useNavigate()
  const { setAuthenticated } = useAuthStore()
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState(null)

  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
  })

  useEffect(() => {
    const mfaToken = sessionStorage.getItem('mfaToken')
    if (!mfaToken) navigate('/login')
  }, [navigate])

  const onSubmit = async (data) => {
    const mfaToken = sessionStorage.getItem('mfaToken')
    if (!mfaToken) { navigate('/login'); return }

    setError(null)
    setIsLoading(true)
    try {
      const response = await authApi.confirmMfa({ mfaToken, mfaCode: data.code })
      const result = response.data.value

      sessionStorage.removeItem('mfaToken')

      if (result.isSetupComplete) {
        navigate('/login', {
          state: { successMessage: 'İki faktörlü doğrulama kurulumu tamamlandı. Lütfen giriş yapın.' }
        })
        return
      }

      setAuthenticated(true)
      navigate('/dashboard')
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AuthCard error={error}>
      <AuthTitle subtitle="Microsoft Authenticator uygulamasındaki 6 haneli kodu girin.">
        Doğrulama Kodu
      </AuthTitle>

      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <div className="mb-8">
          <Label>Doğrulama Kodu</Label>
          <input
            {...register('code')}
            type="text"
            inputMode="numeric"
            maxLength={6}
            placeholder="000000"
            className={`w-full h-16 rounded-2xl px-5 text-2xl font-mono font-bold text-center text-slate-900 bg-slate-50 outline-none transition-all border-2 tracking-widest
              ${errors.code ? 'border-red-400' : 'border-slate-100 focus:border-emerald-400'}
              focus:bg-white focus:shadow-[0_0_0_4px_rgba(52,211,153,0.08)]`}
          />
          {errors.code && <p className="text-sm text-red-500 mt-2">{errors.code.message}</p>}
        </div>

        <FormButton
          loading={isLoading}
          loadingText="Doğrulanıyor..."
          className="w-full h-16 rounded-2xl bg-slate-900 hover:bg-slate-800 text-white text-base hover:-translate-y-0.5 hover:shadow-xl"
        >
          Doğrula
          <HiArrowRight size={20} />
        </FormButton>
      </form>
      <FooterText className="mt-6" />
    </AuthCard>
  )
}