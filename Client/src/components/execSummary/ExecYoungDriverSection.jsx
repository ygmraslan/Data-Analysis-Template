import { useState, useEffect } from 'react'
import { getExecYoungDriver } from '../../api/execSummaryApi'
import { SkeletonBlock, SkeletonCard } from '../ui/Skeleton'

const YoungDriverSkeleton = () => (
  <SkeletonCard>
    <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
    <div className="space-y-3">
      {[1, 2, 3, 4].map(i => <SkeletonBlock key={i} width="w-full" height="h-6" />)}
    </div>
  </SkeletonCard>
)

export default function ExecYoungDriverSection({ productGroup, startDate, endDate }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!startDate || !endDate) return
    setLoading(true)
    setError('')
    getExecYoungDriver(productGroup, startDate, endDate)
      .then(res => setData(res.data))
      .catch(() => setError('Veri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, startDate, endDate])

  if (!startDate || !endDate) return null
  if (loading) return <YoungDriverSkeleton />
  if (error) return <div className="text-sm text-red-400 py-10 text-center">{error}</div>
  if (!data?.brands?.length) return null

  const premiumBrands = ['BMW', 'MERCEDES', 'AUDI']
  const riskBrands = ['BMW', 'FIAT']
  const maxCount = Math.max(...data.brands.map(b => b.count))
  const totalYoung = data.brands.reduce((sum, b) => sum + b.count, 0)

  // Renk belirleme
  const getBarColor = (label) => {
    const upper = label?.toUpperCase()
    if (riskBrands.includes(upper)) return 'bg-red-500'
    if (premiumBrands.includes(upper)) return 'bg-amber-500'
    if (upper === 'VOLKSWAGEN') return 'bg-purple-500'
    return 'bg-blue-500'
  }

  const isRiskBrand = (label) => riskBrands.includes(label?.toUpperCase())

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl overflow-hidden">
      <div className="px-5 py-4 border-b border-slate-100 dark:border-white/6 flex items-center justify-between">
        <div>
          <p className="text-sm font-semibold text-slate-800 dark:text-white">Marka × Genç Sürücü (18-25)</p>
          <p className="text-xs text-slate-400 mt-0.5">En yüksek kaza frekansı yaş grubu</p>
        </div>
        <div className="flex items-center gap-2 px-3 py-1.5 bg-red-50 dark:bg-red-500/10 rounded-lg">
          <span className="text-xs text-slate-500 dark:text-slate-400">Toplam:</span>
          <span className="text-sm font-bold text-red-600 dark:text-red-400">{totalYoung.toLocaleString('tr-TR')}</span>
        </div>
      </div>
      
      <div className="p-5 space-y-3">
        {data.brands.slice(0, 6).map((brand, i) => {
          const ratio = maxCount > 0 ? (brand.count / maxCount) * 100 : 0
          const pct = totalYoung > 0 ? ((brand.count / totalYoung) * 100).toFixed(1) : 0
          const isRisk = isRiskBrand(brand.label)
          
          return (
            <div key={i} className="flex items-center gap-3">
              <div className={`w-24 text-xs truncate ${isRisk ? 'text-red-600 dark:text-red-400 font-medium' : 'text-slate-600 dark:text-slate-400'}`}>
                {brand.label}
              </div>
              <div className="flex-1 h-2 bg-slate-100 dark:bg-white/10 rounded-full overflow-hidden">
                <div
                  className={`h-full rounded-full ${getBarColor(brand.label)}`}
                  style={{ width: `${ratio}%` }}
                />
              </div>
              <div className="w-20 text-right flex items-center justify-end gap-2">
                <span className="text-xs font-mono text-slate-600 dark:text-slate-400">
                  {brand.count?.toLocaleString('tr-TR')}
                </span>
                <span className="text-[10px] text-slate-400">%{pct}</span>
              </div>
            </div>
          )
        })}
      </div>

      {/* Risk Özeti */}
      <div className="px-5 py-3 border-t border-slate-100 dark:border-white/6 bg-amber-50 dark:bg-amber-500/10">
        <p className="text-xs text-amber-700 dark:text-amber-300">
          <strong>Fiat + genç sürücü</strong> klasik yüksek frekans segmenti. <strong>BMW + genç sürücü</strong> hem frekans hem maliyet riski taşır.
        </p>
      </div>
    </div>
  )
}