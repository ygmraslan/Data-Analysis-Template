import { usePermission } from '../../hooks/usePermission'
import { exportRegionReport } from '../../api/exportApi'
import useDetailFilterStore from '../../store/detailFilterStore'
import PageTitle from '../../components/ui/PageTitle'
import SectionLabel from '../../components/ui/SectionLabel'
import ReportExportButton from '../../components/ui/ReportExportButton'
import DetailFilterModal from '../../components/filters/DetailFilterModal'
import RegionKpiSection from '../../components/region/RegionKpiSection'
import RegionTrendSection from '../../components/region/RegionTrendSection'
import RegionHeatmapSection from '../../components/region/RegionHeatmapSection'

export default function Region() {
  const { hasPermission } = usePermission()
  const productGroup    = useDetailFilterStore(s => s.productGroup)
  const filter          = useDetailFilterStore(s => s.filter)
  const setProductGroup = useDetailFilterStore(s => s.setProductGroup)
  const setFilter       = useDetailFilterStore(s => s.setFilter)

  const today = new Date().toISOString().slice(0, 10).replace(/-/g, '')

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
      {hasPermission('Region.Report.Export') && (
        <ReportExportButton
          exportFn={() => exportRegionReport(productGroup,filter)}
          fileName={`BolgeRaporu_${productGroup}_${today}.pdf`}
        />
      )}
    </div>
  )

  return (
    <div className="space-y-6">

      <PageTitle
        title="Bölge Analizi — Haftalık"
        action={actions}
      />

      {hasPermission('Region.Kpi.View') && (
        <div>
          <SectionLabel>Bölge Üretim</SectionLabel>
          <RegionKpiSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Region.Trend.View') && (
        <div>
          <SectionLabel>Haftalık Net Prim Trendi</SectionLabel>
          <RegionTrendSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Region.Heatmap.View') && (
        <div>
          <SectionLabel>Net Prim Heatmap</SectionLabel>
          <RegionHeatmapSection productGroup={productGroup} filter={filter} />
        </div>
      )}

    </div>
  )
}