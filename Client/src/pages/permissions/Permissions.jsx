import { useState, useEffect, useCallback } from 'react'
import { useSearchParams } from 'react-router-dom'
import usersApi from '../../api/usersApi'
import permissionsApi from '../../api/permissionsApi'
import AlertMessage from '../../components/ui/AlertMessage'
import FormButton from '../../components/ui/FormButton'
import ConfirmModal from '../../components/ui/ConfirmModal'
import { usePermission } from '../../hooks/usePermission'
import PageTitle from '../../components/ui/PageTitle'

export default function Permissions() {
  const { hasPermission } = usePermission()
  const [searchParams] = useSearchParams()

  // Arama
  const [search, setSearch] = useState('')
  const [users, setUsers] = useState([])
  const [searchLoading, setSearchLoading] = useState(false)
  const [showDropdown, setShowDropdown] = useState(false)

  // Seçili kullanıcı
  const [selectedUser, setSelectedUser] = useState(null)

  // Modüller ve izinler
  const [modules, setModules] = useState([])
  const [selectedModuleId, setSelectedModuleId] = useState(null)
  const [checkedIds, setCheckedIds] = useState([])
  const [originalIds, setOriginalIds] = useState([])
  const [permissionsLoading, setPermissionsLoading] = useState(false)
  const [permissionsError, setPermissionsError] = useState('')

  // Kaydet
  const [saveLoading, setSaveLoading] = useState(false)
  const [saveError, setSaveError] = useState('')
  const [saveSuccess, setSaveSuccess] = useState('')
  const [confirmOpen, setConfirmOpen] = useState(false)

  // Kullanıcı değiştirme uyarısı
  const [pendingUser, setPendingUser] = useState(null)
  const [unsavedConfirmOpen, setUnsavedConfirmOpen] = useState(false)

  const isDirty = JSON.stringify([...checkedIds].sort()) !== JSON.stringify([...originalIds].sort())

  const selectedModule = modules.find(m => m.id === selectedModuleId) || null

  // URL'den userId parametresi gelirse otomatik yükle
  useEffect(() => {
    const userId = searchParams.get('userId')
    if (!userId) return
    const loadUser = async () => {
      try {
        const response = await usersApi.getUserById(parseInt(userId))
        const user = response.data.value
        applyUser(user)
      } catch {
        // Kullanıcı bulunamazsa sessiz geç
      }
    }
    loadUser()
  }, [])

  // Debounce arama
  useEffect(() => {
    if (!search.trim()) { setUsers([]); setShowDropdown(false); return }
    const timer = setTimeout(async () => {
      setSearchLoading(true)
      try {
        const response = await usersApi.getUsers({ search, page: 1, pageSize: 6 })
        setUsers(response.data.value.users)
        setShowDropdown(true)
      } catch {
        setUsers([])
      } finally {
        setSearchLoading(false)
      }
    }, 300)
    return () => clearTimeout(timer)
  }, [search])

  const fetchPermissions = useCallback(async (user) => {
    setPermissionsLoading(true)
    setPermissionsError('')
    setSaveSuccess('')
    setSaveError('')
    setSelectedModuleId(null)
    try {
      const response = await permissionsApi.getPermissions(user.id)
      const data = response.data.value
      setModules(data.modules)
      const assigned = data.modules.flatMap(m => m.permissions.filter(p => p.isAssigned).map(p => p.id))
      setCheckedIds(assigned)
      setOriginalIds(assigned)
      // İlk modülü varsayılan seç
      if (data.modules.length > 0) setSelectedModuleId(data.modules[0].id)
    } catch (err) {
      setPermissionsError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setPermissionsLoading(false)
    }
  }, [])

  const applyUser = (user) => {
    setSelectedUser(user)
    setSearch('')
    setShowDropdown(false)
    fetchPermissions(user)
  }

  const handleSelectUser = (user) => {
    if (isDirty) { setPendingUser(user); setUnsavedConfirmOpen(true); return }
    applyUser(user)
  }

  const handleClearUser = () => {
    if (isDirty) { setPendingUser(null); setUnsavedConfirmOpen(true); return }
    resetAll()
  }

  const resetAll = () => {
    setSelectedUser(null)
    setModules([])
    setCheckedIds([])
    setOriginalIds([])
    setSelectedModuleId(null)
    setSaveSuccess('')
    setSaveError('')
  }

  const handleUnsavedConfirm = () => {
    setUnsavedConfirmOpen(false)
    if (pendingUser) { applyUser(pendingUser); setPendingUser(null) }
    else resetAll()
  }

  const togglePermission = (id) => {
    setCheckedIds(prev => prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id])
  }

  const toggleModuleAll = (module) => {
    const ids = module.permissions.map(p => p.id)
    const allChecked = ids.every(id => checkedIds.includes(id))
    setCheckedIds(prev =>
      allChecked ? prev.filter(id => !ids.includes(id)) : [...new Set([...prev, ...ids])]
    )
  }

  const isModuleAllChecked = (module) =>
    module.permissions.length > 0 && module.permissions.every(p => checkedIds.includes(p.id))

  const isModulePartialChecked = (module) =>
    module.permissions.some(p => checkedIds.includes(p.id)) && !isModuleAllChecked(module)

  const handleConfirmSave = async () => {
    setConfirmOpen(false)
    setSaveLoading(true)
    setSaveError('')
    setSaveSuccess('')
    try {
      await permissionsApi.assignPermissions({ userId: selectedUser.id, permissionIds: checkedIds })
      setOriginalIds([...checkedIds])
      setSaveSuccess('Yetkiler başarıyla güncellendi.')
    } catch (err) {
      setSaveError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setSaveLoading(false)
    }
  }

  if (!hasPermission('Permissions.View')) {
    return (
      <div className="flex flex-col items-center justify-center py-24 text-slate-400 dark:text-slate-500">
        <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" className="mb-3">
          <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
        </svg>
        <p className="text-sm font-semibold">Bu sayfaya erişim yetkiniz yok.</p>
      </div>
    )
  }

  return (
    <div className="space-y-4 md:space-y-5">

      <ConfirmModal
        isOpen={unsavedConfirmOpen}
        onConfirm={handleUnsavedConfirm}
        onCancel={() => { setUnsavedConfirmOpen(false); setPendingUser(null) }}
        title="Kaydedilmemiş Değişiklikler"
        description="Kaydedilmemiş değişiklikleriniz var. Devam ederseniz bu değişiklikler kaybolacak."
        confirmText="Devam Et"
        cancelText="İptal"
        variant="warning"
      />

      <ConfirmModal
        isOpen={confirmOpen}
        onConfirm={handleConfirmSave}
        onCancel={() => setConfirmOpen(false)}
        title="Yetkileri Güncelle"
        description={`${selectedUser?.firstName} ${selectedUser?.lastName} adlı kullanıcının yetkileri güncellenecek. Onaylıyor musunuz?`}
        confirmText="Güncelle"
        cancelText="İptal"
        variant="primary"
        loading={saveLoading}
      />

      <PageTitle
        icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" /></svg>}
        title="Yetki Yönetimi"
        subtitle="Kullanıcı bazlı izin yönetimi"
      />

      {/* Ana Kart */}
      <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8 overflow-hidden">

        {/* ── Kullanıcı Seçim Alanı ── */}
        <div className="px-4 md:px-6 py-4 md:py-5 border-b border-slate-100 dark:border-white/8">
          {!selectedUser ? (
            <div className="relative max-w-lg">
              <div className="flex items-center gap-3 h-12 px-4 rounded-xl border-2 border-dashed border-slate-300 dark:border-white/15
                bg-slate-50 dark:bg-white/3 focus-within:border-emerald-400 focus-within:bg-white dark:focus-within:bg-white/6
                focus-within:border-solid transition-all">
                {searchLoading ? (
                  <svg className="animate-spin text-emerald-400 flex-shrink-0" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M21 12a9 9 0 1 1-6.219-8.56" />
                  </svg>
                ) : (
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-400 flex-shrink-0">
                    <circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" />
                  </svg>
                )}
                <input
                  type="text"
                  value={search}
                  onChange={(e) => { setSearch(e.target.value); setShowDropdown(true) }}
                  onBlur={() => setTimeout(() => setShowDropdown(false), 150)}
                  placeholder="Yetki düzenlemek istediğiniz kullanıcıyı arayın..."
                  className="flex-1 w-full bg-transparent outline-none text-sm text-slate-800 dark:text-white placeholder-slate-400 dark:placeholder-white/25"
                />
              </div>

              {showDropdown && search.trim() && (
                <div className="absolute z-20 top-full mt-2 w-full bg-white dark:bg-[#001830] rounded-xl border border-slate-200 dark:border-white/10 shadow-xl overflow-hidden">
                  {users.length === 0 ? (
                    <div className="px-4 py-4 text-sm text-slate-400 dark:text-slate-500">Kullanıcı bulunamadı.</div>
                  ) : (
                    <>
                      <p className="px-4 pt-3 pb-1.5 text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider">
                        Kullanıcılar
                      </p>
                      {users.map((user) => (
                        <button
                          key={user.id}
                          onMouseDown={() => handleSelectUser(user)}
                          className="w-full flex items-center justify-between px-4 py-3 hover:bg-emerald-50 dark:hover:bg-emerald-500/8 transition-colors group"
                        >
                          <div className="text-left pr-2 break-all">
                            <p className="text-sm font-semibold text-slate-800 dark:text-white group-hover:text-emerald-700 dark:group-hover:text-emerald-400 transition-colors">
                              {user.firstName} {user.lastName}
                            </p>
                            <p className="text-xs text-slate-400 dark:text-slate-500">{user.email}</p>
                          </div>
                          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                            className="text-slate-300 dark:text-slate-600 group-hover:text-emerald-400 transition-colors flex-shrink-0">
                            <polyline points="9 18 15 12 9 6" />
                          </svg>
                        </button>
                      ))}
                    </>
                  )}
                </div>
              )}
            </div>
          ) : (
            // RESPONSİVE: Mobilde alt alta, sm ve üzeri yan yana
            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
              <div className="flex items-start sm:items-center gap-3">
                <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500 to-emerald-700 flex items-center justify-center text-white text-sm font-bold flex-shrink-0 shadow-sm mt-0.5 sm:mt-0">
                  {selectedUser.firstName?.[0]}{selectedUser.lastName?.[0]}
                </div>
                <div>
                  <p className="text-sm font-bold text-slate-800 dark:text-white">
                    {selectedUser.firstName} {selectedUser.lastName}
                  </p>
                  <div className="flex flex-wrap items-center gap-2 mt-0.5">
                    <p className="text-xs text-slate-400 dark:text-slate-500">{selectedUser.email}</p>
                    {isDirty && (
                      <span className="flex items-center gap-1 text-[10px] md:text-xs text-amber-500 dark:text-amber-400 font-medium px-2 py-0.5 md:py-1 bg-amber-50 dark:bg-amber-500/10 rounded-full border border-amber-200 dark:border-amber-500/20">
                        <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                          <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
                        </svg>
                        Kaydedilmemiş değişiklik
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="flex flex-col-reverse sm:flex-row items-center gap-2 w-full sm:w-auto">
                <button
                  onClick={handleClearUser}
                  className="w-full sm:w-auto flex justify-center items-center gap-1.5 px-3 py-2.5 md:py-2 rounded-lg text-sm md:text-xs font-medium text-slate-500 dark:text-slate-400
                    hover:bg-slate-100 dark:hover:bg-white/8 hover:text-slate-700 dark:hover:text-slate-200 transition-colors"
                >
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="md:w-3 md:h-3"><polyline points="15 18 9 12 15 6" /></svg>
                  Farklı Kullanıcı Seç
                </button>
                {hasPermission('Permissions.Assign') && (
                  <FormButton
                    loading={saveLoading}
                    loadingText="Kaydediliyor..."
                    disabled={!isDirty}
                    className="w-full sm:w-auto flex justify-center items-center gap-2 px-5 md:px-4 py-2.5 md:py-2 rounded-lg text-sm font-semibold transition-all
                      bg-slate-900 dark:bg-emerald-500 text-white hover:bg-slate-700 dark:hover:bg-emerald-400
                      disabled:opacity-40 disabled:cursor-not-allowed"
                    onClick={() => setConfirmOpen(true)}
                  >
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="md:w-3.5 md:h-3.5">
                      <polyline points="20 6 9 17 4 12" />
                    </svg>
                    Kaydet
                  </FormButton>
                )}
              </div>
            </div>
          )}
        </div>

        {/* ── İki Sütun İzin Paneli ── */}
        {selectedUser && (
          <>
            {(saveError || saveSuccess) && (
              <div className="px-4 md:px-6 pt-4 md:pt-5 pb-0">
                <AlertMessage type={saveError ? 'error' : 'success'} message={saveError || saveSuccess} />
              </div>
            )}

            {permissionsLoading ? (
              <div className="flex items-center justify-center py-20">
                <svg className="animate-spin text-emerald-500" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M21 12a9 9 0 1 1-6.219-8.56" />
                </svg>
              </div>
            ) : permissionsError ? (
              <div className="px-4 md:px-6 py-4 md:py-5">
                <AlertMessage type="error" message={permissionsError} />
              </div>
            ) : (
              // RESPONSİVE: Mobilde flex-col (alt alta), md'de flex-row (yan yana)
              <div className="flex flex-col md:flex-row" style={{ minHeight: 420 }}>

                {/* Sol — Modül Listesi (Mobilde yatay kaydırılabilir çipler, masaüstünde dikey liste) */}
                <div className="w-full md:w-64 flex-shrink-0 border-b md:border-b-0 md:border-r border-slate-100 dark:border-white/8">
                  <p className="hidden md:block px-4 pt-4 pb-2 text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider">
                    Modüller
                  </p>
                  <div className="flex md:flex-col overflow-x-auto scrollbar-hide px-3 py-3 md:p-0 md:py-2 gap-2 md:gap-0">
                    {modules.map((module) => {
                      const activeCount = module.permissions.filter(p => checkedIds.includes(p.id)).length
                      const totalCount = module.permissions.length
                      const isSelected = selectedModuleId === module.id

                      return (
                        <button
                          key={module.id}
                          onClick={() => setSelectedModuleId(module.id)}
                          className={`flex-shrink-0 md:w-full flex items-center justify-between gap-3 px-4 py-2.5 md:py-3 text-left transition-all relative rounded-xl md:rounded-none whitespace-nowrap
                            ${isSelected
                              ? 'bg-emerald-50 dark:bg-emerald-500/10 text-emerald-700 dark:text-emerald-400'
                              : 'bg-slate-50 md:bg-transparent text-slate-600 dark:text-slate-300 hover:bg-slate-100 md:hover:bg-slate-50 dark:hover:bg-white/4 hover:text-slate-800 dark:hover:text-white'
                            }`}
                        >
                          {/* Aktif çizgisi (Sadece masaüstünde solda görünür) */}
                          {isSelected && (
                            <span className="hidden md:block absolute left-0 top-1 bottom-1 w-0.5 bg-emerald-500 rounded-full" />
                          )}
                          <span className="text-sm font-semibold truncate">{module.name}</span>
                          <span className={`text-xs font-semibold px-2 py-0.5 rounded-full flex-shrink-0 transition-colors
                            ${activeCount > 0
                              ? isSelected
                                ? 'bg-emerald-100 dark:bg-emerald-500/20 text-emerald-700 dark:text-emerald-400'
                                : 'bg-emerald-100/50 md:bg-emerald-50 dark:bg-emerald-500/10 text-emerald-600 dark:text-emerald-400'
                              : 'bg-slate-200/50 md:bg-slate-100 dark:bg-white/8 text-slate-400 dark:text-slate-500'
                            }`}>
                            {activeCount}/{totalCount}
                          </span>
                        </button>
                      )
                    })}
                  </div>
                </div>

                {/* Sağ — Seçili Modülün İzinleri */}
                <div className="flex-1 flex flex-col min-w-0">
                  {selectedModule ? (
                    <>
                      {/* Modül başlığı */}
                      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 px-4 md:px-6 py-4 border-b border-slate-100 dark:border-white/8">
                        <div>
                          <h2 className="text-base font-bold text-slate-800 dark:text-white">
                            {selectedModule.name}
                          </h2>
                          <p className="text-xs text-slate-400 dark:text-slate-500 mt-0.5">
                            {selectedModule.permissions.filter(p => checkedIds.includes(p.id)).length} aktif · {selectedModule.permissions.length} toplam izin
                          </p>
                        </div>
                        <button
                          onClick={() => toggleModuleAll(selectedModule)}
                          className="w-full sm:w-auto flex justify-center items-center gap-2 px-4 py-2 sm:py-1.5 rounded-lg text-sm sm:text-xs font-semibold transition-all
                            border border-slate-200 dark:border-white/10 text-slate-600 dark:text-slate-300
                            hover:border-emerald-400 hover:text-emerald-600 dark:hover:text-emerald-400 hover:bg-emerald-50 dark:hover:bg-emerald-500/8"
                        >
                          {isModuleAllChecked(selectedModule) ? (
                            <>
                              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" className="sm:w-3 sm:h-3">
                                <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                              </svg>
                              Tümünü Kaldır
                            </>
                          ) : (
                            <>
                              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" className="sm:w-3 sm:h-3">
                                <polyline points="20 6 9 17 4 12" />
                              </svg>
                              Tümünü Seç
                            </>
                          )}
                        </button>
                      </div>

                      {/* İzin listesi */}
                      <div className="flex-1 divide-y divide-slate-50 dark:divide-white/4 overflow-y-auto">
                        {selectedModule.permissions.map((permission) => {
                          const isChecked = checkedIds.includes(permission.id)
                          return (
                            <label
                              key={permission.id}
                              className={`flex items-center gap-4 px-4 md:px-6 py-4 cursor-pointer transition-colors
                                ${isChecked
                                  ? 'bg-emerald-50/50 dark:bg-emerald-500/5 hover:bg-emerald-50 dark:hover:bg-emerald-500/8'
                                  : 'hover:bg-slate-50 dark:hover:bg-white/3'
                                }`}
                            >
                              <input
                                type="checkbox"
                                checked={isChecked}
                                onChange={() => togglePermission(permission.id)}
                                className="w-5 h-5 md:w-4 md:h-4 rounded accent-emerald-500 cursor-pointer flex-shrink-0"
                              />
                              <div className="flex-1">
                                <p className={`text-base md:text-sm font-medium transition-colors
                                  ${isChecked ? 'text-slate-800 dark:text-white' : 'text-slate-600 dark:text-slate-400'}`}>
                                  {permission.description || permission.name}
                                </p>
                              </div>
                              {isChecked && (
                                <span className="text-[10px] md:text-xs font-semibold px-2 py-0.5 rounded-full flex-shrink-0
                                  bg-emerald-100 dark:bg-emerald-500/20 text-emerald-700 dark:text-emerald-400
                                  border border-emerald-200 dark:border-emerald-500/30">
                                  Aktif
                                </span>
                              )}
                            </label>
                          )
                        })}
                      </div>
                    </>
                  ) : (
                    <div className="flex-1 flex items-center justify-center py-20 text-slate-300 dark:text-slate-600 px-4 text-center">
                      <p className="text-sm">Bir modül seçin.</p>
                    </div>
                  )}
                </div>
              </div>
            )}
          </>
        )}

        {/* Kullanıcı seçilmedi — boş durum */}
        {!selectedUser && (
          <div className="flex flex-col items-center justify-center py-20 text-slate-300 dark:text-slate-600 px-4 text-center">
            <svg width="44" height="44" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" className="mb-4">
              <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
            </svg>
            <p className="text-sm font-medium">Yetki düzenlemek için bir kullanıcı seçin.</p>
          </div>
        )}
      </div>
    </div>
  )
}