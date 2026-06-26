export default function AlertMessage({ type = 'error', message }) {
  if (!message) return null

  const styles = {
    error: 'bg-red-50 dark:bg-red-500/10 border-red-200 dark:border-red-500/20 text-red-600 dark:text-red-400',
    success: 'bg-emerald-50 dark:bg-emerald-500/10 border-emerald-200 dark:border-emerald-500/20 text-emerald-600 dark:text-emerald-400',
  }

  const icons = {
    error: (
      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="flex-shrink-0">
        <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
      </svg>
    ),
    success: (
      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="flex-shrink-0">
        <polyline points="20 6 9 17 4 12" />
      </svg>
    ),
  }

  return (
    <div className={`flex items-center gap-2 px-4 py-3 rounded-xl border text-sm ${styles[type]}`}>
      {icons[type]}
      {message}
    </div>
  )
}