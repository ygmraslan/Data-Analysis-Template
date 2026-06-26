import { useEffect } from 'react'

export default function ConfirmModal({
  isOpen,
  onConfirm,
  onCancel,
  title,
  description,
  confirmText = 'Onayla',
  cancelText = 'İptal',
  variant = 'danger',
  loading = false,
}) {
  useEffect(() => {
    if (isOpen) document.body.style.overflow = 'hidden'
    else document.body.style.overflow = ''
    return () => { document.body.style.overflow = '' }
  }, [isOpen])

  if (!isOpen) return null

  const confirmStyles = {
    danger: 'bg-red-600 hover:bg-red-500 text-white',
    warning: 'bg-amber-500 hover:bg-amber-400 text-white',
    primary: 'bg-slate-900 dark:bg-emerald-500 hover:bg-slate-700 dark:hover:bg-emerald-400 text-white',
  }

  const iconStyles = {
    danger: 'bg-red-50 dark:bg-red-500/10 text-red-500',
    warning: 'bg-amber-50 dark:bg-amber-500/10 text-amber-500',
    primary: 'bg-emerald-50 dark:bg-emerald-500/10 text-emerald-500',
  }

  const icons = {
    danger: (
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
        <polyline points="3 6 5 6 21 6" />
        <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
        <path d="M10 11v6M14 11v6" />
        <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" />
      </svg>
    ),
    warning: (
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
        <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
        <line x1="12" y1="9" x2="12" y2="13" />
        <line x1="12" y1="17" x2="12.01" y2="17" />
      </svg>
    ),
    primary: (
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
        <circle cx="12" cy="12" r="10" />
        <line x1="12" y1="8" x2="12" y2="12" />
        <line x1="12" y1="16" x2="12.01" y2="16" />
      </svg>
    ),
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/40 dark:bg-black/60 backdrop-blur-sm"
        onClick={onCancel}
      />

      {/* Modal */}
      <div className="relative w-full max-w-md bg-white dark:bg-[#002147] rounded-2xl shadow-2xl border border-slate-200 dark:border-white/8 p-6 animate-in fade-in zoom-in-95 duration-200">

        <div className="flex items-start gap-4">
          <div className={`w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0 ${iconStyles[variant]}`}>
            {icons[variant]}
          </div>
          <div className="flex-1 min-w-0">
            <h3 className="text-base font-bold text-slate-800 dark:text-white mb-1" style={{ fontFamily: 'Montserrat, sans-serif' }}>
              {title}
            </h3>
            {description && (
              <p className="text-sm text-slate-500 dark:text-slate-400">{description}</p>
            )}
          </div>
        </div>

        <div className="flex justify-end gap-2 mt-6">
          <button
            onClick={onCancel}
            disabled={loading}
            className="px-4 py-2 rounded-lg text-sm font-medium transition-colors
              text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-white/8
              disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {cancelText}
          </button>
          <button
            onClick={onConfirm}
            disabled={loading}
            className={`flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-semibold transition-all
              disabled:opacity-50 disabled:cursor-not-allowed ${confirmStyles[variant]}`}
          >
            {loading && (
              <svg className="animate-spin" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M21 12a9 9 0 1 1-6.219-8.56" />
              </svg>
            )}
            {confirmText}
          </button>
        </div>
      </div>
    </div>
  )
}