import { useState, useEffect, useMemo } from 'react'
import * as agencyApi from '../../api/agencyApi'
import { exportAgencyHeatmap } from '../../api/exportApi'
import { useExport } from '../../hooks/useExport'
import HeatmapTable from '../ui/HeatmapTable'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'

const HeatmapSkeleton = () => (
  <SkeletonCard>
    <div className="flex items-center justify-between mb-4">
      <div>
        <SkeletonBlock width="w-48" height="h-4" />
        <SkeletonBlock width="w-32" height="h-3" className="mt-1" />
      </div>
      <SkeletonBlock width="w-24" height="h-7" rounded="rounded-lg" />
    </div>
    <div className="space-y-2">
      <SkeletonRow>
        <SkeletonBlock width="w-36" height="h-6" rounded="rounded-sm" />
        {[1,2,3,4,5,6,7,8].map(i => (
          <SkeletonBlock key={i} width="w-full" height="h-6" rounded="rounded-sm" />
        ))}
      </SkeletonRow>
      {[1,2,3,4,5,6,7,8,9,10].map(i => (
        <SkeletonRow key={i}>
          <SkeletonBlock width="w-36" height="h-8" rounded="rounded-sm" />
          {[1,2,3,4,5,6,7,8].map(j => (
            <SkeletonBlock key={j} width="w-full" height="h-8" rounded="rounded-sm" />
          ))}
        </SkeletonRow>
      ))}
    </div>
  </SkeletonCard>
)

export default function AgencyHeatmapSection({ productGroup, filter }) {
  const [data,    setData]    = useState(null)
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  const fileName = useMemo(() => {
    if (!data?.rows?.length) return `AcenteHeatmap_${productGroup}.xlsx`
    const weeks = data.weeks || []
    const first = weeks[0]
    const last  = weeks[weeks.length - 1]
    return `AcenteHeatmap_${productGroup}_${first}-${last}.xlsx`
  }, [data, productGroup])

  const { trigger: handleExport, loading: exportLoading } = useExport(
    () => exportAgencyHeatmap(productGroup,filter),
    fileName
  )

  useEffect(() => {
    setLoading(true)
    setError('')
    agencyApi.getAgencyHeatmap(productGroup, 1, 100, filter)
      .then(r => setData(r.data))
      .catch(() => setError('Heatmap verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <HeatmapSkeleton />
  if (error)   return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  if (!data?.rows?.length) return null

  // HeatmapTable için veriyi dönüştür
  const mapped = []
  data.rows.forEach(row => {
    row.cells.forEach(cell => {
      mapped.push({
        brand: row.agencyName,
        week: cell.week,
        avgNetPremium: cell.avgPremium,
        policyRatio: cell.policyRatio
      })
    })
  })

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <HeatmapTable
        data={mapped}
        onExport={handleExport}
        exportLoading={exportLoading}
        rowLabel="Acente"
      />
    </div>
  )
}