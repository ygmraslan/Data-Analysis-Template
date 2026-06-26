import { useState, useEffect } from 'react'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from 'recharts'
import { getExecDistribution } from '../../api/execSummaryApi'
import { SkeletonBlock, SkeletonCard, SkeletonChart } from '../ui/Skeleton'

function DistributionSkeleton() {
  return (
    <div className="flex flex-col gap-6">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {[1, 2].map(i => (
          <SkeletonCard key={i} className="h-[400px]">
            <SkeletonBlock width="w-48" height="h-4" className="mb-2" />
            <SkeletonBlock width="w-32" height="h-3" className="mb-6" />
            <div className="space-y-4">
              {[...Array(8)].map((_, j) => (
                <div key={j} className="flex items-center gap-4">
                  <SkeletonBlock width="w-20" height="h-3" />
                  <SkeletonBlock width="w-full" height="h-2.5" rounded="rounded-full" />
                  <SkeletonBlock width="w-16" height="h-3" />
                </div>
              ))}
            </div>
          </SkeletonCard>
        ))}
      </div>
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {[1, 2].map(i => (
          <SkeletonCard key={i} className="h-[460px]">
            <SkeletonBlock width="w-48" height="h-4" className="mb-2" />
            <SkeletonBlock width="w-32" height="h-3" className="mb-6" />
            <SkeletonChart height={300} />
          </SkeletonCard>
        ))}
      </div>
    </div>
  )
}

function formatDateRange(startStr, endStr) {
  if (!startStr || !endStr) return ''
  const months = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']
  const parseDate = (str) => {
    if (!str) return null
    if (str.includes('T') || str.includes('-')) {
      const d = new Date(str); return isNaN(d.getTime()) ? null : d
    }
    if (str.length === 8) {
      const year = parseInt(str.substring(0, 4)), month = parseInt(str.substring(4, 6)) - 1, day = parseInt(str.substring(6, 8))
      return new Date(year, month, day)
    }
    return null
  }
  const start = parseDate(startStr), end = parseDate(endStr)
  if (!start || !end) return ''
  return `${start.getDate()} ${months[start.getMonth()]} — ${end.getDate()} ${months[end.getMonth()]} ${end.getFullYear()}`
}

const PREMIUM_BRANDS = ['BMW', 'MERCEDES', 'AUDI', 'PORSCHE', 'LAND ROVER', 'JAGUAR', 'VOLVO', 'LEXUS']
const isPremium = (brand) => brand && PREMIUM_BRANDS.some(p => brand.toUpperCase().includes(p))

const sortChronologically = (data) => {
  return [...data].sort((a, b) => (parseInt(a.label) || 0) - (parseInt(b.label) || 0))
}

const HorizontalProgressBar = ({ label, labelClassName = "", count, percentage, fillRatio, barColor }) => (
  <div className="flex items-center gap-4 group">
    <div className={`w-24 text-xs truncate flex-shrink-0 font-medium text-slate-700 dark:text-slate-200 ${labelClassName}`} title={label}>
      {label}
    </div>
    <div className="flex-1 h-2.5 bg-slate-100 dark:bg-white/10 rounded-full overflow-hidden">
      <div 
        className={`h-full rounded-full transition-all duration-500 ${barColor}`} 
        style={{ width: `${fillRatio}%` }} 
      />
    </div>
    <div className="w-24 text-right text-[11px] flex-shrink-0">
      <span className="font-bold text-slate-800 dark:text-white">{count?.toLocaleString('tr-TR')}</span>
      <span className="ml-1.5 text-slate-400 dark:text-slate-500 font-medium">%{percentage}</span>
    </div>
  </div>
)

const VerticalColumnBar = ({ label, count, fillRatio, barColor, percentage }) => (
  <div 
    className={`flex-1 min-w-0 transition-all duration-700 hover:brightness-90 cursor-pointer ${barColor} rounded-t-[3px] relative group`}
    style={{ height: `${Math.max(fillRatio, 2)}%` }}
  >
    {/* Hover tooltip */}
    <div className="absolute -top-8 left-1/2 -translate-x-1/2 bg-slate-800 text-white text-[10px] font-bold px-2 py-1 rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap pointer-events-none z-20">
      {count?.toLocaleString('tr-TR')}
    </div>
  </div>
)

const CustomTooltip = ({ active, payload }) => {
  if (!active || !payload?.length) return null
  const item = payload[0].payload
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg">
      <p className="text-xs font-semibold text-slate-700 dark:text-slate-200">{item.displayLabel}</p>
      <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">
        {item.count?.toLocaleString('tr-TR')} poliçe
      </p>
      <p className="text-xs text-slate-400">%{item.percentage} pay</p>
    </div>
  )
}

