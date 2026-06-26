import { useState, useEffect } from 'react'
import * as regionApi from '../../api/regionApi'
import { SkeletonBlock, SkeletonCard, SkeletonChart } from '../ui/Skeleton'
import {
  ResponsiveContainer, LineChart, Line, XAxis, YAxis,
  CartesianGrid, Tooltip
} from 'recharts'

const COLORS = [
  '#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6',
  '#06B6D4', '#F97316', '#EC4899', '#84CC16',
]

const RegionTrendSkeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
    <SkeletonCard className="lg:col-span-2">
      <SkeletonBlock width="w-40" height="h-4" className="mb-4" />
      <SkeletonChart height={220} />
    </SkeletonCard>
    <SkeletonCard>
      <SkeletonBlock width="w-32" height="h-4" className="mb-4" />
      {[1,2,3,4,5,6,7,8,9].map(i => (
        <div key={i} className="flex items-center justify-between py-2 border-b border-slate-100 dark:border-white/6 last:border-0">
          <SkeletonBlock width="w-24" height="h-3" />
          <SkeletonBlock width="w-16" height="h-3" />
          <SkeletonBlock width="w-10" height="h-3" />
        </div>
      ))}
    </SkeletonCard>
  </div>
)

const CustomTooltip = ({ active, payload, label }) => {
  if (!active || !payload?.length) return null
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg text-xs">
      <p className="text-slate-500 dark:text-slate-400 mb-1.5">{label}</p>
      {payload.map(p => (
        <div key={p.dataKey} className="flex items-center gap-2 mb-0.5 last:mb-0">
          <span className="w-2 h-2 rounded-full flex-shrink-0" style={{ background: p.color }} />
          <span className="text-slate-600 dark:text-slate-300">{p.name}</span>
          <span className="font-semibold text-slate-900 dark:text-white ml-auto pl-3">
            ₺{Number(p.value).toLocaleString('tr-TR', { maximumFractionDigits: 0 })}
          </span>
        </div>
      ))}
    </div>
  )
}

export default function RegionTrendSection({ productGroup, filter }) {
  const [data,            setData]           = useState([])
  const [loading,         setLoading]        = useState(true)
  const [error,           setError]          = useState('')
  const [selectedRegions, setSelectedRegions] = useState(['ALL'])

  useEffect(() => {
    setLoading(true)
    setError('')
regionApi.getRegionTrend(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('Trend verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <RegionTrendSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  const regions = [...new Set(data.map(d => d.region))]
  const weeks   = [...new Set(data.map(d => d.weekLabel))]

  const activeRegions = selectedRegions.includes('ALL') ? regions : selectedRegions

  // Grafik verisi
  const chartData = weeks.map(week => {
    const point = { week }
    regions.forEach(r => {
      const found = data.find(d => d.region === r && d.weekLabel === week)
      point[r] = found ? found.totalPremium : null
    })
    return point
  })

  // Özet tablo — son hafta
  const lastWeek = weeks[weeks.length - 1]
  const summary  = regions.map(r => {
    const cur  = data.find(d => d.region === r && d.weekLabel === lastWeek)
    return { region: r, premium: cur?.totalPremium ?? 0, wow: cur?.wow ?? 0 }
  }).sort((a, b) => b.premium - a.premium)

  const toggleRegion = (r) => {
    if (r === 'ALL') { setSelectedRegions(['ALL']); return }
    setSelectedRegions(prev => {
      const without = prev.filter(x => x !== 'ALL')
      if (without.includes(r)) {
        const next = without.filter(x => x !== r)
        return next.length === 0 ? ['ALL'] : next
      }
      return [...without, r]
    })
  }

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">

      {/* Grafik */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 lg:col-span-2">
        <div className="flex items-start justify-between mb-4">
          <p className="text-sm font-semibold text-slate-800 dark:text-white">Haftalık Net Prim Trendi</p>
        </div>

        {/* Bölge seçici */}
        <div className="flex flex-wrap gap-1.5 mb-4">
          <button
            onClick={() => toggleRegion('ALL')}
            className={`px-3 py-1 rounded-md text-xs font-semibold transition-all ${
              selectedRegions.includes('ALL')
                ? 'bg-slate-800 dark:bg-white/15 text-white dark:text-white'
                : 'bg-slate-100 dark:bg-white/8 text-slate-500 dark:text-slate-400 hover:text-slate-700'
            }`}
          >
            Tümü
          </button>
          {regions.map((r, i) => (
            <button
              key={r}
              onClick={() => toggleRegion(r)}
              className={`px-3 py-1 rounded-md text-xs font-semibold transition-all ${
                activeRegions.includes(r)
                  ? 'text-white'
                  : 'bg-slate-100 dark:bg-white/8 text-slate-500 dark:text-slate-400 hover:text-slate-700'
              }`}
              style={activeRegions.includes(r) ? { background: COLORS[i % COLORS.length] } : {}}
            >
              {r}
            </button>
          ))}
        </div>

        <ResponsiveContainer width="100%" height={240}>
          <LineChart data={chartData} margin={{ top: 8, right: 16, bottom: 8, left: 8 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="rgba(148,163,184,0.12)" vertical={false} />
            <XAxis dataKey="week" tick={{ fontSize: 10, fill: '#94a3b8', dy: 6 }} tickLine={false} axisLine={false} />
            <YAxis tick={{ fontSize: 10, fill: '#94a3b8' }} tickLine={false} axisLine={false}
              tickFormatter={v => `₺${(v/1000000).toFixed(1)}M`} width={58} />
            <Tooltip content={<CustomTooltip />} />
            {activeRegions.map((r, i) => (
              <Line
                key={r}
                type="monotone"
                dataKey={r}
                name={r}
                stroke={COLORS[regions.indexOf(r) % COLORS.length]}
                strokeWidth={2}
                dot={{ r: 3, fill: COLORS[regions.indexOf(r) % COLORS.length], strokeWidth: 0 }}
                activeDot={{ r: 5, fill: COLORS[regions.indexOf(r) % COLORS.length], strokeWidth: 2, stroke: '#fff' }}
                connectNulls
              />
            ))}
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* Özet tablo */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <p className="text-sm font-semibold text-slate-800 dark:text-white mb-4">
          8 Haftalık Özet
        </p>
        <div>
          <div className="grid grid-cols-3 gap-2 mb-2">
            <span className="text-xs font-semibold text-slate-400 uppercase">Bölge</span>
            <span className="text-xs font-semibold text-slate-400 uppercase text-right">Son Hafta</span>
            <span className="text-xs font-semibold text-slate-400 uppercase text-right">WoW</span>
          </div>
          {summary.map(s => (
            <div key={s.region} className="grid grid-cols-3 gap-2 py-2 border-b border-slate-100 dark:border-white/6 last:border-0">
              <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate">{s.region}</span>
              <span className="text-xs text-slate-600 dark:text-slate-300 text-right">
                ₺{(s.premium / 1000000).toFixed(2)}M
              </span>
              <span className={`text-xs font-semibold text-right ${
                s.wow > 0 ? 'text-emerald-500' : s.wow < 0 ? 'text-red-500' : 'text-slate-400'
              }`}>
                {s.wow > 0 ? '+' : ''}{s.wow}%
              </span>
            </div>
          ))}
        </div>
      </div>

    </div>
  )
}