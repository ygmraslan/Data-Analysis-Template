import { useState, useEffect } from 'react'
import * as dashboardApi from '../../api/dashboardApi'
import { SkeletonBlock, SkeletonCard, SkeletonChart } from '../ui/Skeleton'
import {
  ResponsiveContainer, LineChart, Line, XAxis, YAxis,
  CartesianGrid, Tooltip
} from 'recharts'

const METRICS = {
  policy:  { key: 'policyCount', wow: 'policyWoW',     label: 'Poliçe Sayısı', color: '#3B82F6' },
  premium: { key: 'netPremium',  wow: 'netPremiumWoW', label: 'Net Prim',      color: '#10B981' },
}

const WeeklyTotalsSkeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
    <SkeletonCard className="lg:col-span-2">
      <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
      <SkeletonChart height={240} />
    </SkeletonCard>
    <SkeletonCard>
      <SkeletonBlock width="w-32" height="h-4" className="mb-4" />
      {[1,2,3,4,5,6,7,8].map(i => (
        <div key={i} className="flex items-center justify-between py-2 border-b border-slate-100 dark:border-white/6 last:border-0">
          <SkeletonBlock width="w-24" height="h-3" />
          <SkeletonBlock width="w-16" height="h-3" />
          <SkeletonBlock width="w-10" height="h-3" />
        </div>
      ))}
    </SkeletonCard>
  </div>
)

const fmtPolicy  = (n) => Number(n).toLocaleString('tr-TR')
const fmtPremium = (n) => '₺' + Number(n).toLocaleString('tr-TR', { maximumFractionDigits: 0 })

function CustomTooltip({ active, payload, label, metric }) {
  if (!active || !payload?.length) return null
  const v = payload[0].value
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg text-xs">
      <p className="text-slate-500 dark:text-slate-400 mb-1">{label}</p>
      <p className="font-semibold text-slate-900 dark:text-white">
        {metric === 'premium' ? fmtPremium(v) : fmtPolicy(v)}
      </p>
    </div>
  )
}

export default function WeeklyTotalsSection({ productGroup, filter }) {
  const [data,    setData]    = useState([])
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')
  const [metric,  setMetric]  = useState('policy')

  useEffect(() => {
    setLoading(true)
    setError('')
    dashboardApi.getWeeklyTotals(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('Toplam üretim verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <WeeklyTotalsSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  const m = METRICS[metric]
  const chartData = data.map(d => ({ week: d.weekLabel, value: d[m.key] }))

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">

      {/* Grafik */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 lg:col-span-2">
        <div className="flex items-center justify-between mb-4">
          <p className="text-sm font-semibold text-slate-800 dark:text-white">
            8 Haftalık Toplam Üretim
          </p>
          {/* Metrik toggle */}
          <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
            {Object.entries(METRICS).map(([k, v]) => (
              <button
                key={k}
                onClick={() => setMetric(k)}
                className={`px-3 py-1 rounded-md text-xs font-semibold transition-all ${
                  metric === k
                    ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                    : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
                }`}
              >
                {v.label}
              </button>
            ))}
          </div>
        </div>

        <ResponsiveContainer width="100%" height={240}>
          <LineChart data={chartData} margin={{ top: 8, right: 16, bottom: 8, left: 8 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="rgba(148,163,184,0.12)" vertical={false} />
            <XAxis dataKey="week" tick={{ fontSize: 10, fill: '#94a3b8', dy: 6 }} tickLine={false} axisLine={false} />
            <YAxis tick={{ fontSize: 10, fill: '#94a3b8' }} tickLine={false} axisLine={false} width={58}
              tickFormatter={v => metric === 'premium'
                ? `₺${(v / 1_000_000).toFixed(1)}M`
                : `${(v / 1000).toFixed(0)}K`} />
            <Tooltip content={<CustomTooltip metric={metric} />} />
            <Line
              type="monotone"
              dataKey="value"
              name={m.label}
              stroke={m.color}
              strokeWidth={2}
              dot={{ r: 3, fill: m.color, strokeWidth: 0 }}
              activeDot={{ r: 5, fill: m.color, strokeWidth: 2, stroke: '#fff' }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* Tablo */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <p className="text-sm font-semibold text-slate-800 dark:text-white mb-4">Haftalık Döküm</p>
        <div>
          <div className="grid grid-cols-4 gap-2 mb-2">
            <span className="text-xs font-semibold text-slate-400 uppercase">Hafta</span>
            <span className="text-xs font-semibold text-slate-400 uppercase text-right">Poliçe</span>
            <span className="text-xs font-semibold text-slate-400 uppercase text-right">Net Prim</span>
            <span className="text-xs font-semibold text-slate-400 uppercase text-right">WoW</span>
          </div>
          {data.map(d => {
            const wow = d[m.wow]
            return (
              <div key={d.weekLabel} className="grid grid-cols-4 gap-2 py-2 border-b border-slate-100 dark:border-white/6 last:border-0">
                <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate">{d.weekLabel}</span>
                <span className="text-xs text-slate-600 dark:text-slate-300 text-right">{fmtPolicy(d.policyCount)}</span>
                <span className="text-xs text-slate-600 dark:text-slate-300 text-right">₺{(d.netPremium / 1_000_000).toFixed(2)}M</span>
                <span className={`text-xs font-semibold text-right ${
                  wow > 0 ? 'text-emerald-500' : wow < 0 ? 'text-red-500' : 'text-slate-400'
                }`}>
                  {wow > 0 ? '+' : ''}{wow}%
                </span>
              </div>
            )
          })}
        </div>
      </div>

    </div>
  )
}