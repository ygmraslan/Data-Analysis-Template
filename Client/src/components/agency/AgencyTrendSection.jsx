import { useState, useEffect, useCallback, useRef } from 'react'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import * as agencyApi from '../../api/agencyApi'
import { getEightWeekRange } from '../../utils/formatDate'
import { SkeletonBlock } from '../ui/Skeleton'

const TrendSkeleton = () => (
  <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
    <div className="flex items-center justify-between mb-4">
      <SkeletonBlock width="w-40" height="h-4" />
      <SkeletonBlock width="w-48" height="h-8" rounded="rounded-lg" />
    </div>
    <div className="grid grid-cols-1 lg:grid-cols-5 gap-5">
      <div className="lg:col-span-2 space-y-3">
        <SkeletonBlock width="w-full" height="h-[160px]" rounded="rounded-lg" />
        <div className="flex gap-3">
          {[1,2,3].map(i => <SkeletonBlock key={i} width="w-full" height="h-12" rounded="rounded-lg" />)}
        </div>
      </div>
      <div className="lg:col-span-3 grid grid-cols-2 gap-4">
        {[1,2,3,4].map(i => <SkeletonBlock key={i} width="w-full" height="h-20" rounded="rounded-lg" />)}
      </div>
    </div>
  </div>
)

// Custom Tooltip - diğer sayfalarla uyumlu
const CustomTooltip = ({ active, payload, label }) => {
  if (!active || !payload?.length) return null
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg">
      <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mb-1">{label}</p>
      <p className="text-sm font-bold text-slate-900 dark:text-white">
        ₺{Number(payload[0].value).toLocaleString('tr-TR', { maximumFractionDigits: 0 })}
      </p>
    </div>
  )
}

