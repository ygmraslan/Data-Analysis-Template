import { useState, useEffect, useMemo } from 'react'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts'
import * as companyApi from '../../api/companyApi'
import { SkeletonBlock } from '../ui/Skeleton'

const COLORS = {
  newBusiness: '#f59e0b', 
  transfer:    '#8b5cf6', 
  renewal:     '#10b981', 
}

const RenewalSkeleton = () => (
  <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
    <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
    <SkeletonBlock width="w-full" height="h-48" />
  </div>
)

const CustomTooltip = ({ active, payload, label }) => {
  if (!active || !payload?.length) return null
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg">
      <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mb-2">{label}</p>
      {payload.map((entry, i) => (
        <div key={i} className="flex items-center gap-2 text-xs">
          <span className="w-2 h-2 rounded-full" style={{ background: entry.color }} />
          <span className="text-slate-600 dark:text-slate-300">{entry.name}:</span>
          <span className="font-bold" style={{ color: entry.color }}>%{Number(entry.value).toFixed(1)}</span>
        </div>
      ))}
    </div>
  )
}

const CustomLegend = ({ payload }) => (
  <div className="flex items-center justify-center gap-4 mt-2">
    {payload?.map((entry, i) => (
      <div key={i} className="flex items-center gap-1.5">
        <span className="w-3 h-3 rounded-sm" style={{ background: entry.color }} />
        <span className="text-xs font-medium text-slate-600 dark:text-slate-300">{entry.value}</span>
      </div>
    ))}
  </div>
)

const VIEW_OPTIONS = [
  { key: 'chart', label: 'Grafik', color: '#64748b' },
  { key: 'newBusiness', label: 'Yeni İş', color: COLORS.newBusiness },
  { key: 'transfer', label: 'Transfer', color: COLORS.transfer },
  { key: 'renewal', label: 'Yenileme', color: COLORS.renewal },
]

