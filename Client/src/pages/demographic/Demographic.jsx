import { usePermission } from '../../hooks/usePermission'
import { exportDemoReport } from '../../api/demoApi'
import useDetailFilterStore from '../../store/detailFilterStore'
import PageTitle from '../../components/ui/PageTitle'
import SectionLabel from '../../components/ui/SectionLabel'
import ReportExportButton from '../../components/ui/ReportExportButton'
import DetailFilterModal from '../../components/filters/DetailFilterModal'
import DemoKpiSection from '../../components/demographic/DemoKpiSection'
import DemoInsuredGenderSection from '../../components/demographic/DemoInsuredGenderSection'
import DemoAgeGroupSection from '../../components/demographic/DemoAgeGroupSection'
import DemoInsuredCitySection from '../../components/demographic/DemoInsuredCitySection'

export default function Demographic() {
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
          <button key={pg} onClick={() => setProductGroup(pg)}
            className={`px-5 py-1.5 rounded-md text-xs font-semibold transition-all ${
              productGroup === pg
                ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
            }`}>{pg}</button>
        ))}
      </div>
      <DetailFilterModal productGroup={productGroup} value={filter} onChange={setFilter} />
      {hasPermission('Demo.Report.Export') && (
        <ReportExportButton
          exportFn={() => exportDemoReport(productGroup,filter)}
          fileName={`DemografikAnaliz_${productGroup}_${today}.pdf`}
        />
      )}
    </div>
  )

  return (
    <div className="space-y-6">

      <PageTitle title="Demografik Analiz — Haftalık" action={actions} />

      {hasPermission('Demo.Kpi.View') && (
        <div>
          <SectionLabel>Özet Göstergeler</SectionLabel>
          <DemoKpiSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {(hasPermission('Demo.InsuredType.View') || hasPermission('Demo.Gender.View')) && (
        <div>
          <SectionLabel>Sigortalı Türü & Cinsiyet Dağılımı</SectionLabel>
          <DemoInsuredGenderSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Demo.AgeGroup.View') && (
        <div>
          <SectionLabel>Yaş Grubu Dağılımı</SectionLabel>
          <DemoAgeGroupSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Demo.InsuredCity.View') && (
        <div>
          <SectionLabel>Sigortalı İli Dağılımı</SectionLabel>
          <DemoInsuredCitySection productGroup={productGroup} filter={filter} />
        </div>
      )}

    </div>
  )
}