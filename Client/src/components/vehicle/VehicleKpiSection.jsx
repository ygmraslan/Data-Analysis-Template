import { useState, useEffect } from 'react'
import * as vehicleApi from '../../api/vehicleApi'
import { getLastWeekRange, getPrevWeekRange } from '../../utils/formatDate'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'

const VehicleKpiSkeleton = () => (
  <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
    {[1, 2, 3, 4].map(i => (
      <SkeletonCard key={i}>
        <SkeletonRow className="justify-between mb-3">
          <SkeletonBlock width="w-24" height="h-3" />
          <SkeletonBlock width="w-8" height="h-8" rounded="rounded-lg" />
        </SkeletonRow>
        <SkeletonBlock width="w-36" height="h-7" className="mb-1" />
        <SkeletonBlock width="w-28" height="h-5" className="mb-3" />
        <SkeletonBlock width="w-full" height="h-px" className="mb-3" />
        <SkeletonBlock width="w-40" height="h-3" />
      </SkeletonCard>
    ))}
  </div>
)

const ICON_COLORS = {
  green: { bg: 'bg-emerald-500/10 dark:bg-emerald-500/15', text: 'text-emerald-500' },
  red:   { bg: 'bg-red-500/10 dark:bg-red-500/15',         text: 'text-red-500'     },
  amber: { bg: 'bg-amber-500/10 dark:bg-amber-500/15',     text: 'text-amber-500'   },
}

function KpiCard({ label, iconColor = 'green', icon, children }) {
  const colors = ICON_COLORS[iconColor]
  return (
    <div className="relative bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 overflow-hidden group transition-all hover:-translate-y-0.5 hover:shadow-lg">
      <div className="absolute top-0 left-0 right-0 h-0.5 bg-gradient-to-r from-transparent via-emerald-500 to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
      <div className="flex items-start justify-between mb-3">
        <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wide">{label}</p>
        <div className={`w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 ${colors.bg} ${colors.text}`}>
          {icon}
        </div>
      </div>
      {children}
    </div>
  )
}

function ChangeKpiCard({ label, iconColor, icon, group, wow, hasValue, noValueText, noValueColor, prevGroup, prevWoW, weekRange, prevWeekRange }) {
  const noValueColors = {
    amber:   { bg: 'bg-amber-50 dark:bg-amber-500/10',     text: 'text-amber-600 dark:text-amber-400'    },
    emerald: { bg: 'bg-emerald-50 dark:bg-emerald-500/10', text: 'text-emerald-600 dark:text-emerald-400' },
  }
  return (
    <KpiCard label={label} iconColor={iconColor} icon={icon}>
      {hasValue ? (
        <>
          <p className="text-xl font-bold text-slate-900 dark:text-white leading-tight">{group}</p>
          <p className="text-2xl font-bold mt-0.5" style={{ color: wow > 0 ? '#10b981' : '#ef4444' }}>
            {wow > 0 ? '+' : ''}{wow}%
          </p>
          <p className="text-xs text-slate-400 dark:text-slate-500 mt-1">{weekRange}</p>
          <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
            <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
            <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mt-0.5">
              {prevGroup} — <span style={{ color: prevWoW > 0 ? '#10b981' : '#ef4444' }}>{prevWoW > 0 ? '+' : ''}{prevWoW}%</span>
            </p>
          </div>
        </>
      ) : (
        <div className={`flex flex-col items-center justify-center py-4 rounded-lg mt-1 ${noValueColors[noValueColor]?.bg}`}>
          <p className={`text-xs font-semibold text-center ${noValueColors[noValueColor]?.text}`}>{noValueText}</p>
        </div>
      )}
    </KpiCard>
  )
}

const TrendUpIcon   = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><polyline points="23 6 13.5 15.5 8.5 10.5 1 18" /><polyline points="17 6 23 6 23 12" /></svg>
const TrendDownIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><polyline points="23 18 13.5 8.5 8.5 13.5 1 6" /><polyline points="17 18 23 18 23 12" /></svg>

export default function VehicleKpiSection({ productGroup, filter }) {
  const [data,    setData]    = useState(null)
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  const weekRange     = getLastWeekRange()
  const prevWeekRange = getPrevWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    vehicleApi.getVehicleKpi(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('KPI verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <VehicleKpiSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data)   return null

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <ChangeKpiCard
        label="En Çok Artan — Yaş Grubu" iconColor="green" icon={TrendUpIcon}
        group={data.topGainerAge} wow={data.topGainerAgeWoW}
        hasValue={data.hasAgeGainer}
        noValueText="Bu hafta artış gösteren yaş grubu yok" noValueColor="amber"
        weekRange={weekRange} prevGroup={data.prevTopGainerAge}
        prevWoW={data.prevTopGainerAgeWoW} prevWeekRange={prevWeekRange}
      />
      <ChangeKpiCard
        label="En Çok Azalan — Yaş Grubu" iconColor="red" icon={TrendDownIcon}
        group={data.topLoserAge} wow={data.topLoserAgeWoW}
        hasValue={data.hasAgeLoser}
        noValueText="Bu hafta tüm yaş grupları artış gösterdi" noValueColor="emerald"
        weekRange={weekRange} prevGroup={data.prevTopLoserAge}
        prevWoW={data.prevTopLoserAgeWoW} prevWeekRange={prevWeekRange}
      />
      <ChangeKpiCard
        label="En Çok Artan — Bedel Aralığı" iconColor="green" icon={TrendUpIcon}
        group={data.topGainerPrice} wow={data.topGainerPriceWoW}
        hasValue={data.hasPriceGainer}
        noValueText="Bu hafta artış gösteren bedel aralığı yok" noValueColor="amber"
        weekRange={weekRange} prevGroup={data.prevTopGainerPrice}
        prevWoW={data.prevTopGainerPriceWoW} prevWeekRange={prevWeekRange}
      />
      <ChangeKpiCard
        label="En Çok Azalan — Bedel Aralığı" iconColor="red" icon={TrendDownIcon}
        group={data.topLoserPrice} wow={data.topLoserPriceWoW}
        hasValue={data.hasPriceLoser}
        noValueText="Bu hafta tüm bedel aralıkları artış gösterdi" noValueColor="emerald"
        weekRange={weekRange} prevGroup={data.prevTopLoserPrice}
        prevWoW={data.prevTopLoserPriceWoW} prevWeekRange={prevWeekRange}
      />
    </div>
  )
}