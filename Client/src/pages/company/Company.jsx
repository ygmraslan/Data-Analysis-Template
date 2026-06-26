import { useState, useEffect, useMemo } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { exportCompanyReport } from '../../api/exportApi'
import * as companyApi from '../../api/companyApi'
import useDetailFilterStore from '../../store/detailFilterStore'
import { stripBusinessSource } from '../../utils/detailFilter'
import PageTitle from '../../components/ui/PageTitle'
import SectionLabel from '../../components/ui/SectionLabel'
import ReportExportButton from '../../components/ui/ReportExportButton'
import DetailFilterModal from '../../components/filters/DetailFilterModal'
import CompanyKpiSection from '../../components/company/CompanyKpiSection'
import CompanyListSection from '../../components/company/CompanyListSection'
import CompanyRenewalSection from '../../components/company/CompanyRenewalSection'
import CompanyTrendSection from '../../components/company/CompanyTrendSection'
import CompanyHeatmapSection from '../../components/company/CompanyHeatmapSection'

export default function Company() {
  const { hasPermission } = usePermission()
  const productGroup    = useDetailFilterStore(s => s.productGroup)
  const filter          = useDetailFilterStore(s => s.filter)
  const setProductGroup = useDetailFilterStore(s => s.setProductGroup)
  const setFilter       = useDetailFilterStore(s => s.setFilter)
   const companyFilter = useMemo(() => stripBusinessSource(filter), [filter])

  const [defaultCompany,   setDefaultCompany]   = useState('')
  const [selectedCompany,  setSelectedCompany]  = useState('')
  const [companyList,      setCompanyList]      = useState([])

  const today = new Date().toISOString().slice(0, 10).replace(/-/g, '')

  useEffect(() => {
    companyApi.getCompanyKpi(productGroup, companyFilter)
      .then(r => {
        const company = r.data?.defaultCompany ?? ''
        setDefaultCompany(company)
        setSelectedCompany(company)
      })
      .catch(() => {})

    companyApi.getCompanyList(productGroup, companyFilter)
      .then(r => setCompanyList(r.data))
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
      <DetailFilterModal
        productGroup={productGroup}
        value={filter}
        onChange={setFilter}
        enabled={['insuredType', 'product', 'vehicleType']}
      />
      {hasPermission('Company.Report.Export') && (
        <ReportExportButton
          exportFn={() => exportCompanyReport(productGroup,filter)}
          fileName={`SirketGecisRaporu_${productGroup}_${today}.pdf`}
        />
      )}
    </div>
  )

  return (
    <div className="space-y-6">

      <PageTitle
        title="Şirket Geçiş Analizi — Haftalık"
        action={actions}
      />

      {/* KPI Section */}
      {hasPermission('Company.Kpi.View') && (
        <div>
          <SectionLabel>Şirket Geçiş</SectionLabel>
          <CompanyKpiSection 
            productGroup={productGroup} 
            filter={companyFilter}
            onDefaultCompany={(company) => {
              setDefaultCompany(company)
              if (!selectedCompany) setSelectedCompany(company)
            }}
          />
        </div>
      )}

      {/* List & Renewal Trend Section */}
      {(hasPermission('Company.List.View') || hasPermission('Company.Renewal.View')) && (
        <div>
          <SectionLabel>Şirket Sıralaması & Yenileme Tipi Trendi</SectionLabel>
          <div className="grid grid-cols-1 xl:grid-cols-2 gap-4">
            {hasPermission('Company.List.View') && (
              <CompanyListSection
                productGroup={productGroup}
                selectedCompany={selectedCompany}
                onCompanySelect={setSelectedCompany}
                filter={companyFilter}
              />
            )}
            {hasPermission('Company.Renewal.View') && (
              <CompanyRenewalSection productGroup={productGroup} filter={companyFilter} />
            )}
          </div>
        </div>
      )}

      {/* Company Trend & Profile Section */}
      {(hasPermission('Company.Trend.View') || hasPermission('Company.Profile.View')) && (
        <div>
          <SectionLabel>Seçili Şirket Profili</SectionLabel>
          <CompanyTrendSection
            productGroup={productGroup}
            defaultCompany={defaultCompany}
            companyList={companyList}
            onCompanyChange={setSelectedCompany}
            filter={companyFilter}
          />
        </div>
      )}

      {/* Heatmap Section */}
      {hasPermission('Company.Heatmap.View') && (
        <div>
          <SectionLabel>Şirket Geçiş Heatmap</SectionLabel>
          <CompanyHeatmapSection productGroup={productGroup} filter={companyFilter} />
        </div>
      )}

    </div>
  )
}