import { useState, useEffect } from 'react'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from 'recharts'
import * as demoApi from '../../api/demoApi'
import { getLastWeekRange } from '../../utils/formatDate'
import { SkeletonBlock, SkeletonCard } from '../ui/Skeleton'

const Skeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
    <SkeletonCard className="h-72" />
    <SkeletonCard className="h-72" />
  </div>
)

const formatCount = n => Number(n || 0).toLocaleString('tr-TR')
const formatMoney = n => '₺' + Number(n || 0).toLocaleString('tr-TR', { maximumFractionDigits: 0 })

const CustomTooltip = ({ active, payload, metric }) => {
  if (!active || !payload?.length) return null
  const item = payload[0].payload
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg">
      <p className="text-xs font-semibold text-slate-700 dark:text-slate-200">{item.label}</p>
      <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">
        {metric === 'policyCount' ? formatCount(item.policyCount) + ' poliçe' : formatMoney(item.netPremium)}
      </p>
      <p className="text-xs text-slate-400">%{item.ratio?.toFixed(1)} pay</p>
    </div>
  )
}

export default function DemoInsuredCitySection({ productGroup, filter }) {
  const [data, setData] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [metric, setMetric] = useState('policyCount')

  const weekRange = getLastWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    demoApi.getDemoInsuredCity(productGroup, filter)
      .then(r => setData(r.data || []))
      .catch(() => setError('Sigortalı ili verisi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <Skeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data || data.length === 0) return null

  // Seçilen metriğe göre sırala ve top 10 al
  const sortedData = [...data].sort((a, b) => (b[metric] || 0) - (a[metric] || 0)).slice(0, 10)
  const maxValue = sortedData.length > 0 ? sortedData[0][metric] : 0

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
      {/* Sol: Grafik */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <div className="flex items-center justify-between mb-1">
          <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">Sigortalı İli Dağılımı — Top 10</p>
          <div className="flex bg-slate-100 dark:bg-white/5 rounded-lg p-0.5">
            <button
              onClick={() => setMetric('policyCount')}
              className={`px-3 py-1 text-xs font-medium rounded-md transition-all ${
                metric === 'policyCount'
                  ? 'bg-white dark:bg-white/10 text-amber-600 dark:text-amber-400 shadow-sm'
                  : 'text-slate-500 dark:text-slate-400'
              }`}
            >
              Poliçe
            </button>
            <button
              onClick={() => setMetric('netPremium')}
              className={`px-3 py-1 text-xs font-medium rounded-md transition-all ${
                metric === 'netPremium'
                  ? 'bg-white dark:bg-white/10 text-amber-600 dark:text-amber-400 shadow-sm'
                  : 'text-slate-500 dark:text-slate-400'
              }`}
            >
              Net Prim
            </button>
          </div>
        </div>
        <p className="text-xs text-slate-400 mb-3">{weekRange}</p>

        <ResponsiveContainer width="100%" height={230}>
          <BarChart data={sortedData} margin={{ top: 10, right: 10, left: -15, bottom: 40 }}>
            <XAxis 
              dataKey="label" 
              tick={{ fontSize: 9, fill: '#94a3b8' }} 
              axisLine={false} 
              tickLine={false}
              interval={0}
              angle={-45}
              textAnchor="end"
            />
            <YAxis 
              tick={{ fontSize: 9, fill: '#94a3b8' }} 
              axisLine={false} 
              tickLine={false}
              tickFormatter={(v) => metric === 'policyCount' ? v : (v >= 1000000 ? `${(v/1000000).toFixed(0)}M` : v >= 1000 ? `${(v/1000).toFixed(0)}K` : v)}
            />
            <Tooltip content={<CustomTooltip metric={metric} />} cursor={{ fill: 'rgba(245,158,11,0.08)' }} />
            <Bar dataKey={metric} radius={[3, 3, 0, 0]} maxBarSize={32}>
              {sortedData.map((entry, index) => (
                <Cell 
                  key={index} 
                  fill={index === 0 ? '#f59e0b' : '#fcd34d'} 
                />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </div>

      {/* Sağ: Değerler Tablosu */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <div className="flex items-center justify-between mb-1">
          <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">Detay Tablosu</p>
        </div>
        <p className="text-xs text-slate-400 mb-3">{weekRange}</p>

        <div className="overflow-y-auto" style={{ maxHeight: 238 }}>
          <table className="w-full text-xs">
            <thead className="sticky top-0 bg-white dark:bg-[#0d1f3c]">
              <tr className="border-b border-slate-100 dark:border-white/8">
                <th className="text-left pb-2 font-semibold text-slate-400 uppercase tracking-wide w-6">#</th>
                <th className="text-left pb-2 font-semibold text-slate-400 uppercase tracking-wide">İl</th>
                <th className="text-right pb-2 font-semibold text-slate-400 uppercase tracking-wide">Poliçe</th>
                <th className="text-right pb-2 font-semibold text-slate-400 uppercase tracking-wide">Net Prim</th>
                <th className="text-right pb-2 font-semibold text-slate-400 uppercase tracking-wide">Pay</th>
                <th className="text-right pb-2 font-semibold text-slate-400 uppercase tracking-wide">WoW</th>
              </tr>
            </thead>
            <tbody>
              {sortedData.map((item, i) => {
                const isFirst = i === 0
                return (
                  <tr key={i} className={`border-b border-slate-50 dark:border-white/4 ${isFirst ? 'bg-amber-50 dark:bg-amber-500/10' : ''}`}>
                    <td className={`py-2 font-medium ${isFirst ? 'text-amber-600 dark:text-amber-400' : 'text-slate-400'}`}>
                      {i + 1}
                    </td>
                    <td className={`py-2 font-medium ${isFirst ? 'text-amber-600 dark:text-amber-400' : 'text-slate-700 dark:text-slate-300'}`}>
                      {item.label}
                    </td>
                    <td className="py-2 text-right text-slate-600 dark:text-slate-400">{formatCount(item.policyCount)}</td>
                    <td className="py-2 text-right text-slate-600 dark:text-slate-400">{formatMoney(item.netPremium)}</td>
                    <td className={`py-2 text-right font-semibold ${isFirst ? 'text-amber-600 dark:text-amber-400' : 'text-slate-700 dark:text-slate-200'}`}>
                      %{item.ratio?.toFixed(1)}
                    </td>
                    <td className="py-2 text-right">
                      {item.woW !== null && item.woW !== undefined ? (
                        <span className={`font-semibold ${item.woW >= 0 ? 'text-emerald-600 dark:text-emerald-400' : 'text-red-600 dark:text-red-400'}`}>
                          {item.woW > 0 ? '+' : ''}{item.woW?.toFixed(1)}%
                        </span>
                      ) : (
                        <span className="text-slate-300">—</span>
                      )}
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}