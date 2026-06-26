import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import usersApi from '../../api/usersApi'
import { formatDate } from '../../utils/formatDate'
import StatusBadge from '../../components/ui/StatusBadge'
import MfaBadge from '../../components/ui/MfaBadge'
import AlertMessage from '../../components/ui/AlertMessage'
import FormButton from '../../components/ui/FormButton'
import ConfirmModal from '../../components/ui/ConfirmModal'
import { usePermission } from '../../hooks/usePermission'
import PageTitle from '../../components/ui/PageTitle'
import Label from '../../components/ui/Label'
import permissionsApi from '../../api/permissionsApi'

const schema = z.object({
  firstName: z.string().min(1, 'Ad zorunludur.').max(50),
  lastName: z.string().min(1, 'Soyad zorunludur.').max(50),
  email: z.string().min(1, 'E-posta zorunludur.').email('Geçerli bir e-posta giriniz.'),
})

export default function UserDetail() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { hasPermission } = usePermission()
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)
  const [activeTab, setActiveTab] = useState('general')
  const [updateError, setUpdateError] = useState('')
  const [updateSuccess, setUpdateSuccess] = useState('')
  const [updateLoading, setUpdateLoading] = useState(false)
  const [actionLoading, setActionLoading] = useState('')
  const [actionError, setActionError] = useState('')
  const [actionSuccess, setActionSuccess] = useState('')
  const [userPermissions, setUserPermissions] = useState([])
  const [permissionsLoading, setPermissionsLoading] = useState(false)
  const [confirmModal, setConfirmModal] = useState({ isOpen: false, action: null, title: '', description: '', variant: 'danger', confirmText: '' })

  const { register, handleSubmit, reset, formState: { errors, isDirty } } = useForm({
    resolver: zodResolver(schema),
  })

  const fetchUser = async () => {
    setLoading(true)
    try {
      const response = await usersApi.getUserById(id)
      const data = response.data.value
      setUser(data)
      reset({ firstName: data.firstName, lastName: data.lastName, email: data.email })
    } catch (err) {
      setUpdateError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setLoading(false)
    }
  }

  const fetchPermissions = async () => {
    setPermissionsLoading(true)
    try {
      const response = await permissionsApi.getPermissions(id)
      const data = response.data.value
      const assigned = data.modules.flatMap(m =>
        m.permissions.filter(p => p.isAssigned).map(p => ({ module: m.name, description: p.description || p.name }))
      )
      setUserPermissions(assigned)
    } catch {
      setUserPermissions([])
    } finally {
      setPermissionsLoading(false)
    }
  }

  useEffect(() => {
    fetchUser()
  }, [id])

  const onSubmit = async (data) => {
    setUpdateLoading(true)
    setUpdateError('')
    setUpdateSuccess('')
    try {
      await usersApi.updateUser(id, data)
      setUpdateSuccess('Kullanıcı bilgileri güncellendi.')
      await fetchUser()
    } catch (err) {
      setUpdateError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setUpdateLoading(false)
    }
  }

  const openConfirm = (action) => {
    const configs = {
      toggle: {
        title: user.isActive ? 'Hesabı Pasife Al' : 'Hesabı Aktive Et',
        description: user.isActive
          ? `${user.firstName} ${user.lastName} adlı kullanıcının hesabını pasife almak istediğinize emin misiniz? Kullanıcı giriş yapamayacaktır.`
          : `${user.firstName} ${user.lastName} adlı kullanıcının hesabını aktive etmek istediğinize emin misiniz?`,
        variant: user.isActive ? 'danger' : 'primary',
        confirmText: user.isActive ? 'Pasife Al' : 'Aktive Et',
      },
      unlock: {
        title: 'Hesap Kilidini Aç',
        description: `${user.firstName} ${user.lastName} adlı kullanıcının hesap kilidini açmak istediğinize emin misiniz?`,
        variant: 'primary',
        confirmText: 'Kilidi Aç',
      },
      resetPassword: {
        title: 'Şifreyi Sıfırla',
        description: `${user.firstName} ${user.lastName} adlı kullanıcı için yeni bir geçici şifre üretilip e-posta ile gönderilecektir. Kullanıcının mevcut şifresi geçersiz olacak ve açık tüm oturumları sonlandırılacaktır. Kullanıcı, bir sonraki girişinde şifresini değiştirmek zorunda kalacaktır. Bu işlemi onaylıyor musunuz?`,
        variant: 'danger',
        confirmText: 'Şifreyi Sıfırla',
      },
      resetMfa: {
        title: 'MFA Sıfırla',
        description: `${user.firstName} ${user.lastName} adlı kullanıcının MFA ayarlarını sıfırlamak istediğinize emin misiniz? Kullanıcı bir sonraki girişte yeniden kurulum yapmalıdır.`,
        variant: 'danger',
        confirmText: 'MFA Sıfırla',
      },
    }
    setConfirmModal({ isOpen: true, action, ...configs[action] })
  }

  const handleConfirm = async () => {
    const { action } = confirmModal
    const successMessages = {
      toggle: user.isActive ? 'Hesap pasife alındı.' : 'Hesap aktive edildi.',
      unlock: 'Hesap kilidi açıldı.',
      resetPassword: 'Yeni şifre üretildi ve kullanıcıya e-posta ile gönderildi.',
      resetMfa: 'MFA sıfırlandı.',
    }

    setActionLoading(action)
    setActionError('')
    setActionSuccess('')
    setConfirmModal(prev => ({ ...prev, isOpen: false }))

    try {
      if (action === 'toggle') await usersApi.toggleStatus(id)
      if (action === 'unlock') await usersApi.unlockUser(id)
      if (action === 'resetPassword') await usersApi.resetPassword(id)
      if (action === 'resetMfa') await usersApi.resetMfa(id)
      setActionSuccess(successMessages[action])
      await fetchUser()
    } catch (err) {
      setActionError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setActionLoading('')
    }
  }

  const tabs = [
    { key: 'general', label: 'Genel Bilgiler' },
    { key: 'security', label: 'Güvenlik' },
    ...(hasPermission('Permissions.View') ? [{ key: 'permissions', label: 'Yetkiler' }] : []),
  ]

  if (!hasPermission('Users.ViewDetail')) {
    return (
      <div className="flex flex-col items-center justify-center py-24 text-slate-400 dark:text-slate-500">
        <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" className="mb-3">
          <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
        </svg>
        <p className="text-sm font-semibold">Bu sayfaya erişim yetkiniz yok.</p>
      </div>
    )
  }

  if (loading) return (
    <div className="flex items-center justify-center py-24">
      <svg className="animate-spin text-emerald-500" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
        <path d="M21 12a9 9 0 1 1-6.219-8.56" />
      </svg>
    </div>
  )

  if (!user) return (
    <div className="flex items-center justify-center py-24 text-sm text-slate-400">
      Kullanıcı bulunamadı.
    </div>
  )

  return (
    <div className="space-y-4 md:space-y-5">

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onConfirm={handleConfirm}
        onCancel={() => setConfirmModal(prev => ({ ...prev, isOpen: false }))}
        title={confirmModal.title}
        description={confirmModal.description}
        variant={confirmModal.variant}
        confirmText={confirmModal.confirmText}
        loading={actionLoading === confirmModal.action}
      />

      <PageTitle
        icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" /></svg>}
        title="Kullanıcı Detayı"
        action={
          <button onClick={() => navigate('/users')} className="w-9 h-9 md:w-8 md:h-8 rounded-lg flex items-center justify-center text-slate-400 hover:text-slate-600 hover:bg-slate-100 dark:hover:bg-white/8 transition-colors">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="md:w-4 md:h-4"><polyline points="15 18 9 12 15 6" /></svg>
          </button>
        }
      />

      {/* RESPONSİVE GRID: Mobilde alt alta, lg'de yan yana (1'e 2 oran) */}
      <div className="flex flex-col lg:grid lg:grid-cols-3 gap-5">

        {/* Sol Panel: Profil Kartı */}
        <div className="lg:col-span-1">
          <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8 p-6 flex flex-col items-center text-center">
            <div className="w-20 h-20 md:w-16 md:h-16 rounded-xl bg-gradient-to-br from-emerald-500 to-emerald-700 flex items-center justify-center text-white text-2xl md:text-xl font-bold shadow-md mb-4">
              {user.firstName?.[0]}{user.lastName?.[0]}
            </div>
            <p className="text-lg md:text-base font-bold text-slate-800 dark:text-white mb-1">
              {user.firstName} {user.lastName}
            </p>
            <p className="text-sm text-slate-400 dark:text-slate-500 mb-5 break-all px-2">{user.email}</p>

            <div className="flex flex-col items-center gap-2 mb-6 w-full">
              <StatusBadge isActive={user.isActive} isLocked={user.isLocked} />
              <MfaBadge hasMfa={user.hasMfa} />
            </div>

            <div className="w-full divide-y divide-slate-100 dark:divide-white/6 border-t border-slate-100 dark:border-white/6 pt-2">
              <div className="py-3 flex justify-between items-center lg:flex-col lg:justify-start lg:items-center">
                <p className="text-xs text-slate-400 dark:text-slate-500 lg:mb-0.5">Son Giriş</p>
                <p className="text-sm lg:text-xs font-semibold text-slate-700 dark:text-slate-300">{formatDate(user.lastLoginDate)}</p>
              </div>
              <div className="py-3 flex justify-between items-center lg:flex-col lg:justify-start lg:items-center">
                <p className="text-xs text-slate-400 dark:text-slate-500 lg:mb-0.5">Kayıt Tarihi</p>
                <p className="text-sm lg:text-xs font-semibold text-slate-700 dark:text-slate-300">{formatDate(user.createdDate)}</p>
              </div>
              {user.updatedDate && (
                <div className="py-3 flex justify-between items-center lg:flex-col lg:justify-start lg:items-center">
                  <p className="text-xs text-slate-400 dark:text-slate-500 lg:mb-0.5">Son Güncelleme</p>
                  <p className="text-sm lg:text-xs font-semibold text-slate-700 dark:text-slate-300">{formatDate(user.updatedDate)}</p>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Sağ Panel: Sekmeler ve İçerik */}
        <div className="lg:col-span-2">
          <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8 overflow-hidden">

            {/* RESPONSİVE SEKMELER (TABS): Yana kaydırılabilir */}
            <div className="flex overflow-x-auto whitespace-nowrap border-b border-slate-100 dark:border-white/8 scrollbar-hide">
              {tabs.map((tab) => (
                <button
                  key={tab.key}
                  onClick={() => {
                    setActiveTab(tab.key)
                    setActionError('')
                    setActionSuccess('')
                    setUpdateError('')
                    setUpdateSuccess('')
                    if (tab.key === 'permissions') fetchPermissions()
                  }}
                  className={`px-5 md:px-6 py-4 text-sm font-semibold transition-colors border-b-2 -mb-px
                    ${activeTab === tab.key
                      ? 'border-emerald-500 text-emerald-500'
                      : 'border-transparent text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
                    }`}
                >
                  {tab.label}
                </button>
              ))}
            </div>

            {/* Genel Bilgiler */}
            {activeTab === 'general' && (
              <form onSubmit={handleSubmit(onSubmit)} className="p-4 md:p-6 space-y-4">
                <AlertMessage type="error" message={updateError} />
                <AlertMessage type="success" message={updateSuccess} />

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <Label>Ad</Label>
                    <input
                      {...register('firstName')}
                      disabled={!hasPermission('Users.Update')}
                      className="w-full h-11 md:h-10 px-3 rounded-lg border text-base md:text-sm transition-all
                        bg-slate-50 border-slate-200 text-slate-800
                        dark:bg-white/6 dark:border-white/10 dark:text-white
                        focus:outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-400/20
                        disabled:opacity-50 disabled:cursor-not-allowed"
                    />
                    {errors.firstName && <p className="text-xs text-red-500 mt-1">{errors.firstName.message}</p>}
                  </div>
                  <div>
                    <Label>Soyad</Label>
                    <input
                      {...register('lastName')}
                      disabled={!hasPermission('Users.Update')}
                      className="w-full h-11 md:h-10 px-3 rounded-lg border text-base md:text-sm transition-all
                        bg-slate-50 border-slate-200 text-slate-800
                        dark:bg-white/6 dark:border-white/10 dark:text-white
                        focus:outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-400/20
                        disabled:opacity-50 disabled:cursor-not-allowed"
                    />
                    {errors.lastName && <p className="text-xs text-red-500 mt-1">{errors.lastName.message}</p>}
                  </div>
                </div>

                <div>
                  <Label>E-posta</Label>
                  <input
                    {...register('email')}
                    disabled={!hasPermission('Users.Update')}
                    className="w-full h-11 md:h-10 px-3 rounded-lg border text-base md:text-sm transition-all
                      bg-slate-50 border-slate-200 text-slate-800
                      dark:bg-white/6 dark:border-white/10 dark:text-white
                      focus:outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-400/20
                      disabled:opacity-50 disabled:cursor-not-allowed"
                  />
                  {errors.email && <p className="text-xs text-red-500 mt-1">{errors.email.message}</p>}
                </div>

                {hasPermission('Users.Update') && (
                  <div className="flex justify-end pt-4 md:pt-2">
                    <FormButton
                      loading={updateLoading}
                      loadingText="Kaydediliyor..."
                      disabled={!isDirty}
                      className="w-full sm:w-auto flex justify-center px-5 py-2.5 md:py-2 rounded-lg text-sm bg-slate-900 dark:bg-emerald-500 text-white hover:bg-slate-700 dark:hover:bg-emerald-400 disabled:opacity-40"
                    >
                      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="mr-2">
                        <polyline points="20 6 9 17 4 12" />
                      </svg>
                      Kaydet
                    </FormButton>
                  </div>
                )}
              </form>
            )}

            {/* Güvenlik */}
            {activeTab === 'security' && (
              <div className="p-4 md:p-6 space-y-4">
                <AlertMessage type="error" message={actionError} />
                <AlertMessage type="success" message={actionSuccess} />

                {/* Hesap Durumu Kartı */}
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 px-4 py-4 rounded-xl bg-slate-50 dark:bg-white/4 border border-slate-100 dark:border-white/8">
                  <div className="flex items-start sm:items-center gap-3">
                    <div className="w-10 h-10 md:w-8 md:h-8 rounded-lg bg-white dark:bg-white/8 flex items-center justify-center flex-shrink-0 mt-1 sm:mt-0">
                      <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-500 dark:text-slate-400 md:w-[15px] md:h-[15px]">
                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" />
                      </svg>
                    </div>
                    <div>
                      <p className="text-sm font-semibold text-slate-800 dark:text-white mb-0.5">Hesap Durumu</p>
                      <p className="text-xs text-slate-500 dark:text-slate-400">
                        {user.isActive ? 'Hesap aktif, kullanıcı giriş yapabilir.' : 'Hesap pasif, kullanıcı giriş yapamaz.'}
                      </p>
                    </div>
                  </div>
                  {hasPermission('Users.ToggleStatus') && (
                    <button
                      onClick={() => openConfirm('toggle')}
                      disabled={actionLoading === 'toggle'}
                      className={`w-full sm:w-auto flex justify-center items-center gap-2 px-4 py-2.5 sm:py-2 rounded-lg text-sm sm:text-xs font-semibold transition-all disabled:opacity-50
                        ${user.isActive
                          ? 'bg-red-50 dark:bg-red-500/10 text-red-600 dark:text-red-400 hover:bg-red-100 dark:hover:bg-red-500/20 border border-red-200 dark:border-red-500/20'
                          : 'bg-emerald-50 dark:bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 hover:bg-emerald-100 dark:hover:bg-emerald-500/20 border border-emerald-200 dark:border-emerald-500/20'
                        }`}
                    >
                      {actionLoading === 'toggle' && (
                        <svg className="animate-spin" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M21 12a9 9 0 1 1-6.219-8.56" /></svg>
                      )}
                      {user.isActive ? 'Pasife Al' : 'Aktive Et'}
                    </button>
                  )}
                </div>

                {/* Kilit Durumu Kartı */}
                {user.isLocked && (
                  <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 px-4 py-4 rounded-xl bg-slate-50 dark:bg-white/4 border border-slate-100 dark:border-white/8">
                    <div className="flex items-start sm:items-center gap-3">
                      <div className="w-10 h-10 md:w-8 md:h-8 rounded-lg bg-white dark:bg-white/8 flex items-center justify-center flex-shrink-0 mt-1 sm:mt-0">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-red-500 md:w-[15px] md:h-[15px]">
                          <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
                        </svg>
                      </div>
                      <div>
                        <p className="text-sm font-semibold text-slate-800 dark:text-white mb-0.5">Hesap Kilitli</p>
                        <p className="text-xs text-slate-500 dark:text-slate-400">
                          Çok fazla hatalı giriş denemesi yapıldı.
                        </p>
                      </div>
                    </div>
                    {hasPermission('Users.Unlock') && (
                      <button
                        onClick={() => openConfirm('unlock')}
                        disabled={actionLoading === 'unlock'}
                        className="w-full sm:w-auto flex justify-center items-center gap-2 px-4 py-2.5 sm:py-2 rounded-lg text-sm sm:text-xs font-semibold transition-all disabled:opacity-50 bg-emerald-50 dark:bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 hover:bg-emerald-100 dark:hover:bg-emerald-500/20 border border-emerald-200 dark:border-emerald-500/20"
                      >
                        {actionLoading === 'unlock' && (
                          <svg className="animate-spin" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M21 12a9 9 0 1 1-6.219-8.56" /></svg>
                        )}
                        Kilidi Aç
                      </button>
                    )}
                  </div>
                )}

                {/* Şifre Sıfırlama Kartı */}
                {hasPermission('Users.ResetPassword') && (
                  <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 px-4 py-4 rounded-xl bg-slate-50 dark:bg-white/4 border border-slate-100 dark:border-white/8">
                    <div className="flex items-start sm:items-center gap-3">
                      <div className="w-10 h-10 md:w-8 md:h-8 rounded-lg bg-white dark:bg-white/8 flex items-center justify-center flex-shrink-0 mt-1 sm:mt-0">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-500 dark:text-slate-400 md:w-[15px] md:h-[15px]">
                          <circle cx="8" cy="15" r="4" /><path d="M10.85 12.15L19 4" /><path d="M18 5l2 2" /><path d="M15 8l2 2" />
                        </svg>
                      </div>
                      <div>
                        <p className="text-sm font-semibold text-slate-800 dark:text-white mb-0.5">Şifre Sıfırlama</p>
                        <p className="text-xs text-slate-500 dark:text-slate-400">
                          Kullanıcıya yeni geçici şifre üretilip e-posta ile gönderilir. Mevcut şifre ve açık oturumlar geçersiz olur.
                        </p>
                      </div>
                    </div>
                    <button
                      onClick={() => openConfirm('resetPassword')}
                      disabled={actionLoading === 'resetPassword'}
                      className="w-full sm:w-auto flex justify-center items-center gap-2 px-4 py-2.5 sm:py-2 rounded-lg text-sm sm:text-xs font-semibold transition-all disabled:opacity-50 bg-red-50 dark:bg-red-500/10 text-red-600 dark:text-red-400 hover:bg-red-100 dark:hover:bg-red-500/20 border border-red-200 dark:border-red-500/20"
                    >
                      {actionLoading === 'resetPassword' && (
                        <svg className="animate-spin" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M21 12a9 9 0 1 1-6.219-8.56" /></svg>
                      )}
                      Şifreyi Sıfırla
                    </button>
                  </div>
                )}

                {/* MFA Kartı */}
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 px-4 py-4 rounded-xl bg-slate-50 dark:bg-white/4 border border-slate-100 dark:border-white/8">
                  <div className="flex items-start sm:items-center gap-3">
                    <div className="w-10 h-10 md:w-8 md:h-8 rounded-lg bg-white dark:bg-white/8 flex items-center justify-center flex-shrink-0 mt-1 sm:mt-0">
                      <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-500 dark:text-slate-400 md:w-[15px] md:h-[15px]">
                        <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
                      </svg>
                    </div>
                    <div>
                      <p className="text-sm font-semibold text-slate-800 dark:text-white mb-0.5">İki Faktörlü Doğrulama</p>
                      <p className="text-xs text-slate-500 dark:text-slate-400">
                        {user.hasMfa ? 'MFA aktif. Sıfırlama sonrası kullanıcı yeniden kurulum yapmalıdır.' : 'MFA kurulmamış.'}
                      </p>
                    </div>
                  </div>
                  {user.hasMfa && hasPermission('Users.ResetMfa') && (
                    <button
                      onClick={() => openConfirm('resetMfa')}
                      disabled={actionLoading === 'resetMfa'}
                      className="w-full sm:w-auto flex justify-center items-center gap-2 px-4 py-2.5 sm:py-2 rounded-lg text-sm sm:text-xs font-semibold transition-all disabled:opacity-50 bg-red-50 dark:bg-red-500/10 text-red-600 dark:text-red-400 hover:bg-red-100 dark:hover:bg-red-500/20 border border-red-200 dark:border-red-500/20"
                    >
                      {actionLoading === 'resetMfa' && (
                        <svg className="animate-spin" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M21 12a9 9 0 1 1-6.219-8.56" /></svg>
                      )}
                      MFA Sıfırla
                    </button>
                  )}
                </div>
              </div>
            )}

            {/* Yetkiler */}
            {activeTab === 'permissions' && (
              <div className="p-4 md:p-6">
                {permissionsLoading ? (
                  <div className="flex items-center justify-center py-10">
                    <svg className="animate-spin text-emerald-500" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                      <path d="M21 12a9 9 0 1 1-6.219-8.56" />
                    </svg>
                  </div>
                ) : userPermissions.length === 0 ? (
                  <div className="flex flex-col items-center justify-center py-12 text-slate-400 dark:text-slate-500 text-center">
                    <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" className="mb-3">
                      <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
                    </svg>
                    <p className="text-sm">Bu kullanıcıya henüz yetki atanmamış.</p>
                  </div>
                ) : (
                  <div className="space-y-4">
                    {Object.entries(
                      userPermissions.reduce((acc, p) => {
                        if (!acc[p.module]) acc[p.module] = []
                        acc[p.module].push(p.description)
                        return acc
                      }, {})
                    ).map(([moduleName, perms]) => (
                      <div key={moduleName} className="rounded-xl border border-slate-100 dark:border-white/8 overflow-hidden">
                        <div className="px-4 py-3 bg-slate-50 dark:bg-white/3 border-b border-slate-100 dark:border-white/8">
                          <p className="text-xs font-bold text-slate-600 dark:text-slate-300 uppercase tracking-wider">{moduleName}</p>
                        </div>
                        <div className="divide-y divide-slate-50 dark:divide-white/4">
                          {perms.map((perm, i) => (
                            <div key={i} className="flex items-center gap-3 px-4 py-3">
                              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" className="text-emerald-500 flex-shrink-0">
                                <polyline points="20 6 9 17 4 12" />
                              </svg>
                              <span className="text-sm text-slate-700 dark:text-slate-300 leading-snug">{perm}</span>
                            </div>
                          ))}
                        </div>
                      </div>
                    ))}
                  </div>
                )}

                {hasPermission('Permissions.Assign') && (
                  <div className="mt-6 flex justify-end">
                    <button
                      onClick={() => navigate(`/permissions?userId=${id}`)}
                      className="w-full sm:w-auto flex justify-center items-center gap-2 px-5 py-2.5 rounded-lg text-sm font-semibold transition-all
                        bg-slate-900 dark:bg-emerald-500 text-white hover:bg-slate-700 dark:hover:bg-emerald-400"
                    >
                      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
                      </svg>
                      Yetki Sayfasında Düzenle
                    </button>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}