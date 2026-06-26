import { useState, useEffect } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { exportBrandReport } from '../../api/exportApi'
import * as brandApi from '../../api/brandApi'
import useDetailFilterStore from '../../store/detailFilterStore'
import PageTitle from '../../components/ui/PageTitle'
import SectionLabel from '../../components/ui/SectionLabel'
import ReportExportButton from '../../components/ui/ReportExportButton'
import DetailFilterModal from '../../components/filters/DetailFilterModal'
import BrandKpiSection from '../../components/brand/BrandKpiSection'
import BrandTrendSection from '../../components/brand/BrandTrendSection'
import BrandModelSection from '../../components/brand/BrandModelSection'
import BrandHeatmapSection from '../../components/brand/BrandHeatmapSection'

export default function Brand() {
  const { hasPermission } = usePermission()
  const productGroup    = useDetailFilterStore(s => s.productGroup)
  const filter          = useDetailFilterStore(s => s.filter)
  const setProductGroup = useDetailFilterStore(s => s.setProductGroup)
  const setFilter       = useDetailFilterStore(s => s.setFilter)

  const [defaultBrand,  setDefaultBrand]  = useState('')
  const [selectedBrand, setSelectedBrand] = useState('')

  const today = new Date().toISOString().slice(0, 10).replace(/-/g, '')

  useEffect(() => {
    brandApi.getBrandKpi(productGroup, filter)
      .then(r => {
        const brand = r.data?.defaultBrand ?? ''
        setDefaultBrand(brand)
        setSelectedBrand(brand)
      })
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
      {hasPermission('Brand.Report.Export') && (
        <ReportExportButton
          exportFn={() => exportBrandReport(productGroup,filter)}
          fileName={`MarkaRaporu_${productGroup}_${today}.pdf`}
        />
      )}
    </div>
  )

  return (
    <div className="space-y-6">

      <PageTitle
        title="Marka Analizi — Haftalık"
        action={actions}
      />

      {hasPermission('Brand.Kpi.View') && (
        <div>
          <SectionLabel>Marka Üretim</SectionLabel>
          <BrandKpiSection productGroup={productGroup} filter={filter} />
        </div>
      )}

      {hasPermission('Brand.Trend.View') && (
        <div>
          <SectionLabel>Haftalık Poliçe Trendi & Model Dağılımı</SectionLabel>
          <div className="space-y-4">
            <BrandTrendSection
              productGroup={productGroup}
              defaultBrand={defaultBrand}
              onBrandChange={setSelectedBrand}
              filter={filter}
            />
            {hasPermission('Brand.Models.View') && selectedBrand && (
              <BrandModelSection
                productGroup={productGroup}
                brand={selectedBrand}
                filter={filter}
              />
            )}
          </div>
        </div>
      )}

      {hasPermission('Brand.Heatmap.View') && (
        <div>
          <SectionLabel>Marka Net Prim Heatmap</SectionLabel>
          <BrandHeatmapSection productGroup={productGroup} filter={filter} />
        </div>
      )}

    </div>
  )
}