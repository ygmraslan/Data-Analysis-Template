import { HiTrendingUp, HiTrendingDown, HiArrowRight, HiChartBar } from 'react-icons/hi'

// ========================================
// METRIC CARD COMPONENT
// ========================================

function MetricCard({ label, value, suffix = '', icon: Icon, color = 'neutral', highlight = false }) {
  const colorStyles = {
    neutral: {
      iconBg: 'bg-slate-100 dark:bg-white/10',
      iconText: 'text-slate-600 dark:text-slate-400',
      valueBg: 'text-slate-800 dark:text-white'
    },
    red: {
      iconBg: 'bg-rose-100 dark:bg-rose-500/15',
      iconText: 'text-rose-600 dark:text-rose-400',
      valueBg: 'text-rose-600 dark:text-rose-400'
    },
    green: {
      iconBg: 'bg-emerald-100 dark:bg-emerald-500/15',
      iconText: 'text-emerald-600 dark:text-emerald-400',
      valueBg: 'text-emerald-600 dark:text-emerald-400'
    },
    amber: {
      iconBg: 'bg-amber-100 dark:bg-amber-500/15',
      iconText: 'text-amber-600 dark:text-amber-400',
      valueBg: 'text-amber-600 dark:text-amber-400'
    }
  }

  const styles = colorStyles[color] || colorStyles.neutral

  return (
    <div className="bg-white dark:bg-white/5 border border-slate-200 dark:border-white/10 rounded-xl p-4 hover:shadow-md hover:-translate-y-0.5 transition-all">
      <div className="flex items-center justify-between mb-3">
        <span className="text-[11px] font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wide">
          {label}
        </span>
        <div className={`w-8 h-8 rounded-lg flex items-center justify-center ${styles.iconBg}`}>
          <Icon className={`w-4 h-4 ${styles.iconText}`} />
        </div>
      </div>
      <div className={`text-2xl font-bold ${styles.valueBg}`}>
        {value}{suffix}
      </div>
    </div>
  )
}

// ========================================
// LOADING SKELETON
// ========================================

function MetricSkeleton() {
  return (
    <div className="bg-white dark:bg-white/5 border border-slate-200 dark:border-white/10 rounded-xl p-4">
      <div className="flex items-center justify-between mb-3">
        <div className="h-3 w-20 bg-slate-100 dark:bg-white/10 rounded animate-pulse" />
        <div className="w-8 h-8 bg-slate-100 dark:bg-white/10 rounded-lg animate-pulse" />
      </div>
      <div className="h-8 w-24 bg-slate-100 dark:bg-white/10 rounded animate-pulse" />
    </div>
  )
}

// ========================================
// MAIN COMPONENT
// ========================================

export default function CustomSegmentMetrics({ data, loading = false }) {
  // data = { startShare, endShare, change, growth }
  
  if (loading) {
    return (
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
        <MetricSkeleton />
        <MetricSkeleton />
        <MetricSkeleton />
        <MetricSkeleton />
      </div>
    )
  }

  if (!data) {
    return null
  }

  const { startShare, endShare, change, growth } = data

  // Değişim pozitif mi negatif mi?
  const isChangePositive = change > 0
  const isChangeNegative = change < 0

  // Değişim değerini formatla
  const formatChange = (val) => {
    if (val === null || val === undefined) return '-'
    const sign = val > 0 ? '+' : ''
    return `%${sign}${val.toFixed(2)}`
  }

  // Büyüme değerini formatla
  const formatGrowth = (val) => {
    if (val === null || val === undefined) return '-'
    return `${val.toFixed(1)}x`
  }

  // Pay değerini formatla
  const formatShare = (val) => {
    if (val === null || val === undefined) return '-'
    return `%${val.toFixed(2)}`
  }

  return (
    <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
      <MetricCard
        label="Başlangıç Payı"
        value={formatShare(startShare)}
        icon={HiChartBar}
        color="neutral"
      />
      
      <MetricCard
        label="Bitiş Payı"
        value={formatShare(endShare)}
        icon={HiChartBar}
        color="neutral"
      />
      
      <MetricCard
        label="Değişim"
        value={formatChange(change)}
        icon={isChangePositive ? HiTrendingUp : HiTrendingDown}
        color={isChangeNegative ? 'red' : isChangePositive ? 'green' : 'neutral'}
      />
      
      <MetricCard
        label="Büyüme"
        value={formatGrowth(growth)}
        icon={HiArrowRight}
        color={growth < 1 ? 'red' : growth > 1 ? 'green' : 'amber'}
      />
    </div>
  )
}