import { useState, useEffect } from 'react'
import * as dashboardApi from '../../api/dashboardApi'
import PieChart from '../ui/PieChart'
import { SkeletonBlock, SkeletonCard, SkeletonRow } from '../ui/Skeleton'

const DistributionSkeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
    {[1, 2, 3, 4].map(i => (
      <SkeletonCard key={i}>
        <SkeletonBlock width="w-36" height="h-4" className="mb-4" />
        <div className="flex items-center gap-4">
          <div className="flex-shrink-0">
            <SkeletonBlock width="w-[130px]" height="h-[130px]" rounded="rounded-full" />
          </div>
          <div className="flex-1 space-y-2">
            {[1, 2, 3, 4, 5].map(j => (
              <SkeletonRow key={j}>
                <SkeletonBlock width="w-2.5" height="h-2.5" rounded="rounded-sm" />
                <SkeletonBlock width="w-24" height="h-3" />
                <SkeletonBlock width="w-8" height="h-3" />
              </SkeletonRow>
            ))}
          </div>
        </div>
      </SkeletonCard>
    ))}
  </div>
)

const DIST_TYPES = [
  { key: 'Brand',       label: 'Marka Dağılımı'         },
  { key: 'Region',      label: 'Bölge Dağılımı'          },
  { key: 'VehicleAge',  label: 'Araç Yaşı Dağılımı'      },
  { key: 'InsuredAge',  label: 'Sigortalı Yaşı Dağılımı' },
]

export default function DistributionSection({ productGroup, filter }) {
  const [allData, setAllData] = useState({})
  const [loading, setLoading] = useState(true)
  const [error,   setError]   = useState('')

  useEffect(() => {
    setLoading(true)
    setError('')
    Promise.all(
      DIST_TYPES.map(t =>
        dashboardApi.getDistribution(productGroup, t.key, filter).then(r => ({ key: t.key, data: r.data }))
      )
    )
      .then(results => {
        const map = {}
        results.forEach(r => { map[r.key] = r.data })
        setAllData(map)
      })
      .catch(() => setError('Dağılım verileri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <DistributionSkeleton />
  if (error) return (
    <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>
  )

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
      {DIST_TYPES.map(type => (
        <div
          key={type.key}
          className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5"
        >
          <PieChart
            title={type.label}
            items={allData[type.key] || []}
            topN={5}
          />
        </div>
      ))}
    </div>
  )
}