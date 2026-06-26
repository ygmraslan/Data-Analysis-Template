import { useState, useEffect } from 'react'
import * as companyApi from '../../api/companyApi'
import { getLastWeekRange, getPrevWeekRange } from '../../utils/formatDate'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'

const CompanyKpiSkeleton = () => (
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

function CompanyPremiumKpiCard({ label, iconColor, icon, company, value, subValue, weekRange, prevCompany, prevValue, prevWeekRange }) {
  return (
    <KpiCard label={label} iconColor={iconColor} icon={icon}>
      <p className="text-lg font-bold text-slate-900 dark:text-white leading-tight truncate" title={company}>{company}</p>
      <p className="text-2xl font-bold text-slate-900 dark:text-white mt-0.5">{value}</p>
      {subValue && <p className="text-sm text-slate-500 dark:text-slate-400">{subValue}</p>}
      <p className="text-xs text-slate-400 dark:text-slate-500 mt-1">{weekRange}</p>
      <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
        <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
        <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mt-0.5 truncate" title={prevCompany}>
          {prevCompany} — <span className="text-slate-600 dark:text-slate-300">{prevValue}</span>
        </p>
      </div>
    </KpiCard>
  )
}

function RatioKpiCard({ label, iconColor, icon, ratio, wow, weekRange, prevRatio, prevWeekRange }) {
  const wowColor = wow >= 0 ? '#10b981' : '#ef4444'
  const wowText = (wow >= 0 ? '+' : '') + wow.toFixed(1) + 'pp'
  return (
    <KpiCard label={label} iconColor={iconColor} icon={icon}>
      <div className="flex items-baseline gap-2">
        <p className="text-3xl font-bold text-slate-900 dark:text-white">%{ratio.toFixed(1)}</p>
        <p className="text-sm font-semibold" style={{ color: wowColor }}>{wowText}</p>
      </div>
      <p className="text-xs text-slate-400 dark:text-slate-500 mt-1">{weekRange}</p>
      <div className="mt-3 pt-3 border-t border-slate-100 dark:border-white/6">
        <p className="text-xs text-slate-400 dark:text-slate-500">{prevWeekRange}</p>
        <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mt-0.5">
          %{prevRatio.toFixed(1)}
        </p>
      </div>
    </KpiCard>
  )
}

const ArrowInIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M5 12h14M12 5l7 7-7 7"/></svg>
const CoinIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><circle cx="12" cy="12" r="10"/><path d="M12 6v12M9 9h6M9 15h6"/></svg>
const NewIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>
const TransferIcon = <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M7 16V4m0 0L3 8m4-4l4 4M17 8v12m0 0l4-4m-4 4l-4-4"/></svg>

export default function CompanyKpiSection({ productGroup, onDefaultCompany, filter }) {
  const [data,    setData]    = useState(null)
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  const weekRange     = getLastWeekRange()
  const prevWeekRange = getPrevWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    companyApi.getCompanyKpi(productGroup, filter)
      .then(r => {
        setData(r.data)
        if (onDefaultCompany && r.data.defaultCompany) {
          onDefaultCompany(r.data.defaultCompany)
        }
      })
      .catch(() => setError('KPI verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <CompanyKpiSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data)   return null

  const fmtPrm = (n) => '₺' + Number(n).toLocaleString('tr-TR', { maximumFractionDigits: 0 })
  const fmtCnt = (n) => Number(n).toLocaleString('tr-TR') + ' Poliçe'

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <CompanyPremiumKpiCard
        label="En Çok Geçiş Gelen" iconColor="green" icon={ArrowInIcon}
        company={data.topCompanyByCount}
        value={fmtCnt(data.topCompanyCount)}
        weekRange={weekRange}
        prevCompany={data.prevTopCompanyByCount}
        prevValue={fmtCnt(data.prevTopCompanyCount)}
        prevWeekRange={prevWeekRange}
      />
      <CompanyPremiumKpiCard
        label="En Çok Prim Gelen" iconColor="blue" icon={CoinIcon}
        company={data.topCompanyByPremium}
        value={fmtPrm(data.topCompanyPremium)}
        weekRange={weekRange}
        prevCompany={data.prevTopCompanyByPremium}
        prevValue={fmtPrm(data.prevTopCompanyPremium)}
        prevWeekRange={prevWeekRange}
      />
      <RatioKpiCard
        label="Yeni İş Oranı" iconColor="amber" icon={NewIcon}
        ratio={data.newBusinessRatio}
        wow={data.newBusinessRatioWoW}
        weekRange={weekRange}
        prevRatio={data.prevNewBusinessRatio}
        prevWeekRange={prevWeekRange}
      />
      <RatioKpiCard
        label="Transfer Oranı" iconColor="purple" icon={TransferIcon}
        ratio={data.renewalRatio}
        wow={data.renewalRatioWoW}
        weekRange={weekRange}
        prevRatio={data.prevRenewalRatio}
        prevWeekRange={prevWeekRange}
      />
    </div>
  )
}