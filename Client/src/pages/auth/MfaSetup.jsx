import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { HiArrowRight } from 'react-icons/hi'
import authApi from '../../api/authApi'
import AlertMessage from '../../components/ui/AlertMessage'
import FormButton from '../../components/ui/FormButton'
import Label from '../../components/ui/Label'
import AuthLayout from '../../components/auth/AuthLayout'
import AuthTitle from '../../components/auth/AuthTitle'
import FooterText from '../../components/ui/FooterText'

const schema = z.object({
  code: z.string().length(6, 'Doğrulama kodu 6 haneli olmalıdır.'),
})

export default function MfaSetup() {
  const navigate = useNavigate()
  const [qrCodeBase64, setQrCodeBase64] = useState('')
  const [manualKey, setManualKey] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [isFetching, setIsFetching] = useState(true)
  const [error, setError] = useState(null)
  const [isSuccess, setIsSuccess] = useState(false)

  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
  })

  useEffect(() => {
    const mfaToken = sessionStorage.getItem('mfaToken')
    if (!mfaToken) { navigate('/login'); return }

    const fetchQr = async () => {
      try {
        const response = await authApi.setupMfa({ mfaToken })
        const result = response.data.value
        setQrCodeBase64(result.qrCodeBase64)
        setManualKey(result.manualEntryKey)
      } catch (err) {
        setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
        navigate('/login')
      } finally {
        setIsFetching(false)
      }
    }

    fetchQr()
  }, [navigate])

  const onSubmit = async (data) => {
    const mfaToken = sessionStorage.getItem('mfaToken')
    if (!mfaToken) { navigate('/login'); return }

    setError(null)
    setIsLoading(true)
    try {
      const response = await authApi.confirmMfa({ mfaToken, mfaCode: data.code, isSetupFlow: true })
      const result = response.data.value
      if (result.isSetupComplete) {
        sessionStorage.removeItem('mfaToken')
        setIsSuccess(true)
      }
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setIsLoading(false)
    }
  }

  if (isSuccess) {
    return (
      <AuthLayout>
        <div className="flex flex-col items-center justify-center py-8 text-center">
          <div className="w-16 h-16 rounded-full bg-emerald-50 flex items-center justify-center mb-4">
            <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#34d399" strokeWidth="2">
              <polyline points="20 6 9 17 4 12" />
            </svg>
          </div>
          <h3 className="text-2xl font-bold text-slate-900 mb-2">Kurulum Tamamlandı!</h3>
          <p className="text-sm text-slate-500 mb-8">
            İki faktörlü doğrulama başarıyla aktif edildi. Devam etmek için giriş yapın.
          </p>
          <button
            onClick={() => navigate('/login')}
            className="flex items-center gap-2 px-6 py-3 rounded-2xl bg-slate-900 hover:bg-slate-800 text-white text-base font-semibold transition-all hover:-translate-y-0.5 hover:shadow-xl"
          >
            Giriş Sayfasına Git
            <HiArrowRight size={18} />
          </button>
        </div>
      </AuthLayout>
    )
  }

  return (
    <AuthLayout maxWidth="max-w-3xl">
      <AuthTitle subtitle="Hesabınızı korumak için Microsoft Authenticator uygulamasını kurun.">
        İki Adımlı Doğrulama Kurulumu
      </AuthTitle>

      <AlertMessage type="error" message={error} />

      {isFetching ? (
        <div className="flex items-center justify-center py-16">
          <svg className="animate-spin text-emerald-400" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M21 12a9 9 0 1 1-6.219-8.56" />
          </svg>
        </div>
      ) : (
        <>
          {qrCodeBase64 && (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">

              {/* QR Kod */}
              <div className="flex flex-col items-center">
                <div className="p-4 bg-white border-2 border-slate-100 rounded-2xl mb-3">
                  <img src={qrCodeBase64} alt="QR Kod" className="w-40 h-40 md:w-48 md:h-48" />
                </div>
                <p className="text-xs text-slate-400 mb-1.5">Manuel giriş anahtarı:</p>
                <code className="text-xs font-mono bg-slate-50 border border-slate-200 rounded-lg px-3 py-2 text-slate-600 tracking-widest select-all text-center break-all w-full">
                  {manualKey}
                </code>
              </div>

              {/* Adımlar */}
              <div className="flex flex-col justify-center space-y-4">
                {[
                  'Microsoft Authenticator uygulamasını açın ve yeni hesap ekleyin.',
                  'QR kodu okutun veya manuel anahtarı uygulamaya girin.',
                  'Uygulamada oluşan 6 haneli kodu aşağıya girin.',
                ].map((text, i) => (
                  <div key={i} className="flex items-start gap-3">
                    <span className="w-6 h-6 rounded-full bg-emerald-400 text-white text-xs font-bold flex items-center justify-center flex-shrink-0 mt-0.5">
                      {i + 1}
                    </span>
                    <p className="text-sm text-slate-600 leading-relaxed">{text}</p>
                  </div>
                ))}
              </div>
            </div>
          )}

          <form onSubmit={handleSubmit(onSubmit)} noValidate>
            <div className="mb-6">
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
              Doğrula ve Etkinleştir
              <HiArrowRight size={20} />
            </FormButton>
          </form>
          <FooterText className="mt-6" />
        </>
      )}
    </AuthLayout>
  )
}