function AgencySearchDropdown({ agencyList, selectedAgency, onChange }) {
  const [open, setOpen] = useState(false)
  const [search, setSearch] = useState('')
  const ref = useRef(null)

  useEffect(() => {
    const handleClick = (e) => {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false)
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  const filtered = agencyList.filter(a =>
    a.agencyName.toLowerCase().includes(search.toLowerCase()) ||
    a.agencyCode.toLowerCase().includes(search.toLowerCase())
  )

  const selected = agencyList.find(a => a.agencyCode === selectedAgency)

  return (
    <div ref={ref} className="relative">
      <button
        onClick={() => setOpen(!open)}
        className="flex items-center gap-2 px-3 py-1.5 bg-slate-50 dark:bg-white/8 border border-slate-200 dark:border-white/10 rounded-lg min-w-[200px]"
      >
        <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate flex-1 text-left">
          {selected?.agencyName || 'Acente seç'}
        </span>
        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-400 flex-shrink-0">
          <polyline points="6 9 12 15 18 9" />
        </svg>
      </button>

      {open && (
        <div className="absolute right-0 top-full mt-1 w-72 bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-xl shadow-xl z-50 overflow-hidden">
          <div className="p-2 border-b border-slate-100 dark:border-white/8">
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Acente ara..."
              className="w-full px-3 py-1.5 text-xs bg-slate-50 dark:bg-white/5 border border-slate-200 dark:border-white/10 rounded-lg focus:outline-none focus:ring-1 focus:ring-emerald-500 text-slate-700 dark:text-slate-200"
              autoFocus
            />
          </div>
          <div className="max-h-64 overflow-y-auto">
            {filtered.length === 0 ? (
              <p className="text-xs text-slate-400 text-center py-4">Sonuç bulunamadı</p>
            ) : (
              filtered.slice(0, 50).map(a => (
                <button
                  key={a.agencyCode}
                  onClick={() => { onChange(a.agencyCode); setOpen(false); setSearch('') }}
                  className={`w-full text-left px-3 py-2 text-xs font-medium transition-colors hover:bg-slate-50 dark:hover:bg-white/5 ${
                    a.agencyCode === selectedAgency
                      ? 'text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-500/10'
                      : 'text-slate-700 dark:text-slate-200'
                  }`}
                >
                  <span className="block truncate">{a.agencyName}</span>
                  <span className="text-[10px] text-slate-400">{a.agencyCode}</span>
                </button>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  )
}

function ProfileBar({ label, ratio, color }) {
  const pct = Math.min(100, Math.max(0, ratio)) // ratio zaten 0-100 arası
  return (
    <div>
      <div className="flex items-center justify-between mb-1">
        <span className="text-[10px] font-medium text-slate-600 dark:text-slate-300 truncate">{label}</span>
        <span className="text-[10px] font-bold text-slate-500 dark:text-slate-400">%{pct.toFixed(0)}</span>
      </div>
      <div className="h-1.5 bg-slate-100 dark:bg-white/10 rounded-full overflow-hidden">
        <div className="h-full rounded-full transition-all duration-500" style={{ width: `${pct}%`, backgroundColor: color }} />
      </div>
    </div>
  )
}

// Basamak için kompakt liste görünümü
function BasamakList({ items }) {
  if (!items?.length) return null
  return (
    <div className="flex flex-wrap gap-x-4 gap-y-1">
      {items.map(item => (
        <div key={item.type} className="flex items-center gap-1.5">
          <span className="text-[10px] font-medium text-slate-600 dark:text-slate-300">Basamak {item.type}:</span>
          <span className="text-[10px] font-bold text-amber-500">%{Number(item.ratio).toFixed(0)}</span>
        </div>
      ))}
    </div>
  )
}

export default function AgencyTrendSection({ productGroup, selectedAgency, onAgencyChange, filter }) {
  const [agencyList, setAgencyList] = useState([])
  const [trendData, setTrendData] = useState([])
  const [profileData, setProfileData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [currentAgency, setCurrentAgency] = useState(selectedAgency || '')

  const weekRange = getEightWeekRange()

  // selectedAgency prop değişince currentAgency'yi güncelle
  useEffect(() => {
    if (selectedAgency && selectedAgency !== currentAgency) {
      setCurrentAgency(selectedAgency)
    }
  }, [selectedAgency])

  // Acente listesini yükle
  useEffect(() => {
    agencyApi.getAgencyList(productGroup, 1, 500, null, filter)
      .then(r => {
        const list = r.data?.items || []
        setAgencyList(list)
        // Eğer currentAgency boşsa ve liste varsa, ilk acenteyi seç
        if (!currentAgency && list.length > 0) {
          setCurrentAgency(list[0].agencyCode)
        }
      })
      .catch(() => setAgencyList([]))
  }, [productGroup, filter])

  // Trend ve profil verilerini yükle
  const loadData = useCallback(async (agencyCode) => {
    if (!agencyCode) return
    setLoading(true)
    setError('')
    try {
      const [trendRes, profileRes] = await Promise.all([
        agencyApi.getAgencyTrend(productGroup, agencyCode, filter),
        agencyApi.getAgencyProfile(productGroup, agencyCode, filter)
      ])
      setTrendData(trendRes.data?.items || [])
      setProfileData(profileRes.data || null)
    } catch {
      setError('Trend verileri yüklenemedi.')
    } finally {
      setLoading(false)
    }
  }, [productGroup, filter])

  useEffect(() => {
    if (currentAgency) loadData(currentAgency)
  }, [currentAgency, loadData])

  const handleAgencyChange = (agencyCode) => {
    setCurrentAgency(agencyCode)
    onAgencyChange?.(agencyCode)
  }

  const totalPrem  = trendData.reduce((s, d) => s + d.netPremium, 0)
  const totalCount = trendData.reduce((s, d) => s + d.policyCount, 0)
  const avgPremium = totalCount > 0 ? totalPrem / totalCount : 0

  // Grafik için week label'ı kullan (tarih formatı backend'den geliyor)
  const chartData = trendData.map(d => ({ week: d.week, prim: d.netPremium }))

  const topBrands     = profileData?.topBrands ?? []
  const sigortaliTuru = profileData?.sigortaliTuru ?? []
  const yenilemeTipi  = profileData?.yenilemeTipi ?? []
  const basamak       = profileData?.basamak ?? []

  const fmtPrm = (n) => '₺' + Number(n).toLocaleString('tr-TR', { maximumFractionDigits: 0 })

  if (loading && !trendData.length) return <TrendSkeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">

      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <div>
          <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">Acente Trend & Profil</p>
          <p className="text-xs text-slate-400 mt-0.5">{weekRange}</p>
        </div>
        <AgencySearchDropdown agencyList={agencyList} selectedAgency={currentAgency} onChange={handleAgencyChange} />
      </div>

      {/* Main Content: Sol Grafik, Sağ Profil */}
      <div className="grid grid-cols-1 lg:grid-cols-5 gap-4">
        
        {/* SOL - Grafik + Özet (2/5) */}
        <div className="lg:col-span-2 space-y-3">
          {/* Grafik */}
          {chartData.length > 0 && (
            <div className="h-[160px]">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData} margin={{ top: 5, right: 5, left: -20, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="rgba(148,163,184,0.15)" />
                  <XAxis 
                    dataKey="week" 
                    tick={{ fontSize: 9, fill: '#94a3b8' }} 
                    axisLine={{ stroke: 'rgba(148,163,184,0.2)' }}
                    tickLine={false}
                    interval={0}
                  />
                  <YAxis 
                    tick={{ fontSize: 9, fill: '#94a3b8' }} 
                    axisLine={{ stroke: 'rgba(148,163,184,0.2)' }}
                    tickLine={false}
                    tickFormatter={(v) => v >= 1_000_000 ? (v/1_000_000).toFixed(0)+'M' : v >= 1_000 ? (v/1_000).toFixed(0)+'K' : v}
                  />
                  <Tooltip content={<CustomTooltip />} />
                  <Line 
                    type="monotone" 
                    dataKey="prim" 
                    stroke="#10b981" 
                    strokeWidth={2}
                    dot={{ fill: '#10b981', strokeWidth: 0, r: 3 }}
                    activeDot={{ r: 5, fill: '#10b981' }}
                  />
                </LineChart>
              </ResponsiveContainer>
            </div>
          )}

          {/* Mini Stat Row */}
          <div className="flex items-stretch divide-x divide-slate-100 dark:divide-white/8 bg-slate-50 dark:bg-white/5 rounded-lg">
            <div className="flex-1 py-2 px-3 text-center">
              <p className="text-[10px] text-slate-400 uppercase">Toplam Prim</p>
              <p className="text-sm font-bold text-slate-800 dark:text-white mt-0.5">{fmtPrm(totalPrem)}</p>
            </div>
            <div className="flex-1 py-2 px-3 text-center">
              <p className="text-[10px] text-slate-400 uppercase">Poliçe</p>
              <p className="text-sm font-bold text-slate-800 dark:text-white mt-0.5">{totalCount.toLocaleString('tr-TR')}</p>
            </div>
            <div className="flex-1 py-2 px-3 text-center">
              <p className="text-[10px] text-slate-400 uppercase">Ort. Prim</p>
              <p className="text-sm font-bold text-slate-800 dark:text-white mt-0.5">{fmtPrm(avgPremium)}</p>
            </div>
          </div>
        </div>

        {/* SAĞ - Profil (3/5) */}
        <div className="lg:col-span-3">
          {profileData && (
            <div className="grid grid-cols-2 gap-x-4 gap-y-3">

              {/* Top 3 Marka */}
              {topBrands.length > 0 && (
                <div>
                  <p className="text-[10px] font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide mb-1.5">Top 3 Marka</p>
                  <div className="space-y-0.5">
                    {topBrands.slice(0, 3).map((b, i) => (
                      <div key={b.brand} className="flex items-center justify-between text-xs py-0.5">
                        <div className="flex items-center gap-1">
                          <span className="font-bold text-slate-400 w-3">{i + 1}.</span>
                          <span className="font-semibold text-slate-800 dark:text-slate-200">{b.brand}</span>
                        </div>
                        <span className="text-slate-400 text-[10px]">{Number(b.policyCount).toLocaleString('tr-TR')}</span>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* Yenileme Tipi */}
              {yenilemeTipi.length > 0 && (
                <div>
                  <p className="text-[10px] font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide mb-1.5">Yenileme Tipi</p>
                  <div className="space-y-1.5">
                    {yenilemeTipi.map(item => (
                      <ProfileBar key={item.type} label={item.type} ratio={item.ratio} color="#8b5cf6" />
                    ))}
                  </div>
                </div>
              )}

              {/* Sigortalı Türü */}
              {sigortaliTuru.length > 0 && (
                <div>
                  <p className="text-[10px] font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide mb-1.5">Sigortalı Türü</p>
                  <div className="space-y-1.5">
                    {sigortaliTuru.map(item => (
                      <ProfileBar key={item.type} label={item.type} ratio={item.ratio} color="#10b981" />
                    ))}
                  </div>
                </div>
              )}

              {/* Basamak - Kompakt liste */}
              {basamak.length > 0 && (
                <div>
                  <p className="text-[10px] font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide mb-1.5">Basamak Dağılımı</p>
                  <BasamakList items={basamak} />
                </div>
              )}

            </div>
          )}
        </div>

      </div>
    </div>
  )
}