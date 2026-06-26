import { useState, useEffect } from 'react'
import * as dashboardApi from '../../api/dashboardApi'
import KpiCard from '../ui/KpiCard'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'
import { getLastWeekRange, getPrevWeekRange } from '../../utils/formatDate'

const KpiSkeleton = () => (
  <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
    {[1, 2, 3].map(i => (
      <SkeletonCard key={i}>
        <SkeletonBlock width="w-24" height="h-3" />
        <SkeletonBlock width="w-16" height="h-2.5" className="mt-1" />
        <SkeletonBlock width="w-32" height="h-8" className="mt-3 mb-2" />
        <SkeletonRow className="mt-1">
          <SkeletonBlock width="w-4" height="h-3" rounded="rounded-sm" />
          <SkeletonBlock width="w-12" height="h-3" />
          <SkeletonBlock width="w-24" height="h-3" />
        </SkeletonRow>
      </SkeletonCard>
    ))}
  </div>
)

export default function KpiSection({ productGroup, filter }) {
  const [data,    setData]    = useState(null)
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  const weekRange     = getLastWeekRange()
  const prevWeekRange = getPrevWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    dashboardApi.getKpi(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('KPI verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <KpiSkeleton />
  if (error) return (
    <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  )
  if (!data) return null

  const fmt    = n => Number(n).toLocaleString('tr-TR')
  const fmtPrm = n => '₺' + Number(n).toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      <KpiCard
        label="Haftalık Poliçe Adedi"
        value={fmt(data.weeklyPolicyCount)}
        weekRange={weekRange}
        change={data.policyWoW}
        changeSuffix="%"
        changeLabel="geçen haftaya göre"
        inverse={false}
        iconColor="blue"
        icon={
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
            <polyline points="14 2 14 8 20 8" />
          </svg>
        }
        prevValue={fmt(data.prevWeeklyPolicyCount) + ' poliçe'}
        prevWeekRange={prevWeekRange}
      />
      <KpiCard
        label="Haftalık Net Prim"
        value={fmtPrm(data.weeklyNetPremium)}
        weekRange={weekRange}
        change={data.netPremiumWoW}
        changeSuffix="%"
        changeLabel="geçen haftaya göre"
        inverse={false}
        iconColor="emerald"
        icon={
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <line x1="12" y1="1" x2="12" y2="23" /><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6" />
          </svg>
        }
        prevValue={fmtPrm(data.prevWeeklyNetPremium)}
        prevWeekRange={prevWeekRange}
      />
      <KpiCard
        label="0. Basamak Oranı"
        value={`%${data.zeroStepRatio}`}
        weekRange={weekRange}
        change={data.zeroStepWoW}
        changeSuffix=""
        changeLabel="puan değişim"
        inverse={true}
        highlight={data.zeroStepRatio > 50}
        warning={data.zeroStepRatio > 50 ? '⚠ Loss ratio riski yüksek' : undefined}
        iconColor="amber"
        icon={
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
            <line x1="12" y1="9" x2="12" y2="13" /><line x1="12" y1="17" x2="12.01" y2="17" />
          </svg>
        }
      />
    </div>
  )
}