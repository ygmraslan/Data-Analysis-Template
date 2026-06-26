import { useState, useEffect } from 'react'
import * as demoApi from '../../api/demoApi'
import KpiCard from '../ui/KpiCard'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'
import { getLastWeekRange, getPrevWeekRange } from '../../utils/formatDate'

const DemoKpiSkeleton = () => (
  <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
    {[1, 2, 3, 4].map(i => (
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

const UserIcon = (
  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
    <circle cx="12" cy="7" r="4"/>
  </svg>
)

const BuildingIcon = (
  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M3 21h18M9 8h1M9 12h1M9 16h1M14 8h1M14 12h1M14 16h1M5 21V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2v16"/>
  </svg>
)

const MapPinIcon = (
  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/>
    <circle cx="12" cy="10" r="3"/>
  </svg>
)

const UsersIcon = (
  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
    <circle cx="9" cy="7" r="4"/>
    <path d="M23 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75"/>
  </svg>
)

export default function DemoKpiSection({ productGroup, filter }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const weekRange = getLastWeekRange()
  const prevWeekRange = getPrevWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    demoApi.getDemoKpi(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('KPI verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <DemoKpiSkeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data) return null

  const fmt = n => Number(n).toLocaleString('tr-TR')

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <KpiCard
        label="Gerçek Kişi"
        value={`%${data.individualRatio}`}
        weekRange={weekRange}
        change={data.individualWoW}
        changeSuffix="%"
        changeLabel="geçen haftaya göre"
        inverse={false}
        iconColor="emerald"
        icon={UserIcon}
        prevValue={`%${data.prevIndividualRatio} — ${fmt(data.prevIndividualCount)} poliçe`}
        prevWeekRange={prevWeekRange}
      />
      <KpiCard
        label="Tüzel Kişi"
        value={`%${data.corporateRatio}`}
        weekRange={weekRange}
        change={data.corporateWoW}
        changeSuffix="%"
        changeLabel="geçen haftaya göre"
        inverse={false}
        iconColor="blue"
        icon={BuildingIcon}
        prevValue={`%${data.prevCorporateRatio} — ${fmt(data.prevCorporateCount)} poliçe`}
        prevWeekRange={prevWeekRange}
      />
      <KpiCard
        label="Top Plaka İli"
        value={data.topPlateCity}
        weekRange={weekRange}
        change={data.topPlateCityWoW}
        changeSuffix=""
        changeLabel="puan değişim"
        inverse={false}
        iconColor="amber"
        icon={MapPinIcon}
        prevValue={`${data.prevTopPlateCity} — %${data.prevTopPlateCityRatio}`}
        prevWeekRange={prevWeekRange}
      />
      <KpiCard
        label="Dominant Yaş Grubu"
        value={data.dominantAgeGroup}
        weekRange={weekRange}
        change={data.dominantAgeWoW}
        changeSuffix=""
        changeLabel="puan değişim"
        inverse={false}
        iconColor="purple"
        icon={UsersIcon}
        prevValue={`${data.prevDominantAgeGroup} — %${data.prevDominantAgeRatio}`}
        prevWeekRange={prevWeekRange}
      />
    </div>
  )
}