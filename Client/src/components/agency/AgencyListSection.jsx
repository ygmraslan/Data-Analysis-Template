import { useState, useEffect } from 'react'
import * as agencyApi from '../../api/agencyApi'
import { getLastWeekRange } from '../../utils/formatDate'
import { SkeletonBlock } from '../ui/Skeleton'

const ListSkeleton = () => (
  <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
    <div className="flex justify-between items-center mb-4">
      <SkeletonBlock width="w-48" height="h-4" />
    </div>
    {[1,2,3,4,5,6,7,8,9,10].map(i => (
      <SkeletonBlock key={i} width="w-full" height="h-9" className="mb-2" rounded="rounded-lg" />
    ))}
  </div>
)

export default function AgencyListSection({ productGroup, selectedAgency, onAgencySelect, filter }) {
  const [data,    setData]    = useState(null)
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  const weekRange = getLastWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    agencyApi.getAgencyList(productGroup, 1, 10, null, filter) // region=null → tüm bölgeler
      .then(r => setData(r.data))
      .catch(() => setError('Acente listesi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <ListSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data?.items?.length) return null

  const { items } = data
  
  // Para formatı - TAM gösterim
  const formatMoney = (n) => {
    return '₺' + Number(n).toLocaleString('tr-TR', { maximumFractionDigits: 0 })
  }

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <div className="flex items-center justify-between mb-4">
        <div>
          <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">
            Genel Top 10 Acente
          </p>
          <p className="text-xs text-slate-400 mt-0.5">{weekRange}</p>
        </div>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-xs">
          <thead>
            <tr className="border-b border-slate-100 dark:border-white/8">
              <th className="text-left pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide w-6">#</th>
              <th className="text-left pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Acente</th>
              <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Poliçe</th>
              <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Net Prim</th>
              <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Ort. Prim</th>
              <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">WoW</th>
            </tr>
          </thead>
          <tbody>
            {items.map((row, i) => {
              const isSelected = row.agencyCode === selectedAgency
              const wowColor = row.wowChange > 0 ? '#10b981' : row.wowChange < 0 ? '#ef4444' : '#94a3b8'
              const rowBg = isSelected
                ? 'bg-emerald-50 dark:bg-emerald-500/10'
                : i % 2 === 1 ? 'bg-slate-50/50 dark:bg-white/2' : ''
              return (
                <tr
                  key={row.agencyCode}
                  onClick={() => onAgencySelect?.(row.agencyCode)}
                  className={`border-b border-slate-50 dark:border-white/4 cursor-pointer hover:bg-slate-50 dark:hover:bg-white/4 transition-colors ${rowBg}`}
                >
                  <td className="py-2.5 text-slate-400 dark:text-slate-500 font-medium">{i + 1}</td>
                  <td 
                    className={`py-2.5 font-semibold truncate max-w-[200px] ${isSelected ? 'text-emerald-600 dark:text-emerald-400' : 'text-slate-800 dark:text-slate-200'}`}
                    title={row.agencyName}
                  >
                    {row.agencyName}
                  </td>
                  <td className="py-2.5 text-right text-slate-600 dark:text-slate-300">
                    {Number(row.policyCount).toLocaleString('tr-TR')}
                  </td>
                  <td className="py-2.5 text-right text-slate-600 dark:text-slate-300 font-medium">
                    {formatMoney(row.netPremium)}
                  </td>
                  <td className="py-2.5 text-right text-slate-500 dark:text-slate-400">
                    {formatMoney(row.avgPremium)}
                  </td>
                  <td className="py-2.5 text-right font-bold" style={{ color: wowColor }}>
                    {row.wowChange > 0 ? '+' : ''}{Number(row.wowChange).toFixed(1)}%
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