import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import useAuthStore from '../../store/authStore'
import axiosInstance from '../../api/axiosInstance'
import { newPasswordSchema } from '../../utils/passwordSchema'
import PasswordInput from '../../components/ui/PasswordInput'
import PasswordRules from '../../components/ui/PasswordRules'
import AlertMessage from '../../components/ui/AlertMessage'
import FormButton from '../../components/ui/FormButton'
import ConfirmModal from '../../components/ui/ConfirmModal'

const schema = z.object({
  currentPassword: z.string().min(1, 'Mevcut şifre zorunludur.'),
  newPassword: newPasswordSchema,
  confirmPassword: z.string().min(1, 'Şifre tekrarı zorunludur.'),
}).refine(data => data.newPassword === data.confirmPassword, {
  message: 'Şifreler uyuşmuyor.',
  path: ['confirmPassword'],
})

export default function UserInfo() {
  const { user } = useAuthStore()
  const [passwordOpen, setPasswordOpen] = useState(false)
  const [confirmOpen, setConfirmOpen] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const { register, handleSubmit, watch, reset, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
  })

  const newPasswordValue = watch('newPassword', '')

  const handleToggle = () => {
    setPasswordOpen(prev => !prev)
    setError('')
    setSuccess(false)
    reset()
  }

  const onSubmit = () => {
    setConfirmOpen(true)
  }

  const handleConfirm = async () => {
    setConfirmOpen(false)
    setLoading(true)
    setError('')
    setSuccess(false)
    try {
      const data = {
        currentPassword: watch('currentPassword'),
        newPassword: watch('newPassword'),
        confirmPassword: watch('confirmPassword'),
      }
      await axiosInstance.post('/auth/change-password', data)
      setSuccess(true)
      setPasswordOpen(false)
      reset()
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-3xl mx-auto space-y-4 md:space-y-5">

      <ConfirmModal
        isOpen={confirmOpen}
        onConfirm={handleConfirm}
        onCancel={() => setConfirmOpen(false)}
        title="Şifre Değiştir"
        description="Şifrenizi değiştirmek istediğinize emin misiniz? Mevcut oturumunuz etkilenmeyecektir."
        confirmText="Evet, Değiştir"
        cancelText="İptal"
        variant="warning"
        loading={loading}
      />

      {/* Sayfa Başlığı */}
      <div className="flex items-center gap-3 pb-1 border-b border-slate-200 dark:border-white/8">
        <div className="w-8 h-8 rounded-lg bg-emerald-500/10 dark:bg-emerald-500/20 flex items-center justify-center flex-shrink-0">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
            className="text-emerald-500">
            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" />
          </svg>
        </div>
        <h1 className="text-base font-bold text-slate-800 dark:text-white" style={{ fontFamily: 'Montserrat, sans-serif' }}>
          Hesap Bilgileriniz
        </h1>
      </div>

      {/* Profil Kartı */}
      <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8 overflow-hidden">
        <div className="flex items-center gap-4 px-4 md:px-6 py-5">
          <div className="w-14 h-14 rounded-xl bg-gradient-to-br from-emerald-500 to-emerald-700 flex items-center justify-center text-white text-lg font-bold shadow-md flex-shrink-0">
            {user?.firstName?.[0]}{user?.lastName?.[0]}
          </div>
          <div className="pr-2 break-all">
            <p className="text-base font-semibold text-slate-800 dark:text-white">
              {user?.firstName} {user?.lastName}
            </p>
            <p className="text-sm text-slate-400 dark:text-slate-500 mt-0.5">{user?.email}</p>
          </div>
        </div>

        {/* RESPONSİVE GRID: Mobilde alt alta (grid-cols-1), tablette yan yana (sm:grid-cols-2) */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-px bg-slate-100 dark:bg-white/6 border-t border-slate-100 dark:border-white/8">
          <div className="bg-white dark:bg-[#002147] px-4 md:px-6 py-4">
            <p className="text-xs font-medium text-slate-400 dark:text-slate-500 uppercase tracking-wider mb-1">Ad Soyad</p>
            <p className="text-sm font-semibold text-slate-800 dark:text-white">{user?.firstName} {user?.lastName}</p>
          </div>
          <div className="bg-white dark:bg-[#002147] px-4 md:px-6 py-4">
            <p className="text-xs font-medium text-slate-400 dark:text-slate-500 uppercase tracking-wider mb-1">E-posta</p>
            <p className="text-sm font-semibold text-slate-800 dark:text-white">{user?.email}</p>
          </div>
        </div>
      </div>

      {/* Güvenlik Kartı (Şifre Değiştirme) */}
      <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8 overflow-hidden">
        <button
          onClick={handleToggle}
          className="w-full flex items-center justify-between px-4 md:px-6 py-4 transition-colors hover:bg-slate-50 dark:hover:bg-white/4"
        >
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-lg bg-slate-100 dark:bg-white/8 flex items-center justify-center flex-shrink-0">
              <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                className="text-slate-500 dark:text-slate-400">
                <rect x="3" y="11" width="18" height="11" rx="2" />
                <path d="M7 11V7a5 5 0 0 1 10 0v4" />
              </svg>
            </div>
            <p className="text-sm font-semibold text-slate-800 dark:text-white">Şifre Değiştir</p>
          </div>
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
            className={`text-slate-400 transition-transform duration-200 ${passwordOpen ? 'rotate-180' : ''}`}>
            <polyline points="6 9 12 15 18 9" />
          </svg>
        </button>

        {passwordOpen && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="px-4 md:px-6 pb-6 border-t border-slate-100 dark:border-white/8 pt-5 space-y-4">

              <AlertMessage type="error" message={error} />
              <AlertMessage type="success" message={success ? 'Şifreniz başarıyla değiştirildi.' : ''} />

              {/* RESPONSİVE GRID: Form ve Şifre Kuralları Mobilde alt alta dizilir */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-4">
                  <PasswordInput
                    register={register}
                    name="currentPassword"
                    label="Mevcut Şifre"
                    error={errors.currentPassword}
                  />
                  <PasswordInput
                    register={register}
                    name="newPassword"
                    label="Yeni Şifre"
                    error={errors.newPassword}
                  />
                  <PasswordInput
                    register={register}
                    name="confirmPassword"
                    label="Yeni Şifre Tekrar"
                    error={errors.confirmPassword}
                  />
                </div>
                {/* Şifre kuralları mobilde formun altında görünecek */}
                <div className="bg-slate-50 dark:bg-white/2 p-4 rounded-xl border border-slate-100 dark:border-white/4">
                  <PasswordRules value={newPasswordValue} />
                </div>
              </div>

              {/* RESPONSİVE BUTONLAR: Mobilde tam genişlik ve alt alta, geniş ekranda sağda yan yana */}
              <div className="flex flex-col-reverse sm:flex-row justify-end gap-3 pt-4 sm:pt-2">
                <button
                  type="button"
                  onClick={() => { setPasswordOpen(false); reset(); setError('') }}
                  className="w-full sm:w-auto px-4 py-2.5 sm:py-2 rounded-lg text-sm font-medium transition-colors
                    text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-white/8"
                >
                  İptal
                </button>
                <FormButton
                  loading={loading}
                  loadingText="Kaydediliyor..."
                  className="w-full sm:w-auto flex justify-center items-center px-5 py-2.5 sm:py-2 rounded-lg text-sm bg-slate-900 dark:bg-emerald-500 text-white hover:bg-slate-700 dark:hover:bg-emerald-400"
                >
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="mr-2">
                    <polyline points="20 6 9 17 4 12" />
                  </svg>
                  Kaydet
                </FormButton>
              </div>
            </div>
          </form>
        )}
      </div>
    </div>
  )
}