const VARIANTS = {
  critical: {
    icon: '⚠',
    defaultLabel: 'Kritik',
    className: 'bg-red-500/10 text-red-500 dark:bg-red-500/15 dark:text-red-400',
  },
  warning: {
    icon: '↑',
    defaultLabel: 'Artış var',
    className: 'bg-amber-500/10 text-amber-600 dark:bg-amber-500/15 dark:text-amber-400',
  },
  warning_down: {
    icon: '↓',
    defaultLabel: 'Düşüş var',
    className: 'bg-amber-500/10 text-amber-600 dark:bg-amber-500/15 dark:text-amber-400',
  },
  critical_down: {
    icon: '⚠',
    defaultLabel: 'Kritik düşüş',
    className: 'bg-red-500/10 text-red-500 dark:bg-red-500/15 dark:text-red-400',
  },
  normal: {
    icon: '✓',
    defaultLabel: 'Normal',
    className: 'bg-emerald-500/10 text-emerald-600 dark:bg-emerald-500/15 dark:text-emerald-400',
  },
  info: {
    icon: 'i',
    defaultLabel: 'Bilgi',
    className: 'bg-blue-500/10 text-blue-600 dark:bg-blue-500/15 dark:text-blue-400',
  },
}

export default function StatBadge({ type = 'normal', label }) {
  const v = VARIANTS[type] || VARIANTS.normal
  return (
    <span className={`inline-flex items-center gap-1 text-xs font-semibold px-2 py-0.5 rounded-full ${v.className}`}>
      <span>{v.icon}</span>
      <span>{label || v.defaultLabel}</span>
    </span>
  )
}