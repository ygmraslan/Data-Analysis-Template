import { useState } from 'react'
import { HiRefresh, HiTrash, HiChevronDown, HiChevronRight, HiArrowSmUp, HiArrowSmDown, HiPlay, HiUser, HiClock } from 'react-icons/hi'
import { formatDateOnly } from '../../utils/formatDate'
import { formatWeekRange, getFilterParts } from '../../utils/segmentDisplay'
import CustomSegmentMetrics from './CustomSegmentMetrics'
import CustomSegmentChart from './CustomSegmentChart'
import CustomSegmentTable from './CustomSegmentTable'
import CustomSegmentAiSection from './CustomSegmentAiSection'
import WeekPicker from './WeekPicker'

function FilterBadge({ label }) {
  return (
    <span className="inline-flex items-center px-2 py-0.5 text-[10px] font-medium rounded-md bg-slate-100 dark:bg-white/10 text-slate-600 dark:text-slate-400">
      {label}
    </span>
  )
}

export default function SegmentCard({
  segment,
  isExpanded,
  onToggle,
  onRun,
  onDelete,
  running,
  result,
  hasRunPermission,
  hasDeletePermission
}) {
  const [selectedWeek, setSelectedWeek] = useState(null)

  const initialDate = result?.endDate

  const filterParts = getFilterParts(segment.filters)

  const weeklyWithWoW = (result?.weeklyData || []).map((w, i, arr) => {
    if (w.woW !== undefined && w.woW !== null) {
      return w
    }
    let wow = null
    if (i > 0) {
      const prevShare = arr[i - 1].segmentShare
      if (prevShare > 0) {
        wow = parseFloat((((w.segmentShare - prevShare) / prevShare) * 100).toFixed(2))
      }
    }
    return { ...w, woW: wow }
  })

  const metrics = result ? {
    startShare: result.startShare,
    endShare: result.endShare,
    change: result.change,
    growth: result.growthMultiple,
    totalPolicy: result.totalPolicy,
    segmentCount: result.segmentCount
  } : null

  const chartData = weeklyWithWoW.map(w => ({
    weekLabel: w.weekLabel,
    share: w.segmentShare,
    segmentCount: w.segmentCount,
    totalCount: w.totalPolicy,
    change: w.woW
  }))

  const tableData = weeklyWithWoW.map(w => ({
    weekLabel: w.weekLabel,
    totalCount: w.totalPolicy,
    segmentCount: w.segmentCount,
    share: w.segmentShare,
    change: w.woW
  }))

  const handleRecalculate = () => {
    if (!selectedWeek) return
    onRun(selectedWeek.startDate, selectedWeek.endDate)
  }

  const last = segment.lastResult
  const lastChange = last?.change
  const isDown = typeof lastChange === 'number' && lastChange < 0

  return (
    <div className="bg-white dark:bg-white/5 border border-slate-200 dark:border-white/10 rounded-xl overflow-hidden transition-colors">
      {/* HEADER */}
      <div
        className="flex items-center gap-4 px-5 py-4 cursor-pointer hover:bg-slate-50 dark:hover:bg-white/[0.02] transition-colors"
        onClick={onToggle}
      >
        {/* Chevron */}
        <div className="flex-shrink-0">
          {isExpanded ? (
            <HiChevronDown className="w-5 h-5 text-slate-400" />
          ) : (
            <HiChevronRight className="w-5 h-5 text-slate-400" />
          )}
        </div>

        {/* Sol: Ad + badges */}
        <div className="flex-shrink-0 w-64 min-w-0">
          <div className="flex items-center gap-2 mb-1.5">
            <h3 className="text-sm font-semibold text-slate-800 dark:text-slate-200 truncate">
              {segment.name}
            </h3>
            <span className="flex-shrink-0 px-2 py-0.5 text-[10px] font-semibold rounded bg-emerald-50 dark:bg-emerald-500/10 text-emerald-700 dark:text-emerald-400">
              {segment.productGroup}
            </span>
          </div>
          <div className="flex flex-wrap gap-1">
            {filterParts.slice(0, 4).map((p, i) => <FilterBadge key={i} label={p} />)}
            {filterParts.length > 4 && (
              <span className="text-[10px] text-slate-400">+{filterParts.length - 4}</span>
            )}
          </div>
        </div>

        {/* Orta: Metrik şeridi */}
        {last ? (
          <div className="flex-1 grid grid-cols-4 gap-3 px-3 min-w-0">
            <div>
              <div className="text-[10px] uppercase tracking-wide text-slate-400 mb-0.5">Dönem</div>
              <div className="text-xs font-medium text-slate-700 dark:text-slate-300 truncate">
                {formatWeekRange(last.startDate, last.endDate)}
              </div>
            </div>
            <div>
              <div className="text-[10px] uppercase tracking-wide text-slate-400 mb-0.5">Bitiş payı</div>
              <div className="text-sm font-semibold text-slate-800 dark:text-slate-200">
                %{last.endShare?.toFixed(2)}
              </div>
            </div>
            <div>
              <div className="text-[10px] uppercase tracking-wide text-slate-400 mb-0.5">Değişim</div>
              <div className={`text-sm font-semibold flex items-center gap-0.5 ${isDown ? 'text-rose-600 dark:text-rose-400' : 'text-emerald-600 dark:text-emerald-400'}`}>
                {isDown ? <HiArrowSmDown className="w-4 h-4" /> : <HiArrowSmUp className="w-4 h-4" />}
                {Math.abs(lastChange)?.toFixed(2)}%
              </div>
            </div>
            <div>
              <div className="text-[10px] uppercase tracking-wide text-slate-400 mb-0.5">Son hesaplama</div>
              <div className="text-xs font-medium text-slate-700 dark:text-slate-300 truncate">
                {last.createdByName || 'Bilinmiyor'}
                <span className="text-slate-400"> · {formatDateOnly(last.createdDate)}</span>
              </div>
            </div>
          </div>
        ) : (
          <div className="flex-1 text-xs text-slate-400 italic">
            Henüz hesaplama yok
          </div>
        )}

        {/* Sağ: Sil */}
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

      {/* EXPANDED */}
      {isExpanded && (
        <div className="border-t border-slate-100 dark:border-white/5 px-5 py-5 space-y-5 bg-slate-50/50 dark:bg-white/[0.01]">
          {/* Oluşturan bilgisi */}
          <div className="flex items-center gap-4 text-[11px] text-slate-500 dark:text-slate-400">
            <div className="flex items-center gap-1.5">
              <HiUser className="w-3.5 h-3.5" />
              <span>Oluşturan: <span className="font-medium text-slate-700 dark:text-slate-300">{segment.createdByName || 'Bilinmiyor'}</span></span>
            </div>
            <div className="flex items-center gap-1.5">
              <HiClock className="w-3.5 h-3.5" />
              <span>{formatDateOnly(segment.createdDate)}</span>
            </div>
          </div>

          {/* Tarih seçici + Yeniden hesapla */}
          {hasRunPermission && (
            <div className="rounded-lg border border-slate-200 dark:border-white/10 bg-white dark:bg-white/5 p-4">
              <div className="flex items-center justify-between mb-3">
                <span className="text-[11px] font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400">
                  Yeni tarih aralığı ile hesapla
                </span>
                {result?.endDate && result?.weeklyData?.length > 0 && (
                  <span className="text-[10px] text-slate-400">
                    Mevcut: {formatWeekRange(
                      result.weeklyData[result.weeklyData.length - 1].weekStart,
                      result.endDate
                    )}
                  </span>
                )}
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
          ) : result ? (
            <>
              <CustomSegmentMetrics data={metrics} />

              <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                <CustomSegmentChart data={chartData} title="Haftalık segment payı trendi" />
                <CustomSegmentTable data={tableData} title="Haftalık detay" />
              </div>

              {(result.aiCommentDeepSeek || result.aiCommentGemini || result.aiCommentGpt) && (
                <CustomSegmentAiSection
                  weekStart={result.weeklyData?.[0]?.weekStart}
                  weekEnd={result.weeklyData?.[result.weeklyData.length - 1]?.weekStart}
                  aiComments={{
                    deepseek: { comment: result.aiCommentDeepSeek, loading: false, error: null },
                    gemini: { comment: result.aiCommentGemini, loading: false, error: null },
                    gpt: { comment: result.aiCommentGpt, loading: false, error: null }
                  }}
                />
              )}

              {result.fromCache && (
                <div className="text-center">
                  <span className="text-[10px] text-slate-400 dark:text-slate-500">
                    📦 Önbellekten yüklendi
                  </span>
                </div>
              )}
            </>
          ) : (
            <div className="flex flex-col items-center justify-center py-6 text-center">
              <p className="text-xs text-slate-400 dark:text-slate-500">
                Henüz bir sonuç yok. Yukarıdan hafta seçip "Yeniden Hesapla" butonuna tıklayın.
              </p>
            </div>
          )}
        </div>
      )}
    </div>
  )
}