const RechartsBarCard = ({ title, dateRange, data }) => {
  const maxValue = Math.max(...data.map(d => d.count || 0))
  
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-4 sm:p-5 flex flex-col h-[320px] sm:h-[360px] w-full">
      <div className="mb-2 shrink-0">
        <p className="text-sm font-bold text-slate-800 dark:text-white uppercase tracking-tight">{title}</p>
        <p className="text-[11px] text-slate-400 font-medium mt-1">{dateRange}</p>
      </div>

      <div className="flex-1 min-h-0">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={data} margin={{ top: 10, right: 5, left: -20, bottom: 35 }}>
            <XAxis 
              dataKey="displayLabel" 
              tick={{ fontSize: 9, fill: '#94a3b8' }} 
              axisLine={{ stroke: '#cbd5e1' }}
              tickLine={false}
              interval={0}
              angle={-45}
              textAnchor="end"
              height={40}
            />
            <YAxis 
              tick={{ fontSize: 10, fill: '#64748b', fontWeight: 600 }} 
              axisLine={{ stroke: '#cbd5e1' }}
              tickLine={false}
              tickFormatter={(v) => v >= 1000 ? `${(v/1000).toFixed(0)}K` : v}
            />
            <Tooltip content={<CustomTooltip />} cursor={{ fill: 'rgba(100,116,139,0.08)' }} />
            <Bar dataKey="count" radius={[3, 3, 0, 0]} maxBarSize={40}>
              {data.map((entry, index) => (
                <Cell 
                  key={index} 
                  fill={entry.color || '#ef4444'}
                />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </div>
    </div>
  )
}

const XYDistributionCard = ({ title, dateRange, data, maxVal }) => {
  const steps = 5;
  const gridTicks = Array.from({ length: steps + 1 }, (_, i) => Math.round(maxVal - (maxVal / steps) * i));

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-4 sm:p-6 flex flex-col h-[400px] sm:h-[440px] w-full">
      {/* Başlık */}
      <div className="mb-3 shrink-0">
        <p className="text-sm font-bold text-slate-800 dark:text-white uppercase tracking-tight">{title}</p>
        <p className="text-[11px] text-slate-400 font-medium mt-1">{dateRange}</p>
      </div>

      {/* Grafik + Label alanı */}
      <div className="flex-1 flex flex-col min-h-0">
        {/* Grafik alanı */}
        <div className="flex-1 flex min-h-0">
          {/* Y ekseni değerleri */}
          <div className="flex flex-col justify-between pr-2 text-[9px] sm:text-[10px] text-slate-400 font-bold text-right w-10 sm:w-12 shrink-0">
            {gridTicks.map((v, i) => <span key={i}>{v.toLocaleString('tr-TR')}</span>)}
          </div>

          {/* Bar container */}
          <div className="flex-1 relative min-w-0">
            {/* Grid çizgileri */}
            <div className="absolute inset-0 flex flex-col justify-between pointer-events-none z-0">
              {gridTicks.map((v, i) => (
                <div key={i} className="w-full border-t border-slate-100 dark:border-white/5 h-0" />
              ))}
            </div>
            {/* Bar'lar */}
            <div className="absolute inset-0 border-l border-b border-slate-300 dark:border-slate-600 flex items-end justify-around px-1 gap-1 z-10">
              {data.map((item, i) => (
                <VerticalColumnBar 
                  key={i}
                  label={item.displayLabel}
                  count={item.count}
                  fillRatio={item.fillRatio}
                  barColor={item.barColor}
                  percentage={item.percentage}
                />
              ))}
            </div>
          </div>
        </div>

        {/* X ekseni label'ları - grafiğin altında */}
        <div className="flex shrink-0 pt-2">
          <div className="w-10 sm:w-12 shrink-0" /> {/* Y ekseni boşluğu */}
          <div className="flex-1 flex justify-around px-1">
            {data.map((item, i) => (
              <div key={i} className="flex flex-col items-center flex-1 min-w-0">
                <span className="text-[8px] sm:text-[9px] text-slate-700 dark:text-slate-300 font-semibold text-center leading-tight">{item.displayLabel}</span>
                <span className="text-[8px] sm:text-[9px] text-slate-500 font-bold">%{item.percentage}</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}

export default function ExecDistributionSection({ productGroup, startDate, endDate }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!startDate || !endDate) return
    setLoading(true)
    setError('')
    getExecDistribution(productGroup, startDate, endDate)
      .then(res => setData(res.data))
      .catch(() => setError('Veri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, startDate, endDate])

  if (!startDate || !endDate) return null
  if (loading) return <DistributionSkeleton />
  if (error) return <div className="text-sm text-red-400 py-10 text-center">{error}</div>
  if (!data) return null

  const formattedDates = formatDateRange(startDate, endDate)
  const { brands = [], vehicleAges = [], steps = [], insuredAges = [] } = data

  const calcTotal = (arr) => arr.reduce((s, d) => s + (d.count || 0), 0)
  const totals = { 
    brands: calcTotal(brands), steps: calcTotal(steps), 
    vAges: calcTotal(vehicleAges), iAges: calcTotal(insuredAges) 
  }

  const sortedInsured = sortChronologically(insuredAges)
  const sortedSteps = sortChronologically(steps)
  const sortedVehicle = sortChronologically(vehicleAges)
  const sortedBrands = [...brands].sort((a, b) => b.count - a.count)

  const maxCountVal = Math.max(...sortedInsured.map(i => i.count || 0), ...sortedSteps.map(s => s.count || 0), 1);
  const stepSize = Math.ceil(maxCountVal / 5 / 50) * 50; 
  const chartMax = stepSize * 5; 

  return (
    <div className="flex flex-col gap-6">
      
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-6 flex flex-col h-full">
          <p className="text-sm font-bold text-slate-800 dark:text-white uppercase tracking-tight">Top 10 Marka — Poliçe Adedi</p>
          <p className="text-[11px] text-slate-400 font-medium mt-1 mb-6">{formattedDates}</p>
          <div className="space-y-4">
            {sortedBrands.slice(0, 10).map((item, i) => (
              <HorizontalProgressBar 
                key={i} label={item.label} count={item.count}
                percentage={((item.count / totals.brands) * 100).toFixed(1)}
                fillRatio={(item.count / Math.max(...sortedBrands.map(b => b.count || 0), 1)) * 100}
                barColor={isPremium(item.label) ? 'bg-red-500' : 'bg-blue-500'}
                labelClassName={isPremium(item.label) ? 'text-red-600 dark:text-red-400 font-bold' : ''}
              />
            ))}
          </div>
        </div>

        <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-6 flex flex-col h-full">
          <p className="text-sm font-bold text-slate-800 dark:text-white uppercase tracking-tight">Araç Yaşı Dağılımı</p>
          <p className="text-[11px] text-slate-400 font-medium mt-1 mb-6">{formattedDates}</p>
          <div className="space-y-4">
            {sortedVehicle.slice(0, 10).map((item, i) => {
              const age = parseInt(item.label) || 0;
              return (
                <HorizontalProgressBar 
                  key={i} label={item.label?.includes('Yaş') ? item.label : `${item.label} Yaş`} count={item.count}
                  percentage={((item.count / totals.vAges) * 100).toFixed(1)}
                  fillRatio={(item.count / Math.max(...sortedVehicle.map(v => v.count || 0), 1)) * 100}
                  barColor={age >= 11 ? 'bg-red-500' : age >= 6 ? 'bg-amber-500' : 'bg-emerald-500'}
                />
              )
            })}
          </div>
        </div>
      </div>

      <div className="flex flex-col lg:flex-row gap-6">
        
        <div style={{ flex: sortedInsured.length || 1 }} className="min-w-0">
          <RechartsBarCard 
            title="Sigortalı Yaşı Dağılımı" 
            dateRange={formattedDates} 
            data={sortedInsured.map(item => {
              const lbl = item.label || '';
              const isRed = lbl.includes('18') || lbl.includes('25');
              const isOrange = lbl.includes('26') || lbl.includes('35');
              return {
                ...item,
                displayLabel: item.label,
                percentage: ((item.count / totals.iAges) * 100).toFixed(1),
                color: isRed ? '#ef4444' : isOrange ? '#f59e0b' : '#10b981'
              }
            })}
          />
        </div>

        <div style={{ flex: sortedSteps.length || 1 }} className="min-w-0">
          <RechartsBarCard 
            title="Hasarsızlık Basamak Dağılımı" 
            dateRange={formattedDates} 
            data={sortedSteps.map(item => {
              const stepNum = parseInt(item.label?.replace(/\D/g, '')) || 0;
              const shortLabel = item.label?.replace(/basamak\s*/i, '').trim() || item.label;
              return {
                ...item,
                displayLabel: shortLabel,
                percentage: ((item.count / totals.steps) * 100).toFixed(1),
                color: stepNum === 0 ? '#ef4444' : stepNum <= 2 ? '#f59e0b' : '#10b981'
              }
            })}
          />
        </div>
      </div>
    </div>
  )
}