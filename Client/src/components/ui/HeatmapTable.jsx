import { useState } from 'react'
import useThemeStore from '../../store/themeStore'
import ExportButton from './ExportButton'

const PAGE_SIZE = 10

function heatColor(ratio) {
  const red    = [248, 105, 107]
  const yellow = [255, 235, 132]
  const green  = [99, 190, 123]
  let r, g, b
  if (ratio <= 0.5) {
    const t = ratio * 2
    r = Math.round(red[0] + (yellow[0] - red[0]) * t)
    g = Math.round(red[1] + (yellow[1] - red[1]) * t)
    b = Math.round(red[2] + (yellow[2] - red[2]) * t)
  } else {
    const t = (ratio - 0.5) * 2
    r = Math.round(yellow[0] + (green[0] - yellow[0]) * t)
    g = Math.round(yellow[1] + (green[1] - yellow[1]) * t)
    b = Math.round(yellow[2] + (green[2] - yellow[2]) * t)
  }
  return `rgba(${r},${g},${b},1)`
}

export default function HeatmapTable({ data = [], onExport, exportLoading, rowLabel = 'Marka', title }) {
  const isDark = useThemeStore(s => s.isDark)
  const [page, setPage] = useState(0)
  const [metric, setMetric] = useState('avgPremium') // 'avgPremium' | 'policyRatio'

  if (!data.length) return (
    <div className="flex items-center justify-center py-10 text-sm text-slate-400">
      Veri bulunamadı
    </div>
  )

 const weeks     = [...new Set(data.map(d => d.week))]
  const lastWeek  = weeks[weeks.length - 1]

  const valueMap = {}
  const ratioMap = {}
  data.forEach(d => {
    valueMap[`${d.brand}__${d.week}`] = d.avgNetPremium
    ratioMap[`${d.brand}__${d.week}`] = d.policyRatio
  })
  const getValue = (brand, week) => {
    if (metric === 'avgPremium') {
      return valueMap[`${brand}__${week}`] || 0
    }
    return ratioMap[`${brand}__${week}`] || 0
  }
  const allBrands = [...new Set(data.map(d => d.brand))]
    .sort((a, b) => getValue(b, lastWeek) - getValue(a, lastWeek))
  const totalPages = Math.ceil(allBrands.length / PAGE_SIZE)
  const brands     = allBrands.slice(page * PAGE_SIZE, (page + 1) * PAGE_SIZE)

  const borderColor = isDark ? 'rgba(255,255,255,0.08)' : 'rgba(0,0,0,0.08)'

  const metricTitle = metric === 'avgPremium' ? 'Ort. Yazılan Prim' : 'Poliçe Payı'
  const displayTitle = title || `${rowLabel} × Hafta — ${metricTitle}`

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <div>
          <p className="text-sm font-semibold text-slate-800 dark:text-white">
            {displayTitle}
          </p>
          <p className="text-xs text-slate-400 mt-0.5">
            {weeks[0]} → {lastWeek} · {weeks.length} hafta · {allBrands.length} {rowLabel.toLowerCase()}
          </p>
        </div>
        <div className="flex items-center gap-3">
          {/* Toggle Butonları */}
          <div className="flex items-center bg-slate-100 dark:bg-white/8 rounded-lg p-0.5">
            <button
              onClick={() => setMetric('avgPremium')}
              className={`px-3 py-1.5 rounded-md text-xs font-medium transition-all ${
                metric === 'avgPremium'
                  ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                  : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
              }`}
            >
              Ortalama Yazılan Prim
            </button>
            <button
              onClick={() => setMetric('policyRatio')}
              className={`px-3 py-1.5 rounded-md text-xs font-medium transition-all ${
                metric === 'policyRatio'
                  ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                  : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
              }`}
            >
              Poliçe Payı
            </button>
          </div>
          {onExport && (
            <ExportButton onClick={onExport} loading={exportLoading} />
          )}
        </div>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-xs" style={{ borderCollapse: 'collapse' }}>
          <thead>
            <tr>
              <th
                className="text-left py-2 px-3 text-slate-500 dark:text-slate-400 font-medium sticky left-0 bg-white dark:bg-[#0d1f3c] min-w-[110px]"
                style={{ borderBottom: `2px solid ${borderColor}` }}
              >
                {rowLabel}
              </th>
              {weeks.map(w => (
                <th
                  key={w}
                  className={`text-center py-2 px-2 font-medium whitespace-nowrap min-w-[90px] text-xs ${
                    w === lastWeek
                      ? 'text-slate-700 dark:text-slate-200'
                      : 'text-slate-400 dark:text-slate-500'
                  }`}
                  style={{ borderBottom: `2px solid ${borderColor}` }}
                >
                  {w}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {brands.map(brand => {
              const rowVals  = weeks.map(w => getValue(brand, w)).filter(v => v > 0)
              const rowMin   = Math.min(...rowVals)
              const rowMax   = Math.max(...rowVals)
              const rowRatio = (v) => rowMax === rowMin ? 0.5 : (v - rowMin) / (rowMax - rowMin)

              return (
                <tr key={brand}>
                  <td
                    className="py-2 px-3 text-slate-700 dark:text-slate-200 font-semibold whitespace-nowrap sticky left-0 bg-white dark:bg-[#0d1f3c]"
                    style={{ borderBottom: `1px solid ${borderColor}`, borderRight: `1px solid ${borderColor}` }}
                  >
                    {brand}
                  </td>
                  {weeks.map(week => {
                    const val   = getValue(brand, week)
                    const ratio = val > 0 ? rowRatio(val) : null
                    const bg    = ratio !== null ? heatColor(ratio) : 'transparent'
                    const color = ratio !== null ? '#1a1a1a' : '#94a3b8'

                    const displayVal = val > 0
                      ? metric === 'avgPremium'
                        ? Number(val).toLocaleString('tr-TR', { maximumFractionDigits: 0 })
                        : `%${Number(val).toLocaleString('tr-TR', { minimumFractionDigits: 1, maximumFractionDigits: 1 })}`
                      : '—'

                    return (
                      <td
                        key={week}
                        className="text-center py-2 px-2 whitespace-nowrap"
                        style={{
                          background: bg,
                          color,
                          fontSize: '11px',
                          fontWeight: week === lastWeek ? '700' : '500',
                          border: `1px solid ${borderColor}`,
                        }}
                      >
                        {displayVal}
                      </td>
                    )
                  })}
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>

      <div className="flex items-center justify-between mt-4">
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2">
            <span className="text-xs text-slate-400">Düşük</span>
            <div className="flex gap-0.5">
              {[0, 0.17, 0.33, 0.5, 0.67, 0.83, 1].map((r, i) => (
                <div key={i} className="w-7 h-3 rounded-sm" style={{ background: heatColor(r) }} />
              ))}
            </div>
            <span className="text-xs text-slate-400">Yüksek</span>
          </div>
          {metric === 'policyRatio' && (
            <span className="text-xs text-slate-400 italic">
              * Her hafta toplam %100 — tüm {rowLabel.toLowerCase()}lerin poliçe adedine göre pay
            </span>
          )}
        </div>

        {totalPages > 1 && (
          <div className="flex items-center gap-3">
            <span className="text-xs text-slate-400">
              {page * PAGE_SIZE + 1}–{Math.min((page + 1) * PAGE_SIZE, allBrands.length)} / {allBrands.length}
            </span>
            <div className="flex items-center gap-1">
              <button
                onClick={() => setPage(p => Math.max(0, p - 1))}
                disabled={page === 0}
                className="p-1.5 rounded-lg border border-slate-200 dark:border-white/10 text-slate-500 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-white/5 disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
              >
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <polyline points="15 18 9 12 15 6" />
                </svg>
              </button>
              <button
                onClick={() => setPage(p => Math.min(totalPages - 1, p + 1))}
                disabled={page === totalPages - 1}
                className="p-1.5 rounded-lg border border-slate-200 dark:border-white/10 text-slate-500 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-white/5 disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
              >
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <polyline points="9 18 15 12 9 6" />
                </svg>
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}