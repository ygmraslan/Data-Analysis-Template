import { HiTrendingUp, HiTrendingDown, HiArrowRight, HiChartBar, HiDocumentText } from 'react-icons/hi'

const COLOR_A = '#1D9E75'
const COLOR_B = '#378ADD'

function formatShare(val) {
  if (val === null || val === undefined) return '-'
  return `%${val.toFixed(2)}`
}

function formatCount(val) {
  if (val === null || val === undefined) return '-'
  return val.toLocaleString('tr-TR')
}

function formatChange(val) {
  if (val === null || val === undefined) return '-'
  const sign = val > 0 ? '+' : ''
  return `%${sign}${val.toFixed(2)}`
}

function formatGrowth(val) {
  if (val === null || val === undefined) return '-'
  return `${val.toFixed(1)}x`
}

function MetricCard({ label, value, icon: Icon, color = 'neutral' }) {
  const colorStyles = {
    neutral: 'text-slate-800 dark:text-white',
    red: 'text-rose-600 dark:text-rose-400',
    green: 'text-emerald-600 dark:text-emerald-400',
    amber: 'text-amber-600 dark:text-amber-400'
  }

  const iconStyles = {
    neutral: 'bg-slate-100 dark:bg-white/10 text-slate-600 dark:text-slate-400',
    red: 'bg-rose-100 dark:bg-rose-500/15 text-rose-600 dark:text-rose-400',
    green: 'bg-emerald-100 dark:bg-emerald-500/15 text-emerald-600 dark:text-emerald-400',
    amber: 'bg-amber-100 dark:bg-amber-500/15 text-amber-600 dark:text-amber-400'
  }

  return (
    <div className="bg-white dark:bg-white/5 border border-slate-200 dark:border-white/10 rounded-xl p-4">
      <div className="flex items-center justify-between mb-3">
        <span className="text-[11px] font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wide">
          {label}
        </span>
        <div className={`w-8 h-8 rounded-lg flex items-center justify-center ${iconStyles[color]}`}>
          <Icon className="w-4 h-4" />
        </div>
      </div>
      <div className={`text-2xl font-bold ${colorStyles[color]}`}>
        {value}
      </div>
    </div>
  )
}

function MetricsBlock({ label, color, result, viewMode }) {
  if (!result) return null

  const isShareMode = viewMode === 'share'
  const isChangePositive = result.change > 0
  const isChangeNegative = result.change < 0

  const weeks = result.weeklyData || []
  const firstWeek = weeks[0]
  const lastWeek = weeks[weeks.length - 1]
  const startCount = firstWeek?.segmentCount
  const endCount = lastWeek?.segmentCount

  return (
    <div className="space-y-3">
      <div className="flex items-center gap-2 px-1">
        <span className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: color }} />
        <span className="text-[11px] font-bold uppercase tracking-wide" style={{ color }}>
          {label}
        </span>
      </div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
        {isShareMode ? (
          <>
            <MetricCard
              label="Başlangıç Payı"
              value={formatShare(result.startShare)}
              icon={HiChartBar}
            />
            <MetricCard
              label="Bitiş Payı"
              value={formatShare(result.endShare)}
              icon={HiChartBar}
            />
          </>
        ) : (
          <>
            <MetricCard
              label="Başlangıç Poliçesi"
              value={formatCount(startCount)}
              icon={HiDocumentText}
            />
            <MetricCard
              label="Bitiş Poliçesi"
              value={formatCount(endCount)}
              icon={HiDocumentText}
            />
          </>
        )}

        <MetricCard
          label="Değişim"
          value={formatChange(result.change)}
          icon={isChangePositive ? HiTrendingUp : HiTrendingDown}
          color={isChangeNegative ? 'red' : isChangePositive ? 'green' : 'neutral'}
        />

        <MetricCard
          label="Büyüme"
          value={formatGrowth(result.growthMultiple)}
          icon={HiArrowRight}
          color={result.growthMultiple < 1 ? 'red' : result.growthMultiple > 1 ? 'green' : 'amber'}
        />
      </div>
    </div>
  )
}

export default function CompareMetricsSection({ resultA, resultB, viewMode = 'share' }) {
  if (!resultA || !resultB) return null

  return (
    <div className="grid grid-cols-1 xl:grid-cols-2 gap-4">
      <MetricsBlock label="Segment A" color={COLOR_A} result={resultA} viewMode={viewMode} />
      <MetricsBlock label="Segment B" color={COLOR_B} result={resultB} viewMode={viewMode} />
    </div>
  )
}