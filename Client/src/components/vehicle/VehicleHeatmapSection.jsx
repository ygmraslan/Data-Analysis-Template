import { useState, useEffect, useMemo } from 'react'
import * as vehicleApi from '../../api/vehicleApi'
import { exportVehicleAgeHeatmap, exportVehiclePriceHeatmap } from '../../api/exportApi'
import { useExport } from '../../hooks/useExport'
import HeatmapTable from '../ui/HeatmapTable'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'

const HeatmapSkeleton = () => (
  <SkeletonCard>
    <div className="flex items-center justify-between mb-4">
      <SkeletonBlock width="w-48" height="h-4" />
      <SkeletonBlock width="w-24" height="h-7" rounded="rounded-lg" />
    </div>
    <div className="space-y-2">
      {[1,2,3,4,5,6].map(i => (
        <SkeletonRow key={i}>
          <SkeletonBlock width="w-24" height="h-8" rounded="rounded-sm" />
          {[1,2,3,4,5,6,7,8].map(j => (
            <SkeletonBlock key={j} width="w-full" height="h-8" rounded="rounded-sm" />
          ))}
        </SkeletonRow>
      ))}
    </div>
  </SkeletonCard>
)

const TABS = [
  { key: 'age',   label: 'Araç Yaşı' },
  { key: 'price', label: 'Araç Bedeli' },
]

function HeatmapTab({ productGroup, type, filter }) {
  const [data,    setData]    = useState([])
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  const fileName = useMemo(() => {
    if (!data.length) return `AracHeatmap_${type}_${productGroup}.xlsx`
    const first = data[0]?.week ?? ''
    const last  = data[data.length - 1]?.week ?? ''
    return `AracHeatmap_${type}_${productGroup}_${first}-${last}.xlsx`
  }, [data, type, productGroup])

  const exportFn  = type === 'age' ? exportVehicleAgeHeatmap : exportVehiclePriceHeatmap
  const rowLabel  = type === 'age' ? 'Yaş Grubu' : 'Araç Bedeli'

  const { trigger: handleExport, loading: exportLoading } = useExport(
    () => exportFn(productGroup,filter),
    fileName
  )

  useEffect(() => {
    setLoading(true)
    setError('')
    const fn = type === 'age' ? vehicleApi.getVehicleAgeHeatmap : vehicleApi.getVehiclePriceHeatmap
    fn(productGroup, filter)
      .then(r => setData(r.data))
      .catch(() => setError('Heatmap verisi yüklenemedi.'))
      .finally(() => setLoading(false))
 }, [productGroup, type, filter])

  if (loading) return <HeatmapSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  const mapped = data.map(d => ({ brand: d.label, week: d.week, avgNetPremium: d.avgNetPremium, policyRatio: d.policyRatio }))

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <HeatmapTable
        data={mapped}
        onExport={handleExport}
        exportLoading={exportLoading}
        rowLabel={rowLabel}
      />
    </div>
  )
}

export default function VehicleHeatmapSection({ productGroup, filter }) {
  const [activeTab, setActiveTab] = useState('age')

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1 w-fit">
        {TABS.map(tab => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`px-4 py-1.5 rounded-md text-xs font-semibold transition-all ${
              activeTab === tab.key
                ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <HeatmapTab productGroup={productGroup} type={activeTab} filter={filter} />
    </div>
  )
}