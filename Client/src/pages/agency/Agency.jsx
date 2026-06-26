import { useState } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { exportAgencyReport } from '../../api/exportApi'
import useDetailFilterStore from '../../store/detailFilterStore'
import PageTitle from '../../components/ui/PageTitle'
import SectionLabel from '../../components/ui/SectionLabel'
import ReportExportButton from '../../components/ui/ReportExportButton'
import DetailFilterModal from '../../components/filters/DetailFilterModal'
import AgencyKpiSection from '../../components/agency/AgencyKpiSection'
import AgencyRegionSection from '../../components/agency/AgencyRegionSection'
import AgencyListSection from '../../components/agency/AgencyListSection'
import AgencyTrendSection from '../../components/agency/AgencyTrendSection'
import AgencyHeatmapSection from '../../components/agency/AgencyHeatmapSection'

export default function Agency() {
  const { hasPermission } = usePermission()
  const productGroup    = useDetailFilterStore(s => s.productGroup)
  const filter          = useDetailFilterStore(s => s.filter)
  const setProductGroup = useDetailFilterStore(s => s.setProductGroup)
  const setFilter       = useDetailFilterStore(s => s.setFilter)

  const [selectedAgency, setSelectedAgency] = useState('')

  const today = new Date().toISOString().slice(0, 10).replace(/-/g, '')

  const handleDefaultAgency = (agencyCode) => {
    if (!selectedAgency) {
      setSelectedAgency(agencyCode)
    }
  }

  const handleAgencySelect = (agencyCode) => {
    setSelectedAgency(agencyCode)
  }

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
      {hasPermission('Agency.Report.Export') && (
        <ReportExportButton
          exportFn={() => exportAgencyReport(productGroup,filter)}
          fileName={`AcenteRaporu_${productGroup}_${today}.pdf`}
        />
      )}
    </div>
  )

  return (
    <div className="space-y-6">

      <PageTitle
        title="Acente Performans — Haftalık"
        action={actions}
      />

      {/* KPI Section */}
      {hasPermission('Agency.Kpi.View') && (
        <div>
          <SectionLabel>Acente Özeti</SectionLabel>
          <AgencyKpiSection 
            productGroup={productGroup} 
            onDefaultAgency={handleDefaultAgency}
            filter={filter}
          />
        </div>
      )}

      {/* Region Section */}
      {hasPermission('Agency.Region.View') && (
        <div>
          <SectionLabel>Bölge Dağılımı</SectionLabel>
          <AgencyRegionSection 
            productGroup={productGroup}
            selectedAgency={selectedAgency}
            onAgencySelect={handleAgencySelect}
            filter={filter}
          />
        </div>
      )}

      {/* List Section */}
      {hasPermission('Agency.List.View') && (
        <div>
          <SectionLabel>Genel Acente Sıralaması</SectionLabel>
          <AgencyListSection 
            productGroup={productGroup}
            selectedAgency={selectedAgency}
            onAgencySelect={handleAgencySelect}
            filter={filter}
          />
        </div>
      )}

      {/* Trend & Profile Section */}
      {(hasPermission('Agency.Trend.View') || hasPermission('Agency.Profile.View')) && (
        <div>
          <SectionLabel>Acente Trend & Profil</SectionLabel>
          <AgencyTrendSection 
            productGroup={productGroup}
            selectedAgency={selectedAgency}
            onAgencyChange={handleAgencySelect}
            filter={filter}
          />
        </div>
      )}

      {/* Heatmap Section */}
      {hasPermission('Agency.Heatmap.View') && (
        <div>
          <SectionLabel>Acente Heatmap</SectionLabel>
          <AgencyHeatmapSection productGroup={productGroup} filter={filter} />
        </div>
      )}

    </div>
  )
}