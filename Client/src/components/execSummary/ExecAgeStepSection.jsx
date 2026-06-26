import { useState, useEffect } from 'react'
import { getExecAgeStep } from '../../api/execSummaryApi'
import { SkeletonBlock, SkeletonCard } from '../ui/Skeleton'

const AgeStepSkeleton = () => (
  <SkeletonCard>
    <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
    <SkeletonBlock width="w-full" height="h-64" />
  </SkeletonCard>
)

// Heatmap renk hesaplama
function heatColor(ratio) {
  const green  = [99, 190, 123]
  const yellow = [255, 235, 132]
  const red    = [248, 105, 107]
  let r, g, b
  if (ratio <= 0.5) {
    const t = ratio * 2
    r = Math.round(green[0] + (yellow[0] - green[0]) * t)
    g = Math.round(green[1] + (yellow[1] - green[1]) * t)
    b = Math.round(green[2] + (yellow[2] - green[2]) * t)
  } else {
    const t = (ratio - 0.5) * 2
    r = Math.round(yellow[0] + (red[0] - yellow[0]) * t)
    g = Math.round(yellow[1] + (red[1] - yellow[1]) * t)
    b = Math.round(yellow[2] + (red[2] - yellow[2]) * t)
  }
  return `rgba(${r},${g},${b},0.85)`
}

