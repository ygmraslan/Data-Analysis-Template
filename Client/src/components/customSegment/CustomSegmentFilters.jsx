import SingleDriftFilters from './SingleDriftFilters'
import CompareDriftFilters from './CompareDriftFilters'

export default function CustomSegmentFilters({
  options,
  optionsLoading,
  filters,
  onChange,
  filtersB,
  onChangeB,
  mode,
  onModeChange,
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
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      {/* Mod Seçimi - Sol üst */}
      <div className="flex items-center gap-1 mb-4 bg-slate-100 dark:bg-white/8 rounded-lg p-1 w-fit">
        <button
          onClick={() => onModeChange('single')}
          className={`px-4 py-1.5 rounded-md text-xs font-semibold transition-all ${
            mode === 'single'
              ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
              : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
          }`}
        >
          Tek Drift
        </button>
        <button
          onClick={() => onModeChange('compare')}
          className={`px-4 py-1.5 rounded-md text-xs font-semibold transition-all ${
            mode === 'compare'
              ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
              : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
          }`}
        >
          Karşılaştırmalı
        </button>
      </div>

      {/* Mod'a göre içerik */}
      {mode === 'single' ? (
        <SingleDriftFilters
          options={options}
          optionsLoading={optionsLoading}
          filters={filters}
          onChange={onChange}
          year={year}
          month={month}
          weeks={weeks}
          selectedWeek={selectedWeek}
          onYearChange={onYearChange}
          onMonthChange={onMonthChange}
          onWeekChange={onWeekChange}
          weeksLoading={weeksLoading}
          onCalculate={onCalculate}
          calculating={calculating}
        />
      ) : (
        <CompareDriftFilters
          options={options}
          optionsLoading={optionsLoading}
          filters={filters}
          onChange={onChange}
          filtersB={filtersB}
          onChangeB={onChangeB}
          year={year}
          month={month}
          weeks={weeks}
          selectedWeek={selectedWeek}
          onYearChange={onYearChange}
          onMonthChange={onMonthChange}
          onWeekChange={onWeekChange}
          weeksLoading={weeksLoading}
          onCalculate={onCalculate}
          calculating={calculating}
        />
      )}
    </div>
  )
}