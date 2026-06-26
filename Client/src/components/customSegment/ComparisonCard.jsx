import { useState } from 'react'
import { HiTrash, HiChevronDown, HiChevronRight, HiPlay, HiUser, HiClock, HiRefresh } from 'react-icons/hi'
import { formatDateOnly } from '../../utils/formatDate'
import { formatWeekRange, getFilterParts } from '../../utils/segmentDisplay'
import CompareMetricsSection from './CompareMetricsSection'
import CustomSegmentChart from './CustomSegmentChart'
import CustomSegmentTable from './CustomSegmentTable'
import CustomSegmentAiSection from './CustomSegmentAiSection'
import WeekPicker from './WeekPicker'

const COLOR_A = '#1D9E75'
const COLOR_B = '#378ADD'

function FilterBadge({ label }) {
  return (
    <span className="inline-flex items-center px-2 py-0.5 text-[10px] font-medium rounded-md bg-slate-100 dark:bg-white/10 text-slate-600 dark:text-slate-400">
      {label}
    </span>
  )
}

function SidePreview({ label, color, parts }) {
  const visible = parts.slice(0, 2)
  const overflow = parts.length - visible.length
  return (
    <div className="flex items-center gap-1.5 min-w-0">
      <span className="w-2 h-2 rounded-full flex-shrink-0" style={{ backgroundColor: color }} />
      <span className="text-[10px] font-bold uppercase tracking-wide flex-shrink-0" style={{ color }}>{label}</span>
      <div className="flex flex-wrap gap-1 min-w-0">
        {visible.map((p, i) => <FilterBadge key={i} label={p} />)}
        {overflow > 0 && <span className="text-[10px] text-slate-400">+{overflow}</span>}
      </div>
    </div>
  )
}

function ViewModeToggle({ value, onChange }) {
  const options = [
    { key: 'share', label: 'Pay' },
    { key: 'count', label: 'Poliçe Sayısı' }
  ]
  return (
    <div className="flex items-center bg-slate-100 dark:bg-white/8 rounded-lg p-0.5">
      {options.map(opt => (
        <button
          key={opt.key}
          onClick={() => onChange(opt.key)}
          className={`px-3 py-1.5 rounded-md text-xs font-medium transition-all ${
            value === opt.key
              ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
              : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
          }`}
        >
          {opt.label}
        </button>
      ))}
    </div>
  )
}

function buildSeriesData(weeklyData) {
  const weeks = weeklyData || []
  return weeks.map((w, i, arr) => {
    let wow = w.woW
    if (wow === undefined || wow === null) {
      if (i > 0) {
        const prev = arr[i - 1].segmentShare
        if (prev > 0) {
          wow = parseFloat((((w.segmentShare - prev) / prev) * 100).toFixed(2))
        } else {
          wow = null
        }
      } else {
        wow = null
      }
    }
    return {
      weekLabel: w.weekLabel,
      share: w.segmentShare,
      segmentCount: w.segmentCount,
      totalCount: w.totalPolicy,
      change: wow
    }
  })
}

