import { useState, useEffect } from 'react'
import { getExecDrift } from '../../api/execSummaryApi'
import TrendSparkline from '../ui/TrendSparkline'
import { SkeletonBlock, SkeletonCard, SkeletonChart } from '../ui/Skeleton'

const DriftSkeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
    <SkeletonCard>
      <SkeletonBlock width="w-40" height="h-4" className="mb-1" />
      <SkeletonBlock width="w-56" height="h-3" className="mb-4" />
      <SkeletonChart height={180} />
      <div className="flex gap-3 mt-4">
        <SkeletonBlock width="w-36" height="h-12" />
        <SkeletonBlock width="w-36" height="h-12" />
      </div>
    </SkeletonCard>
    <SkeletonCard>
      <SkeletonBlock width="w-40" height="h-4" className="mb-4" />
      <div className="space-y-2">
        {[1,2,3,4,5,6,7].map(i => <SkeletonBlock key={i} width="w-full" height="h-6" />)}
      </div>
    </SkeletonCard>
  </div>
)

// Tarih formatla: "26 Oca"
function formatShortDate(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return ''
  const months = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']
  return `${d.getDate()} ${months[d.getMonth()]}`
}

// Hafta tarih aralığı formatla: "26 Oca - 1 Şub"
function formatWeekRange(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return ''
  const end = new Date(d)
  end.setDate(end.getDate() + 6)
  const months = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']
  
  // Aynı ay ise: "26 - 1 Şub" değil "26 Oca - 1 Şub"
  return `${d.getDate()} ${months[d.getMonth()]} - ${end.getDate()} ${months[end.getMonth()]}`
}

// Tarih aralığı formatla: "26 Oca — 9 Mar"
function formatDateRange(startStr, endStr) {
  if (!startStr || !endStr) return ''
  return `${formatShortDate(startStr)} — ${formatShortDate(endStr)}`
}

// Hafta label formatla: "09.03-15.03"
function formatWeekLabel(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return ''
  const end = new Date(d)
  end.setDate(end.getDate() + 6)
  const f = x => `${String(x.getDate()).padStart(2,'0')}.${String(x.getMonth()+1).padStart(2,'0')}`
  return `${f(d)}-${f(end)}`
}

// Trend yönü hesapla
function getTrendInfo(startVal, endVal) {
  if (!startVal || startVal === 0) return { multiple: '—', direction: '→', label: 'stabil', badgeBg: 'bg-slate-100 dark:bg-white/10', badgeText: 'text-slate-500' }
  
  const ratio = endVal / startVal
  const multiple = ratio.toFixed(1)
  
  if (ratio >= 1.5) return { multiple, direction: '↑', label: 'artıyor', badgeBg: 'bg-red-50 dark:bg-red-500/10', badgeText: 'text-red-600 dark:text-red-400' }
  if (ratio >= 1.1) return { multiple, direction: '↑', label: 'artıyor', badgeBg: 'bg-amber-50 dark:bg-amber-500/10', badgeText: 'text-amber-600 dark:text-amber-400' }
  if (ratio <= 0.7) return { multiple, direction: '↓', label: 'azalıyor', badgeBg: 'bg-emerald-50 dark:bg-emerald-500/10', badgeText: 'text-emerald-600 dark:text-emerald-400' }
  if (ratio <= 0.9) return { multiple, direction: '↓', label: 'azalıyor', badgeBg: 'bg-emerald-50 dark:bg-emerald-500/10', badgeText: 'text-emerald-600 dark:text-emerald-400' }
  return { multiple, direction: '→', label: 'stabil', badgeBg: 'bg-slate-100 dark:bg-white/10', badgeText: 'text-slate-500 dark:text-slate-400' }
}

