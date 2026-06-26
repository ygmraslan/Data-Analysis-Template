import { useState, useEffect } from 'react'
import * as brandApi from '../../api/brandApi'
import { getLastWeekRange, getPrevWeekRange } from '../../utils/formatDate'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'

const BrandKpiSkeleton = () => (
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
  blue:  { bg: 'bg-blue-500/10 dark:bg-blue-500/15',       text: 'text-blue-500'    },
  red:   { bg: 'bg-red-500/10 dark:bg-red-500/15',         text: 'text-red-500'     },
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

function CountKpiCard({ label, iconColor, icon, brand, count, weekRange, prevBrand, prevCount, prevWeekRange }) {
  return (
    <KpiCard label={label} iconColor={iconColor} icon={icon}>
      <p className="text-xl font-bold text-slate-900 dark:text-white leading-tight">{brand}</p>
      <p className="text-2xl font-bold text-slate-900 dark:text-white mt-0.5">
        {Number(count).toLocaleString('tr-TR')} <span className="text-sm font-normal text-slate-400">Poliçe</span>
      </p>
      <p className="text-xs text-slate-400 dark:text-slate-500 mt-1">{weekRange}</p>
      <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
        <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
        <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mt-0.5">
          {prevBrand} — <span className="text-slate-600 dark:text-slate-300">{Number(prevCount).toLocaleString('tr-TR')}</span>
        </p>
      </div>
    </KpiCard>
  )
}

function ChangeKpiCard({ label, iconColor, icon, brand, wow, hasValue, noValueText, noValueColor, prevBrand, prevWoW, prevWeekRange, weekRange }) {
  const noValueColors = {
    amber:   { bg: 'bg-amber-50 dark:bg-amber-500/10',     text: 'text-amber-600 dark:text-amber-400'    },
    emerald: { bg: 'bg-emerald-50 dark:bg-emerald-500/10', text: 'text-emerald-600 dark:text-emerald-400' },
  }
  return (
    <KpiCard label={label} iconColor={iconColor} icon={icon}>
      {hasValue ? (
        <>
          <p className="text-xl font-bold text-slate-900 dark:text-white leading-tight">{brand}</p>
          <p className="text-2xl font-bold mt-0.5" style={{ color: wow > 0 ? '#10b981' : '#ef4444' }}>
            {wow > 0 ? '+' : ''}{wow}%
          </p>
          <p className="text-xs text-slate-400 dark:text-slate-500 mt-1">{weekRange}</p>
          {prevBrand && prevWeekRange && (
            <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
              <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
              <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mt-0.5">
                {prevBrand} — <span style={{ color: prevWoW > 0 ? '#10b981' : '#ef4444' }}>{prevWoW > 0 ? '+' : ''}{prevWoW}%</span>
              </p>
            </div>
          )}
        </>
      ) : (
        <>
          <div className={`flex flex-col items-center justify-center py-3 rounded-lg mt-1 ${noValueColors[noValueColor]?.bg}`}>
            <p className={`text-xs font-semibold text-center leading-relaxed ${noValueColors[noValueColor]?.text}`}>
              {noValueText}
            </p>
          </div>
          {prevBrand && prevWeekRange && (
            <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
              <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
              <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mt-0.5">
                {prevBrand} — <span style={{ color: prevWoW > 0 ? '#10b981' : '#ef4444' }}>{prevWoW > 0 ? '+' : ''}{prevWoW}%</span>
              </p>
            </div>
          )}
        </>
      )}
    </KpiCard>
  )
}

const UpIcon       = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><polyline points="18 15 12 9 6 15" /></svg>
const DownIcon     = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><polyline points="6 9 12 15 18 9" /></svg>
const TrendUpIcon  = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><polyline points="23 6 13.5 15.5 8.5 10.5 1 18" /><polyline points="17 6 23 6 23 12" /></svg>
const TrendDownIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><polyline points="23 18 13.5 8.5 8.5 13.5 1 6" /><polyline points="17 18 23 18 23 12" /></svg>

export default function BrandKpiSection({ productGroup, filter }) {
  const [data,    setData]    = useState(null)
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  const weekRange     = getLastWeekRange()
  const prevWeekRange = getPrevWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    brandApi.getBrandKpi(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('KPI verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <BrandKpiSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data)   return null

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <CountKpiCard
        label="En Yüksek Poliçe" iconColor="green" icon={UpIcon}
        brand={data.topBrand} count={data.topBrandCount} weekRange={weekRange}
        prevBrand={data.topBrandPrev} prevCount={data.topBrandPrevCount} prevWeekRange={prevWeekRange}
      />
      <CountKpiCard
        label="En Düşük Poliçe" iconColor="blue" icon={DownIcon}
        brand={data.bottomBrand} count={data.bottomBrandCount} weekRange={weekRange}
        prevBrand={data.bottomBrandPrev} prevCount={data.bottomBrandPrevCount} prevWeekRange={prevWeekRange}
      />
      <ChangeKpiCard
        label="En Yüksek Artış" iconColor="green" icon={TrendUpIcon}
        brand={data.topGainerBrand} wow={data.topGainerWoW}
        hasValue={data.hasGainer}
        noValueText="Bu hafta artış gösteren marka yok" noValueColor="amber"
        weekRange={weekRange} prevBrand={data.prevTopGainerBrand}
        prevWoW={data.prevTopGainerWoW} prevWeekRange={prevWeekRange}
      />
      <ChangeKpiCard
        label="En Yüksek Azalış" iconColor="red" icon={TrendDownIcon}
        brand={data.topLoserBrand} wow={data.topLoserWoW}
        hasValue={data.hasLoser}
        noValueText="Bu hafta tüm markalar artış gösterdi" noValueColor="emerald"
        weekRange={weekRange} prevBrand={data.prevTopLoserBrand}
        prevWoW={data.prevTopLoserWoW} prevWeekRange={prevWeekRange}
      />
    </div>
  )
}