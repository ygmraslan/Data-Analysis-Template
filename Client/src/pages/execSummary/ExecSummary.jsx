import { useState, useEffect, useCallback, useRef } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { exportExecSummaryPdf, getAvailableWeeks, getCurrentWeek } from '../../api/execSummaryApi'
import PageTitle from '../../components/ui/PageTitle'
import SectionLabel from '../../components/ui/SectionLabel'
import ReportExportButton from '../../components/ui/ReportExportButton'
import ExecAiSection from '../../components/execSummary/ExecAiSection'
import ExecDriftSection from '../../components/execSummary/ExecDriftSection'
import ExecBrandAgeSection from '../../components/execSummary/ExecBrandAgeSection'
import ExecAgeStepSection from '../../components/execSummary/ExecAgeStepSection'
import ExecDistributionSection from '../../components/execSummary/ExecDistributionSection'
import ExecRiskSection from '../../components/execSummary/ExecRiskSection'
import ExecPortfolioSummary from '../../components/execSummary/ExecPortfolioSummary'
import { HiRefresh, HiChartBar, HiChevronDown } from 'react-icons/hi'

function Dropdown({ value, options, onChange, placeholder, minWidth = 'min-w-[60px]' }) {
  const [open, setOpen] = useState(false)
  const ref = useRef(null)
  
  useEffect(() => {
    const handleClick = (e) => { if (ref.current && !ref.current.contains(e.target)) setOpen(false) }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])
  
  const selected = options.find(o => o.value === value)
  
  return (
    <div ref={ref} className={`relative ${minWidth}`}>
      <button
        onClick={() => setOpen(!open)}
        className={`w-full flex items-center justify-between gap-1 px-2 py-1 text-xs font-medium rounded-md transition-all
          ${open 
            ? 'bg-emerald-50 dark:bg-emerald-500/20 text-emerald-700 dark:text-emerald-300' 
            : 'bg-white dark:bg-white/15 text-slate-700 dark:text-white hover:bg-slate-50 dark:hover:bg-white/20'
          }`}
      >
        <span className="truncate">{selected?.label || placeholder}</span>
        <HiChevronDown className={`w-3 h-3 flex-shrink-0 text-slate-400 dark:text-slate-400 transition-transform ${open ? 'rotate-180' : ''}`} />
      </button>
      
      {open && (
        <div className="absolute top-full left-0 mt-1 bg-white dark:bg-[#1a3a5c] border border-slate-200 dark:border-white/15 rounded-lg shadow-xl z-50 py-1 max-h-56 overflow-y-auto">
          {options.map(opt => (
            <button
              key={opt.value}
              onClick={() => { onChange(opt.value); setOpen(false) }}
              className={`w-full text-left px-3 py-1.5 text-xs transition-colors whitespace-nowrap
                ${opt.value === value 
                  ? 'bg-emerald-50 dark:bg-emerald-500/20 text-emerald-600 dark:text-emerald-300 font-medium' 
                  : 'text-slate-600 dark:text-slate-200 hover:bg-slate-50 dark:hover:bg-white/10'
                }`}
            >
              {opt.label}
            </button>
          ))}
        </div>
      )}
    </div>
  )
}

const MONTHS = [
  { value: 1, label: 'Ocak' },
  { value: 2, label: 'Şubat' },
  { value: 3, label: 'Mart' },
  { value: 4, label: 'Nisan' },
  { value: 5, label: 'Mayıs' },
  { value: 6, label: 'Haziran' },
  { value: 7, label: 'Temmuz' },
  { value: 8, label: 'Ağustos' },
  { value: 9, label: 'Eylül' },
  { value: 10, label: 'Ekim' },
  { value: 11, label: 'Kasım' },
  { value: 12, label: 'Aralık' }
]

function formatWeekRange(startDate, endDate) {
  if (!startDate || !endDate) return ''
  
  const start = new Date(startDate)
  const end = new Date(endDate)
  
  const months = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']
  
  const startDay = start.getDate()
  const startMonth = months[start.getMonth()]
  const endDay = end.getDate()
  const endMonth = months[end.getMonth()]
  const year = end.getFullYear()
  
  if (start.getMonth() === end.getMonth()) {
    return `${startDay} - ${endDay} ${endMonth} ${year}`
  }
  
  return `${startDay} ${startMonth} - ${endDay} ${endMonth} ${year}`
}

