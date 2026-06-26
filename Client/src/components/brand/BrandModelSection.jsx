import { useState, useEffect } from 'react'
import * as brandApi from '../../api/brandApi'
import { SkeletonBlock } from '../ui/Skeleton'

const ModelSkeleton = () => (
  <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
    <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
    <div className="space-y-2">
      {[1,2,3,4,5,6,7].map(i => (
        <SkeletonBlock key={i} width="w-full" height="h-9" rounded="rounded-lg" />
      ))}
    </div>
  </div>
)

export default function BrandModelSection({ productGroup, brand, filter }) {
  const [data,    setData]    = useState([])
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  useEffect(() => {
    if (!brand) return
    setLoading(true)
    setError('')
    brandApi.getBrandModels(productGroup, brand, filter)
      .then(r => setData(r.data))
      .catch(() => setError('Model verisi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, brand, filter])

  if (loading) return <ModelSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data.length) return null

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">
        Model Dağılımı — {brand}
      </p>

      <div className="overflow-x-auto">
        <table className="w-full text-xs">
          <thead>
            <tr className="border-b border-slate-100 dark:border-white/8">
              <th className="text-left pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Model</th>
              <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Poliçe</th>
              <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Net Prim</th>
              <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Ort. Prim</th>
              <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">WoW</th>
            </tr>
          </thead>
          <tbody>
            {data.map((row, i) => {
              const wowColor = row.wow > 0 ? '#10b981' : row.wow < 0 ? '#ef4444' : '#94a3b8'
              const rowBg    = i % 2 === 1 ? 'bg-slate-50 dark:bg-white/2' : ''
              return (
                <tr key={row.model} className={`border-b border-slate-50 dark:border-white/4 ${rowBg}`}>
                  <td className="py-2.5 font-bold text-slate-800 dark:text-slate-200">{row.model}</td>
                  <td className="py-2.5 text-right text-slate-600 dark:text-slate-300">
                    {Number(row.policyCount).toLocaleString('tr-TR')}
                  </td>
                  <td className="py-2.5 text-right text-slate-600 dark:text-slate-300">
                    ₺{Number(row.netPremium).toLocaleString('tr-TR', { maximumFractionDigits: 0 })}
                  </td>
                  <td className="py-2.5 text-right text-slate-600 dark:text-slate-300">
                    ₺{Number(row.avgPremium).toLocaleString('tr-TR', { maximumFractionDigits: 0 })}
                  </td>
                  <td className="py-2.5 text-right font-bold" style={{ color: wowColor }}>
                    {row.wow > 0 ? '+' : ''}{row.wow}%
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
    </div>
  )
}