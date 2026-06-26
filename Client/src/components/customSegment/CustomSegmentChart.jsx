import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import { SkeletonBlock } from '../ui/Skeleton'

const CHART_COLOR = '#059669' 
const DANGER_COLOR = '#e11d48' 

const COLOR_A = '#1D9E75'
const COLOR_B = '#378ADD' 

function CustomTooltip({ active, payload, label }) {
  if (!active || !payload?.length) return null

  const hasCompare = payload.some(p => p.dataKey === 'shareA' || p.dataKey === 'shareB')

  if (hasCompare) {
    const a = payload.find(p => p.dataKey === 'shareA')
    const b = payload.find(p => p.dataKey === 'shareB')

    return (
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg">
        <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mb-2">{label}</p>
        <div className="space-y-1.5">
          {a?.value !== undefined && (
            <div className="flex items-center gap-2">
              <span className="w-2 h-2 rounded-full" style={{ backgroundColor: COLOR_A }} />
              <span className="text-xs text-slate-700 dark:text-slate-300">Segment A:</span>
              <span className="text-xs font-bold" style={{ color: COLOR_A }}>%{a.value?.toFixed(2)}</span>
            </div>
          )}
          {b?.value !== undefined && (
            <div className="flex items-center gap-2">
              <span className="w-2 h-2 rounded-full" style={{ backgroundColor: COLOR_B }} />
              <span className="text-xs text-slate-700 dark:text-slate-300">Segment B:</span>
              <span className="text-xs font-bold" style={{ color: COLOR_B }}>%{b.value?.toFixed(2)}</span>
            </div>
          )}
        </div>
      </div>
    )
  }

  const data = payload[0].payload
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg">
      <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mb-1">{label}</p>
      <div className="space-y-1">
        <p className="text-sm font-bold text-emerald-600 dark:text-emerald-400">
          %{data.share?.toFixed(2)} Pay
        </p>
        <p className="text-xs text-slate-600 dark:text-slate-300">
          {data.segmentCount?.toLocaleString('tr-TR')} / {data.totalCount?.toLocaleString('tr-TR')} Poliçe
        </p>
      </div>
    </div>
  )
}

function ChartSkeleton() {
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
      <SkeletonBlock width="w-full" height="h-40" className="mb-4" />
      <div className="grid grid-cols-3 gap-3">
        {[1,2,3].map(i => <SkeletonBlock key={i} width="w-full" height="h-12" rounded="rounded-lg" />)}
      </div>
    </div>
  )
}

function EmptyChart({ title }) {
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">{title}</p>
      <div className="flex items-center justify-center h-40 text-sm text-slate-400">
        Grafik verisi bulunamadı
      </div>
    </div>
  )
}

