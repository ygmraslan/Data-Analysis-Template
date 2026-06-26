import { useState, useEffect, useRef } from 'react'
import * as agencyApi from '../../api/agencyApi'
import { getLastWeekRange } from '../../utils/formatDate'
import { SkeletonBlock, SkeletonCard } from '../ui/Skeleton'

// Para formatı - TAM gösterim
const formatMoney = (n) => {
  return '₺' + Number(n).toLocaleString('tr-TR', { maximumFractionDigits: 0 })
}

const RegionSkeleton = () => (
  <SkeletonCard>
    <div className="flex items-center justify-between mb-4">
      <SkeletonBlock width="w-36" height="h-4" />
      <SkeletonBlock width="w-40" height="h-8" rounded="rounded-lg" />
    </div>
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
      <div>
        <SkeletonBlock width="w-full" height="h-[320px]" rounded="rounded-xl" />
      </div>
      <div className="lg:col-span-2">
        <SkeletonBlock width="w-full" height="h-[320px]" rounded="rounded-lg" />
      </div>
    </div>
  </SkeletonCard>
)

function RegionDropdown({ regions, selectedRegion, onChange }) {
  const [open, setOpen] = useState(false)
  const ref = useRef(null)

  useEffect(() => {
    const handleClick = (e) => {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false)
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  const selected = regions.find(r => r.region === selectedRegion)

  return (
    <div ref={ref} className="relative">
      <button
        onClick={() => setOpen(!open)}
        className="flex items-center gap-2 px-3 py-1.5 bg-slate-50 dark:bg-white/8 border border-slate-200 dark:border-white/10 rounded-lg min-w-[180px] hover:border-emerald-300 dark:hover:border-emerald-500/30 transition-colors"
      >
        <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate flex-1 text-left">
          {selected?.region || 'Bölge seç'}
        </span>
        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-400 flex-shrink-0">
          <polyline points="6 9 12 15 18 9" />
        </svg>
      </button>

      {open && (
        <div className="absolute right-0 top-full mt-1 w-56 bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-xl shadow-xl z-50 overflow-hidden">
          <div className="max-h-64 overflow-y-auto">
            {regions.map(r => (
              <button
                key={r.region}
                onClick={() => { onChange(r.region); setOpen(false) }}
                className={`w-full text-left px-3 py-2 text-xs font-medium transition-colors hover:bg-slate-50 dark:hover:bg-white/5 flex items-center justify-between gap-2 ${
                  r.region === selectedRegion
                    ? 'text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-500/10'
                    : 'text-slate-700 dark:text-slate-200'
                }`}
              >
                <span className="truncate">{r.region}</span>
                <span className="text-slate-400 text-[10px]">%{Number(r.ratio).toFixed(1)}</span>
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}

// Mini Donut/Pie Chart Component
function MiniDonut({ percentage, size = 60, strokeWidth = 8, color = '#10b981' }) {
  const radius = (size - strokeWidth) / 2
  const circumference = 2 * Math.PI * radius
  const offset = circumference - (percentage / 100) * circumference
  
  return (
    <div className="relative" style={{ width: size, height: size }}>
      <svg width={size} height={size} className="transform -rotate-90">
        {/* Background circle */}
        <circle
          cx={size / 2}
          cy={size / 2}
          r={radius}
          fill="none"
          stroke="currentColor"
          strokeWidth={strokeWidth}
          className="text-slate-200 dark:text-white/10"
        />
        {/* Progress circle */}
        <circle
          cx={size / 2}
          cy={size / 2}
          r={radius}
          fill="none"
          stroke={color}
          strokeWidth={strokeWidth}
          strokeLinecap="round"
          strokeDasharray={circumference}
          strokeDashoffset={offset}
          className="transition-all duration-500"
        />
      </svg>
      {/* Center text */}
      <div className="absolute inset-0 flex items-center justify-center">
        <span className="text-xs font-bold text-slate-800 dark:text-white">%{percentage.toFixed(0)}</span>
      </div>
    </div>
  )
}

// Progress Bar Component
function MiniProgress({ value, max, color = '#10b981' }) {
  const percentage = max > 0 ? (value / max) * 100 : 0
  return (
    <div className="w-full h-2 bg-slate-200 dark:bg-white/10 rounded-full overflow-hidden">
      <div 
        className="h-full rounded-full transition-all duration-500"
        style={{ width: `${Math.min(percentage, 100)}%`, backgroundColor: color }}
      />
    </div>
  )
}

// İkon componentleri
const LocationIcon = () => (
  <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="1.5">
    <path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z" />
    <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z" />
  </svg>
)

const TrendUpIcon = () => (
  <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5">
    <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 19.5l15-15m0 0H8.25m11.25 0v11.25" />
  </svg>
)

const TrendDownIcon = () => (
  <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5">
    <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 4.5l15 15m0 0V8.25m0 11.25H8.25" />
  </svg>
)

export default function AgencyRegionSection({ productGroup, selectedAgency, onAgencySelect, filter }) {
  const [regionData, setRegionData] = useState([])
  const [selectedRegion, setSelectedRegion] = useState('')
  const [agencyList, setAgencyList] = useState([])
  const [loading, setLoading] = useState(true)
  const [agencyLoading, setAgencyLoading] = useState(false)
  const [error, setError] = useState('')

  const weekRange = getLastWeekRange()

  // Bölge verilerini yükle
  useEffect(() => {
    setLoading(true)
    setError('')
    agencyApi.getAgencyRegion(productGroup, filter)
      .then(r => {
        const items = r.data?.items || []
        setRegionData(items)
        if (items.length > 0) {
          setSelectedRegion(items[0].region)
        }
      })
      .catch(() => setError('Bölge verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  // Seçili bölgenin acentelerini yükle
  useEffect(() => {
    if (!selectedRegion) return
    setAgencyLoading(true)
    agencyApi.getAgencyList(productGroup, 1, 10, selectedRegion, filter)
      .then(r => setAgencyList(r.data?.items || []))
      .catch(() => setAgencyList([]))
      .finally(() => setAgencyLoading(false))
  }, [productGroup, selectedRegion, filter])

  if (loading) return <RegionSkeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!regionData.length) return null

  const currentRegion = regionData.find(r => r.region === selectedRegion)
  const avgPremium = currentRegion ? currentRegion.netPremium / currentRegion.policyCount : 0
  const wowChange = currentRegion?.wowChange || 0
  const isPositive = wowChange >= 0

  // Toplam poliçe sayısı (tüm bölgeler)
  const totalPolicies = regionData.reduce((sum, r) => sum + r.policyCount, 0)
  const policyPercentage = totalPolicies > 0 ? (currentRegion?.policyCount / totalPolicies) * 100 : 0

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <div>
          <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">Bölge Dağılımı</p>
          <p className="text-xs text-slate-400 mt-0.5">{weekRange}</p>
        </div>
        <RegionDropdown regions={regionData} selectedRegion={selectedRegion} onChange={setSelectedRegion} />
      </div>

      {/* Main Content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
        
        {/* SOL - Bölge Özet Kartı */}
        <div>
          {currentRegion && (
            <div className="bg-gradient-to-br from-slate-50 to-slate-100/50 dark:from-slate-800/50 dark:to-slate-900/30 rounded-xl border border-slate-200/50 dark:border-white/5 p-5 flex flex-col justify-between min-h-[380px]">
              
              {/* Bölge Başlığı */}
              <div className="flex items-center gap-3 pb-4 border-b border-slate-200/70 dark:border-white/10">
                <div className="w-10 h-10 rounded-lg bg-emerald-500 flex items-center justify-center text-white shadow-lg shadow-emerald-500/25">
                  <LocationIcon />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-base font-bold text-slate-800 dark:text-white truncate" title={currentRegion.region}>
                    {currentRegion.region}
                  </p>
                  <div className={`inline-flex items-center gap-1 text-xs font-bold ${
                    isPositive ? 'text-emerald-500' : 'text-red-500'
                  }`}>
                    {isPositive ? <TrendUpIcon /> : <TrendDownIcon />}
                    {isPositive ? '+' : ''}{wowChange.toFixed(1)}% WoW
                  </div>
                </div>
              </div>

              {/* Toplam Prim */}
              <div className="py-5 border-b border-slate-200/50 dark:border-white/5">
                <p className="text-[10px] font-medium text-slate-400 uppercase tracking-wider mb-1">Toplam Prim</p>
                <p className="text-2xl font-bold text-slate-900 dark:text-white">{formatMoney(currentRegion.netPremium)}</p>
              </div>

              {/* Ort. Prim */}
              <div className="py-5 border-b border-slate-200/50 dark:border-white/5">
                <p className="text-[10px] font-medium text-slate-400 uppercase tracking-wider mb-1">Ort. Prim</p>
                <p className="text-2xl font-bold text-slate-700 dark:text-slate-200">{formatMoney(avgPremium)}</p>
              </div>

              {/* Prim Payı ve Poliçe yan yana */}
              <div className="grid grid-cols-2 gap-4 pt-5">
                {/* Prim Payı - Mini Donut */}
                <div className="flex items-center gap-3">
                  <MiniDonut percentage={Number(currentRegion.ratio)} size={44} strokeWidth={5} color="#10b981" />
                  <div>
                    <p className="text-[10px] font-medium text-slate-400 uppercase tracking-wider">Prim Payı</p>
                    <p className="text-lg font-bold text-emerald-600 dark:text-emerald-400">%{Number(currentRegion.ratio).toFixed(1)}</p>
                  </div>
                </div>

                {/* Poliçe */}
                <div>
                  <p className="text-[10px] font-medium text-slate-400 uppercase tracking-wider mb-1">Poliçe</p>
                  <p className="text-lg font-bold text-slate-800 dark:text-white">{Number(currentRegion.policyCount).toLocaleString('tr-TR')}</p>
                  <MiniProgress value={currentRegion.policyCount} max={totalPolicies} color="#10b981" />
                </div>
              </div>
            </div>
          )}
        </div>

        {/* SAĞ - Bölgedeki Top 10 Acente Tablosu */}
        <div className="lg:col-span-2">
          {agencyLoading ? (
            <div className="space-y-2">
              {[1,2,3,4,5].map(i => <SkeletonBlock key={i} width="w-full" height="h-9" rounded="rounded" />)}
            </div>
          ) : agencyList.length === 0 ? (
            <div className="flex items-center justify-center h-full min-h-[200px]">
              <p className="text-xs text-slate-400">Bu bölgede acente bulunamadı</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-xs">
                <thead>
                  <tr className="border-b border-slate-200 dark:border-white/10">
                    <th className="text-left pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider text-[10px] w-8">#</th>
                    <th className="text-left pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider text-[10px]">Acente</th>
                    <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider text-[10px] w-16">Poliçe</th>
                    <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider text-[10px] w-28">Net Prim</th>
                    <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider text-[10px] w-24">Ort.</th>
                    <th className="text-right pb-2.5 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider text-[10px] w-16">WoW</th>
                  </tr>
                </thead>
                <tbody>
                  {agencyList.map((row, i) => {
                    const isSelected = row.agencyCode === selectedAgency
                    const rowWow = Number(row.wowChange || 0)
                    const wowPositive = rowWow >= 0
                    return (
                      <tr
                        key={row.agencyCode}
                        onClick={() => onAgencySelect?.(row.agencyCode)}
                        className={`border-b border-slate-100 dark:border-white/5 cursor-pointer transition-colors ${
                          isSelected 
                            ? 'bg-emerald-50 dark:bg-emerald-500/10' 
                            : 'hover:bg-slate-50 dark:hover:bg-white/3'
                        }`}
                      >
                        <td className="py-2.5 text-slate-400 dark:text-slate-500 font-medium">{i + 1}</td>
                        <td className="py-2.5 pr-2">
                          <span 
                            className={`font-semibold ${
                              isSelected ? 'text-emerald-600 dark:text-emerald-400' : 'text-slate-800 dark:text-slate-200'
                            }`}
                            title={row.agencyName}
                          >
                            {row.agencyName}
                          </span>
                        </td>
                        <td className="py-2.5 text-right text-slate-600 dark:text-slate-300 tabular-nums">
                          {Number(row.policyCount).toLocaleString('tr-TR')}
                        </td>
                        <td className="py-2.5 text-right font-semibold text-slate-700 dark:text-slate-200 tabular-nums">
                          {formatMoney(row.netPremium)}
                        </td>
                        <td className="py-2.5 text-right text-slate-500 dark:text-slate-400 tabular-nums">
                          {formatMoney(row.avgPremium)}
                        </td>
                        <td className="py-2.5 text-right">
                          <span className={`font-bold tabular-nums ${
                            wowPositive ? 'text-emerald-500' : 'text-red-500'
                          }`}>
                            {wowPositive ? '+' : ''}{rowWow.toFixed(1)}%
                          </span>
                        </td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}