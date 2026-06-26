import StatBadge from './StatBadge'

const ICON_COLORS = {
  emerald: { bg: 'bg-emerald-500/10 dark:bg-emerald-500/15', text: 'text-emerald-500' },
  blue:    { bg: 'bg-blue-500/10 dark:bg-blue-500/15',       text: 'text-blue-500'    },
  amber:   { bg: 'bg-amber-500/10 dark:bg-amber-500/15',     text: 'text-amber-500'   },
  red:     { bg: 'bg-red-500/10 dark:bg-red-500/15',         text: 'text-red-500'     },
  purple:  { bg: 'bg-purple-500/10 dark:bg-purple-500/15',   text: 'text-purple-500'  },
}

const DefaultIcon = (
  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <polyline points="22 12 18 12 15 21 9 3 6 12 2 12" />
  </svg>
)

function getBadgeType(change, inverse) {
  if (change === 0) return 'normal'

  const isGood = inverse ? change < 0 : change > 0
  const abs = Math.abs(change)

  if (isGood) return 'normal'

  // Kötü yönde değişim
  const isUp = change > 0
  if (abs > 10) return isUp ? 'critical' : 'critical_down'
  return isUp ? 'warning' : 'warning_down'
}

function getBadgeLabel(change, changeSuffix, changeLabel) {
  const sign = change > 0 ? '+' : change < 0 ? '-' : ''
  return `${sign}${Math.abs(change)}${changeSuffix} ${changeLabel}`
}

export default function KpiCard({
  label,
  value,
  weekRange,
  change,
  changeSuffix = '%',
  changeLabel = 'geçen haftaya göre',
  inverse = false,
  warning,
  highlight = false,
  iconColor = 'emerald',
  icon,
  prevValue,
  prevWeekRange,
}) {
  const colors = ICON_COLORS[highlight ? 'red' : iconColor] || ICON_COLORS.emerald

  return (
    <div className={`relative bg-white dark:bg-[#0d1f3c] rounded-xl p-5 border overflow-hidden group transition-all hover:-translate-y-0.5 hover:shadow-lg ${
      highlight
        ? 'border-red-200 dark:border-red-500/20'
        : 'border-slate-200 dark:border-white/7'
    }`}>
      {/* Hover çizgisi */}
      <div className="absolute top-0 left-0 right-0 h-0.5 bg-gradient-to-r from-transparent via-emerald-500 to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />

      <div className="flex items-start justify-between mb-3">
        <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wide">{label}</p>
        <div className={`w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 ${colors.bg} ${colors.text}`}>
          {icon || DefaultIcon}
        </div>
      </div>

      {weekRange && (
        <p className="text-xs text-slate-400 dark:text-slate-500 mb-2">{weekRange}</p>
      )}

      <p className={`text-3xl font-bold ${
        highlight ? 'text-red-500' : 'text-slate-900 dark:text-white'
      }`}>
        {value}
      </p>

      {change !== undefined && change !== null && (
        <div className="mt-2">
          <StatBadge
            type={getBadgeType(change, inverse)}
            label={getBadgeLabel(change, changeSuffix, changeLabel)}
          />
        </div>
      )}

      {warning && (
        <p className="text-xs text-red-400 mt-2">{warning}</p>
      )}

      {prevValue && prevWeekRange && (
        <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
          <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
          <p className="text-sm font-semibold text-slate-600 dark:text-slate-300 mt-0.5">{prevValue}</p>
        </div>
      )}
    </div>
  )
}