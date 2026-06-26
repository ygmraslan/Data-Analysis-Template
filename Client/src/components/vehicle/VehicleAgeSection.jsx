import { useState, useEffect, useRef } from 'react'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import * as vehicleApi from '../../api/vehicleApi'
import { SkeletonBlock } from '../ui/Skeleton'

const CHART_COLOR = '#10b981'

const AgeSkeleton = () => (
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

function AgeDropdown({ ageList, selected, onChange, grouped }) {
  const [open, setOpen] = useState(false)
  const [query, setQuery] = useState('')
  const ref = useRef(null)

  useEffect(() => {
    const handler = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false) }
    document.addEventListener('mousedown', handler)
    return () => document.removeEventListener('mousedown', handler)
  }, [])

  const filtered = ageList.filter(a => a.ageGroup.toLowerCase().includes(query.toLowerCase()))

  return (
    <div ref={ref} className="relative">
      <div
        onClick={() => setOpen(true)}
        className="flex items-center gap-2 px-3 py-1.5 bg-slate-50 dark:bg-white/8 border border-slate-200 dark:border-white/10 rounded-lg cursor-pointer min-w-[120px]"
      >
        {open ? (
          <input autoFocus value={query} onChange={e => setQuery(e.target.value)}
            placeholder="Ara..." className="text-xs font-semibold bg-transparent outline-none w-full text-slate-700 dark:text-slate-200 placeholder:text-slate-400" />
        ) : (
          <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate flex-1">{selected || 'Seç'}</span>
        )}
        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-400 flex-shrink-0"><polyline points="6 9 12 15 18 9" /></svg>
      </div>
      {open && (
        <div className="absolute right-0 top-full mt-1 w-44 bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-xl shadow-xl z-50 max-h-48 overflow-y-auto">
          {filtered.map(a => (
            <button key={a.ageGroup} onClick={() => { onChange(a.ageGroup); setQuery(''); setOpen(false) }}
              className={`w-full text-left px-4 py-2.5 text-xs font-medium hover:bg-slate-50 dark:hover:bg-white/5 flex justify-between gap-2 ${a.ageGroup === selected ? 'text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-500/10' : 'text-slate-700 dark:text-slate-200'}`}>
              <span>{a.ageGroup}</span>
              <span className="text-slate-400">{Number(a.policyCount).toLocaleString('tr-TR')}</span>
            </button>
          ))}
        </div>
      )}
    </div>
  )
}

export default function VehicleAgeSection({ productGroup, defaultAgeGroup, filter }) {
  const [grouped,      setGrouped]      = useState(true)
  const [ageList,      setAgeList]      = useState([])
  const [selectedAge,  setSelectedAge]  = useState('')
  const [trendData,    setTrendData]    = useState([])
  const [loading,      setLoading]      = useState(true)
  const [trendLoading, setTrendLoading] = useState(false)
  const [error,        setError]        = useState('')

  useEffect(() => {
    setLoading(true)
    vehicleApi.getVehicleAge(productGroup, grouped, filter)
      .then(r => setAgeList(r.data))
      .catch(() => setError('Araç yaşı verisi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, grouped, filter])

  useEffect(() => {
    if (defaultAgeGroup) setSelectedAge(defaultAgeGroup)
  }, [defaultAgeGroup, grouped])

  useEffect(() => {
    if (!selectedAge) return
    setTrendLoading(true)
    vehicleApi.getVehicleTrend(productGroup, 'Age', selectedAge, grouped, filter)
      .then(r => setTrendData(r.data))
      .finally(() => setTrendLoading(false))
  }, [productGroup, selectedAge, filter])

  const chartData    = trendData.map(d => ({ week: d.weekLabel, prim: d.netPremium }))
  const totalPrem    = trendData.reduce((s, d) => s + d.netPremium, 0)
  const totalCount   = trendData.reduce((s, d) => s + d.policyCount, 0)
  const lastWoW      = trendData[trendData.length - 1]?.wow ?? 0
  const fmt          = (n) => '₺' + Number(n).toLocaleString('tr-TR', { maximumFractionDigits: 0 })

  if (loading) return <AgeSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 space-y-4">
      <div className="flex items-center justify-between">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">Yaş Grubu — 8 Haftalık Trend</p>
        <div className="flex items-center gap-2">
          <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
            {[{ key: true, label: 'Aralıklı' }, { key: false, label: 'Tek Tek' }].map(opt => (
              <button key={String(opt.key)} onClick={() => { setGrouped(opt.key); setSelectedAge(opt.key ? (defaultAgeGroup || '') : '') }}
                className={`px-3 py-1 rounded-md text-xs font-semibold transition-all ${grouped === opt.key ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm' : 'text-slate-500 dark:text-slate-400 hover:text-slate-700'}`}>
                {opt.label}
              </button>
            ))}
          </div>
          <AgeDropdown ageList={ageList} selected={selectedAge} onChange={setSelectedAge} grouped={grouped} />
        </div>
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