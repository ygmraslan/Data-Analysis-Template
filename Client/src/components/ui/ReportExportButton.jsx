import { useState } from 'react'
import { useExport } from '../../hooks/useExport'
import { exportRegionReport } from '../../api/exportApi'

export default function ReportExportButton({ exportFn, fileName, disabled = false }) {
  const { trigger, loading } = useExport(exportFn, fileName)

  return (
    <button
      onClick={trigger}
      disabled={disabled || loading}
      className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-semibold
        text-red-500 dark:text-red-400
        border border-red-200 dark:border-red-500/30
        rounded-lg hover:bg-red-50 dark:hover:bg-red-500/10
        transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
    >
      {loading ? (
        <svg className="animate-spin" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M21 12a9 9 0 1 1-6.219-8.56" />
        </svg>
      ) : (
        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
          <polyline points="14 2 14 8 20 8" />
          <line x1="9" y1="13" x2="15" y2="13" />
          <line x1="9" y1="17" x2="15" y2="17" />
        </svg>
      )}
      {loading ? 'Hazırlanıyor...' : 'PDF İndir'}
    </button>
  )
}