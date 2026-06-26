import { useState, useEffect, useCallback } from 'react'
import auditLogsApi from '../../api/auditLogsApi'
import { formatDate } from '../../utils/formatDate'
import Pagination from '../../components/ui/Pagination'
import AlertMessage from '../../components/ui/AlertMessage'

const STATUS_RANGES = [
  { label: 'Tümü', value: '' },
  { label: 'Başarılı', value: '200' },
  { label: 'Hatalı', value: 'error' },
]

const DATE_SHORTCUTS = [
  { label: 'Bugün', value: 'today' },
  { label: 'Bu Hafta', value: 'week' },
  { label: 'Bu Ay', value: 'month' },
  { label: 'Özel', value: 'custom' },
]

function getDateRange(shortcut) {
  const now = new Date()
  const start = new Date()
  if (shortcut === 'today') {
    start.setHours(0, 0, 0, 0)
    return { startDate: start.toISOString(), endDate: now.toISOString() }
  }
  if (shortcut === 'week') {
    start.setDate(now.getDate() - 7)
    return { startDate: start.toISOString(), endDate: now.toISOString() }
  }
  if (shortcut === 'month') {
    start.setMonth(now.getMonth() - 1)
    return { startDate: start.toISOString(), endDate: now.toISOString() }
  }
  return { startDate: '', endDate: '' }
}

function StatusBadge({ code }) {
  if (!code) return null
  const isSuccess = code >= 200 && code < 300
  const isWarning = code >= 400 && code < 500
  const isError   = code >= 500
  const cls = isSuccess
    ? 'bg-emerald-50 dark:bg-emerald-500/10 text-emerald-700 dark:text-emerald-400 border-emerald-200 dark:border-emerald-500/20'
    : isWarning
    ? 'bg-amber-50 dark:bg-amber-500/10 text-amber-700 dark:text-amber-400 border-amber-200 dark:border-amber-500/20'
    : isError
    ? 'bg-red-50 dark:bg-red-500/10 text-red-700 dark:text-red-400 border-red-200 dark:border-red-500/20'
    : 'bg-slate-50 dark:bg-white/6 text-slate-600 dark:text-slate-400 border-slate-200 dark:border-white/10'
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-semibold border ${cls}`}>
      {code}
    </span>
  )
}

function MethodBadge({ method }) {
  if (!method) return null
  const colors = {
    GET:    'bg-blue-50 dark:bg-blue-500/10 text-blue-700 dark:text-blue-400 border-blue-200 dark:border-blue-500/20',
    POST:   'bg-emerald-50 dark:bg-emerald-500/10 text-emerald-700 dark:text-emerald-400 border-emerald-200 dark:border-emerald-500/20',
    PUT:    'bg-amber-50 dark:bg-amber-500/10 text-amber-700 dark:text-amber-400 border-amber-200 dark:border-amber-500/20',
    DELETE: 'bg-red-50 dark:bg-red-500/10 text-red-700 dark:text-red-400 border-red-200 dark:border-red-500/20',
    PATCH:  'bg-purple-50 dark:bg-purple-500/10 text-purple-700 dark:text-purple-400 border-purple-200 dark:border-purple-500/20',
  }
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-semibold border ${colors[method] || 'bg-slate-50 text-slate-600 border-slate-200'}`}>
      {method}
    </span>
  )
}

