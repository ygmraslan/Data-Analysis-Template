import { useState, useEffect } from 'react'
import * as companyApi from '../../api/companyApi'
import { SkeletonBlock } from '../ui/Skeleton'

const ListSkeleton = () => (
  <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
    <div className="flex justify-between items-center mb-4">
      <SkeletonBlock width="w-48" height="h-4" />
      <SkeletonBlock width="w-32" height="h-8" rounded="rounded-lg" />
    </div>
    {[1,2,3,4,5,6,7,8,9,10].map(i => (
      <SkeletonBlock key={i} width="w-full" height="h-9" className="mb-2" rounded="rounded-lg" />
    ))}
  </div>
)

const PAGE_SIZE = 8

export default function CompanyListSection({ productGroup, onCompanySelect, selectedCompany, filter }) {
  const [data,    setData]    = useState([])
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')
  const [sortBy,  setSortBy]  = useState('netPremium')
  const [page,    setPage]    = useState(0)

  useEffect(() => {
    setLoading(true)
    setError('')
    companyApi.getCompanyList(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('Şirket listesi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <ListSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data.length) return null

  const sorted = [...data].sort((a, b) =>
    sortBy === 'netPremium' ? b.netPremium - a.netPremium : b.policyCount - a.policyCount
  )
  const totalPages = Math.ceil(sorted.length / PAGE_SIZE)
  const paged      = sorted.slice(page * PAGE_SIZE, (page + 1) * PAGE_SIZE)

  const fmtPrm = (n) => '₺' + Number(n).toLocaleString('tr-TR', { maximumFractionDigits: 0 })

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <div className="flex items-center justify-between mb-4">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">
          Şirket Geçiş Sıralaması
          <span className="ml-2 text-xs font-normal text-slate-400">({data.length} şirket)</span>
        </p>
        <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
          {[
            { key: 'netPremium', label: 'Net Prim' },
            { key: 'policyCount', label: 'Poliçe' },
          ].map(opt => (
            <button
              key={opt.key}
              onClick={() => { setSortBy(opt.key); setPage(0) }}
              className={`px-3 py-1 rounded-md text-xs font-semibold transition-all ${
                sortBy === opt.key
                  ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                  : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
              }`}
            >
              {opt.label}
            </button>
          ))}
        </div>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-xs">
          <thead>
            <tr className="border-b border-slate-100 dark:border-white/8">
              <th className="text-left pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Önceki Şirket</th>
              <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Poliçe</th>
              <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Net Prim</th>
              <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Ort. Prim</th>
              <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">WoW</th>
            </tr>
          </thead>
          <tbody>
            {paged.map((row, i) => {
              const isSelected = row.company === selectedCompany
              const wowColor   = row.wow > 0 ? '#10b981' : row.wow < 0 ? '#ef4444' : '#94a3b8'
              const rowBg      = isSelected
                ? 'bg-emerald-50 dark:bg-emerald-500/10'
                : i % 2 === 1 ? 'bg-slate-50 dark:bg-white/2' : ''
              return (
                <tr
                  key={row.company}
                  onClick={() => onCompanySelect?.(row.company)}
                  className={`border-b border-slate-50 dark:border-white/4 cursor-pointer hover:bg-slate-50 dark:hover:bg-white/4 transition-colors ${rowBg}`}
                >
                  <td className={`py-2.5 font-bold truncate max-w-[200px] ${isSelected ? 'text-emerald-600 dark:text-emerald-400' : 'text-slate-800 dark:text-slate-200'}`} title={row.company}>
                    {row.company}
                  </td>
                  <td className="h-10 text-right text-slate-600 dark:text-slate-300">
                    {Number(row.policyCount).toLocaleString('tr-TR')}
                  </td>
                  <td className="h-10 text-right text-slate-600 dark:text-slate-300">
                    {fmtPrm(row.netPremium)}
                  </td>
                  <td className="h-10 text-right text-slate-600 dark:text-slate-300">
                    {fmtPrm(row.avgPremium)}
                  </td>
                  <td className="h-10 text-right font-bold" style={{ color: wowColor }}>
                    {row.wow > 0 ? '+' : ''}{row.wow}%
                  </td>
                </tr>
              )
            })}
            {Array.from({ length: Math.max(0, PAGE_SIZE - paged.length) }).map((_, i) => (
              <tr key={`empty-${i}`} className="border-b border-slate-50 dark:border-white/4">
                <td className="h-10">&nbsp;</td>
                <td className="h-10" /><td className="h-10" /><td className="h-10" /><td className="h-10" />
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-between mt-4 pt-3 border-t border-slate-100 dark:border-white/6">
          <span className="text-xs text-slate-400">
            {page * PAGE_SIZE + 1}–{Math.min((page + 1) * PAGE_SIZE, sorted.length)} / {sorted.length} şirket
          </span>
          <div className="flex items-center gap-1">
            <button
              onClick={() => setPage(p => Math.max(0, p - 1))}
              disabled={page === 0}
              className="p-1.5 rounded-lg border border-slate-200 dark:border-white/10 text-slate-500 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-white/5 disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
            >
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><polyline points="15 18 9 12 15 6" /></svg>
            </button>
            <button
              onClick={() => setPage(p => Math.min(totalPages - 1, p + 1))}
              disabled={page === totalPages - 1}
              className="p-1.5 rounded-lg border border-slate-200 dark:border-white/10 text-slate-500 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-white/5 disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
            >
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><polyline points="9 18 15 12 9 6" /></svg>
            </button>
          </div>
        </div>
      )}
    </div>
  )
}