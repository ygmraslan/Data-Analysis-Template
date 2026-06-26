import { useState, useEffect } from 'react'
import * as dashboardApi from '../../api/dashboardApi'
import TrendSparkline from '../ui/TrendSparkline'
import StatBadge from '../ui/StatBadge'
import { SkeletonBlock, SkeletonCard, SkeletonRow, SkeletonChart } from '../ui/Skeleton'

const SegmentDriftSkeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
    <SkeletonCard>
      <SkeletonBlock width="w-40" height="h-4" className="mb-4" />
      {[1, 2].map(i => (
        <div key={i} className="pb-5 mb-5 border-b border-slate-100 dark:border-white/6 last:border-0 last:mb-0 last:pb-0">
          <SkeletonRow className="mb-3">
            <div className="flex-1">
              <SkeletonBlock width="w-24" height="h-4" className="mb-1" />
              <SkeletonBlock width="w-40" height="h-3" />
            </div>
            <SkeletonBlock width="w-16" height="h-5" rounded="rounded-full" />
          </SkeletonRow>
          <SkeletonRow>
            <SkeletonBlock width="w-16" height="h-8" />
            <SkeletonBlock width="w-16" height="h-8" />
            <SkeletonBlock width="w-16" height="h-8" />
          </SkeletonRow>
        </div>
      ))}
    </SkeletonCard>
    <SkeletonCard>
      <SkeletonBlock width="w-32" height="h-4" className="mb-1" />
      <SkeletonBlock width="w-48" height="h-3" className="mb-4" />
      <SkeletonChart height={180} />
    </SkeletonCard>
  </div>
)

function getBadgeType(wow) {
  if (wow > 0.2)  return 'critical'
  if (wow > 0)    return 'warning'
  return 'normal'
}

export default function SegmentDriftSection({ productGroup, filter }) {
  const [data,    setData]    = useState([])
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  useEffect(() => {
    setLoading(true)
    setError('')
    dashboardApi.getSegmentDrift(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('Segment drift verileri yüklenemedi.'))
      .finally(() => setLoading(false))
 }, [productGroup, filter])

  if (loading) return <SegmentDriftSkeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  const last = data[data.length - 1]

  const segments = last ? [
    {
      name:    'Segment 1',
      desc:    'Genç sürücü (18-25) + Premium marka + 11+ yaş araç',
      share:   last.seg1Share,
      wow:     last.seg1WoW,
      rolling: last.seg1Rolling4,
    },
    {
      name:    'Segment 2',
      desc:    'Tüzel müşteri + Premium marka + 11+ yaş araç',
      share:   last.seg2Share,
      wow:     last.seg2WoW,
      rolling: last.seg2Rolling4,
    },
  ] : []

  const trendSeries = [
    { key: 'seg1Share', label: 'Seg.1', color: '#378ADD', dashed: false },
    { key: 'seg2Share', label: 'Seg.2', color: '#1D9E75', dashed: true  },
  ]

  const startDate = data[0]?.weekStart
  const endDate   = data[data.length - 1]?.weekStart
  const fmtD = (str) => {
    if (!str) return ''
    const d = new Date(str)
    return `${String(d.getDate()).padStart(2,'0')}.${String(d.getMonth()+1).padStart(2,'0')}.${d.getFullYear()}`
  }
  const fmtEnd = (str) => {
    if (!str) return ''
    const d = new Date(str)
    d.setDate(d.getDate() + 6)
    return `${String(d.getDate()).padStart(2,'0')}.${String(d.getMonth()+1).padStart(2,'0')}.${d.getFullYear()}`
  }

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">

      {/* Tablo */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <p className="text-sm font-semibold text-slate-800 dark:text-white mb-4">Segment Drift Tablosu</p>
        <div className="space-y-5">
            {segments.map((seg) => (
              <div key={seg.name} className="pb-5 border-b border-slate-100 dark:border-white/6 last:border-0 last:pb-0">
                <div className="flex items-start justify-between gap-2 mb-3">
                  <div>
                    <p className="text-sm font-semibold text-slate-800 dark:text-white">{seg.name}</p>
                    <p className="text-xs text-slate-400 mt-0.5">{seg.desc}</p>
                  </div>
                  <StatBadge type={getBadgeType(seg.wow)} />
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                  <div>
                    <p className="text-2xl font-bold text-slate-900 dark:text-white">%{seg.share}</p>
                    <p className="text-xs text-slate-400 mt-0.5">portföy payı</p>
                  </div>
                  <div>
                    <p className={`text-lg font-bold ${
                      seg.wow > 0 ? 'text-red-500' : seg.wow < 0 ? 'text-emerald-500' : 'text-slate-400'
                    }`}>
                      {seg.wow > 0 ? '+' : ''}{seg.wow} p.
                    </p>
                    <p className="text-xs text-slate-400 mt-0.5">geçen haftaya göre</p>
                  </div>
                  <div>
                    <p className="text-lg font-bold text-slate-600 dark:text-slate-300">%{seg.rolling}</p>
                    <p className="text-xs text-slate-400 mt-0.5">4 haftalık ort.</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
      </div>

      {/* Trend Grafik */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <div className="flex items-center justify-between mb-4">
          <div>
            <p className="text-sm font-semibold text-slate-800 dark:text-white">Drift Trendi</p>
            {startDate && endDate && (
              <p className="text-xs text-slate-400 mt-0.5">
                {fmtD(startDate)} — {fmtEnd(endDate)} · {data.length} hafta
              </p>
            )}
          </div>
          <div className="flex items-center gap-3">
            <div className="flex items-center gap-1.5">
              <div className="w-4 h-0.5 bg-blue-500 rounded" />
              <span className="text-xs text-slate-400">Seg.1</span>
            </div>
            <div className="flex items-center gap-1.5">
              <div className="w-4 h-px bg-emerald-500" style={{ borderTop: '2px dashed #10b981' }} />
              <span className="text-xs text-slate-400">Seg.2</span>
            </div>
          </div>
        </div>
        <TrendSparkline
          data={data}
          series={trendSeries}
          height={155}
          showSlope={true}
          suffix="%"
        />
      </div>
    </div>
  )
}