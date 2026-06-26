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

export default function SingleDriftFilters({
  options,
  optionsLoading,
  filters,
  onChange,
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

  const handleFilterChange = (key, value) => {
    onChange(applyInsuredTypeRule(filters, key, value))
  }

  const tuzelLock = isTuzelSelected(filters?.insuredTypes)

  const getFilterSummary = () => {
    const parts = []
    if (filters?.brands?.length > 0) parts.push(filters.brands.length === 1 ? filters.brands[0] : `${filters.brands.length} marka`)
    if (filters?.insuredAges?.length > 0) parts.push(filters.insuredAges.length === 1 ? filters.insuredAges[0] : `${filters.insuredAges.length} yaş`)
    if (filters?.insuredTypes?.length > 0) parts.push(filters.insuredTypes.join(', '))
    if (filters?.genders?.length > 0) parts.push(filters.genders.join(', '))
    if (filters?.vehicleAges?.length > 0) parts.push(filters.vehicleAges.length === 1 ? `${filters.vehicleAges[0]} yaş araç` : `${filters.vehicleAges.length} araç yaşı`)
    if (filters?.vehicleValues?.length > 0) parts.push(filters.vehicleValues.length === 1 ? filters.vehicleValues[0] : `${filters.vehicleValues.length} bedel`)
    return parts.length > 0 ? parts.join(' • ') : 'Filtre seçilmedi'
  }

  return (
    <div className="border-2 border-emerald-200 dark:border-emerald-500/30 rounded-xl p-4 bg-emerald-50/30 dark:bg-emerald-500/5">
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {/* Satır 1 - Filtreler */}
        <Dropdown
          label="Marka"
          value={filters.brands}
          options={options?.brands || []}
          onChange={(v) => handleFilterChange('brands', v)}
          loading={optionsLoading}
        />
        <Dropdown
          label="Sigortalı Yaşı"
          value={filters.insuredAges}
          options={options?.insuredAges || []}
          onChange={(v) => handleFilterChange('insuredAges', v)}
          loading={optionsLoading}
          disabled={tuzelLock}
          lockedHint={tuzelLock ? 'Tüzel müşteride yaş otomatik 0-17' : null}
        />
        <Dropdown
          label="Sigortalı Türü"
          value={filters.insuredTypes}
          options={options?.insuredTypes || []}
          onChange={(v) => handleFilterChange('insuredTypes', v)}
          loading={optionsLoading}
        />

        {/* Satır 2 - Filtreler */}
        <Dropdown
          label="Cinsiyet"
          value={filters.genders}
          options={options?.genders || []}
          onChange={(v) => handleFilterChange('genders', v)}
          loading={optionsLoading}
        />
        <Dropdown
          label="Araç Yaşı"
          value={filters.vehicleAges}
          options={options?.vehicleAges || []}
          onChange={(v) => handleFilterChange('vehicleAges', v)}
          loading={optionsLoading}
        />
        <Dropdown
          label="Araç Bedeli"
          value={filters.vehicleValues}
          options={options?.vehicleValues || []}
          onChange={(v) => handleFilterChange('vehicleValues', v)}
          loading={optionsLoading}
        />

        {/* Satır 3 - Tarih */}
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
      <div className="flex items-center justify-between mt-4 pt-4 border-t border-emerald-200 dark:border-emerald-500/20">
        <div className="text-[11px]">
          <span className="font-semibold text-slate-400 dark:text-slate-500 uppercase">Seçilen:</span>
          <span className="text-slate-600 dark:text-slate-300 ml-2">{getFilterSummary()}</span>
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
          Drift Hesapla
        </button>
      </div>
    </div>
  )
}