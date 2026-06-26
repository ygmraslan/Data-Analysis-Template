import { useState } from 'react'
import { usePermission } from '../../hooks/usePermission'
import PageTitle from '../../components/ui/PageTitle'
import AuditLogsList from './AuditLogsList'
import AuthLogsList from './AuthLogsList'

const AuditLogIcon = (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
    <polyline points="14 2 14 8 20 8" />
    <line x1="16" y1="13" x2="8" y2="13" /><line x1="16" y1="17" x2="8" y2="17" />
  </svg>
)

const TABS = [
  { key: 'audit',  label: 'İşlem Logları',      permission: 'AuditLogs.View' },
  { key: 'auth',   label: 'Giriş Hareketleri',  permission: 'AuthLogs.View'  },
]

export default function AuditLogs() {
  const { hasPermission } = usePermission()
  const [activeTab, setActiveTab] = useState('audit')

  const visibleTabs = TABS.filter(t => hasPermission(t.permission))

  if (visibleTabs.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-24 text-slate-400 dark:text-slate-500">
        <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" className="mb-3">
          <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
        </svg>
        <p className="text-sm font-semibold">Bu sayfaya erişim yetkiniz yok.</p>
      </div>
    )
  }

  const tabs = (
    <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
      {visibleTabs.map(t => (
        <button
          key={t.key}
          onClick={() => setActiveTab(t.key)}
          className={`px-4 py-1.5 rounded-md text-xs font-semibold transition-all ${
            activeTab === t.key
              ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
              : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
          }`}
        >
          {t.label}
        </button>
      ))}
    </div>
  )

  return (
    <div className="space-y-4 md:space-y-5">
      <PageTitle
        icon={AuditLogIcon}
        title="Denetim Kayıtları"
        action={tabs}
      />
      {activeTab === 'audit' && hasPermission('AuditLogs.View') && <AuditLogsList />}
      {activeTab === 'auth'  && hasPermission('AuthLogs.View')  && <AuthLogsList />}
    </div>
  )
}