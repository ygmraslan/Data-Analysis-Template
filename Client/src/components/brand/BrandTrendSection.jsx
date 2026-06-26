import { useState, useEffect, useCallback, useRef } from 'react'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import * as brandApi from '../../api/brandApi'
import { SkeletonBlock } from '../ui/Skeleton'

const CHART_COLOR = '#10b981'

const TrendSkeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <div className="flex justify-between items-center mb-4">
        <SkeletonBlock width="w-40" height="h-4" />
        <SkeletonBlock width="w-32" height="h-8" rounded="rounded-lg" />
      </div>
      <SkeletonBlock width="w-full" height="h-40" className="mb-4" />
      <div className="grid grid-cols-3 gap-3">
        {[1,2,3].map(i => <SkeletonBlock key={i} width="w-full" height="h-12" rounded="rounded-lg" />)}
      </div>
    </div>
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
      {[1,2,3,4,5,6,7,8].map(i => <SkeletonBlock key={i} width="w-full" height="h-8" className="mb-2" />)}
    </div>
  </div>
)

const CustomTooltip = ({ active, payload, label }) => {
  if (!active || !payload?.length) return null
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg">
      <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 mb-1">{label}</p>
      <p className="text-sm font-bold text-slate-900 dark:text-white">
        {Number(payload[0].value).toLocaleString('tr-TR')} Poliçe
      </p>
    </div>
  )
}