export default function ComparisonCard({
  comparison,
  isExpanded,
  onToggle,
  onRun,
  onDelete,
  running,
  detail,
  hasRunPermission,
  hasDeletePermission
}) {
  const [selectedWeek, setSelectedWeek] = useState(null)
  const [viewMode, setViewMode] = useState('share')

  const partsA = getFilterParts(comparison.segmentA)
  const partsB = getFilterParts(comparison.segmentB)

  const endShareA = comparison.segmentA?.endShare
  const endShareB = comparison.segmentB?.endShare
  const diff = (endShareB ?? 0) - (endShareA ?? 0)

  const handleRecalculate = () => {
    if (!selectedWeek) return
    onRun(selectedWeek.startDate, selectedWeek.endDate)
  }

  const initialDate = comparison.weekEnd

  const sideA = detail?.segmentA
  const sideB = detail?.segmentB

  const chartDataA = buildSeriesData(sideA?.weeklyData)
  const chartDataB = buildSeriesData(sideB?.weeklyData)

  const aiComments = detail ? {
    deepseek: { comment: detail.aiCommentDeepSeek, loading: false, error: null },
    gemini: { comment: detail.aiCommentGemini, loading: false, error: null },
    gpt: { comment: detail.aiCommentGpt, loading: false, error: null }
  } : null

  const hasAnyAi = detail && (detail.aiCommentDeepSeek || detail.aiCommentGemini || detail.aiCommentGpt)

  return (
    <div className="bg-white dark:bg-white/5 border border-slate-200 dark:border-white/10 rounded-xl overflow-hidden transition-colors">
      <div
        className="flex items-center gap-4 px-5 py-4 cursor-pointer hover:bg-slate-50 dark:hover:bg-white/[0.02] transition-colors"
        onClick={onToggle}
      >
        <div className="flex-shrink-0">
          {isExpanded ? (
            <HiChevronDown className="w-5 h-5 text-slate-400" />
          ) : (
            <HiChevronRight className="w-5 h-5 text-slate-400" />
          )}
        </div>

        <div className="flex-shrink-0 w-72 min-w-0">
          <div className="flex items-center gap-2 mb-2">
            <h3 className="text-sm font-semibold text-slate-800 dark:text-slate-200 truncate">
              {comparison.name}
            </h3>
            <span className="flex-shrink-0 px-2 py-0.5 text-[10px] font-semibold rounded bg-emerald-50 dark:bg-emerald-500/10 text-emerald-700 dark:text-emerald-400">
              {comparison.productGroup}
            </span>
          </div>
          <div className="space-y-1">
            <SidePreview label="A" color={COLOR_A} parts={partsA} />
            <SidePreview label="B" color={COLOR_B} parts={partsB} />
          </div>
        </div>

        <div className="flex-1 grid grid-cols-4 gap-3 px-3 min-w-0">
          <div>
            <div className="text-[10px] uppercase tracking-wide text-slate-400 mb-0.5">Dönem</div>
            <div className="text-xs font-medium text-slate-700 dark:text-slate-300 truncate">
              {formatWeekRange(comparison.weekStart, comparison.weekEnd)}
            </div>
          </div>
          <div>
            <div className="text-[10px] uppercase tracking-wide mb-0.5" style={{ color: COLOR_A }}>A Bitiş Payı</div>
            <div className="text-sm font-semibold text-slate-800 dark:text-slate-200">
              {endShareA !== undefined && endShareA !== null ? `%${endShareA.toFixed(2)}` : '-'}
            </div>
          </div>
          <div>
            <div className="text-[10px] uppercase tracking-wide mb-0.5" style={{ color: COLOR_B }}>B Bitiş Payı</div>
            <div className="text-sm font-semibold text-slate-800 dark:text-slate-200">
              {endShareB !== undefined && endShareB !== null ? `%${endShareB.toFixed(2)}` : '-'}
            </div>
          </div>
          <div>
            <div className="text-[10px] uppercase tracking-wide text-slate-400 mb-0.5">Fark</div>
            <div className="text-sm font-semibold" style={{ color: diff > 0 ? COLOR_B : diff < 0 ? COLOR_A : '#94a3b8' }}>
              {diff > 0 ? '+' : ''}{diff.toFixed(2)} puan
            </div>
          </div>
        </div>

        <div className="flex-shrink-0" onClick={e => e.stopPropagation()}>
          {hasDeletePermission && (
            <button
              onClick={onDelete}
              className="p-2 rounded-lg text-slate-400 hover:text-rose-500 hover:bg-rose-50 dark:hover:bg-rose-500/10 transition-colors"
              title="Sil"
            >
              <HiTrash className="w-4 h-4" />
            </button>
          )}
        </div>
      </div>

      {isExpanded && (
        <div className="border-t border-slate-100 dark:border-white/5 px-5 py-5 space-y-5 bg-slate-50/50 dark:bg-white/[0.01]">
          <div className="flex items-center gap-4 text-[11px] text-slate-500 dark:text-slate-400">
            <div className="flex items-center gap-1.5">
              <HiUser className="w-3.5 h-3.5" />
              <span>Oluşturan: <span className="font-medium text-slate-700 dark:text-slate-300">{comparison.createdByName || 'Bilinmiyor'}</span></span>
            </div>
            <div className="flex items-center gap-1.5">
              <HiClock className="w-3.5 h-3.5" />
              <span>{formatDateOnly(comparison.createdDate)}</span>
            </div>
          </div>

          {hasRunPermission && (
            <div className="rounded-lg border border-slate-200 dark:border-white/10 bg-white dark:bg-white/5 p-4">
              <div className="flex items-center justify-between mb-3">
                <span className="text-[11px] font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400">
                  Yeni tarih aralığı ile hesapla
                </span>
                <span className="text-[10px] text-slate-400">
                  Mevcut: {formatWeekRange(comparison.weekStart, comparison.weekEnd)}
                </span>
              </div>

              <div className="flex flex-col sm:flex-row gap-3 items-stretch sm:items-end">
                <div className="flex-1">
                  <WeekPicker
                    value={selectedWeek}
                    onChange={setSelectedWeek}
                    initialDate={initialDate}
                  />
                </div>
                <button
                  onClick={handleRecalculate}
                  disabled={running || !selectedWeek}
                  className="flex items-center justify-center gap-2 px-5 py-2.5 text-xs font-semibold rounded-lg bg-emerald-600 hover:bg-emerald-700 text-white transition-colors disabled:opacity-50 disabled:cursor-not-allowed whitespace-nowrap"
                >
                  {running ? (
                    <HiRefresh className="w-3.5 h-3.5 animate-spin" />
                  ) : (
                    <HiPlay className="w-3.5 h-3.5" />
                  )}
                  Yeniden Hesapla
                </button>
              </div>
            </div>
          )}

          {running ? (
            <div className="flex flex-col items-center justify-center py-10">
              <HiRefresh className="w-8 h-8 text-emerald-500 animate-spin mb-3" />
              <p className="text-sm text-slate-500 dark:text-slate-400">Analiz hesaplanıyor...</p>
            </div>
          ) : detail ? (
            <>
              <div className="flex items-center justify-between">
                <span className="text-[11px] font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400">
                  Drift Metrikleri
                </span>
                <ViewModeToggle value={viewMode} onChange={setViewMode} />
              </div>

              <CompareMetricsSection
                resultA={sideA}
                resultB={sideB}
                viewMode={viewMode}
              />

              <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                <CustomSegmentChart dataA={chartDataA} dataB={chartDataB} viewMode={viewMode} />
                <CustomSegmentTable dataA={chartDataA} dataB={chartDataB} viewMode={viewMode} />
              </div>

              {hasAnyAi && (
                <CustomSegmentAiSection
                  title="Karşılaştırma Değerlendirmesi"
                  subtitle="Segment A vs Segment B"
                  aiComments={aiComments}
                />
              )}
            </>
          ) : (
            <div className="flex flex-col items-center justify-center py-6 text-center">
              <p className="text-xs text-slate-400 dark:text-slate-500">
                Karşılaştırma detayı yükleniyor...
              </p>
            </div>
          )}
        </div>
      )}
    </div>
  )
}