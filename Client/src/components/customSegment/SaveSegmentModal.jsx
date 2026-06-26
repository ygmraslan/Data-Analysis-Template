import { HiRefresh, HiSave } from 'react-icons/hi'
import AlertMessage from '../ui/AlertMessage'

export default function SaveSegmentModal({
  isOpen,
  mode,
  saveName,
  setSaveName,
  saveError,
  setSaveError,
  saving,
  onSave,
  onClose
}) {
  if (!isOpen) return null

  const isCompare = mode === 'compare'

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div
        className="absolute inset-0 bg-black/50"
        onClick={onClose}
      />

      <div className="relative bg-white dark:bg-[#0d1f3c] rounded-xl shadow-2xl w-full max-w-md mx-4 p-6">
        <h3 className="text-lg font-semibold text-slate-800 dark:text-white mb-4">
          {isCompare ? 'Karşılaştırma Adı' : 'Segment Adı'}
        </h3>

        <input
          type="text"
          value={saveName}
          onChange={(e) => { setSaveName(e.target.value); setSaveError(null) }}
          placeholder={isCompare ? 'Karşılaştırma adı girin...' : 'Segment adı girin...'}
          className="w-full px-3 py-2.5 text-sm rounded-lg border border-slate-200 dark:border-white/10 bg-white dark:bg-white/5 text-slate-700 dark:text-slate-200 placeholder-slate-400 focus:outline-none focus:border-emerald-300 dark:focus:border-emerald-500/50 focus:ring-2 focus:ring-emerald-500/20"
          autoFocus
        />

        {saveError && (
          <div className="mt-3">
            <AlertMessage type="error" message={saveError} />
          </div>
        )}

        {saving && isCompare && (
          <div className="mt-3 flex items-start gap-2 px-3 py-2.5 rounded-lg bg-emerald-50 dark:bg-emerald-500/10 border border-emerald-200 dark:border-emerald-500/30">
            <HiRefresh className="w-4 h-4 mt-0.5 animate-spin text-emerald-600 dark:text-emerald-400 flex-shrink-0" />
            <div className="text-xs text-emerald-700 dark:text-emerald-300">
              Karşılaştırma kaydediliyor...
            </div>
          </div>
        )}

        <div className="flex justify-end gap-3 mt-6">
          <button
            onClick={onClose}
            className="px-4 py-2 text-sm font-medium rounded-lg border border-slate-200 dark:border-white/10 text-slate-600 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-white/5 transition-colors"
          >
            İptal
          </button>
          <button
            onClick={onSave}
            disabled={saving || !saveName.trim()}
            className="flex items-center gap-2 px-4 py-2 text-sm font-medium rounded-lg bg-emerald-500 hover:bg-emerald-600 text-white transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {saving ? <HiRefresh className="w-4 h-4 animate-spin" /> : <HiSave className="w-4 h-4" />}
            Kaydet
          </button>
        </div>
      </div>
    </div>
  )
}