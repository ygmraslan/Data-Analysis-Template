import { usePermission } from '../../hooks/usePermission'
import useDetailFilterStore from '../../store/detailFilterStore'
import KpiSection          from '../../components/dashboard/KpiSection'
import WeeklyTotalsSection from '../../components/dashboard/WeeklyTotalsSection'
import SegmentDriftSection from '../../components/dashboard/SegmentDriftSection'
import DistributionSection from '../../components/dashboard/DistributionSection'
import HeatmapSection      from '../../components/dashboard/HeatmapSection'
import PageTitle           from '../../components/ui/PageTitle'
import SectionLabel        from '../../components/ui/SectionLabel'
import DetailFilterModal   from '../../components/filters/DetailFilterModal'

export default function Dashboard() {
  const { hasPermission } = usePermission()
  const productGroup    = useDetailFilterStore(s => s.productGroup)
  const filter          = useDetailFilterStore(s => s.filter)
  const setProductGroup = useDetailFilterStore(s => s.setProductGroup)
  const setFilter       = useDetailFilterStore(s => s.setFilter)

  const actions = (
    <div className="flex items-center gap-2">
      <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
        {['KASKO', 'TRAFIK'].map(pg => (
          <button
            key={pg}
            onClick={() => setProductGroup(pg)}
            className={`px-5 py-1.5 rounded-md text-xs font-semibold transition-all ${
              productGroup === pg
                ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
            }`}
          >
            {pg}
          </button>
        ))}
      </div>
      <DetailFilterModal productGroup={productGroup} value={filter} onChange={setFilter} />
    </div>
  )

  return (
    <div className="space-y-6">

      <PageTitle
        title="Üretim Özeti — Haftalık"
        action={actions}
      />

      {hasPermission('Dashboard.Kpi.View') && (
        <div>
          <SectionLabel>Üretim Analizi</SectionLabel>
          <KpiSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Dashboard.Kpi.View') && (
        <div>
          <SectionLabel>8 Haftalık Toplam Üretim</SectionLabel>
          <WeeklyTotalsSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Dashboard.Distribution.View') && (
        <div>
          <SectionLabel>Haftalık Portföy Dağılımı</SectionLabel>
          <DistributionSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Dashboard.Heatmap.View') && (
        <div>
          <SectionLabel>Net Prim Heatmap</SectionLabel>
          <HeatmapSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Dashboard.SegmentDrift.View') && (
        <div>
          <SectionLabel>Portföy Drift Analizi</SectionLabel>
          <SegmentDriftSection productGroup={productGroup} filter={filter} />
        </div>
      )}

    </div>
  )
}