import { useState } from 'react'
import { useNavigate, useLocation, Link } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { HiOutlineMail, HiArrowRight } from 'react-icons/hi'
import authApi from '../../api/authApi'
import useAuthStore from '../../store/authStore'
import AuthCard from '../../components/auth/AuthCard'
import AuthTitle from '../../components/auth/AuthTitle'
import PasswordInput from '../../components/ui/PasswordInput'
import FormButton from '../../components/ui/FormButton'
import AlertMessage from '../../components/ui/AlertMessage'
import Label from '../../components/ui/Label'
import FooterText from '../../components/ui/FooterText'


const schema = z.object({
  email: z.string().min(1, 'E-posta zorunludur.').email('Geçerli bir e-posta adresi giriniz.'),
  password: z.string().min(1, 'Şifre zorunludur.'),
})

export default function Login() {
  const navigate = useNavigate()
  const location = useLocation()
  const { setAuthenticated } = useAuthStore()
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState(null)

  const successMessage = location.state?.successMessage || null

  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
  })

  const onSubmit = async (data) => {
    setError(null)
    setIsLoading(true)
    try {
      const response = await authApi.login(data)
      const result = response.data.value
      if (result.requiresPasswordChange) {
        sessionStorage.setItem('passwordChangeToken', result.passwordChangeToken)
        navigate('/change-password')
        return
      }
      if (result.requiresMfaSetup) { sessionStorage.setItem('mfaToken', result.mfaToken); navigate('/mfa-setup'); return }
      if (result.requiresMfa) { sessionStorage.setItem('mfaToken', result.mfaToken); navigate('/mfa'); return }
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
      <AuthTitle className="mb-6">Giriş Yapın</AuthTitle>

      {successMessage && (
        <div className="mb-6">
          <AlertMessage type="success" message={successMessage} />
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <div className="mb-6">
          <Label>E-posta Adresi</Label>
          <div className="relative">
            <input
              {...register('email')}
              type="text"
              placeholder="name.surname@example.com"
              className={`w-full h-16 rounded-2xl pl-14 pr-5 text-base font-medium text-slate-900 bg-slate-50 outline-none transition-all border-2
                ${errors.email ? 'border-red-400' : 'border-slate-100 focus:border-emerald-400'}
                focus:bg-white focus:shadow-[0_0_0_4px_rgba(52,211,153,0.08)]`}
            />
            <span className="absolute left-5 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none">
              <HiOutlineMail size={22} />
            </span>
          </div>
          {errors.email && <p className="text-sm text-red-500 mt-2">{errors.email.message}</p>}
        </div>

        <div className="mb-6">
          <Label>Şifre</Label>
          <PasswordInput
            register={register}
            name="password"
            size="lg"
            error={errors.password}
          />
        </div>

        <div className="flex justify-end mb-8">
          <Link to="/forgot-password" className="text-sm font-semibold text-emerald-400 hover:text-emerald-600 transition-colors">
            Şifrenizi mi unuttunuz?
          </Link>
        </div>

        <FormButton
          loading={isLoading}
          loadingText="Doğrulanıyor..."
          className="w-full h-16 rounded-2xl bg-slate-900 hover:bg-slate-800 text-white text-base hover:-translate-y-0.5 hover:shadow-xl"
        >
          Giriş Yap
          <HiArrowRight size={20} />
        </FormButton>
      </form>
           <FooterText className="mt-6" />
    </AuthCard>
    
  )
}