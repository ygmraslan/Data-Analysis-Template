import { useState, useRef, useEffect } from 'react'
import { Outlet, NavLink, useNavigate, useLocation } from 'react-router-dom'
import useAuthStore from '../store/authStore'
import useThemeStore from '../store/themeStore'
import axiosInstance from '../api/axiosInstance'
import ConfirmModal from '../components/ui/ConfirmModal'
import { usePermission } from '../hooks/usePermission'
import FooterText from '../components/ui/FooterText'
import ScrollToTop from '../components/ui/ScrollToTop'

const navConfig = [
  {
    label: 'Dashboard',
    path: '/dashboard',
    permission:'Dashboard.Kpi.View',
    icon: (
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
        <rect x="3" y="3" width="7" height="7" rx="1" /><rect x="14" y="3" width="7" height="7" rx="1" />
        <rect x="3" y="14" width="7" height="7" rx="1" /><rect x="14" y="14" width="7" height="7" rx="1" />
      </svg>
    ),
  },
  {
    label: 'Portföy Analizi',
    icon: (
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
        <path d="M21 21H4.6C4.03995 21 3.75992 21 3.54601 20.891C3.35785 20.7951 3.20487 20.6422 3.10899 20.454C3 20.2401 3 19.9601 3 19.4V3" />
        <path d="M7 14.5L11.5 10L14.5 13L21 6.5" />
      </svg>
    ),
    children: [
      {
        label: 'Yönetici Özeti',
        path: '/exec-summary',
        permission: 'ExecSummary.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
            <polyline points="14 2 14 8 20 8" />
            <line x1="16" y1="13" x2="8" y2="13" />
            <line x1="16" y1="17" x2="8" y2="17" />
          </svg>
        ),
      },
      {
        label: 'Özel Segment',
        path: '/custom-segment',
        permission: 'CustomSegment.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="12" cy="12" r="10" />
            <path d="M12 2a10 10 0 0 1 10 10" />
            <path d="M12 12l6.5-6.5" />
            <path d="M12 12V2" />
          </svg>
        ),
      },
    ],
  },
  {
    label: 'Detay Veriler',
    icon: (
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
        <line x1="18" y1="20" x2="18" y2="10" /><line x1="12" y1="20" x2="12" y2="4" />
        <line x1="6" y1="20" x2="6" y2="14" />
      </svg>
    ),
    children: [
      {
        label: 'Bölge Verileri',
        path: '/region',
        permission: 'Region.Kpi.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7z" />
            <circle cx="12" cy="9" r="2.5" />
          </svg>
        ),
      },
      {
        label: 'Marka Verileri',
        path: '/brand',
        permission: 'Brand.Kpi.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <rect x="2" y="7" width="20" height="14" rx="2" />
            <path d="M16 7V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v2" />
            <line x1="12" y1="12" x2="12" y2="16" />
            <line x1="10" y1="14" x2="14" y2="14" />
          </svg>
        ),
      },
      {
        label: 'Coğrafi Analiz',
        path: '/city',
        permission: 'City.Kpi.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="12" cy="12" r="10" />
            <line x1="2" y1="12" x2="22" y2="12" />
            <path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z" />
          </svg>
        ),
      },
      {
        label: 'Araç Analiz',
        path: '/vehicle',
        permission: 'Vehicle.Kpi.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M5 17H3a2 2 0 0 1-2-2V9a2 2 0 0 1 2-2h14l4 4v4a2 2 0 0 1-2 2h-2" />
            <circle cx="7" cy="17" r="2" />
            <circle cx="17" cy="17" r="2" />
          </svg>
        ),
      },
      {
        label: 'Şirket Geçiş Analiz',
        path: '/company',
        permission: 'Company.Kpi.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M17 1l4 4-4 4" />
            <path d="M3 11V9a4 4 0 0 1 4-4h14" />
            <path d="M7 23l-4-4 4-4" />
            <path d="M21 13v2a4 4 0 0 1-4 4H3" />
          </svg>
        ),
      },
      {
        label: 'Acente Analiz',
        path: '/agency',
        permission: 'Agency.Kpi.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M3 21h18" />
            <path d="M5 21V7l8-4v18" />
            <path d="M19 21V11l-6-4" />
            <path d="M9 9v.01" />
            <path d="M9 12v.01" />
            <path d="M9 15v.01" />
            <path d="M9 18v.01" />
          </svg>
        ),
      },
      {
        label: 'Demografik Analiz',
        path: '/demographic',
        permission: 'Demo.Kpi.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
            <circle cx="9" cy="7" r="4" />
            <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
            <path d="M16 3.13a4 4 0 0 1 0 7.75" />
          </svg>
        ),
      }
    ],
  },
  {
    label: 'Admin',
    icon: (
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
        <circle cx="12" cy="8" r="4" /><path d="M20 21a8 8 0 1 0-16 0" />
      </svg>
    ),
    children: [
      {
        label: 'Kullanıcılar',
        path: '/users',
        permission: 'Users.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
          </svg>
        ),
      },
      {
        label: 'Yetki Yönetimi',
        path: '/permissions',
        permission: 'Permissions.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
          </svg>
        ),
      },
      {
        label: 'Denetim Kayıtları',
        path: '/audit-logs',
        permission: 'AuditLogs.View',
        icon: (
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
            <polyline points="14 2 14 8 20 8" />
            <line x1="16" y1="13" x2="8" y2="13" /><line x1="16" y1="17" x2="8" y2="17" />
          </svg>
        ),
      },
    ],
  },
]

