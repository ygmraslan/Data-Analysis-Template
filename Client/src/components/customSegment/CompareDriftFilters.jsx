import { Dropdown, SingleDropdown } from './Dropdown'
import { applyInsuredTypeRule, isTuzelSelected } from '../../utils/customSegmentFilters'

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

// ========================================
// SEGMENT PANEL
// ========================================

function SegmentPanel({ title, color, filters, onChange, options, optionsLoading }) {
  const handleChange = (key, value) => {
    onChange(applyInsuredTypeRule(filters, key, value))
  }

  const tuzelLock = isTuzelSelected(filters?.insuredTypes)

  const borderColor = color === 'emerald' 
    ? 'border-emerald-200 dark:border-emerald-500/30' 
    : 'border-blue-200 dark:border-blue-500/30'
  
  const headerBg = color === 'emerald'
    ? 'bg-emerald-50 dark:bg-emerald-500/10'
    : 'bg-blue-50 dark:bg-blue-500/10'

  const headerText = color === 'emerald'
    ? 'text-emerald-700 dark:text-emerald-300'
    : 'text-blue-700 dark:text-blue-300'

  return (
    <div className={`border ${borderColor} rounded-lg`}>
      {/* Header */}
      <div className={`px-4 py-2.5 ${headerBg} rounded-t-lg`}>
        <h4 className={`text-sm font-semibold ${headerText}`}>{title}</h4>
      </div>

      {/* Filtreler - 3 kolon grid */}
      <div className="p-4 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        <Dropdown
          label="Marka"
          value={filters.brands}
          options={options?.brands || []}
          onChange={(v) => handleChange('brands', v)}
          loading={optionsLoading}
        />
        <Dropdown
          label="Sigortalı Yaşı"
          value={filters.insuredAges}
          options={options?.insuredAges || []}
          onChange={(v) => handleChange('insuredAges', v)}
          loading={optionsLoading}
          disabled={tuzelLock}
          lockedHint={tuzelLock ? 'Tüzel müşteride yaş otomatik 0-17' : null}
        />
        <Dropdown
          label="Sigortalı Türü"
          value={filters.insuredTypes}
          options={options?.insuredTypes || []}
          onChange={(v) => handleChange('insuredTypes', v)}
          loading={optionsLoading}
        />
        <Dropdown
          label="Cinsiyet"
          value={filters.genders}
          options={options?.genders || []}
          onChange={(v) => handleChange('genders', v)}
          loading={optionsLoading}
        />
        <Dropdown
          label="Araç Yaşı"
          value={filters.vehicleAges}
          options={options?.vehicleAges || []}
          onChange={(v) => handleChange('vehicleAges', v)}
          loading={optionsLoading}
        />
        <Dropdown
          label="Araç Bedeli"
          value={filters.vehicleValues}
          options={options?.vehicleValues || []}
          onChange={(v) => handleChange('vehicleValues', v)}
          loading={optionsLoading}
        />
      </div>
    </div>
  )
}

// ========================================
// MAIN COMPONENT
// ========================================

export default function CompareDriftFilters({
  options,
  optionsLoading,
  filters,
  onChange,
  filtersB,
  onChangeB,
  year,
  month,
  weeks,
  selectedWeek,
  onYearChange,
  onMonthChange,
  onWeekChange,
  weeksLoading,
  onCalculate,
  calculating
}) {
  const currentYear = new Date().getFullYear()
  const currentMonth = new Date().getMonth() + 1
  const years = Array.from({ length: 3 }, (_, i) => currentYear - i)

  const formatWeekRange = (startDate, endDate) => {
    if (!startDate || !endDate) return ''
    const start = new Date(startDate)
    const end = new Date(endDate)
    const months = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']
    return `${start.getDate()} - ${end.getDate()} ${months[end.getMonth()]} ${end.getFullYear()}`
  }

  const getFilterSummary = (f) => {
    const parts = []
    if (f?.brands?.length > 0) parts.push(f.brands.length === 1 ? f.brands[0] : `${f.brands.length} marka`)
    if (f?.insuredAges?.length > 0) parts.push(f.insuredAges.length === 1 ? f.insuredAges[0] : `${f.insuredAges.length} yaş`)
    if (f?.insuredTypes?.length > 0) parts.push(f.insuredTypes.join(', '))
    if (f?.genders?.length > 0) parts.push(f.genders.join(', '))
    if (f?.vehicleAges?.length > 0) parts.push(f.vehicleAges.length === 1 ? `${f.vehicleAges[0]} yaş araç` : `${f.vehicleAges.length} araç yaşı`)
    if (f?.vehicleValues?.length > 0) parts.push(f.vehicleValues.length === 1 ? f.vehicleValues[0] : `${f.vehicleValues.length} bedel`)
    return parts.length > 0 ? parts.join(' • ') : 'Filtre seçilmedi'
  }

  return (
    <>
      {/* İki Segment Paneli - Alt Alta */}
      <div className="space-y-4 mb-4">
        <SegmentPanel
          title="Segment A"
          color="emerald"
          filters={filters}
          onChange={onChange}
          options={options}
          optionsLoading={optionsLoading}
        />
        <SegmentPanel
          title="Segment B"
          color="blue"
          filters={filtersB}
          onChange={onChangeB}
          options={options}
          optionsLoading={optionsLoading}
        />
      </div>

      {/* Tarih Seçimi - Ortak */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 mb-4 p-3 bg-slate-50 dark:bg-white/5 rounded-lg">
        <SingleDropdown
          label="Yıl"
          value={year}
          options={years.filter(y => y <= currentYear).map(y => ({ value: y, label: String(y) }))}
          onChange={onYearChange}
        />
        <SingleDropdown
          label="Ay"
          value={month}
          options={MONTHS.filter(m => !(year === currentYear && m.value > currentMonth))}
          onChange={onMonthChange}
        />
        <SingleDropdown
          label="Hafta"
          value={selectedWeek?.weekIndex}
          options={
            weeksLoading
              ? [{ value: null, label: 'Yükleniyor...' }]
              : weeks
                  .filter(w => new Date(w.endDate) <= new Date())
                  .map(w => ({ value: w.weekIndex, label: formatWeekRange(w.startDate, w.endDate) }))
          }
          onChange={(val) => {
            const week = weeks.find(w => w.weekIndex === val)
            if (week) onWeekChange(week)
          }}
          loading={weeksLoading}
          placeholder="Hafta seçin"
        />
      </div>

      {/* Alt Kısım - Özet + Buton */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 pt-4 border-t border-slate-100 dark:border-white/5">
        <div className="flex flex-col sm:flex-row gap-2 sm:gap-6 text-[11px]">
          <div>
            <span className="font-semibold text-emerald-600 dark:text-emerald-400">A:</span>
            <span className="text-slate-600 dark:text-slate-300 ml-1">{getFilterSummary(filters)}</span>
          </div>
          <div>
            <span className="font-semibold text-blue-600 dark:text-blue-400">B:</span>
            <span className="text-slate-600 dark:text-slate-300 ml-1">{getFilterSummary(filtersB)}</span>
          </div>
        </div>
        <button
          onClick={onCalculate}
          disabled={calculating}
          className="flex items-center gap-2 px-4 py-2 text-xs font-semibold rounded-lg bg-emerald-500 hover:bg-emerald-600 text-white transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {calculating ? (
            <svg className="w-3.5 h-3.5 animate-spin" viewBox="0 0 24 24" fill="none">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
            </svg>
          ) : (
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
              <polyline points="22 4 12 14.01 9 11.01" />
            </svg>
          )}
          Karşılaştır
        </button>
      </div>
    </>
  )
}