export default function CompanyRenewalSection({ productGroup, filter }) {
  const [data, setData] = useState([])
  const [stepData, setStepData] = useState([])
  const [loading, setLoading] = useState(true)
  const [stepLoading, setStepLoading] = useState(false)
  const [error, setError] = useState('')
  const [viewMode, setViewMode] = useState('chart')

  useEffect(() => {
    setLoading(true)
    setError('')
    companyApi.getCompanyRenewal(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('Yenileme tipi verileri yüklenemedi.'))
      .finally(() => setLoading(false))
}, [productGroup, filter])

  useEffect(() => {
    if (viewMode === 'chart') return
    
    setStepLoading(true)
    const typeMap = { newBusiness: 'Yeni İş', transfer: 'Transfer', renewal: 'Yenileme' }
    companyApi.getRenewalSteps(productGroup, typeMap[viewMode], filter)
      .then(r => setStepData(r.data))
      .catch(() => setStepData([]))
      .finally(() => setStepLoading(false))
  }, [productGroup, viewMode, filter])

  const { weeks, steps, tableData, analysis } = useMemo(() => {
    if (!stepData.length) return { weeks: [], steps: [], tableData: {}, analysis: '' }
    
    const weeksSet = [...new Set(stepData.map(d => d.week))]
    const stepsSet = [...new Set(stepData.map(d => d.step))].sort((a, b) => a - b)
    
    const map = {}
    const weekTotals = {}
    
    stepData.forEach(d => {
      const key = `${d.week}_${d.step}`
      map[key] = d.policyCount
      weekTotals[d.week] = (weekTotals[d.week] || 0) + d.policyCount
    })
    

    let analysisText = ''
    if (weeksSet.length >= 2) {
      const lastWeek = weeksSet[weeksSet.length - 1]
      const prevWeek = weeksSet[weeksSet.length - 2]
      const lastTotal = weekTotals[lastWeek] || 0
      const prevTotal = weekTotals[prevWeek] || 0
      
      if (prevTotal > 0) {
        const change = ((lastTotal - prevTotal) / prevTotal * 100).toFixed(1)
        const direction = lastTotal > prevTotal ? 'artış' : 'düşüş'
        const absChange = Math.abs(Number(change))
        
        if (absChange >= 5) {
          analysisText = `Son hafta (${lastWeek}) toplam ${lastTotal.toLocaleString('tr-TR')} poliçe ile önceki haftaya göre %${absChange} ${direction} gösterdi.`
        }
      }
    }
    
    return { weeks: weeksSet, steps: stepsSet, tableData: map, analysis: analysisText }
  }, [stepData])

  if (loading) return <RenewalSkeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data.length) return null

  const chartData = data.map(d => ({
    week: d.weekLabel,
    'Yeni İş': d.newBusinessRatio ?? 0,
    'Transfer': d.transferRatio ?? 0,
    'Yenileme': d.renewalRatio ?? 0,
  }))

  const lastWeek = data[data.length - 1]
  const firstWeek = data[0]
  const dateRange = firstWeek && lastWeek ? `${firstWeek.weekLabel} — ${lastWeek.weekLabel}` : ''

  const selectedOption = VIEW_OPTIONS.find(o => o.key === viewMode)
  const title = viewMode === 'chart' 
    ? 'Yenileme Tipi Trendi' 
    : `${selectedOption?.label} — Basamak Dağılımı`

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">
          {title}
          {dateRange && <span className="ml-2 text-xs font-normal text-slate-400">({dateRange})</span>}
        </p>
        <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-0.5">
          {VIEW_OPTIONS.map(option => (
            <button
              key={option.key}
              onClick={() => setViewMode(option.key)}
              className={`px-3 py-1.5 rounded-md text-xs font-medium transition-all ${
                viewMode === option.key
                  ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                  : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
              }`}
            >
              {option.label}
            </button>
          ))}
        </div>
      </div>

      {/* Content */}
      {viewMode === 'chart' ? (
        <>
          <ResponsiveContainer width="100%" height={220}>
            <LineChart data={chartData} margin={{ top: 5, right: 5, left: -15, bottom: 5 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} />
              <XAxis 
                dataKey="week" 
                tick={{ fontSize: 10, fill: '#94a3b8' }} 
                tickLine={false} 
                axisLine={false}
                interval={0}
              />
              <YAxis 
                tick={{ fontSize: 10, fill: '#94a3b8' }} 
                tickLine={false} 
                axisLine={false}
                tickFormatter={v => `%${v}`}
                domain={[0, 'auto']}
              />
              <Tooltip content={<CustomTooltip />} />
              <Legend content={<CustomLegend />} />
              <Line type="monotone" dataKey="Yeni İş" stroke={COLORS.newBusiness} strokeWidth={2}
                dot={{ r: 3, fill: COLORS.newBusiness, strokeWidth: 2, stroke: 'white' }} activeDot={{ r: 5 }} />
              <Line type="monotone" dataKey="Transfer" stroke={COLORS.transfer} strokeWidth={2}
                dot={{ r: 3, fill: COLORS.transfer, strokeWidth: 2, stroke: 'white' }} activeDot={{ r: 5 }} />
              <Line type="monotone" dataKey="Yenileme" stroke={COLORS.renewal} strokeWidth={2}
                dot={{ r: 3, fill: COLORS.renewal, strokeWidth: 2, stroke: 'white' }} activeDot={{ r: 5 }} />
            </LineChart>
          </ResponsiveContainer>

          {/* Summary Cards */}
          <div className="mt-4 pt-4 border-t border-slate-100 dark:border-white/6">
            <div className="grid grid-cols-2 gap-3 mb-3">
              <div className="text-center py-2 bg-slate-50 dark:bg-white/3 rounded-lg">
                <p className="text-xs text-slate-400 dark:text-slate-500">Yeni İş</p>
                <p className="text-xl font-bold mt-1" style={{ color: COLORS.newBusiness }}>
                  %{(lastWeek?.newBusinessRatio ?? 0).toFixed(1)}
                </p>
                <p className="text-xs text-slate-400">{(lastWeek?.newBusinessCount ?? 0).toLocaleString('tr-TR')} poliçe</p>
              </div>
              <div className="text-center py-2 bg-slate-50 dark:bg-white/3 rounded-lg">
                <p className="text-xs text-slate-400 dark:text-slate-500">Transfer</p>
                <p className="text-xl font-bold mt-1" style={{ color: COLORS.transfer }}>
                  %{(lastWeek?.transferRatio ?? 0).toFixed(1)}
                </p>
                <p className="text-xs text-slate-400">{(lastWeek?.transferCount ?? 0).toLocaleString('tr-TR')} poliçe</p>
              </div>
            </div>
            <div className="flex justify-center">
              <div className="w-[calc(50%-6px)] text-center py-2 bg-slate-50 dark:bg-white/3 rounded-lg">
                <p className="text-xs text-slate-400 dark:text-slate-500">Yenileme</p>
                <p className="text-xl font-bold mt-1" style={{ color: COLORS.renewal }}>
                  %{(lastWeek?.renewalRatio ?? 0).toFixed(1)}
                </p>
                <p className="text-xs text-slate-400">{(lastWeek?.renewalCount ?? 0).toLocaleString('tr-TR')} poliçe</p>
              </div>
            </div>
          </div>
        </>
      ) : (
        stepLoading ? (
          <SkeletonBlock width="w-full" height="h-64" />
        ) : weeks.length > 0 ? (
          <div>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-slate-200 dark:border-white/10">
                    <th className="text-left py-3 px-3 text-xs font-semibold text-slate-500 dark:text-slate-400">
                      Hafta
                    </th>
                    {steps.map(step => (
                      <th key={step} className="text-right py-3 px-3 text-xs font-semibold text-slate-500 dark:text-slate-400">
                        {step}. Basamak
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {weeks.map((week, i) => (
                    <tr key={week} className="border-b border-slate-100 dark:border-white/5 hover:bg-slate-50 dark:hover:bg-white/3">
                      <td className="py-2.5 px-3 text-xs font-medium text-slate-600 dark:text-slate-300 whitespace-nowrap">
                        {week}
                      </td>
                      {steps.map(step => {
                        const val = tableData[`${week}_${step}`] || 0
                        return (
                          <td key={step} className="py-2.5 px-3 text-right text-xs font-medium text-slate-700 dark:text-slate-200">
                            {val > 0 ? val.toLocaleString('tr-TR') : '—'}
                          </td>
                        )
                      })}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            
            {/* Analiz Notu */}
            {analysis && (
              <div className="mt-4 pt-3 border-t border-slate-100 dark:border-white/6">
                <p className="text-xs text-slate-500 dark:text-slate-400 italic">
                  📊 {analysis}
                </p>
              </div>
            )}
          </div>
        ) : (
          <div className="flex items-center justify-center py-16 text-sm text-slate-400">
            Basamak verisi bulunamadı
          </div>
        )
      )}
    </div>
  )
}