function getInitials(user) {
  if (user?.firstName && user?.lastName)
    return (user.firstName[0] + user.lastName[0]).toUpperCase()
  if (user?.email)
    return user.email.substring(0, 2).toUpperCase()
  return 'KI'
}

function getDisplayName(user) {
  if (user?.firstName && user?.lastName)
    return user.firstName + ' ' + user.lastName
  if (user?.email) {
    const local = user.email.split('@')[0]
    const parts = local.split('.')
    if (parts.length >= 2)
      return parts[0].charAt(0).toUpperCase() + parts[0].slice(1) + ' ' +
        parts[1].charAt(0).toUpperCase() + parts[1].slice(1)
    return local.charAt(0).toUpperCase() + local.slice(1)
  }
  return 'Kullanıcı'
}

export default function PanelLayout() {
  const navigate = useNavigate()
  const location = useLocation()
  const { user, logout } = useAuthStore()
  const { isDark, toggleTheme } = useThemeStore()
  const { hasPermission } = usePermission()
  const [userMenuOpen, setUserMenuOpen] = useState(false)
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false)
  const [openGroups, setOpenGroups] = useState({})
  const [logoutModalOpen, setLogoutModalOpen] = useState(false)
  const [logoutLoading, setLogoutLoading] = useState(false)
  const userMenuRef = useRef(null)

  useEffect(() => {
    const handler = (e) => {
      if (userMenuRef.current && !userMenuRef.current.contains(e.target))
        setUserMenuOpen(false)
    }
    document.addEventListener('mousedown', handler)
    return () => document.removeEventListener('mousedown', handler)
  }, [])

  useEffect(() => {
    if (mobileMenuOpen) document.body.style.overflow = 'hidden'
    else document.body.style.overflow = ''
    return () => { document.body.style.overflow = '' }
  }, [mobileMenuOpen])

  useEffect(() => {
    setMobileMenuOpen(false)
    setUserMenuOpen(false)
  }, [location.pathname])

  useEffect(() => {
    setOpenGroups(prev => {
      const next = { ...prev }

      navConfig.forEach(item => {
        if (!item.children) return

        const hasActive = item.children.some(c =>
          location.pathname.startsWith(c.path)
        )

        if (hasActive) next[item.label] = true
      })

      return next
    })
  }, [location.pathname])

  const toggleGroup = (label) => {
    setOpenGroups(prev => ({ ...prev, [label]: !prev[label] }))
  }

  const getVisibleChildren = (children) =>
    children.filter(c => !c.permission || hasPermission(c.permission))

  const handleLogout = async () => {
    setLogoutLoading(true)
    try {
      await axiosInstance.post('/auth/logout')
    } catch (error) {
      console.error('Logout error:', error)
    }
    finally {
      logout()
      navigate('/login')
      setLogoutLoading(false)
    }
  }

  const desktopLinkClass = ({ isActive }) =>
    `flex items-center gap-2 px-4 h-full text-sm font-medium border-b-2 transition-all duration-200 whitespace-nowrap
    ${isActive
      ? 'text-slate-900 dark:text-white border-emerald-500 bg-emerald-50 dark:bg-white/6'
      : 'text-slate-500 dark:text-slate-300 border-transparent hover:text-slate-800 dark:hover:text-white hover:bg-slate-100 dark:hover:bg-white/4'
    }`

  const mobileLinkClass = ({ isActive }) =>
    `flex items-center gap-3 px-3 py-2.5 text-sm font-medium rounded-lg transition-colors
    ${isActive
      ? 'text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-500/10'
      : 'text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-white/8'
    }`

  return (
    <div className="min-h-screen bg-slate-100 dark:bg-slate-950 transition-colors duration-300">

      <ConfirmModal
        isOpen={logoutModalOpen}
        onConfirm={handleLogout}
        onCancel={() => setLogoutModalOpen(false)}
        title="Çıkış Yap"
        description="Oturumunuzu kapatmak istediğinize emin misiniz?"
        confirmText="Çıkış Yap"
        cancelText="İptal"
        variant="danger"
        loading={logoutLoading}
      />

      {/* ── Mobil Sidebar ── */}
      {mobileMenuOpen && (
        <div className="fixed inset-0 z-50 md:hidden">
          <div className="absolute inset-0 bg-black/50" onClick={() => setMobileMenuOpen(false)} />
          <div className="absolute left-0 top-0 bottom-0 w-64 bg-white dark:bg-[#002147] shadow-2xl flex flex-col">

            {/* Sidebar Header */}
            <div className="flex items-center justify-between px-4 h-14 border-b border-slate-200 dark:border-white/8 flex-shrink-0">
              <div className="flex items-center gap-2.5 cursor-pointer" onClick={() => navigate('/dashboard')}>
                <div className="w-7 h-7 rounded-lg bg-gradient-to-br from-emerald-400 to-emerald-600 flex items-center justify-center flex-shrink-0">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.5">
                    <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5" />
                  </svg>
                </div>
                <span className="font-bold text-sm text-slate-900 dark:text-white">
                  Data<span className="text-emerald-500">Analysis</span>
                </span>
              </div>
              <button
                onClick={() => setMobileMenuOpen(false)}
                className="w-7 h-7 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-100 dark:hover:bg-white/8"
              >
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                </svg>
              </button>
            </div>

            {/* Sidebar Nav */}
            <nav className="flex-1 overflow-y-auto px-2 py-3 space-y-0.5">
              {navConfig.map((item) => {
                if (item.path) {
                  // Permission kontrolü - yetkisi yoksa gösterme
                  if (item.permission && !hasPermission(item.permission)) return null
                  
                  // Direkt link
                  return (
                    <NavLink key={item.path} to={item.path} className={mobileLinkClass}>
                      <span className="text-slate-400 dark:text-slate-500 flex-shrink-0">{item.icon}</span>
                      {item.label}
                    </NavLink>
                  )
                }

                if (item.children) {
                  const visible = getVisibleChildren(item.children)
                  if (visible.length === 0) return null
                  const isOpen = openGroups[item.label] || false
                  const isGroupActive = visible.some(c => location.pathname.startsWith(c.path))

                  return (
                    <div key={item.label}>
                      {/* Grup başlığı */}
                      <button
                        onClick={() => toggleGroup(item.label)}
                        className={`w-full flex items-center gap-3 px-3 py-2.5 text-sm font-medium rounded-lg transition-colors
                          ${isGroupActive
                            ? 'text-emerald-600 dark:text-emerald-400'
                            : 'text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-white/8'
                          }`}
                      >
                        <span className={`flex-shrink-0 ${isGroupActive ? 'text-emerald-500' : 'text-slate-400 dark:text-slate-500'}`}>
                          {item.icon}
                        </span>
                        <span className="flex-1 text-left">{item.label}</span>
                        <svg
                          width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                          className={`flex-shrink-0 text-slate-400 transition-transform duration-200 ${isOpen ? 'rotate-180' : ''}`}
                        >
                          <polyline points="6 9 12 15 18 9" />
                        </svg>
                      </button>

                      {/* Alt menüler */}
                      {isOpen && (
                        <div className="ml-3 pl-3 border-l border-slate-200 dark:border-white/8 space-y-0.5 mt-0.5 mb-1">
                          {visible.map((child) => (
                            <NavLink key={child.path} to={child.path} className={mobileLinkClass}>
                              <span className="text-slate-400 dark:text-slate-500 flex-shrink-0">{child.icon}</span>
                              {child.label}
                            </NavLink>
                          ))}
                        </div>
                      )}
                    </div>
                  )
                }

                return null
              })}
            </nav>

            {/* Sidebar Footer — kompakt */}
            <div className="px-2 py-3 border-t border-slate-200 dark:border-white/8 flex-shrink-0">
              <div className="flex items-center gap-2 px-3 py-2 rounded-lg">
                <div className="w-7 h-7 rounded-full bg-gradient-to-br from-emerald-500 to-emerald-700 flex items-center justify-center text-white text-xs font-bold flex-shrink-0">
                  {getInitials(user)}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-xs font-semibold text-slate-800 dark:text-white truncate">{getDisplayName(user)}</p>
                </div>
                <button
                  onClick={() => { navigate('/account'); setMobileMenuOpen(false) }}
                  className="w-7 h-7 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-100 dark:hover:bg-white/8 transition-colors flex-shrink-0"
                  title="Hesap Bilgileri"
                >
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" />
                  </svg>
                </button>
                <button
                  onClick={() => { setMobileMenuOpen(false); setLogoutModalOpen(true) }}
                  className="w-7 h-7 rounded-lg flex items-center justify-center text-red-400 hover:bg-red-50 dark:hover:bg-red-500/10 transition-colors flex-shrink-0"
                  title="Çıkış Yap"
                >
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                    <polyline points="16 17 21 12 16 7" />
                    <line x1="21" y1="12" x2="9" y2="12" />
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* ── Üst Header ── */}
      <header className="sticky top-0 z-40 h-14 bg-white dark:bg-[#002147] border-b border-slate-200 dark:border-white/8 shadow-sm">
        <div className="flex items-center h-full px-4 md:px-6 gap-3">

          {/* Hamburger — sadece mobilde */}
          <button
            onClick={() => setMobileMenuOpen(true)}
            className="md:hidden w-8 h-8 rounded-lg flex items-center justify-center text-slate-500 hover:bg-slate-100 dark:hover:bg-white/8 dark:text-slate-400 transition-colors"
          >
            <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <line x1="3" y1="6" x2="21" y2="6" /><line x1="3" y1="12" x2="21" y2="12" /><line x1="3" y1="18" x2="21" y2="18" />
            </svg>
          </button>

          {/* Logo */}
          <div className="flex items-center gap-2.5 cursor-pointer flex-shrink-0" onClick={() => navigate('/dashboard')}>
            <div className="w-8 h-8 rounded-xl bg-gradient-to-br from-emerald-400 to-emerald-600 flex items-center justify-center shadow-lg shadow-emerald-500/20">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.5">
                <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5" />
              </svg>
            </div>
            <div className="hidden sm:block">
              <div className="font-bold text-sm text-slate-900 dark:text-white leading-tight">
                Data<span className="text-emerald-500">Analysis</span>
              </div>
              <div className="text-[10px] text-emerald-500 uppercase tracking-widest font-semibold leading-tight">
                Analytics
              </div>
            </div>
          </div>

          {/* Sağ Alan */}
          <div className="ml-auto flex items-center gap-1.5">


            {/* Tema Toggle */}
            <button
              onClick={toggleTheme}
              className="w-8 h-8 rounded-lg flex items-center justify-center transition-colors
                text-slate-500 hover:bg-slate-100 hover:text-slate-700
                dark:text-yellow-400 dark:hover:bg-white/8"
            >
              {isDark ? (
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <circle cx="12" cy="12" r="5" />
                  <line x1="12" y1="1" x2="12" y2="3" /><line x1="12" y1="21" x2="12" y2="23" />
                  <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" /><line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
                  <line x1="1" y1="12" x2="3" y2="12" /><line x1="21" y1="12" x2="23" y2="12" />
                  <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" /><line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
                </svg>
              ) : (
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
                </svg>
              )}
            </button>

            <div className="w-px h-5 bg-slate-200 dark:bg-white/10 mx-0.5 hidden md:block" />

            {/* Kullanıcı Menüsü — sadece masaüstünde */}
            <div className="relative hidden md:block" ref={userMenuRef}>
              <button
                onClick={() => setUserMenuOpen(!userMenuOpen)}
                className="flex items-center gap-2 px-2.5 py-1.5 rounded-lg transition-colors hover:bg-slate-100 dark:hover:bg-white/8"
              >
                <div className="w-7 h-7 rounded-full bg-gradient-to-br from-emerald-500 to-emerald-700 flex items-center justify-center text-white text-xs font-bold flex-shrink-0">
                  {getInitials(user)}
                </div>
                <span className="text-sm font-medium text-slate-800 dark:text-white hidden lg:block">
                  {getDisplayName(user)}
                </span>
                <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                  className={`text-slate-400 transition-transform duration-200 ${userMenuOpen ? 'rotate-180' : ''}`}>
                  <polyline points="6 9 12 15 18 9" />
                </svg>
              </button>

              {userMenuOpen && (
                <div className="absolute right-0 top-full mt-2 w-44 rounded-xl shadow-xl border overflow-hidden z-50
                  bg-white border-slate-200 dark:bg-[#002147] dark:border-white/10">
                  <div className="p-1">
                    <button
                      onClick={() => { setUserMenuOpen(false); navigate('/account') }}
                      className="w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm font-medium
                        text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-white/8 transition-colors"
                    >
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" />
                      </svg>
                      Hesap Bilgileri
                    </button>
                    <div className="my-1 border-t border-slate-100 dark:border-white/8" />
                    <button
                      onClick={() => { setUserMenuOpen(false); setLogoutModalOpen(true) }}
                      className="w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm font-medium
                        text-red-500 hover:bg-red-50 dark:hover:bg-red-500/10 transition-colors"
                    >
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                        <polyline points="16 17 21 12 16 7" />
                        <line x1="21" y1="12" x2="9" y2="12" />
                      </svg>
                      Çıkış Yap
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </header>

      {/* ── Alt Navigasyon — sadece masaüstünde ── */}
      <nav className="hidden md:block sticky top-14 z-30 border-b border-slate-200 dark:border-white/6 shadow-sm bg-slate-50 dark:bg-[#001229]">
        <div className="flex items-center h-11 px-6 gap-1">
          {navConfig.map((item) => {
            if (item.path) {
              // Permission kontrolü - yetkisi yoksa gösterme
              if (item.permission && !hasPermission(item.permission)) return null
              
              return (
                <NavLink key={item.path} to={item.path} className={desktopLinkClass}>
                  {item.icon}
                  {item.label}
                </NavLink>
              )
            }

            if (item.children) {
              const visible = getVisibleChildren(item.children)
              if (visible.length === 0) return null
              const isGroupActive = visible.some(c => location.pathname.startsWith(c.path))

              return (
                <div
                  key={item.label}
                  className="relative h-full flex items-center group"
                >
                  <button
                    className={`flex items-center gap-2 px-4 h-full text-sm font-medium border-b-2 transition-all duration-200 whitespace-nowrap
                      ${isGroupActive
                        ? 'text-slate-900 dark:text-white border-emerald-500 bg-emerald-50 dark:bg-white/6'
                        : 'text-slate-500 dark:text-slate-300 border-transparent hover:text-slate-800 dark:hover:text-white hover:bg-slate-100 dark:hover:bg-white/4'
                      }`}
                  >
                    {item.icon}
                    {item.label}
                    <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                      className="transition-transform duration-200 group-hover:rotate-180">
                      <polyline points="6 9 12 15 18 9" />
                    </svg>
                  </button>

                  {/* Dropdown — hover ile */}
                  <div className="absolute top-full left-0 w-48 border rounded-b-xl shadow-lg overflow-hidden
                    opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-150
                    bg-slate-50 border-slate-200 dark:bg-[#001229] dark:border-white/6">
                    {visible.map((child) => (
                      <NavLink
                        key={child.path}
                        to={child.path}
                        className={({ isActive }) =>
                          `flex items-center gap-2.5 px-4 py-2.5 text-sm font-medium transition-colors
                          ${isActive
                            ? 'text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-500/10'
                            : 'text-slate-600 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white hover:bg-slate-100 dark:hover:bg-white/6'
                          }`
                        }
                      >
                        <span className={location.pathname === child.path ? 'text-emerald-500' : 'text-slate-400 dark:text-slate-500'}>
                          {child.icon}
                        </span>
                        {child.label}
                      </NavLink>
                    ))}
                  </div>
                </div>
              )
            }

            return null
          })}
        </div>
      </nav>

      {/* ── İçerik ── */}
      <main className="p-4 md:p-6">
        <Outlet />
         <FooterText className="mt-8 mb-2" />
      </main>
       <ScrollToTop />
    </div>
  )
}