import { useState, useEffect } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { exportCityReport } from '../../api/exportApi'
import * as cityApi from '../../api/cityApi'
import useDetailFilterStore from '../../store/detailFilterStore'
import PageTitle from '../../components/ui/PageTitle'
import SectionLabel from '../../components/ui/SectionLabel'
import ReportExportButton from '../../components/ui/ReportExportButton'
import DetailFilterModal from '../../components/filters/DetailFilterModal'
import CityKpiSection from '../../components/city/CityKpiSection'
import CityListSection from '../../components/city/CityListSection'
import CityTrendSection from '../../components/city/CityTrendSection'
import CityHeatmapSection from '../../components/city/CityHeatmapSection'

export default function City() {
  const { hasPermission } = usePermission()
  const productGroup    = useDetailFilterStore(s => s.productGroup)
  const filter          = useDetailFilterStore(s => s.filter)
  const setProductGroup = useDetailFilterStore(s => s.setProductGroup)
  const setFilter       = useDetailFilterStore(s => s.setFilter)

  const [defaultCity,   setDefaultCity]   = useState('')
  const [selectedCity,  setSelectedCity]  = useState('')
  const [cityList,      setCityList]      = useState([])

  const today = new Date().toISOString().slice(0, 10).replace(/-/g, '')

  useEffect(() => {
    cityApi.getCityKpi(productGroup, filter)
      .then(r => {
        const city = r.data?.defaultCity ?? ''
        setDefaultCity(city)
        setSelectedCity(city)
      })
      .catch(() => {})

    cityApi.getCityList(productGroup, filter)
      .then(r => setCityList(r.data))
      .catch(() => {})
  }, [productGroup, filter])

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
      {hasPermission('City.Report.Export') && (
        <ReportExportButton
          exportFn={() => exportCityReport(productGroup,filter)}
          fileName={`IlRaporu_${productGroup}_${today}.pdf`}
        />
      )}
    </div>
  )

  return (
    <div className="space-y-6">

      <PageTitle
        title="Coğrafi Analiz — Haftalık"
        action={actions}
      />

      {hasPermission('City.Kpi.View') && (
        <div>
          <SectionLabel>İl Üretim</SectionLabel>
          <CityKpiSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {(hasPermission('City.List.View') || hasPermission('City.Trend.View')) && (
        <div>
          <SectionLabel>İl Sıralaması & Trend</SectionLabel>
          <div className="grid grid-cols-1 xl:grid-cols-2 gap-4">
            {hasPermission('City.List.View') && (
              <CityListSection
                productGroup={productGroup}
                selectedCity={selectedCity}
                onCitySelect={setSelectedCity}
                filter={filter}
              />
            )}
            {hasPermission('City.Trend.View') && (
              <CityTrendSection
                productGroup={productGroup}
                defaultCity={defaultCity}
                cityList={cityList}
                onCityChange={setSelectedCity}
                filter={filter}
              />
            )}
          </div>
        </div>
      )}

      {hasPermission('City.Heatmap.View') && (
        <div>
          <SectionLabel>İl Net Prim Heatmap</SectionLabel>
          <CityHeatmapSection productGroup={productGroup} filter={filter} />
        </div>
      )}

    </div>
  )
}