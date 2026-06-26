import { useState, useEffect, useCallback, useRef } from 'react'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import * as cityApi from '../../api/cityApi'
import { SkeletonBlock } from '../ui/Skeleton'

const CHART_COLOR = '#10b981'

const TrendSkeleton = () => (
  <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 space-y-4">
    <div className="flex justify-between items-center">
      <SkeletonBlock width="w-40" height="h-4" />
      <SkeletonBlock width="w-36" height="h-8" rounded="rounded-lg" />
    </div>
    <SkeletonBlock width="w-full" height="h-40" />
    <div className="grid grid-cols-3 gap-3">
      {[1,2,3].map(i => <SkeletonBlock key={i} width="w-full" height="h-12" rounded="rounded-lg" />)}
    </div>
    <div className="grid grid-cols-2 gap-3">
      {[1,2,3,4].map(i => <SkeletonBlock key={i} width="w-full" height="h-16" rounded="rounded-lg" />)}
    </div>
  </div>
)

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

function CitySearchDropdown({ cityList, selectedCity, onChange }) {
  const [query, setQuery] = useState('')
  const [open,  setOpen]  = useState(false)
  const ref = useRef(null)

  const filtered = cityList.filter(c =>
    c.city.toLowerCase().includes(query.toLowerCase())
  ).slice(0, 50)

  useEffect(() => {
    const handleClick = (e) => {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false)
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  const handleSelect = (city) => {
    onChange(city)
    setQuery('')
    setOpen(false)
  }

  return (
    <div ref={ref} className="relative">
      <div
        className="flex items-center gap-2 px-3 py-1.5 bg-slate-50 dark:bg-white/8 border border-slate-200 dark:border-white/10 rounded-lg cursor-text min-w-[160px]"
        onClick={() => setOpen(true)}
      >
        {open ? (
          <input
            autoFocus
            value={query}
            onChange={e => setQuery(e.target.value)}
            placeholder="İl ara..."
            className="text-xs font-semibold text-slate-700 dark:text-slate-200 bg-transparent outline-none w-full placeholder:text-slate-400"
          />
        ) : (
          <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate flex-1">
            {selectedCity || 'İl seç'}
          </span>
        )}
        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-400 flex-shrink-0">
          <polyline points="6 9 12 15 18 9" />
        </svg>
      </div>

      {open && (
        <div className="absolute right-0 top-full mt-1 w-56 bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-xl shadow-xl z-50 overflow-hidden">
          <div className="max-h-48 overflow-y-auto">
            {filtered.length === 0 ? (
              <p className="text-xs text-slate-400 text-center py-4">Sonuç bulunamadı</p>
            ) : (
              filtered.map(c => (
                <button
                  key={c.city}
                  onClick={() => handleSelect(c.city)}
                  className={`w-full text-left px-4 py-2.5 text-xs font-medium transition-colors hover:bg-slate-50 dark:hover:bg-white/5 flex items-center justify-between gap-2 ${
                    c.city === selectedCity
                      ? 'text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-500/10'
                      : 'text-slate-700 dark:text-slate-200'
                  }`}
                >
                  <span>{c.city}</span>
                  <span className="text-slate-400 text-xs">₺{(c.netPremium / 1_000_000).toFixed(1)}M</span>
                </button>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  )
}

export default function CityTrendSection({ productGroup, defaultCity, onCityChange, cityList = [], filter }) {
  const [selectedCity, setSelectedCity] = useState('')
  const [trendData,    setTrendData]    = useState([])
  const [profileData,  setProfileData]  = useState(null)
  const [loading,      setLoading]      = useState(true)
  const [error,        setError]        = useState('')

  useEffect(() => {
    if (defaultCity && !selectedCity) {
      setSelectedCity(defaultCity)
      onCityChange?.(defaultCity)
    }
  }, [defaultCity])

  const loadData = useCallback((city) => {
    if (!city) return
    setLoading(true)
    setError('')
    Promise.all([
      cityApi.getCityTrend(productGroup, city, filter),
      cityApi.getCityProfile(productGroup, city, filter),
    ])
      .then(([trendRes, profileRes]) => {
        setTrendData(trendRes.data)
        setProfileData(profileRes.data)
      })
      .catch(() => setError('Veriler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  useEffect(() => {
    if (selectedCity) loadData(selectedCity)
  }, [selectedCity, loadData])

  const handleCityChange = (city) => {
    setSelectedCity(city)
    onCityChange?.(city)
  }

  const lastWeek  = trendData[trendData.length - 1]
  const totalPrem = trendData.reduce((s, d) => s + d.netPremium, 0)
  const totalCount = trendData.reduce((s, d) => s + d.policyCount, 0)
  const lastWoW   = lastWeek?.wow ?? 0

  const chartData = trendData.map(d => ({ week: d.weekLabel, prim: d.netPremium }))

  const topBrands     = profileData?.topBrands ?? []
  const sigortaliTuru = (profileData?.profile ?? []).filter(x => x.category === 'SigortaliTuru')
  const aracYasi      = (profileData?.profile ?? []).filter(x => x.category === 'AracYasi').sort((a, b) => a.type.localeCompare(b.type))
  const basamak       = (profileData?.profile ?? []).filter(x => x.category === 'Basamak').sort((a, b) => Number(a.type) - Number(b.type))
  const totalSigortali = sigortaliTuru.reduce((s, x) => s + x.policyCount, 0)

  if (loading && !trendData.length) return <TrendSkeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 space-y-5">

      {/* Header */}
      <div className="flex items-center justify-between">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">Haftalık Net Prim Trendi</p>
        <CitySearchDropdown cityList={cityList} selectedCity={selectedCity} onChange={handleCityChange} />
      </div>

      {/* Grafik */}
      <ResponsiveContainer width="100%" height={180}>
        <LineChart data={chartData} margin={{ top: 4, right: 4, left: -10, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} />
          <XAxis dataKey="week" tick={{ fontSize: 9, fill: '#94a3b8' }} tickLine={false} axisLine={false} />
          <YAxis tick={{ fontSize: 9, fill: '#94a3b8' }} tickLine={false} axisLine={false}
            tickFormatter={v => v >= 1_000_000 ? `₺${(v/1_000_000).toFixed(1)}M` : `₺${(v/1000).toFixed(0)}K`} />
          <Tooltip content={<CustomTooltip />} />
          <Line
            type="monotone" dataKey="prim" stroke={CHART_COLOR}
            strokeWidth={2} dot={{ r: 3, fill: CHART_COLOR, strokeWidth: 2, stroke: 'white' }}
            activeDot={{ r: 5 }}
          />
        </LineChart>
      </ResponsiveContainer>

      {/* Özet */}
      <div className="grid grid-cols-3 gap-3 pt-2 border-t border-slate-100 dark:border-white/6">
        <div className="text-center">
          <p className="text-xs text-slate-400 dark:text-slate-500">Net Prim</p>
          <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">
            ₺{Number(totalPrem).toLocaleString('tr-TR', { maximumFractionDigits: 0 })}
          </p>
        </div>
        <div className="text-center border-x border-slate-100 dark:border-white/6">
          <p className="text-xs text-slate-400 dark:text-slate-500">Toplam Poliçe</p>
          <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">
            {totalCount.toLocaleString('tr-TR')}
          </p>
        </div>
        <div className="text-center">
          <p className="text-xs text-slate-400 dark:text-slate-500">Son Hafta WoW</p>
          <p className="text-sm font-bold mt-1" style={{ color: lastWoW > 0 ? '#10b981' : lastWoW < 0 ? '#ef4444' : '#94a3b8' }}>
            {lastWoW > 0 ? '+' : ''}{lastWoW}%
          </p>
        </div>
      </div>

      {/* Profil */}
      <div className="pt-2 border-t border-slate-100 dark:border-white/6">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">
          İl Profili — {selectedCity}
        </p>

        <div className="grid grid-cols-2 gap-x-8 gap-y-5">

          {/* Top 3 Marka */}
          {topBrands.length > 0 && (
            <div>
              <p className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide mb-2">Top 3 Marka</p>
              <div className="space-y-1.5">
                {topBrands.map((b, i) => (
                  <div key={b.brand} className="flex items-center justify-between py-1.5 border-b border-slate-50 dark:border-white/4">
                    <div className="flex items-center gap-2">
                      <span className="text-xs font-bold text-slate-400 w-4">{i + 1}.</span>
                      <span className="text-xs font-bold text-slate-800 dark:text-slate-200">{b.brand}</span>
                    </div>
                    <span className="text-xs text-slate-500 dark:text-slate-400">
                      {Number(b.policyCount).toLocaleString('tr-TR')} Poliçe
                    </span>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Sigortalı Türü */}
          {sigortaliTuru.length > 0 && (
            <div>
              <p className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide mb-2">Sigortalı Türü</p>
              <div className="space-y-2">
                {sigortaliTuru.map(item => {
                  const ratio = totalSigortali > 0 ? item.policyCount / totalSigortali * 100 : 0
                  return (
                    <div key={item.type} className="space-y-1">
                      <div className="flex justify-between text-xs">
                        <span className="font-medium text-slate-700 dark:text-slate-300">{item.type}</span>
                        <span className="text-slate-400">%{ratio.toFixed(1)}</span>
                      </div>
                      <div className="h-1.5 bg-slate-100 dark:bg-white/8 rounded-full overflow-hidden">
                        <div className="h-full bg-emerald-500 rounded-full" style={{ width: `${ratio}%` }} />
                      </div>
                    </div>
                  )
                })}
              </div>
            </div>
          )}

          {/* Araç Yaşı */}
          {aracYasi.length > 0 && (
            <div>
              <p className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide mb-2">Araç Yaşı Dağılımı</p>
              <div className="flex flex-wrap gap-1.5">
                {aracYasi.map(item => (
                  <div key={item.type} className="flex items-center gap-1 px-2 py-1 bg-slate-50 dark:bg-white/5 rounded-lg">
                    <span className="text-xs font-semibold text-slate-700 dark:text-slate-300">{item.type}</span>
                    <span className="text-xs text-slate-400">{Number(item.policyCount).toLocaleString('tr-TR')}</span>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Basamak */}
          {basamak.length > 0 && (
            <div>
              <p className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide mb-2">Basamak Dağılımı</p>
              <div className="flex flex-wrap gap-1.5">
                {basamak.map(item => (
                  <div key={item.type} className="flex items-center gap-1 px-2 py-1 bg-slate-50 dark:bg-white/5 rounded-lg">
                    <span className="text-xs font-semibold text-slate-700 dark:text-slate-300">{item.type}.</span>
                    <span className="text-xs text-slate-400">{Number(item.policyCount).toLocaleString('tr-TR')}</span>
                  </div>
                ))}
              </div>
            </div>
          )}

        </div>
      </div>
    </div>
  )
}