function BrandSearchDropdown({ brandList, selectedBrand, onChange }) {
  const [query,    setQuery]    = useState('')
  const [open,     setOpen]     = useState(false)
  const ref = useRef(null)

  const filtered = brandList.filter(b =>
    b.brand.toLowerCase().includes(query.toLowerCase())
  ).slice(0, 50)

  useEffect(() => {
    const handleClick = (e) => {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false)
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  const handleSelect = (brand) => {
    onChange(brand)
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
            placeholder="Marka ara..."
            className="text-xs font-semibold text-slate-700 dark:text-slate-200 bg-transparent outline-none w-full placeholder:text-slate-400"
          />
        ) : (
          <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate flex-1">
            {selectedBrand || 'Marka seç'}
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
              filtered.map(b => (
                <button
                  key={b.brand}
                  onClick={() => handleSelect(b.brand)}
                  className={`w-full text-left px-4 py-2.5 text-xs font-medium transition-colors hover:bg-slate-50 dark:hover:bg-white/5 flex items-center justify-between gap-2 ${
                    b.brand === selectedBrand
                      ? 'text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-500/10'
                      : 'text-slate-700 dark:text-slate-200'
                  }`}
                >
                  <span>{b.brand}</span>
                  <span className="text-slate-400 dark:text-slate-500 text-xs">
                    {Number(b.policyCount).toLocaleString('tr-TR')}
                  </span>
                </button>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  )
}

export default function BrandTrendSection({ productGroup, defaultBrand, onBrandChange, filter }) {
  const [brandList,     setBrandList]     = useState([])
  const [selectedBrand, setSelectedBrand] = useState('')
  const [trendData,     setTrendData]     = useState([])
  const [loading,       setLoading]       = useState(true)
  const [error,         setError]         = useState('')

  useEffect(() => {
    brandApi.getBrandList(productGroup, filter)
      .then(r => {
        setBrandList(r.data)
        const initial = defaultBrand || (r.data[0]?.brand ?? '')
        setSelectedBrand(initial)
        onBrandChange?.(initial)
      })
      .catch(() => setError('Marka listesi yüklenemedi.'))
  }, [productGroup, defaultBrand, filter])

  const loadTrend = useCallback((brand) => {
    if (!brand) return
    setLoading(true)
    setError('')
    brandApi.getBrandTrend(productGroup, brand, filter)
      .then(r => setTrendData(r.data))
      .catch(() => setError('Trend verisi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  useEffect(() => {
    if (selectedBrand) loadTrend(selectedBrand)
  }, [selectedBrand, loadTrend])

  const handleBrandChange = (brand) => {
    setSelectedBrand(brand)
    onBrandChange?.(brand)
  }

  const lastWeek   = trendData[trendData.length - 1]
  const totalCount = trendData.reduce((s, d) => s + d.policyCount, 0)
  const totalPrem  = trendData.reduce((s, d) => s + d.netPremium, 0)
  const lastWoW    = lastWeek?.wow ?? 0

  const chartData = trendData.map(d => ({ week: d.weekLabel, count: d.policyCount }))

  if (loading && !trendData.length) return <TrendSkeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">

      {/* Sol: Trend Grafik */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <div className="flex items-center justify-between mb-4">
          <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">Haftalık Poliçe Trendi</p>
          <BrandSearchDropdown
            brandList={brandList}
            selectedBrand={selectedBrand}
            onChange={handleBrandChange}
          />
        </div>

        <ResponsiveContainer width="100%" height={160}>
          <LineChart data={chartData} margin={{ top: 4, right: 4, left: -20, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} />
            <XAxis dataKey="week" tick={{ fontSize: 9, fill: '#94a3b8' }} tickLine={false} axisLine={false} />
            <YAxis tick={{ fontSize: 9, fill: '#94a3b8' }} tickLine={false} axisLine={false}
              tickFormatter={v => v >= 1000 ? `${(v/1000).toFixed(1)}K` : v} />
            <Tooltip content={<CustomTooltip />} />
            <Line
              type="monotone" dataKey="count" stroke={CHART_COLOR}
              strokeWidth={2} dot={{ r: 3, fill: CHART_COLOR, strokeWidth: 2, stroke: 'white' }}
              activeDot={{ r: 5 }}
            />
          </LineChart>
        </ResponsiveContainer>

        {/* Özet */}
        <div className="grid grid-cols-3 gap-3 mt-4 pt-4 border-t border-slate-100 dark:border-white/6">
          <div className="text-center">
            <p className="text-xs text-slate-400 dark:text-slate-500">Toplam Poliçe</p>
            <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">
              {totalCount.toLocaleString('tr-TR')}
            </p>
          </div>
          <div className="text-center border-x border-slate-100 dark:border-white/6">
            <p className="text-xs text-slate-400 dark:text-slate-500">Net Prim</p>
            <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">
              ₺{Number(totalPrem).toLocaleString('tr-TR', { maximumFractionDigits: 0 })}
            </p>
          </div>
          <div className="text-center">
            <p className="text-xs text-slate-400 dark:text-slate-500">Son Hafta WoW</p>
            <p className="text-sm font-bold mt-1" style={{ color: lastWoW > 0 ? '#10b981' : lastWoW < 0 ? '#ef4444' : '#94a3b8' }}>
              {lastWoW > 0 ? '+' : ''}{lastWoW}%
            </p>
          </div>
        </div>
      </div>

      {/* Sağ: Haftalık Detay Tablosu */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">
          8 Haftalık Detay — {selectedBrand}
        </p>
        <div className="overflow-x-auto">
          <table className="w-full text-xs">
            <thead>
              <tr className="border-b border-slate-100 dark:border-white/8">
                <th className="text-left pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Hafta</th>
                <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Poliçe</th>
                <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Net Prim</th>
                <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">WoW</th>
              </tr>
            </thead>
            <tbody>
              {trendData.map((row, i) => {
                const isLast   = i === trendData.length - 1
                const wowColor = row.wow > 0 ? '#10b981' : row.wow < 0 ? '#ef4444' : '#94a3b8'
                return (
                  <tr key={row.weekLabel}
                    className={`border-b border-slate-50 dark:border-white/4 ${isLast ? 'bg-slate-50 dark:bg-white/4' : ''}`}>
                    <td className={`py-2 font-medium ${isLast ? 'text-slate-900 dark:text-white' : 'text-slate-600 dark:text-slate-300'}`}>
                      {row.weekLabel}
                    </td>
                    <td className={`py-2 text-right ${isLast ? 'font-bold text-slate-900 dark:text-white' : 'text-slate-500 dark:text-slate-400'}`}>
                      {Number(row.policyCount).toLocaleString('tr-TR')}
                    </td>
                    <td className={`py-2 text-right ${isLast ? 'font-bold text-slate-900 dark:text-white' : 'text-slate-500 dark:text-slate-400'}`}>
                      ₺{Number(row.netPremium).toLocaleString('tr-TR', { maximumFractionDigits: 0 })}
                    </td>
                    <td className="py-2 text-right font-bold" style={{ color: wowColor }}>
                      {row.wow > 0 ? '+' : ''}{row.wow}%
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>

    </div>
  )
}