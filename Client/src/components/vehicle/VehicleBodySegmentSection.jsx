import { useState, useEffect } from 'react'
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer } from 'recharts'
import * as vehicleApi from '../../api/vehicleApi'
import { SkeletonBlock } from '../ui/Skeleton'

const COLORS = ['#0f3460','#10b981','#3b82f6','#f59e0b','#8b5cf6','#94a3b8']

const Skeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
    {[1,2].map(i => (
      <div key={i} className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <SkeletonBlock width="w-40" height="h-4" className="mb-4" />
        <SkeletonBlock width="w-full" height="h-48" rounded="rounded-full" />
      </div>
    ))}
  </div>
)

const CustomTooltip = ({ active, payload, valueFormatter }) => {
  if (!active || !payload?.length) return null
  const item = payload[0]
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg">
      <p className="text-xs font-semibold text-slate-700 dark:text-slate-200 mb-1">{item.name}</p>
      <p className="text-sm font-bold text-slate-900 dark:text-white">{valueFormatter(item.value)}</p>
      <p className="text-xs text-slate-400 mt-0.5">%{item.payload.ratio}</p>
    </div>
  )
}

function PieSection({ title, data, labelKey, dataKey, valueFormatter }) {
  const sorted = [...data].sort((a, b) => b[dataKey] - a[dataKey])
  const top5   = sorted.slice(0, 5)
  const rest   = sorted.slice(5)
  const total  = data.reduce((s, d) => s + d[dataKey], 0)

  const pieData = [
    ...top5.map(d => ({
      name:  d[labelKey],
      value: d[dataKey],
      ratio: total > 0 ? (d[dataKey] / total * 100).toFixed(1) : '0.0',
    })),
    ...(rest.length > 0 ? [{
      name:  'Diğer',
      value: rest.reduce((s, d) => s + d[dataKey], 0),
      ratio: total > 0 ? (rest.reduce((s, d) => s + d[dataKey], 0) / total * 100).toFixed(1) : '0.0',
    }] : []),
  ]

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">{title}</p>
      <div className="flex items-center gap-6">
        <div className="flex-shrink-0" style={{ width: 160, height: 160 }}>
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie data={pieData} dataKey="value" cx="50%" cy="50%"
                innerRadius={45} outerRadius={75} paddingAngle={2} strokeWidth={0}>
                {pieData.map((_, i) => (
                  <Cell key={i} fill={COLORS[i % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip content={<CustomTooltip valueFormatter={valueFormatter} />} />
            </PieChart>
          </ResponsiveContainer>
        </div>
        <div className="flex-1 space-y-2">
          {pieData.map((item, i) => (
            <div key={item.name} className="flex items-center justify-between gap-2">
              <div className="flex items-center gap-2 min-w-0">
                <div className="w-2.5 h-2.5 rounded-sm flex-shrink-0" style={{ backgroundColor: COLORS[i % COLORS.length] }} />
                <span className="text-xs font-semibold text-slate-700 dark:text-slate-200 truncate">{item.name}</span>
              </div>
              <div className="flex items-center gap-2 flex-shrink-0">
                <span className="text-xs text-slate-500 dark:text-slate-400">{valueFormatter(item.value)}</span>
                <span className="text-xs font-bold text-slate-400 w-10 text-right">%{item.ratio}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

export default function VehicleBodySegmentSection({ productGroup, filter }) {
  const [bodyData,    setBodyData]    = useState([])
  const [segmentData, setSegmentData] = useState([])
  const [loading,     setLoading]     = useState(true)
  const [error,       setError]       = useState('')

  useEffect(() => {
    setLoading(true)
    setError('')
    Promise.all([
      vehicleApi.getVehicleBody(productGroup, filter),
      vehicleApi.getVehicleSegment(productGroup, filter),
    ])
      .then(([bodyRes, segRes]) => {
        setBodyData(bodyRes.data)
        setSegmentData(segRes.data)
      })
      .catch(() => setError('Gövde tipi / segment verisi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <Skeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  const fmtPolicy = (v) => Number(v).toLocaleString('tr-TR') + ' poliçe'
  const fmtPrem   = (v) => '₺' + Number(v).toLocaleString('tr-TR', { maximumFractionDigits: 0 })

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
      <PieSection title="Gövde Tipi — Poliçe Dağılımı" data={bodyData}    labelKey="bodyType" dataKey="policyCount" valueFormatter={fmtPolicy} />
      <PieSection title="Segment — Net Prim Dağılımı"  data={segmentData} labelKey="segment"  dataKey="netPremium"  valueFormatter={fmtPrem}   />
    </div>
  )
}