function DetailModal({ log, onClose }) {
  if (!log) return null
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black/50" onClick={onClose} />
      <div className="relative w-full max-w-lg bg-white dark:bg-[#002147] rounded-2xl shadow-2xl border border-slate-200 dark:border-white/10 overflow-hidden">
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100 dark:border-white/8">
          <h3 className="text-sm font-bold text-slate-800 dark:text-white">Log Detayı</h3>
          <button onClick={onClose} className="w-7 h-7 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-100 dark:hover:bg-white/8 transition-colors">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
            </svg>
          </button>
        </div>
        <div className="p-5 space-y-3">
          {[
            { label: 'Kullanıcı',    value: `${log.fullName} (${log.email})` },
            { label: 'Modül',        value: log.module },
            { label: 'İşlem',        value: log.actionDescription?.trim() || log.action },
            { label: 'Tarih',        value: formatDate(log.createdDate) },
            { label: 'IP Adresi',    value: log.ipAddress },
            { label: 'Tarayıcı',     value: log.browser },
            { label: 'Metot',        value: log.method },
            { label: 'Endpoint',     value: log.request },
            { label: 'Durum Kodu',   value: <StatusBadge code={log.statusCode} /> },
          ].map(({ label, value }) => (
            <div key={label} className="flex items-start gap-3">
              <p className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider w-28 flex-shrink-0 pt-0.5">{label}</p>
              <p className="text-sm text-slate-700 dark:text-slate-300 break-all">{value}</p>
            </div>
          ))}
          {log.userAgent && (
            <div className="flex items-start gap-3">
              <p className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider w-28 flex-shrink-0 pt-0.5">User Agent</p>
              <p className="text-xs text-slate-500 dark:text-slate-400 break-all leading-relaxed">{log.userAgent}</p>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default function AuditLogsList() {
  const [logs,        setLogs]        = useState([])
  const [loading,     setLoading]     = useState(true)
  const [error,       setError]       = useState('')
  const [totalCount,  setTotalCount]  = useState(0)
  const [totalPages,  setTotalPages]  = useState(1)
  const [page,        setPage]        = useState(1)
  const [selectedLog, setSelectedLog] = useState(null)
  const [dateShortcut, setDateShortcut] = useState('today')
  const [customStart,  setCustomStart]  = useState('')
  const [customEnd,    setCustomEnd]    = useState('')
  const [statusFilter, setStatusFilter] = useState('')
  const [search,       setSearch]       = useState('')
  const pageSize = 20

  const buildParams = useCallback(() => {
    const params = { page, pageSize }
    if (dateShortcut !== 'custom') {
      const { startDate, endDate } = getDateRange(dateShortcut)
      if (startDate) params.startDate = startDate
      if (endDate)   params.endDate   = endDate
    } else {
      if (customStart) params.startDate = new Date(customStart).toISOString()
      if (customEnd)   params.endDate   = new Date(customEnd + 'T23:59:59').toISOString()
    }
    if (statusFilter === '200')   params.statusCode = 200
    if (statusFilter === 'error') params.statusCode = 400
    return params
  }, [page, dateShortcut, customStart, customEnd, statusFilter])

  const fetchLogs = useCallback(async () => {
    setLoading(true)
    setError('')
    try {
      const response = await auditLogsApi.getLogs(buildParams())
      const data = response.data.value
      setLogs(data.logs)
      setTotalCount(data.totalCount)
      setTotalPages(data.totalPages)
    } catch (err) {
      setError(err.friendlyMessage || 'Bir hata oluştu. Lütfen tekrar deneyiniz.')
    } finally {
      setLoading(false)
    }
  }, [buildParams])

  useEffect(() => { fetchLogs() }, [fetchLogs])

  const filteredLogs = search.trim()
    ? logs.filter(l =>
        l.fullName?.toLowerCase().includes(search.toLowerCase()) ||
        l.email?.toLowerCase().includes(search.toLowerCase())
      )
    : logs

  return (
    <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8 overflow-hidden">
      <DetailModal log={selectedLog} onClose={() => setSelectedLog(null)} />

      {/* Filtreler */}
      <div className="px-4 md:px-6 py-4 border-b border-slate-100 dark:border-white/8 space-y-3">
        <div className="flex flex-col sm:flex-row sm:items-center gap-3">
          <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
            {DATE_SHORTCUTS.map((s) => (
              <button key={s.value} onClick={() => { setDateShortcut(s.value); setPage(1) }}
                className={`flex-1 sm:flex-none whitespace-nowrap px-3 py-2 sm:py-1.5 rounded-md text-sm sm:text-xs font-semibold transition-all ${
                  dateShortcut === s.value
                    ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                    : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
                }`}>
                {s.label}
              </button>
            ))}
          </div>
          <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
            {STATUS_RANGES.map((s) => (
              <button key={s.value} onClick={() => { setStatusFilter(s.value); setPage(1) }}
                className={`flex-1 sm:flex-none whitespace-nowrap px-3 py-2 sm:py-1.5 rounded-md text-sm sm:text-xs font-semibold transition-all ${
                  statusFilter === s.value
                    ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                    : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
                }`}>
                {s.label}
              </button>
            ))}
          </div>
        </div>
        {dateShortcut === 'custom' && (
          <div className="flex flex-col sm:flex-row gap-3">
            <div className="flex items-center gap-2 flex-1">
              <label className="text-xs font-semibold text-slate-400 whitespace-nowrap">Başlangıç</label>
              <input type="date" value={customStart} onChange={(e) => { setCustomStart(e.target.value); setPage(1) }}
                className="flex-1 h-9 px-3 rounded-lg border text-sm bg-slate-50 border-slate-200 text-slate-800 dark:bg-white/6 dark:border-white/10 dark:text-white focus:outline-none focus:border-emerald-400" />
            </div>
            <div className="flex items-center gap-2 flex-1">
              <label className="text-xs font-semibold text-slate-400 whitespace-nowrap">Bitiş</label>
              <input type="date" value={customEnd} onChange={(e) => { setCustomEnd(e.target.value); setPage(1) }}
                className="flex-1 h-9 px-3 rounded-lg border text-sm bg-slate-50 border-slate-200 text-slate-800 dark:bg-white/6 dark:border-white/10 dark:text-white focus:outline-none focus:border-emerald-400" />
            </div>
          </div>
        )}
        <div className="relative">
          <input type="text" value={search} onChange={(e) => setSearch(e.target.value)}
            placeholder="Ad, soyad veya e-posta filtrele..."
            className="w-full h-9 pl-9 pr-4 rounded-lg border text-sm bg-slate-50 border-slate-200 text-slate-800 placeholder-slate-300 dark:bg-white/6 dark:border-white/10 dark:text-white dark:placeholder-white/20 focus:outline-none focus:border-emerald-400" />
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
            className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none">
            <circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" />
          </svg>
        </div>
      </div>

      {error ? (
        <div className="px-4 md:px-6 py-4"><AlertMessage type="error" message={error} /></div>
      ) : loading ? (
        <div className="flex items-center justify-center py-16">
          <svg className="animate-spin text-emerald-500" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M21 12a9 9 0 1 1-6.219-8.56" />
          </svg>
        </div>
      ) : filteredLogs.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-16 text-slate-400">
          <p className="text-sm">Kayıt bulunamadı.</p>
        </div>
      ) : (
        <>
          {/* Masaüstü Tablo */}
          <div className="hidden md:block overflow-x-auto">
            <table className="w-full text-left">
              <thead>
                <tr className="border-b border-slate-100 dark:border-white/8 bg-slate-50/50 dark:bg-white/2">
                  {['Tarih', 'Kullanıcı', 'Modül / İşlem', 'Metot', 'Durum', 'IP / Tarayıcı'].map((h) => (
                    <th key={h} className="px-5 py-3.5 text-xs font-bold text-slate-400 dark:text-slate-500 uppercase tracking-wider whitespace-nowrap">{h}</th>
                  ))}
                  <th className="px-5 py-3.5" />
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100 dark:divide-white/6">
                {filteredLogs.map((log) => (
                  <tr key={log.id} onClick={() => setSelectedLog(log)}
                    className="hover:bg-slate-50 dark:hover:bg-white/4 cursor-pointer transition-colors">
                    <td className="px-5 py-3.5 text-xs text-slate-500 dark:text-slate-400 whitespace-nowrap">{formatDate(log.createdDate)}</td>
                    <td className="px-5 py-3.5">
                      <p className="text-sm font-semibold text-slate-800 dark:text-white">{log.fullName}</p>
                      <p className="text-xs text-slate-400">{log.email}</p>
                    </td>
                    <td className="px-5 py-3.5">
                      <p className="text-sm font-medium text-slate-700 dark:text-slate-300">{log.module}</p>
                      <p className="text-xs text-slate-400">{log.actionDescription?.trim() || log.action}</p>
                    </td>
                    <td className="px-5 py-3.5"><MethodBadge method={log.method} /></td>
                    <td className="px-5 py-3.5"><StatusBadge code={log.statusCode} /></td>
                    <td className="px-5 py-3.5">
                      <p className="text-xs font-mono text-slate-600 dark:text-slate-400">{log.ipAddress}</p>
                      <p className="text-xs text-slate-400">{log.browser}</p>
                    </td>
                    <td className="px-5 py-3.5">
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-300 dark:text-slate-600">
                        <polyline points="9 18 15 12 9 6" />
                      </svg>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Mobil Kart */}
          <div className="block md:hidden divide-y divide-slate-100 dark:divide-white/6">
            {filteredLogs.map((log) => (
              <div key={log.id} onClick={() => setSelectedLog(log)}
                className="p-4 space-y-2.5 cursor-pointer active:bg-slate-50 dark:active:bg-white/4 transition-colors">
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <p className="text-sm font-semibold text-slate-800 dark:text-white">{log.fullName}</p>
                    <p className="text-xs text-slate-400 mt-0.5">{log.email}</p>
                  </div>
                  <StatusBadge code={log.statusCode} />
                </div>
                <div className="flex items-center gap-2 flex-wrap">
                  <span className="text-xs font-medium text-slate-600 dark:text-slate-300">{log.module}</span>
                  <span className="text-slate-300 dark:text-slate-600">·</span>
                  <span className="text-xs text-slate-500 dark:text-slate-400">{log.actionDescription?.trim() || log.action}</span>
                </div>
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <MethodBadge method={log.method} />
                    <span className="text-xs text-slate-400">{log.browser}</span>
                  </div>
                  <span className="text-xs text-slate-400">{formatDate(log.createdDate)}</span>
                </div>
              </div>
            ))}
          </div>

          <Pagination page={page} totalPages={totalPages} totalCount={totalCount} onPageChange={setPage} />
        </>
      )}
    </div>
  )
}