export default function ExecAgeStepSection({ productGroup, startDate, endDate }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!startDate || !endDate) return
    setLoading(true)
    setError('')
    getExecAgeStep(productGroup, startDate, endDate)
      .then(res => setData(res.data))
      .catch(() => setError('Veri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, startDate, endDate])

  if (!startDate || !endDate) return null
  if (loading) return <AgeStepSkeleton />
  if (error) return <div className="text-sm text-red-400 py-10 text-center">{error}</div>
  if (!data?.matrix?.length) return null

  // Tüm değerler için min/max hesapla
  const allValues = data.matrix.flatMap(row => 
    [row.step0, row.step1, row.step2, row.step3, row.step4Plus].filter(v => v > 0)
  )
  const minVal = Math.min(...allValues)
  const maxVal = Math.max(...allValues)

  const getHeatmapStyle = (value) => {
    if (!value || value === 0) return {}
    const ratio = maxVal === minVal ? 0.5 : (value - minVal) / (maxVal - minVal)
    return {
      background: heatColor(ratio),
      color: '#1a1a1a',
      fontWeight: ratio > 0.7 ? '700' : '500'
    }
  }

  // Risk satırları (6-10 ve 11-15 yaş)
  const isRiskRow = (ageGroup) => ageGroup?.includes('6-10') || ageGroup?.includes('11-15')

  // Toplam Step 0 hesapla
  const totalStep0 = data.matrix.reduce((sum, row) => sum + (row.step0 || 0), 0)
  const totalAll = data.matrix.reduce((sum, row) => sum + (row.total || 0), 0)
  const step0Ratio = totalAll > 0 ? ((totalStep0 / totalAll) * 100).toFixed(1) : 0

  // Risk bölgesi hesapla (6-15 yaş + Basamak 0)
  const riskZone = data.matrix
    .filter(row => isRiskRow(row.ageGroup))
    .reduce((sum, row) => sum + (row.step0 || 0), 0)

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl overflow-hidden">
      <div className="px-5 py-4 border-b border-slate-100 dark:border-white/6 flex items-center justify-between">
        <div>
          <p className="text-sm font-semibold text-slate-800 dark:text-white">Araç Yaşı × Basamak Matrisi</p>
          <p className="text-xs text-slate-400 mt-0.5">Renk yoğunluğu poliçe sayısını gösterir</p>
        </div>
        <div className="flex items-center gap-3">
          <div className="flex items-center gap-2 px-3 py-1.5 bg-amber-50 dark:bg-amber-500/10 rounded-lg">
            <span className="text-xs text-slate-500 dark:text-slate-400">Basamak 0:</span>
            <span className="text-sm font-bold text-amber-600 dark:text-amber-400">%{step0Ratio}</span>
          </div>
          <div className="flex items-center gap-2 px-3 py-1.5 bg-red-50 dark:bg-red-500/10 rounded-lg">
            <span className="text-xs text-slate-500 dark:text-slate-400">Risk Bölgesi:</span>
            <span className="text-sm font-bold text-red-600 dark:text-red-400">{riskZone.toLocaleString('tr-TR')}</span>
          </div>
        </div>
      </div>
      <div className="overflow-auto">
        <table className="w-full text-xs">
          <thead className="bg-slate-50 dark:bg-white/5">
            <tr>
              <th className="px-4 py-3 text-left font-semibold text-slate-500 dark:text-slate-400 sticky left-0 bg-slate-50 dark:bg-[#0d1f3c]">Araç Yaşı</th>
              <th className="px-4 py-3 text-center font-semibold text-red-500">Basamak 0</th>
              <th className="px-4 py-3 text-center font-semibold text-slate-500 dark:text-slate-400">Basamak 1</th>
              <th className="px-4 py-3 text-center font-semibold text-slate-500 dark:text-slate-400">Basamak 2</th>
              <th className="px-4 py-3 text-center font-semibold text-slate-500 dark:text-slate-400">Basamak 3</th>
              <th className="px-4 py-3 text-center font-semibold text-slate-500 dark:text-slate-400">Basamak 4+</th>
              <th className="px-4 py-3 text-center font-semibold text-slate-500 dark:text-slate-400">Toplam</th>
            </tr>
          </thead>
          <tbody>
            {data.matrix.map((row, i) => {
              const isRisk = isRiskRow(row.ageGroup)
              return (
                <tr key={i} className={`border-t border-slate-100 dark:border-white/5 ${isRisk ? 'bg-red-50/30 dark:bg-red-500/5' : ''}`}>
                  <td className={`px-4 py-2.5 font-medium sticky left-0 bg-white dark:bg-[#0d1f3c] ${isRisk ? 'text-red-600 dark:text-red-400' : 'text-slate-700 dark:text-slate-300'}`}>
                    {row.ageGroup}
                  </td>
                  <td className="px-4 py-2.5 text-center font-mono" style={getHeatmapStyle(row.step0)}>
                    {row.step0?.toLocaleString('tr-TR') || '-'}
                  </td>
                  <td className="px-4 py-2.5 text-center font-mono" style={getHeatmapStyle(row.step1)}>
                    {row.step1?.toLocaleString('tr-TR') || '-'}
                  </td>
                  <td className="px-4 py-2.5 text-center font-mono" style={getHeatmapStyle(row.step2)}>
                    {row.step2?.toLocaleString('tr-TR') || '-'}
                  </td>
                  <td className="px-4 py-2.5 text-center font-mono" style={getHeatmapStyle(row.step3)}>
                    {row.step3?.toLocaleString('tr-TR') || '-'}
                  </td>
                  <td className="px-4 py-2.5 text-center font-mono" style={getHeatmapStyle(row.step4Plus)}>
                    {row.step4Plus?.toLocaleString('tr-TR') || '-'}
                  </td>
                  <td className="px-4 py-2.5 text-center font-mono font-semibold text-slate-700 dark:text-slate-300">
                    {row.total?.toLocaleString('tr-TR') || '-'}
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
      {/* Renk skalası */}
      <div className="px-5 py-3 border-t border-slate-100 dark:border-white/6 flex items-center gap-4">
        <span className="text-xs text-slate-400">Düşük</span>
        <div className="flex gap-0.5">
          {[0, 0.17, 0.33, 0.5, 0.67, 0.83, 1].map((r, i) => (
            <div key={i} className="w-6 h-3 rounded-sm" style={{ background: heatColor(r) }} />
          ))}
        </div>
        <span className="text-xs text-slate-400">Yüksek</span>
      </div>
    </div>
  )
}