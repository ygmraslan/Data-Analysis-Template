export default function StatusBadge({ isActive, isLocked }) {
  if (isLocked) return (
    <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-red-50 dark:bg-red-500/10 text-red-600 dark:text-red-400 border border-red-200 dark:border-red-500/20">
      <span className="w-1.5 h-1.5 rounded-full bg-red-500 flex-shrink-0" />
      Kilitli
    </span>
  )
  if (isActive) return (
    <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-emerald-50 dark:bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border border-emerald-200 dark:border-emerald-500/20">
      <span className="w-1.5 h-1.5 rounded-full bg-emerald-500 flex-shrink-0" />
      Aktif
    </span>
  )
  return (
    <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-slate-100 dark:bg-white/8 text-slate-500 dark:text-slate-400 border border-slate-200 dark:border-white/10">
      <span className="w-1.5 h-1.5 rounded-full bg-slate-400 flex-shrink-0" />
      Pasif
    </span>
  )
}