export default function ExecSummary() {
  const { hasPermission } = usePermission()
  const currentYear = new Date().getFullYear()
  const currentMonth = new Date().getMonth() + 1

  const [productGroup, setProductGroup] = useState('KASKO')
  const [year, setYear] = useState(currentYear)
  const [month, setMonth] = useState(currentMonth)
  const [weeks, setWeeks] = useState([])
  const [selectedWeek, setSelectedWeek] = useState(null)
  const [pendingWeek, setPendingWeek] = useState(null) 
  const [weeksLoading, setWeeksLoading] = useState(false)
  const [initialLoading, setInitialLoading] = useState(true)
  const [analyzing, setAnalyzing] = useState(false)

  const startDate = selectedWeek?.startDate
  const endDate = selectedWeek?.endDate
  const today = new Date().toISOString().slice(0, 10).replace(/-/g, '')
  const years = Array.from({ length: 3 }, (_, i) => currentYear - i)

  useEffect(() => {
    setInitialLoading(true)
    getCurrentWeek()
      .then(res => {
        const week = res.data
        setYear(week.year)
        setMonth(week.month)
        setSelectedWeek(week) 
        setPendingWeek(week)
        return getAvailableWeeks(week.year, week.month)
      })
      .then(res => {
        setWeeks(res.data || [])
      })
      .catch(console.error)
      .finally(() => setInitialLoading(false))
  }, [])

  useEffect(() => {
    if (!year || !month || initialLoading) return
    
    setWeeksLoading(true)
    getAvailableWeeks(year, month)
      .then(res => {
        const weekList = res.data || []
        setWeeks(weekList)
        if (weekList.length > 0) {
          setPendingWeek(weekList[weekList.length - 1]) 
          setPendingWeek(null)
        }
      })
      .catch(console.error)
      .finally(() => setWeeksLoading(false))
  }, [year, month, initialLoading])

  const handleYearChange = (e) => {
    setYear(parseInt(e.target.value))
  }

  const handleMonthChange = (e) => {
    setMonth(parseInt(e.target.value))
  }

  const handleWeekChange = (e) => {
    const weekIndex = parseInt(e.target.value)
    const week = weeks.find(w => w.weekIndex === weekIndex)
    if (week) {
      setPendingWeek(week)
    }
  }

  const handleAnalyze = useCallback(() => {
    if (!pendingWeek) return
    setAnalyzing(true)
    setTimeout(() => {
      setSelectedWeek(pendingWeek)
      setAnalyzing(false)
    }, 200)
  }, [pendingWeek])

  const hasChanges = pendingWeek?.weekIndex !== selectedWeek?.weekIndex

  const productGroupTabs = (
    <div className="flex items-center gap-0.5 bg-slate-100 dark:bg-white/8 rounded-lg p-0.5">
      {['KASKO', 'TRAFİK'].map(pg => (
        <button
          key={pg}
          onClick={() => setProductGroup(pg === 'TRAFİK' ? 'TRAFIK' : pg)}
          className={`px-3 py-1 rounded-md text-xs font-medium transition-all ${
            (pg === 'TRAFİK' ? 'TRAFIK' : pg) === productGroup
              ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
              : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
          }`}
        >
          {pg}
        </button>
      ))}
    </div>
  )

  if (initialLoading) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <HiRefresh className="w-8 h-8 text-emerald-500 animate-spin mb-3" />
        <p className="text-sm text-slate-500 dark:text-slate-400">Veriler yükleniyor...</p>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <PageTitle
        title="Yönetici Özeti"
        subtitle={selectedWeek ? formatWeekRange(selectedWeek.startDate, selectedWeek.endDate) : ''}
        action={
          <div className="flex items-center gap-3 flex-wrap">
            {/* Dönem Seçici */}
            <div className="flex items-center bg-slate-100 dark:bg-slate-800 rounded-lg p-0.5 gap-0.5">
              {/* Yıl */}
              <Dropdown
                value={year}
                options={years.filter(y => y <= currentYear).map(y => ({ value: y, label: String(y) }))}
                onChange={val => setYear(val)}
                minWidth="min-w-[52px]"
              />

              {/* Ay */}
              <Dropdown
                value={month}
                options={MONTHS.filter(m => !(year === currentYear && m.value > currentMonth)).map(m => ({ value: m.value, label: m.label }))}
                onChange={val => setMonth(val)}
                minWidth="min-w-[65px]"
              />

              {/* Hafta */}
              <Dropdown
                value={pendingWeek?.weekIndex}
                options={
                  weeksLoading 
                    ? [{ value: null, label: 'Yükleniyor...' }]
                    : weeks
                        .filter(w => new Date(w.endDate) <= new Date())
                        .map(w => ({ value: w.weekIndex, label: formatWeekRange(w.startDate, w.endDate) }))
                }
                onChange={val => {
                  const week = weeks.find(w => w.weekIndex === val)
                  if (week) setPendingWeek(week)
                }}
                placeholder="Hafta"
                minWidth="min-w-[115px]"
              />
            </div>

            {/* Analiz Et Butonu */}
            <button
              onClick={handleAnalyze}
              disabled={!pendingWeek || analyzing || !hasChanges}
              className={`flex items-center gap-1.5 px-3 py-1.5 text-xs font-semibold rounded-lg transition-all ${
                hasChanges
                  ? 'bg-emerald-500 hover:bg-emerald-600 text-white shadow-sm'
                  : 'bg-slate-100 dark:bg-white/10 text-slate-400 cursor-not-allowed'
              }`}
            >
              {analyzing ? (
                <HiRefresh className="w-3.5 h-3.5 animate-spin" />
              ) : (
                <HiChartBar className="w-3.5 h-3.5" />
              )}
              <span>Analiz Et</span>
            </button>

            {/* Ürün Grubu */}
            {productGroupTabs}

            {/* PDF Export */}
            {hasPermission('ExecSummary.Export') && startDate && endDate && (
              <ReportExportButton
                exportFn={() => exportExecSummaryPdf(productGroup, startDate, endDate)}
                fileName={`YoneticiOzeti_${productGroup}_${today}.pdf`}
              />
            )}
          </div>
        }
      />

      {/* Section'lar */}
      {startDate && endDate ? (
        <>
          {/* 1. AI  Özet */}
          {hasPermission('ExecSummary.View') && (
            <div>
              <SectionLabel>Yapay Zeka Değerlendirmesi</SectionLabel>
              <ExecAiSection productGroup={productGroup} startDate={startDate} endDate={endDate} />
            </div>
          )}

          {/* 2. Portföy Drift Analizi */}
          {hasPermission('ExecSummary.View') && (
            <div>
              <SectionLabel>Portföy Drift Analizi</SectionLabel>
              <ExecDriftSection productGroup={productGroup} startDate={startDate} endDate={endDate} />
            </div>
          )}

          {/* 3. Marka × Araç Yaşı Risk Matrisi */}
          {hasPermission('ExecSummary.View') && (
            <div>
              <SectionLabel>Marka & Araç Yaşı Risk Matrisi</SectionLabel>
              <ExecBrandAgeSection productGroup={productGroup} startDate={startDate} endDate={endDate} />
            </div>
          )}

          {/* 4. Araç Yaşı × Basamak Matrisi */}
          {hasPermission('ExecSummary.View') && (
            <div>
              <SectionLabel>Araç Yaşı × Hasarsızlık Basamak</SectionLabel>
              <ExecAgeStepSection productGroup={productGroup} startDate={startDate} endDate={endDate} />
            </div>
          )}

          {/* 5. Dağılımlar */}
          {hasPermission('ExecSummary.View') && (
            <div>
              <SectionLabel>Portföy Dağılımları</SectionLabel>
              <ExecDistributionSection productGroup={productGroup} startDate={startDate} endDate={endDate} />
            </div>
          )}

          {/* 6. Riskli Segmentler */}
          {hasPermission('ExecSummary.View') && (
            <div>
              <SectionLabel>Riskli Segmentler</SectionLabel>
              <ExecRiskSection productGroup={productGroup} startDate={startDate} endDate={endDate} />
            </div>
          )}

          {/* 7. Portföy Özeti */}
          {hasPermission('ExecSummary.View') && (
            <div>
              <SectionLabel>Portföy Özeti</SectionLabel>
              <ExecPortfolioSummary productGroup={productGroup} startDate={startDate} endDate={endDate} />
            </div>
          )}
        </>
      ) : (
        <div className="flex flex-col items-center justify-center py-16 text-center">
          <div className="w-14 h-14 rounded-xl bg-slate-100 dark:bg-white/5 flex items-center justify-center mb-4">
            <HiChartBar className="w-7 h-7 text-slate-400" />
          </div>
          <p className="text-sm text-slate-500 dark:text-slate-400">
            Dönem seçin ve "Analiz Et" butonuna tıklayın.
          </p>
        </div>
      )}
    </div>
  )
}