// Dinamik özet cümlesi oluştur
function generateSummary(seg1Info, seg2Info, seg1Start, seg1End, seg2Start, seg2End) {
  const parts = []
  
  if (parseFloat(seg1Info.multiple) >= 2) {
    parts.push(`Segment 1 portföy payı ${seg1Info.multiple}× artarak %${seg1Start}'ten %${seg1End}'e yükseldi`)
  } else if (parseFloat(seg1Info.multiple) >= 1.3) {
    parts.push(`Segment 1 %${seg1End} seviyesine ulaştı`)
  }
  
  if (parseFloat(seg2Info.multiple) >= 1.5) {
    parts.push(`Segment 2 de ${seg2Info.multiple}× büyüme gösterdi`)
  }
  
  if (parts.length === 0) {
    if (parseFloat(seg1Info.multiple) <= 0.8) {
      parts.push(`Segment 1 %${seg1Start}'ten %${seg1End}'e geriledi`)
    } else {
      parts.push(`Her iki segment de stabil seyrediyor`)
    }
  }
  
  return parts.join('. ') + '.'
}

export default function ExecDriftSection({ productGroup, startDate, endDate }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!startDate || !endDate) return
    setLoading(true)
    setError('')
    getExecDrift(productGroup, startDate, endDate)
      .then(res => setData(res.data))
      .catch(() => setError('Veri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, startDate, endDate])

  if (!startDate || !endDate) return null
  if (loading) return <DriftSkeleton />
  if (error) return <div className="text-sm text-red-400 py-10 text-center">{error}</div>
  if (!data?.weeklyTrend?.length) return null

  const trend = data.weeklyTrend
  const firstWeek = trend[0]
  const lastWeek = trend[trend.length - 1]
  
  // Segment değerleri
  const seg1Start = firstWeek?.seg1Share || 0
  const seg1End = lastWeek?.seg1Share || 0
  const seg2Start = firstWeek?.seg2Share || 0
  const seg2End = lastWeek?.seg2Share || 0
  
  // Trend bilgileri
  const seg1Info = getTrendInfo(seg1Start, seg1End)
  const seg2Info = getTrendInfo(seg2Start, seg2End)
  
  // Özet cümlesi
  const summaryText = generateSummary(seg1Info, seg2Info, seg1Start, seg1End, seg2Start, seg2End)

  // TrendSparkline için veriyi hazırla (tarih label'ı ekle)
  const chartData = trend.map(week => ({
    ...week,
    weekStart: week.weekStart, // TrendSparkline bunu kullanacak
  }))

  // TrendSparkline için series
  const trendSeries = [
    { key: 'seg1Share', label: 'Seg.1', color: '#EF4444', dashed: false },
    { key: 'seg2Share', label: 'Seg.2', color: '#F59E0B', dashed: true },
  ]

  return (
    <div className="space-y-4">
      {/* Segment Tanımları */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
        <div className="bg-red-50 dark:bg-red-500/10 border border-red-200 dark:border-red-500/20 rounded-lg px-4 py-3">
          <p className="text-sm font-semibold text-red-700 dark:text-red-400">Segment 1 — Riskli Bireysel</p>
          <p className="text-xs text-red-600 dark:text-red-300 mt-1">Genç sürücü (18-25) + Premium marka + 11+ yaş araç</p>
        </div>
        <div className="bg-amber-50 dark:bg-amber-500/10 border border-amber-200 dark:border-amber-500/20 rounded-lg px-4 py-3">
          <p className="text-sm font-semibold text-amber-700 dark:text-amber-400">Segment 2 — Riskli Tüzel</p>
          <p className="text-xs text-amber-600 dark:text-amber-300 mt-1">Tüzel müşteri + Premium marka + 11+ yaş araç</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
      {/* Sol: Trend Grafiği */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <div className="flex items-center justify-between mb-1">
          <p className="text-sm font-semibold text-slate-800 dark:text-white">Segment Drift Trendi</p>
          <div className="flex items-center gap-3">
            <div className="flex items-center gap-1.5">
              <div className="w-4 h-0.5 rounded bg-red-500" />
              <span className="text-xs text-slate-400">Seg.1</span>
            </div>
            <div className="flex items-center gap-1.5">
              <div className="w-4 h-px" style={{ borderTop: '2px dashed #F59E0B' }} />
              <span className="text-xs text-slate-400">Seg.2</span>
            </div>
          </div>
        </div>
        <p className="text-xs text-slate-400 mb-4">
          {formatDateRange(firstWeek?.weekStart, lastWeek?.weekStart)} · {trend.length} hafta
        </p>
        
        <TrendSparkline data={chartData} series={trendSeries} suffix="%" showSlope={false} />
        
        {/* Özet cümlesi - eğim yerine */}
        <div className={`mt-3 px-3 py-2 rounded-lg ${
          parseFloat(seg1Info.multiple) >= 2 
            ? 'bg-red-50 dark:bg-red-500/10' 
            : parseFloat(seg1Info.multiple) >= 1.3 
              ? 'bg-amber-50 dark:bg-amber-500/10' 
              : 'bg-slate-50 dark:bg-white/5'
        }`}>
          <p className={`text-xs ${
            parseFloat(seg1Info.multiple) >= 2 
              ? 'text-red-600 dark:text-red-300' 
              : parseFloat(seg1Info.multiple) >= 1.3 
                ? 'text-amber-600 dark:text-amber-300' 
                : 'text-slate-500 dark:text-slate-400'
          }`}>
            {summaryText}
          </p>
        </div>
        
        {/* Segment Özet Kartları */}
        <div className="grid grid-cols-2 gap-3 mt-3">
          <div className="bg-slate-50 dark:bg-white/5 rounded-lg p-3">
            <div className="flex items-center justify-between mb-1">
              <span className="text-xs font-medium text-slate-600 dark:text-slate-300">Segment 1</span>
              <span className={`text-[10px] font-medium px-2 py-0.5 rounded-full ${seg1Info.badgeBg} ${seg1Info.badgeText}`}>
                {seg1Info.direction} {seg1Info.label}
              </span>
            </div>
            <div className="flex items-baseline gap-2">
              <span className="text-xl font-bold text-red-500">{seg1Info.multiple}×</span>
              <span className="text-xs text-slate-400">%{seg1Start} → %{seg1End}</span>
            </div>
          </div>
          
          <div className="bg-slate-50 dark:bg-white/5 rounded-lg p-3">
            <div className="flex items-center justify-between mb-1">
              <span className="text-xs font-medium text-slate-600 dark:text-slate-300">Segment 2</span>
              <span className={`text-[10px] font-medium px-2 py-0.5 rounded-full ${seg2Info.badgeBg} ${seg2Info.badgeText}`}>
                {seg2Info.direction} {seg2Info.label}
              </span>
            </div>
            <div className="flex items-baseline gap-2">
              <span className="text-xl font-bold text-amber-500">{seg2Info.multiple}×</span>
              <span className="text-xs text-slate-400">%{seg2Start} → %{seg2End}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Sağ: Haftalık Drift Tablosu */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <p className="text-sm font-semibold text-slate-800 dark:text-white mb-4">Haftalık Drift Tablosu</p>
        
        <table className="w-full text-xs">
          <thead>
            <tr className="border-b border-slate-100 dark:border-white/6">
              <th className="text-left py-2 text-slate-500 dark:text-slate-400 font-medium">Hafta</th>
              <th className="text-right py-2 text-slate-500 dark:text-slate-400 font-medium">Toplam</th>
              <th className="text-right py-2 text-red-500 font-medium">S1 Pay</th>
              <th className="text-right py-2 text-amber-500 font-medium">S2 Pay</th>
            </tr>
          </thead>
          <tbody>
            {trend.map((week, i) => {
              const isLast = i === trend.length - 1
              return (
                <tr 
                  key={i} 
                  className={`border-b border-slate-50 dark:border-white/4 ${isLast ? 'bg-amber-50/50 dark:bg-amber-500/5' : ''}`}
                >
                  <td className={`py-2 text-slate-600 dark:text-slate-300 ${isLast ? 'font-semibold' : ''}`}>
                    {formatWeekRange(week.weekStart)}
                  </td>
                  <td className={`py-2 text-right text-slate-600 dark:text-slate-300 ${isLast ? 'font-semibold' : ''}`}>
                    {week.totalPolicy?.toLocaleString('tr-TR')}
                  </td>
                  <td className={`py-2 text-right text-red-500 ${isLast ? 'font-semibold' : ''}`}>
                    %{week.seg1Share}
                  </td>
                  <td className={`py-2 text-right text-amber-500 ${isLast ? 'font-semibold' : ''}`}>
                    %{week.seg2Share}
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