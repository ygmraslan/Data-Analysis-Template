import { useState, useEffect } from 'react'
import * as agencyApi from '../../api/agencyApi'
import { getLastWeekRange, getPrevWeekRange } from '../../utils/formatDate'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'

const AgencyKpiSkeleton = () => (
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
  green:  { bg: 'bg-emerald-500/10 dark:bg-emerald-500/15', text: 'text-emerald-500' },
  blue:   { bg: 'bg-blue-500/10 dark:bg-blue-500/15',       text: 'text-blue-500'    },
  amber:  { bg: 'bg-amber-500/10 dark:bg-amber-500/15',     text: 'text-amber-500'   },
  purple: { bg: 'bg-purple-500/10 dark:bg-purple-500/15',   text: 'text-purple-500'  },
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

// Acente adı + prim gösteren kart (En Yüksek Primli, En Yüksek Ort.)
function AgencyPremiumKpiCard({ label, iconColor, icon, agency, value, weekRange, prevAgency, prevValue, prevWeekRange }) {
  return (
    <KpiCard label={label} iconColor={iconColor} icon={icon}>
      <p className="text-lg font-bold text-slate-900 dark:text-white leading-tight truncate" title={agency}>
        {agency || '—'}
      </p>
      <p className="text-2xl font-bold text-slate-900 dark:text-white mt-0.5">{value}</p>
      <p className="text-xs text-slate-400 dark:text-slate-500 mt-1">{weekRange}</p>
      <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
        <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
        <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mt-0.5 truncate" title={prevAgency}>
          {prevAgency || '—'}
        </p>
        <p className="text-sm font-bold text-slate-600 dark:text-slate-300">{prevValue}</p>
      </div>
    </KpiCard>
  )
}

// Sayı + WoW gösteren kart (Aktif Acente, Ort. Prim)
function StatKpiCard({ label, iconColor, icon, value, wow, weekRange, prevValue, prevWow, prevWeekRange }) {
  const wowColor = wow >= 0 ? '#10b981' : '#ef4444'
  const wowText = (wow >= 0 ? '+' : '') + wow.toFixed(1) + '%'
  const prevWowColor = prevWow >= 0 ? '#10b981' : '#ef4444'
  const prevWowText = (prevWow >= 0 ? '+' : '') + prevWow.toFixed(1) + '%'
  return (
    <KpiCard label={label} iconColor={iconColor} icon={icon}>
      <p className="text-3xl font-bold text-slate-900 dark:text-white">{value}</p>
      <p className="text-sm font-semibold mt-0.5" style={{ color: wowColor }}>{wowText}</p>
      <p className="text-xs text-slate-400 dark:text-slate-500 mt-1">{weekRange}</p>
      <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
        <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
        <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mt-0.5">
          {prevValue} — <span style={{ color: prevWowColor }}>{prevWowText}</span>
        </p>
      </div>
    </KpiCard>
  )
}

const TrophyIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M6 9H4.5a2.5 2.5 0 0 1 0-5H6"/><path d="M18 9h1.5a2.5 2.5 0 0 0 0-5H18"/><path d="M4 22h16"/><path d="M10 14.66V17c0 .55-.47.98-.97 1.21C7.85 18.75 7 20.24 7 22"/><path d="M14 14.66V17c0 .55.47.98.97 1.21C16.15 18.75 17 20.24 17 22"/><path d="M18 2H6v7a6 6 0 0 0 12 0V2Z"/></svg>
const CoinIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><circle cx="12" cy="12" r="10"/><path d="M12 6v12M9 9h6M9 15h6"/></svg>
const UsersIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
const AvgIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M3 3v18h18"/><path d="m19 9-5 5-4-4-3 3"/></svg>

export default function AgencyKpiSection({ productGroup, onDefaultAgency, filter }) {
  const [data,    setData]    = useState(null)
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  const weekRange     = getLastWeekRange()
  const prevWeekRange = getPrevWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    agencyApi.getAgencyKpi(productGroup, filter)
      .then(r => {
        setData(r.data)
        // Default acente kodunu parent'a bildir
        if (onDefaultAgency && r.data.defaultAgencyCode) {
          onDefaultAgency(r.data.defaultAgencyCode)
        }
      })
      .catch(() => setError('KPI verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <AgencyKpiSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data)   return null

  const fmtPrm = (n) => '₺' + Number(n || 0).toLocaleString('tr-TR', { maximumFractionDigits: 0 })
  const fmtCnt = (n) => Number(n || 0).toLocaleString('tr-TR')

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <AgencyPremiumKpiCard
        label="En Yüksek Primli Acente"
        iconColor="green"
        icon={TrophyIcon}
        agency={data.topPremiumAgency}
        value={fmtPrm(data.topPremiumAmount)}
        weekRange={weekRange}
        prevAgency={data.prevTopPremiumAgency}
        prevValue={fmtPrm(data.prevTopPremiumAmount)}
        prevWeekRange={prevWeekRange}
      />
      <AgencyPremiumKpiCard
        label="En Yüksek Ort. Primli"
        iconColor="blue"
        icon={CoinIcon}
        agency={data.topAvgPremiumAgency}
        value={fmtPrm(data.topAvgPremiumAmount)}
        weekRange={weekRange}
        prevAgency={data.prevTopAvgPremiumAgency}
        prevValue={fmtPrm(data.prevTopAvgPremiumAmount)}
        prevWeekRange={prevWeekRange}
      />
      <StatKpiCard
        label="Aktif Acente Sayısı"
        iconColor="amber"
        icon={UsersIcon}
        value={fmtCnt(data.activeAgencyCount)}
        wow={data.activeAgencyCountWoW || 0}
        weekRange={weekRange}
        prevValue={fmtCnt(data.prevActiveAgencyCount)}
        prevWow={data.prevActiveAgencyCountWoW || 0}
        prevWeekRange={prevWeekRange}
      />
      <StatKpiCard
        label="Ortalama Prim / Acente"
        iconColor="purple"
        icon={AvgIcon}
        value={fmtPrm(data.avgPremiumPerAgency)}
        wow={data.avgPremiumPerAgencyWoW || 0}
        weekRange={weekRange}
        prevValue={fmtPrm(data.prevAvgPremiumPerAgency)}
        prevWow={data.prevAvgPremiumPerAgencyWoW || 0}
        prevWeekRange={prevWeekRange}
      />
    </div>
  )
}