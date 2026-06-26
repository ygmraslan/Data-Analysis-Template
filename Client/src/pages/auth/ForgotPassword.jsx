import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { HiOutlineMail, HiArrowRight } from 'react-icons/hi'
import authApi from '../../api/authApi'
import AuthCard from '../../components/auth/AuthCard'
import AuthTitle from '../../components/auth/AuthTitle'
import FormButton from '../../components/ui/FormButton'
import Label from '../../components/ui/Label'
import FooterText from '../../components/ui/FooterText'

const schema = z.object({
  email: z.string().min(1, 'E-posta zorunludur.').email('Geçerli bir e-posta adresi giriniz.'),
})

export default function ForgotPassword() {
  const [isLoading, setIsLoading] = useState(false)
  const [isSent, setIsSent] = useState(false)
  const [error, setError] = useState(null)

  const { register, handleSubmit, getValues, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
  })

  const onSubmit = async (data) => {
    setError(null)
    setIsLoading(true)
    try {
      await authApi.forgotPassword(data)
      setIsSent(true)
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AuthCard error={error}>
      {isSent ? (
        <>
          <div className="w-14 h-14 rounded-2xl bg-emerald-50 flex items-center justify-center mb-8">
            <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="#34d399" strokeWidth="2">
              <path d="M22 16.92v3a2 2 0 01-2.18 2 19.79 19.79 0 01-8.63-3.07A19.5 19.5 0 013.07 9.81a19.79 19.79 0 01-3.07-8.67A2 2 0 012 0h3a2 2 0 012 1.72c.127.96.361 1.903.7 2.81a2 2 0 01-.45 2.11L6.09 7.91a16 16 0 006 6l1.27-1.27a2 2 0 012.11-.45c.907.339 1.85.573 2.81.7A2 2 0 0122 14.92z" />
            </svg>
          </div>
          <AuthTitle subtitle={`${getValues('email')} adresine şifre sıfırlama bağlantısı gönderdik. Gelen kutunuzu kontrol edin.`}>
            E-posta Gönderildi
          </AuthTitle>
          <Link
            to="/login"
            className="w-full h-16 flex items-center justify-center gap-2 rounded-2xl bg-slate-900 hover:bg-slate-800 text-white text-base font-semibold transition-all hover:-translate-y-0.5 hover:shadow-xl"
          >
            Giriş Sayfasına Dön
            <HiArrowRight size={20} />
          </Link>
        </>
      ) : (
        <>
          <AuthTitle subtitle="Şifrenizi sıfırlamak için sisteme kayıtlı e-posta adresinizi giriniz.">
            Şifremi Unuttum
          </AuthTitle>

          <form onSubmit={handleSubmit(onSubmit)} noValidate>
            <div className="mb-8">
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

            <FormButton
              loading={isLoading}
              loadingText="Gönderiliyor..."
              className="w-full h-16 rounded-2xl bg-slate-900 hover:bg-slate-800 text-white text-base hover:-translate-y-0.5 hover:shadow-xl mb-4"
            >
              Bağlantı Gönder
              <HiArrowRight size={20} />
            </FormButton>

            <div className="flex justify-center">
              <Link to="/login" className="text-sm font-semibold text-emerald-400 hover:text-emerald-600 transition-colors">
                Giriş sayfasına dön
              </Link>
              
            </div>
          </form>
          <FooterText className="mt-6" />
        </>
      )}
    </AuthCard>
  )
}