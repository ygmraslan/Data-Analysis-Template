import { useState, useEffect } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { exportVehicleReport } from '../../api/exportApi'
import * as vehicleApi from '../../api/vehicleApi'
import useDetailFilterStore from '../../store/detailFilterStore'
import PageTitle from '../../components/ui/PageTitle'
import SectionLabel from '../../components/ui/SectionLabel'
import ReportExportButton from '../../components/ui/ReportExportButton'
import DetailFilterModal from '../../components/filters/DetailFilterModal'
import VehicleKpiSection from '../../components/vehicle/VehicleKpiSection'
import VehicleAgeSection from '../../components/vehicle/VehicleAgeSection'
import VehiclePriceSection from '../../components/vehicle/VehiclePriceSection'
import VehicleBodySegmentSection from '../../components/vehicle/VehicleBodySegmentSection'
import VehicleHeatmapSection from '../../components/vehicle/VehicleHeatmapSection'

export default function Vehicle() {
  const { hasPermission }  = usePermission()
  const productGroup    = useDetailFilterStore(s => s.productGroup)
  const filter          = useDetailFilterStore(s => s.filter)
  const setProductGroup = useDetailFilterStore(s => s.setProductGroup)
  const setFilter       = useDetailFilterStore(s => s.setFilter)

  const [defaultAgeGroup,   setDefaultAgeGroup]   = useState(null)
  const [defaultPriceRange, setDefaultPriceRange] = useState(null)

  const today = new Date().toISOString().slice(0, 10).replace(/-/g, '')

  useEffect(() => {
    setDefaultAgeGroup(null)
    setDefaultPriceRange(null)
    vehicleApi.getVehicleKpi(productGroup, filter)
      .then(r => {
        setDefaultAgeGroup(r.data?.topGainerAge ?? '')
        setDefaultPriceRange(r.data?.topGainerPrice ?? '')
      })
      .catch(() => {
        setDefaultAgeGroup('')
        setDefaultPriceRange('')
      })
  }, [productGroup, filter])

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
      {hasPermission('Vehicle.Report.Export') && (
        <ReportExportButton
          exportFn={() => exportVehicleReport(productGroup,filter)}
          fileName={`AracRaporu_${productGroup}_${today}.pdf`}
        />
      )}
    </div>
  )

  return (
    <div className="space-y-6">

      <PageTitle title="Araç Analizi — Haftalık" action={actions} />

      {hasPermission('Vehicle.Kpi.View') && (
        <div>
          <SectionLabel>Araç Üretim</SectionLabel>
          <VehicleKpiSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {(hasPermission('Vehicle.Age.View') || hasPermission('Vehicle.Price.View')) && (
        <div>
          <SectionLabel>Araç Yaşı & Bedel Trend</SectionLabel>
          <div className="grid grid-cols-1 xl:grid-cols-2 gap-4">
            {hasPermission('Vehicle.Age.View') && (
              <VehicleAgeSection productGroup={productGroup} defaultAgeGroup={defaultAgeGroup ?? ''} filter={filter} />
            )}
            {hasPermission('Vehicle.Price.View') && (
              <VehiclePriceSection productGroup={productGroup} defaultPriceRange={defaultPriceRange ?? ''} filter={filter} />
            )}
          </div>
        </div>
      )}

      {(hasPermission('Vehicle.Body.View') || hasPermission('Vehicle.Segment.View')) && (
        <div>
          <SectionLabel>Gövde Tipi & Segment Dağılımı</SectionLabel>
          <VehicleBodySegmentSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Vehicle.Heatmap.View') && (
        <div>
          <SectionLabel>Araç Net Prim Heatmap</SectionLabel>
          <VehicleHeatmapSection productGroup={productGroup} filter={filter} />
        </div>
      )}

    </div>
  )
}