import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import usersApi from '../../api/usersApi'
import { formatDate } from '../../utils/formatDate'
import StatusBadge from '../../components/ui/StatusBadge'
import MfaBadge from '../../components/ui/MfaBadge'
import Pagination from '../../components/ui/Pagination'
import PageTitle from '../../components/ui/PageTitle'
import TableHead from '../../components/ui/TableHead'
import { usePermission } from '../../hooks/usePermission'

const UsersIcon = (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
    <path d="M23 21v-2a4 4 0 0 0-3-3.87" /><path d="M16 3.13a4 4 0 0 1 0 7.75" />
  </svg>
)

export default function Users() {
  const navigate = useNavigate()
  const { hasPermission } = usePermission()
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [isActiveFilter, setIsActiveFilter] = useState('')
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [totalCount, setTotalCount] = useState(0)
  const pageSize = 10

  const fetchUsers = useCallback(async () => {
    setLoading(true)
    setError('')
    try {
      const params = { page, pageSize }
      if (search) params.search = search
      if (isActiveFilter !== '') params.isActive = isActiveFilter === 'true'
      const response = await usersApi.getUsers(params)
      const data = response.data.value
      setUsers(data.users)
      setTotalPages(data.totalPages)
      setTotalCount(data.totalCount)
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setLoading(false)
    }
  }, [page, search, isActiveFilter])

  useEffect(() => { fetchUsers() }, [fetchUsers])

  const filterOptions = [
    { value: '', label: 'Tümü' },
    { value: 'true', label: 'Aktif' },
    { value: 'false', label: 'Pasif' },
  ]

  if (!hasPermission('Users.View')) {
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
      {/* Sayfa Başlığı ve Aksiyon Butonu */}
      <PageTitle
        icon={UsersIcon}
        title="Kullanıcılar"
        subtitle={`${totalCount} kullanıcı`}
        action={hasPermission('Users.Create') && (
          <button
            onClick={() => navigate('/users/new')}
            className="flex items-center justify-center gap-2 px-4 py-2 w-full sm:w-auto rounded-lg text-sm font-semibold transition-all bg-slate-900 dark:bg-emerald-500 text-white hover:bg-slate-700 dark:hover:bg-emerald-400"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" />
            </svg>
            Yeni Kullanıcı
          </button>
        )}
      />

      <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8 overflow-hidden">
        
        {/* Arama ve Filtreleme Alanı (Responsive) */}
        <div className="flex flex-col md:flex-row items-start md:items-center gap-3 px-4 md:px-6 py-4 border-b border-slate-100 dark:border-white/8">
          <div className="relative w-full md:flex-1">
            <input
              type="text"
              value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(1) }}
              placeholder="Ad, soyad veya e-posta ara..."
              className="w-full h-10 md:h-9 pl-9 pr-4 rounded-lg border text-sm transition-all bg-slate-50 border-slate-200 text-slate-800 placeholder-slate-300 dark:bg-white/6 dark:border-white/10 dark:text-white dark:placeholder-white/20 focus:outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-400/20"
            />
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
              className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none">
              <circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
          </div>

          <div className="flex w-full md:w-auto items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1 overflow-x-auto">
            {filterOptions.map((opt) => (
              <button
                key={opt.value}
                onClick={() => { setIsActiveFilter(opt.value); setPage(1) }}
                className={`flex-1 md:flex-none whitespace-nowrap px-3 py-2 md:py-1.5 rounded-md text-sm md:text-xs font-semibold transition-all
                  ${isActiveFilter === opt.value
                    ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                    : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
                  }`}
              >
                {opt.label}
              </button>
            ))}
          </div>
        </div>

        {/* İçerik Alanı (Hata, Yükleniyor, Boş Durum, Veri) */}
        {error ? (
          <div className="flex items-center justify-center py-16 text-sm text-red-500">{error}</div>
        ) : loading ? (
          <div className="flex items-center justify-center py-16">
            <svg className="animate-spin text-emerald-500" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M21 12a9 9 0 1 1-6.219-8.56" />
            </svg>
          </div>
        ) : users.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-16 text-slate-400 dark:text-slate-500">
            <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" className="mb-3">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
            </svg>
            <p className="text-sm">Kullanıcı bulunamadı.</p>
          </div>
        ) : (
          <>
            {/* MOBİL GÖRÜNÜM: KART YAPISI (Sadece md'den küçük ekranlarda) */}
            <div className="block md:hidden divide-y divide-slate-100 dark:divide-white/6">
              {users.map((user) => (
                <div 
                  key={user.id}
                  onClick={() => hasPermission('Users.ViewDetail') ? navigate(`/users/${user.id}`) : null}
                  className={`p-4 space-y-3 transition-colors ${hasPermission('Users.ViewDetail') ? 'active:bg-slate-50 dark:active:bg-white/4 cursor-pointer' : ''}`}
                >
                  <div className="flex justify-between items-start">
                    <div className="pr-4">
                      <h4 className="text-base font-semibold text-slate-800 dark:text-white">
                        {user.firstName} {user.lastName}
                      </h4>
                      <p className="text-sm text-slate-500 dark:text-slate-400 mt-0.5 break-all">{user.email}</p>
                    </div>
                    <div className="shrink-0">
                      <StatusBadge isActive={user.isActive} isLocked={user.isLocked} />
                    </div>
                  </div>
                  
                  <div className="flex items-center justify-between text-sm text-slate-500 dark:text-slate-400 pt-3 border-t border-slate-50 dark:border-white/4">
                    <div className="flex items-center gap-2">
                      <span className="text-xs font-medium">MFA:</span>
                      <MfaBadge hasMfa={user.hasMfa} />
                    </div>
                    <span className="text-xs flex items-center gap-1">
                      <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <circle cx="12" cy="12" r="10" /><polyline points="12 6 12 12 16 14" />
                      </svg>
                      {formatDate(user.lastLoginDate)}
                    </span>
                  </div>
                </div>
              ))}
            </div>

            {/* MASAÜSTÜ GÖRÜNÜM: TABLO YAPISI (md ve üzeri ekranlarda) */}
            <div className="hidden md:block overflow-x-auto w-full">
              <table className="w-full text-left">
                <thead>
                  <tr className="border-b border-slate-100 dark:border-white/8 bg-slate-50/50 dark:bg-white/2">
                    <TableHead>Ad Soyad</TableHead>
                    <TableHead>E-posta</TableHead>
                    <TableHead>Son Giriş</TableHead>
                    <TableHead>MFA</TableHead>
                    <TableHead>Durum</TableHead>
                    <th className="px-6 py-3.5" />
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100 dark:divide-white/6">
                  {users.map((user) => (
                    <tr
                      key={user.id}
                      onClick={() => hasPermission('Users.ViewDetail') ? navigate(`/users/${user.id}`) : null}
                      className={`transition-colors ${hasPermission('Users.ViewDetail') ? 'hover:bg-slate-50 dark:hover:bg-white/4 cursor-pointer' : 'cursor-default'}`}
                    >
                      <td className="px-6 py-4 text-sm font-semibold text-slate-800 dark:text-white">
                        {user.firstName} {user.lastName}
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-500 dark:text-slate-400">{user.email}</td>
                      <td className="px-6 py-4 text-sm text-slate-500 dark:text-slate-400">{formatDate(user.lastLoginDate)}</td>
                      <td className="px-6 py-4"><MfaBadge hasMfa={user.hasMfa} /></td>
                      <td className="px-6 py-4"><StatusBadge isActive={user.isActive} isLocked={user.isLocked} /></td>
                      <td className="px-6 py-4 text-right">
                        {hasPermission('Users.ViewDetail') && (
                          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                            className="text-slate-300 dark:text-slate-600 ml-auto">
                            <polyline points="9 18 15 12 9 6" />
                          </svg>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <Pagination page={page} totalPages={totalPages} totalCount={totalCount} onPageChange={setPage} />
          </>
        )}
      </div>
    </div>
  )
}