export default function CustomSegmentChart({
  data,
  dataA,
  dataB,
  loading = false,
  title = 'Haftalık Segment Payı Trendi',
  viewMode = 'share'
}) {
  if (loading) return <ChartSkeleton />

  const isCompare = Array.isArray(dataA) && Array.isArray(dataB)

  if (isCompare) {
    if (dataA.length === 0 || dataB.length === 0) {
      return <EmptyChart title={title} />
    }

    const fmtWeekRange = (label) => {
      if (!label) return ''
      const monthMap = { 'Oca': '01', 'Şub': '02', 'Mar': '03', 'Nis': '04', 'May': '05', 'Haz': '06',
                         'Tem': '07', 'Ağu': '08', 'Eyl': '09', 'Eki': '10', 'Kas': '11', 'Ara': '12' }
      const m = label.match(/(\d+)\s+(\w+)\s*-\s*(\d+)\s+(\w+)/)
      if (m) {
        const [, d1, mo1, d2, mo2] = m
        return `${d1.padStart(2, '0')}.${monthMap[mo1] || mo1}-${d2.padStart(2, '0')}.${monthMap[mo2] || mo2}`
      }
      return label
    }

    const isShareMode = viewMode === 'share'

    const merged = dataA.map((row, i) => ({
      _label: fmtWeekRange(row.weekLabel),
      valueA: isShareMode ? row.share : row.segmentCount,
      valueB: isShareMode ? dataB[i]?.share : dataB[i]?.segmentCount
    }))

    const calcSlope = (values) => {
      const n = values.length
      if (n < 2) return 0
      const xMean = (n - 1) / 2
      const yMean = values.reduce((s, v) => s + v, 0) / n
      let num = 0, den = 0
      values.forEach((y, x) => {
        num += (x - xMean) * (y - yMean)
        den += (x - xMean) ** 2
      })
      return den === 0 ? 0 : num / den
    }

    const seriesA = isShareMode ? dataA.map(d => Number(d.share ?? 0)) : dataA.map(d => Number(d.segmentCount ?? 0))
    const seriesB = isShareMode ? dataB.map(d => Number(d.share ?? 0)) : dataB.map(d => Number(d.segmentCount ?? 0))
    const slopeA = calcSlope(seriesA)
    const slopeB = calcSlope(seriesB)

    const slopeThreshold = isShareMode ? 0.005 : 1
    const slopeColor = (s) => s > slopeThreshold ? '#ef4444' : s < -slopeThreshold ? '#10b981' : '#94a3b8'
    const slopeArrow = (s) => s > slopeThreshold ? '↑' : s < -slopeThreshold ? '↓' : '→'

    const formatValue = (v) => {
      if (v === null || v === undefined) return '-'
      return isShareMode ? `%${v.toFixed(2)}` : v.toLocaleString('tr-TR')
    }
    const formatSlope = (s) => isShareMode
      ? `${s >= 0 ? '+' : ''}${s.toFixed(2)}`
      : `${s >= 0 ? '+' : ''}${Math.round(s).toLocaleString('tr-TR')}`

    const startLabel = merged[0]?._label?.split('-')[0]
    const endLabel = merged[merged.length - 1]?._label?.split('-')[1]

    const TrendTooltip = ({ active, payload, label }) => {
      if (!active || !payload?.length) return null
      return (
        <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg text-xs">
          <p className="text-slate-500 dark:text-slate-400 mb-1.5">{label}</p>
          {payload.map(p => (
            <div key={p.dataKey} className="flex items-center gap-2 mb-0.5 last:mb-0">
              <span className="w-2 h-2 rounded-full flex-shrink-0" style={{ background: p.color }} />
              <span className="text-slate-600 dark:text-slate-300">{p.name}</span>
              <span className="font-semibold text-slate-900 dark:text-white ml-auto pl-3">
                {formatValue(p.value)}
              </span>
            </div>
          ))}
        </div>
      )
    }

    const chartTitle = isShareMode ? 'Pay Trendi' : 'Poliçe Sayısı Trendi'
    const lastA = isShareMode ? dataA[dataA.length - 1]?.share : dataA[dataA.length - 1]?.segmentCount
    const lastB = isShareMode ? dataB[dataB.length - 1]?.share : dataB[dataB.length - 1]?.segmentCount

    return (
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <div className="flex items-center justify-between mb-4 flex-wrap gap-2">
          <div>
            <p className="text-sm font-semibold text-slate-800 dark:text-white">{chartTitle}</p>
            {startLabel && endLabel && (
              <p className="text-xs text-slate-400 mt-0.5">
                {startLabel} — {endLabel} · {merged.length} hafta
              </p>
            )}
          </div>
          <div className="flex items-center gap-3">
            <div className="flex items-center gap-1.5">
              <div className="w-4 h-0.5 rounded" style={{ backgroundColor: COLOR_A }} />
              <span className="text-xs text-slate-400">Segment A</span>
            </div>
            <div className="flex items-center gap-1.5">
              <span className="w-4 h-px" style={{ borderTop: `2px dashed ${COLOR_B}` }} />
              <span className="text-xs text-slate-400">Segment B</span>
            </div>
          </div>
        </div>

        <ResponsiveContainer width="100%" height={200}>
          <LineChart data={merged} margin={{ top: 16, right: 30, bottom: 8, left: 8 }}>
            <CartesianGrid
              strokeDasharray="3 3"
              stroke="rgba(148,163,184,0.12)"
              vertical={false}
            />
            <XAxis
              dataKey="_label"
              tick={{ fontSize: 10, fill: '#94a3b8' }}
              tickLine={false}
              axisLine={false}
              interval="preserveStartEnd"
              dy={5}
            />
            <YAxis
              tick={{ fontSize: 10, fill: '#94a3b8' }}
              tickLine={false}
              axisLine={false}
              tickFormatter={v => isShareMode ? `%${v}` : v.toLocaleString('tr-TR')}
              width={isShareMode ? 45 : 60}
              dx={-5}
            />
            <Tooltip content={<TrendTooltip />} />
            <Line
              type="monotone"
              dataKey="valueA"
              name="Segment A"
              stroke={COLOR_A}
              strokeWidth={2}
              dot={{ r: 3, fill: COLOR_A, strokeWidth: 0 }}
              activeDot={{ r: 5, fill: COLOR_A, strokeWidth: 2, stroke: '#fff' }}
            />
            <Line
              type="monotone"
              dataKey="valueB"
              name="Segment B"
              stroke={COLOR_B}
              strokeWidth={2}
              strokeDasharray="6 3"
              dot={{ r: 3, fill: COLOR_B, strokeWidth: 0 }}
              activeDot={{ r: 5, fill: COLOR_B, strokeWidth: 2, stroke: '#fff' }}
            />
          </LineChart>
        </ResponsiveContainer>

        <div className="grid grid-cols-2 gap-2 mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
          <div className="flex items-center justify-between gap-2 px-3 py-2 rounded-md" style={{ backgroundColor: `${COLOR_A}10` }}>
            <div className="flex items-center gap-2 min-w-0">
              <span className="w-2 h-2 rounded-full flex-shrink-0" style={{ backgroundColor: COLOR_A }} />
              <span className="text-[11px] font-semibold uppercase tracking-wide" style={{ color: COLOR_A }}>A</span>
              <span className="text-sm font-bold text-slate-900 dark:text-white">{formatValue(lastA)}</span>
            </div>
            <div className="flex items-center gap-1.5 whitespace-nowrap">
              <span className="text-[10px] uppercase tracking-wide text-slate-400">Eğim</span>
              <span className="text-xs font-semibold" style={{ color: slopeColor(slopeA) }}>
                {formatSlope(slopeA)} {slopeArrow(slopeA)}
              </span>
            </div>
          </div>

          <div className="flex items-center justify-between gap-2 px-3 py-2 rounded-md" style={{ backgroundColor: `${COLOR_B}10` }}>
            <div className="flex items-center gap-2 min-w-0">
              <span className="w-2 h-2 rounded-full flex-shrink-0" style={{ backgroundColor: COLOR_B }} />
              <span className="text-[11px] font-semibold uppercase tracking-wide" style={{ color: COLOR_B }}>B</span>
              <span className="text-sm font-bold text-slate-900 dark:text-white">{formatValue(lastB)}</span>
            </div>
            <div className="flex items-center gap-1.5 whitespace-nowrap">
              <span className="text-[10px] uppercase tracking-wide text-slate-400">Eğim</span>
              <span className="text-xs font-semibold" style={{ color: slopeColor(slopeB) }}>
                {formatSlope(slopeB)} {slopeArrow(slopeB)}
              </span>
            </div>
          </div>
        </div>
      </div>
    )
  }

  if (!Array.isArray(data) || data.length === 0) {
    return <EmptyChart title={title} />
  }

  const chartData = data.map((d, index) => ({
    week: d.weekLabel,
    share: d.share,
    segmentCount: d.segmentCount,
    totalCount: d.totalCount,
    change: d.change,
    isLast: index === data.length - 1
  }))

  const shares = data.map(d => d.share)
  const minShare = Math.min(...shares)
  const maxShare = Math.max(...shares)
  const yMin = Math.max(0, Math.floor(minShare * 0.8 * 100) / 100)
  const yMax = Math.ceil(maxShare * 1.2 * 100) / 100

  const lastWeek = data[data.length - 1]

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">{title}</p>

      <ResponsiveContainer width="100%" height={160}>
        <LineChart data={chartData} margin={{ top: 4, right: 4, left: -20, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} className="dark:opacity-20" />
          <XAxis dataKey="week" tick={{ fontSize: 9, fill: '#94a3b8' }} tickLine={false} axisLine={false} />
          <YAxis
            tick={{ fontSize: 9, fill: '#94a3b8' }}
            tickLine={false}
            axisLine={false}
            domain={[yMin, yMax]}
            tickFormatter={v => `%${v.toFixed(1)}`}
          />
          <Tooltip content={<CustomTooltip />} />
          <Line
            type="linear"
            dataKey="share"
            stroke={CHART_COLOR}
            strokeWidth={2}
            dot={(props) => {
              const { cx, cy, index } = props
              const isLast = index === chartData.length - 1
              return (
                <circle
                  key={index}
                  cx={cx}
                  cy={cy}
                  r={isLast ? 5 : 3}
                  fill={isLast ? DANGER_COLOR : CHART_COLOR}
                  stroke="white"
                  strokeWidth={2}
                />
              )
            }}
            activeDot={{ r: 5 }}
          />
        </LineChart>
      </ResponsiveContainer>

      <div className="grid grid-cols-3 gap-3 mt-4 pt-4 border-t border-slate-100 dark:border-white/6">
        <div className="text-center">
          <p className="text-xs text-slate-400 dark:text-slate-500">Son Hafta Poliçe</p>
          <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">
            {lastWeek.segmentCount?.toLocaleString('tr-TR')}
          </p>
        </div>
        <div className="text-center border-x border-slate-100 dark:border-white/6">
          <p className="text-xs text-slate-400 dark:text-slate-500">Son Hafta Pay</p>
          <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">
            %{lastWeek.share?.toFixed(2)}
          </p>
        </div>
        <div className="text-center">
          <p className="text-xs text-slate-400 dark:text-slate-500">Son Hafta WoW</p>
          {(() => {
            const prev = data.length > 1 ? data[data.length - 2] : null
            const isNewFromZero =
              (lastWeek.change === null || lastWeek.change === undefined) &&
              prev?.share === 0 &&
              lastWeek.share > 0

            if (isNewFromZero) {
              return (
                <p className="text-sm font-bold mt-1 text-emerald-600 dark:text-emerald-400">
                  Yeni ↑
                </p>
              )
            }

            if (lastWeek.change === null || lastWeek.change === undefined) {
              return <p className="text-sm font-bold mt-1 text-slate-400">—</p>
            }

            return (
              <p className={`text-sm font-bold mt-1 ${
                lastWeek.change > 0 ? 'text-emerald-500' : lastWeek.change < 0 ? 'text-rose-500' : 'text-slate-400'
              }`}>
                {lastWeek.change > 0 ? '+' : ''}{lastWeek.change.toFixed(2)}%
              </p>
            )
          })()}
        </div>
      </div>
    </div>
  )
}