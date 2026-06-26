import { useState, useEffect, useRef } from 'react'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import * as vehicleApi from '../../api/vehicleApi'
import { SkeletonBlock } from '../ui/Skeleton'

const CHART_COLOR = '#3b82f6'

const PriceSkeleton = () => (
  <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 space-y-4">
    <div className="flex justify-between items-center">
      <SkeletonBlock width="w-40" height="h-4" />
      <SkeletonBlock width="w-36" height="h-8" rounded="rounded-lg" />
    </div>
    <SkeletonBlock width="w-full" height="h-40" />
    <div className="grid grid-cols-3 gap-3">
      {[1,2,3].map(i => <SkeletonBlock key={i} width="w-full" height="h-12" rounded="rounded-lg" />)}
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

function PriceDropdown({ priceList, selected, onChange }) {
  const [open, setOpen] = useState(false)
  const ref = useRef(null)

  useEffect(() => {
    const handler = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false) }
    document.addEventListener('mousedown', handler)
    return () => document.removeEventListener('mousedown', handler)
  }, [])

  return (
    <div ref={ref} className="relative">
      <div onClick={() => setOpen(!open)}
        className="flex items-center gap-2 px-3 py-1.5 bg-slate-50 dark:bg-white/8 border border-slate-200 dark:border-white/10 rounded-lg cursor-pointer min-w-[130px]">
        <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate flex-1">{selected || 'Seç'}</span>
        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-400 flex-shrink-0"><polyline points="6 9 12 15 18 9" /></svg>
      </div>
      {open && (
        <div className="absolute right-0 top-full mt-1 w-52 bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-xl shadow-xl z-50 max-h-48 overflow-y-auto">
          {priceList.map(p => (
            <button key={p.priceRange} onClick={() => { onChange(p.priceRange); setOpen(false) }}
              className={`w-full text-left px-4 py-2.5 text-xs font-medium hover:bg-slate-50 dark:hover:bg-white/5 flex justify-between gap-2 ${p.priceRange === selected ? 'text-blue-600 dark:text-blue-400 bg-blue-50 dark:bg-blue-500/10' : 'text-slate-700 dark:text-slate-200'}`}>
              <span>{p.priceRange}</span>
              <span className="text-slate-400">{Number(p.policyCount).toLocaleString('tr-TR')}</span>
            </button>
          ))}
        </div>
      )}
    </div>
  )
}

export default function VehiclePriceSection({ productGroup, defaultPriceRange, filter }) {
  const [priceList,    setPriceList]    = useState([])
  const [selected,     setSelected]     = useState('')
  const [trendData,    setTrendData]    = useState([])
  const [loading,      setLoading]      = useState(true)
  const [trendLoading, setTrendLoading] = useState(false)
  const [error,        setError]        = useState('')

  useEffect(() => {
    setLoading(true)
    vehicleApi.getVehiclePrice(productGroup, filter)
      .then(r => setPriceList(r.data))
      .catch(() => setError('Araç bedeli verisi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  useEffect(() => {
    if (defaultPriceRange) setSelected(defaultPriceRange)
  }, [defaultPriceRange])

  useEffect(() => {
    if (!selected) return
    setTrendLoading(true)
    vehicleApi.getVehicleTrend(productGroup, 'Price', selected, true, filter)
      .then(r => setTrendData(r.data))
      .finally(() => setTrendLoading(false))
  }, [productGroup, selected, filter])

  const chartData  = trendData.map(d => ({ week: d.weekLabel, prim: d.netPremium }))
  const totalPrem  = trendData.reduce((s, d) => s + d.netPremium, 0)
  const totalCount = trendData.reduce((s, d) => s + d.policyCount, 0)
  const lastWoW    = trendData[trendData.length - 1]?.wow ?? 0
  const fmt        = (n) => '₺' + Number(n).toLocaleString('tr-TR', { maximumFractionDigits: 0 })

  if (loading) return <PriceSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 space-y-4">
      <div className="flex items-center justify-between">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">Bedel Aralığı — 8 Haftalık Trend</p>
        <PriceDropdown priceList={priceList} selected={selected} onChange={setSelected} />
      </div>

      {trendLoading ? (
        <SkeletonBlock width="w-full" height="h-40" />
      ) : (
        <ResponsiveContainer width="100%" height={160}>
          <LineChart data={chartData} margin={{ top: 4, right: 4, left: -10, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} />
            <XAxis dataKey="week" tick={{ fontSize: 9, fill: '#94a3b8' }} tickLine={false} axisLine={false} />
            <YAxis tick={{ fontSize: 9, fill: '#94a3b8' }} tickLine={false} axisLine={false}
              tickFormatter={v => v >= 1_000_000 ? `₺${(v/1_000_000).toFixed(1)}M` : `₺${(v/1000).toFixed(0)}K`} />
            <Tooltip content={<CustomTooltip />} />
            <Line type="monotone" dataKey="prim" stroke={CHART_COLOR} strokeWidth={2}
              dot={{ r: 3, fill: CHART_COLOR, strokeWidth: 2, stroke: 'white' }} activeDot={{ r: 5 }} />
          </LineChart>
        </ResponsiveContainer>
      )}

      <div className="grid grid-cols-3 gap-3 pt-2 border-t border-slate-100 dark:border-white/6">
        <div className="text-center">
          <p className="text-xs text-slate-400 dark:text-slate-500">Toplam Poliçe</p>
          <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">{totalCount.toLocaleString('tr-TR')}</p>
        </div>
        <div className="text-center border-x border-slate-100 dark:border-white/6">
          <p className="text-xs text-slate-400 dark:text-slate-500">Net Prim</p>
          <p className="text-sm font-bold text-slate-900 dark:text-white mt-1">{fmt(totalPrem)}</p>
        </div>
        <div className="text-center">
          <p className="text-xs text-slate-400 dark:text-slate-500">Son Hafta WoW</p>
          <p className="text-sm font-bold mt-1" style={{ color: lastWoW > 0 ? '#10b981' : lastWoW < 0 ? '#ef4444' : '#94a3b8' }}>
            {lastWoW > 0 ? '+' : ''}{lastWoW}%
          </p>
        </div>
      </div>
    </div>
  )
}