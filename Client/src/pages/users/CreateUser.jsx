import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import usersApi from '../../api/usersApi'
import AlertMessage from '../../components/ui/AlertMessage'
import FormButton from '../../components/ui/FormButton'
import ConfirmModal from '../../components/ui/ConfirmModal'
import { usePermission } from '../../hooks/usePermission'
import PageTitle from '../../components/ui/PageTitle'
import Label from '../../components/ui/Label'
import SectionTitle from '../../components/ui/SectionTitle'

const schema = z.object({
  firstName: z.string().min(1, 'Ad zorunludur.').max(50, 'Ad en fazla 50 karakter olabilir.'),
  lastName: z.string().min(1, 'Soyad zorunludur.').max(50, 'Soyad en fazla 50 karakter olabilir.'),
  email: z.string().min(1, 'E-posta zorunludur.').email('Geçerli bir e-posta giriniz.'),
})

export default function CreateUser() {
  const navigate = useNavigate()
  const { hasPermission } = usePermission()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [confirmOpen, setConfirmOpen] = useState(false)
  const [pendingData, setPendingData] = useState(null)

  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
  })

  if (!hasPermission('Users.Create')) {
    return (
      <div className="flex flex-col items-center justify-center py-24 text-slate-400 dark:text-slate-500">
        <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" className="mb-3">
          <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
        </svg>
        <p className="text-sm font-semibold">Bu sayfaya erişim yetkiniz yok.</p>
      </div>
    )
  }

  const onSubmit = (data) => {
    setPendingData(data)
    setConfirmOpen(true)
  }

  const handleConfirm = async () => {
    setLoading(true)
    setError('')
    setConfirmOpen(false)
    try {
      const response = await usersApi.createUser(pendingData)
      navigate(`/users/${response.data.value}`)
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-5">

      <ConfirmModal
        isOpen={confirmOpen}
        onConfirm={handleConfirm}
        onCancel={() => setConfirmOpen(false)}
        title="Kullanıcı Oluştur"
        description={`${pendingData?.firstName} ${pendingData?.lastName} adlı kullanıcıyı oluşturmak istediğinize emin misiniz? Geçici şifre ${pendingData?.email} adresine gönderilecektir.`}
        confirmText="Oluştur"
        cancelText="İptal"
        variant="primary"
        loading={loading}
      />

      <PageTitle
        icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" /></svg>}
        title="Yeni Kullanıcı"
        subtitle="Geçici şifre e-posta ile gönderilecektir."
        action={
          <button onClick={() => navigate('/users')} className="w-8 h-8 rounded-lg flex items-center justify-center text-slate-400 hover:text-slate-600 hover:bg-slate-100 dark:hover:bg-white/8 transition-colors">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><polyline points="15 18 9 12 15 6" /></svg>
          </button>
        }
      />

      <div className="w-full max-w-2xl">
        <AlertMessage type="error" message={error} />

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 mt-4">

          {/* Kişisel Bilgiler */}
          <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8">
            <SectionTitle icon={<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" /></svg>}>
              Kişisel Bilgiler
            </SectionTitle>
            <div className="p-6 space-y-4">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <Label>Ad</Label>
                  <input
                    {...register('firstName')}
                    placeholder="Örn: Ahmet"
                    className={`w-full h-10 px-3 rounded-lg border text-sm transition-all
                      bg-slate-50 text-slate-800 placeholder-slate-300
                      dark:bg-white/6 dark:text-white dark:placeholder-white/20
                      focus:outline-none focus:ring-2 focus:ring-emerald-400/20
                      ${errors.firstName
                        ? 'border-red-400 focus:border-red-400'
                        : 'border-slate-200 dark:border-white/10 focus:border-emerald-400'
                      }`}
                  />
                  {errors.firstName && <p className="text-xs text-red-500 mt-1.5">{errors.firstName.message}</p>}
                </div>
                <div>
                  <Label>Soyad</Label>
                  <input
                    {...register('lastName')}
                    placeholder="Örn: Yılmaz"
                    className={`w-full h-10 px-3 rounded-lg border text-sm transition-all
                      bg-slate-50 text-slate-800 placeholder-slate-300
                      dark:bg-white/6 dark:text-white dark:placeholder-white/20
                      focus:outline-none focus:ring-2 focus:ring-emerald-400/20
                      ${errors.lastName
                        ? 'border-red-400 focus:border-red-400'
                        : 'border-slate-200 dark:border-white/10 focus:border-emerald-400'
                      }`}
                  />
                  {errors.lastName && <p className="text-xs text-red-500 mt-1.5">{errors.lastName.message}</p>}
                </div>
              </div>
            </div>
          </div>

          {/* İletişim Bilgileri */}
          <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8">
            <SectionTitle icon={<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z" /><polyline points="22,6 12,13 2,6" /></svg>}>
              İletişim
            </SectionTitle>
            <div className="p-6">
              <Label>Kurumsal E-posta</Label>
              <input
                {...register('email')}
                type="text"
                placeholder="name.surname@example.com"
                className={`w-full h-10 px-3 rounded-lg border text-sm transition-all
                  bg-slate-50 text-slate-800 placeholder-slate-300
                  dark:bg-white/6 dark:text-white dark:placeholder-white/20
                  focus:outline-none focus:ring-2 focus:ring-emerald-400/20
                  ${errors.email
                    ? 'border-red-400 focus:border-red-400'
                    : 'border-slate-200 dark:border-white/10 focus:border-emerald-400'
                  }`}
              />
              {errors.email && <p className="text-xs text-red-500 mt-1.5">{errors.email.message}</p>}
            </div>
          </div>

          {/* Bilgi Notu */}
          <div className="flex items-start gap-3 px-4 py-3.5 rounded-xl bg-emerald-50 dark:bg-emerald-500/10 border border-emerald-200 dark:border-emerald-500/20">
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
              className="text-emerald-500 flex-shrink-0 mt-0.5">
              <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            <div className="space-y-1">
              <p className="text-xs font-semibold text-emerald-700 dark:text-emerald-400">Otomatik Şifre Oluşturma</p>
              <p className="text-xs text-emerald-600 dark:text-emerald-500">
                Kullanıcı oluşturulduğunda geçici bir şifre otomatik üretilecek ve girilen e-posta adresine gönderilecektir. Kullanıcı ilk girişte şifresini değiştirmek ve iki faktörlü doğrulamayı kurmak zorunda kalacaktır.
              </p>
            </div>
          </div>

          {/* Aksiyonlar */}
          <div className="flex flex-col-reverse sm:flex-row sm:items-center sm:justify-between gap-3 pt-1">
            <button
              type="button"
              onClick={() => navigate('/users')}
              className="w-full sm:w-auto flex justify-center items-center gap-1.5 px-4 py-2.5 sm:py-2 rounded-lg text-sm font-medium transition-colors
                text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-white/8"
            >
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <polyline points="15 18 9 12 15 6" />
              </svg>
              Kullanıcılara Dön
            </button>
            <FormButton
              loading={loading}
              loadingText="Oluşturuluyor..."
              className="w-full sm:w-auto flex justify-center items-center gap-2 px-5 py-2.5 sm:py-2 rounded-lg text-sm font-semibold bg-slate-900 dark:bg-emerald-500 text-white hover:bg-slate-700 dark:hover:bg-emerald-400 transition-all"
            >
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" />
              </svg>
              Kullanıcı Oluştur
            </FormButton>
          </div>

        </form>
      </div>
    </div>
  )
}