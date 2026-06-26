import { SkeletonBlock } from '../ui/Skeleton'

const COLOR_A = '#1D9E75'
const COLOR_B = '#378ADD'

function TableSkeleton() {
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
      {[1,2,3,4,5,6,7,8].map(i => <SkeletonBlock key={i} width="w-full" height="h-8" className="mb-2" />)}
    </div>
  )
}

function getWowColor(row) {
  const prevZero = row.prevShare === 0 && row.share > 0
  const isNewFromZero = (row.change === null || row.change === undefined) && prevZero
  if (isNewFromZero) return '#059669'
  if (row.change > 0) return '#059669'
  if (row.change < 0) return '#e11d48'
  return '#94a3b8'
}

function formatWow(row) {
  const prevZero = row.prevShare === 0 && row.share > 0
  const isNewFromZero = (row.change === null || row.change === undefined) && prevZero
  if (isNewFromZero) return 'Yeni ↑'
  if (row.change === null || row.change === undefined) return '—'
  return `${row.change > 0 ? '+' : ''}${row.change.toFixed(2)}%`
}

export default function CustomSegmentTable({ data, dataA, dataB, loading = false, viewMode = 'share' }) {
  if (loading) return <TableSkeleton />

  const isCompare = Array.isArray(dataA) && Array.isArray(dataB)

  if (isCompare) {
    if (dataA.length === 0 || dataB.length === 0) {
      return (
        <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
          <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">Haftalık Karşılaştırma Detayı</p>
          <div className="flex items-center justify-center h-40 text-sm text-slate-400">
            Tablo verisi bulunamadı
          </div>
        </div>
      )
    }

    const isShareMode = viewMode === 'share'

    const merged = dataA.map((rowA, i) => {
      const rowB = dataB[i] || {}
      const valueA = isShareMode ? rowA.share : rowA.segmentCount
      const valueB = isShareMode ? rowB.share : rowB.segmentCount
      return {
        weekLabel: rowA.weekLabel,
        a: { ...rowA, prevShare: i > 0 ? dataA[i - 1].share : null },
        b: { ...rowB, prevShare: i > 0 ? dataB[i - 1].share : null },
        valueA,
        valueB,
        diff: (valueB ?? 0) - (valueA ?? 0)
      }
    })

    const formatValue = (v) => {
      if (v === null || v === undefined) return '-'
      return isShareMode ? `%${v.toFixed(2)}` : v.toLocaleString('tr-TR')
    }

    const formatDiff = (v) => {
      const sign = v > 0 ? '+' : ''
      return isShareMode
        ? `${sign}${v.toFixed(2)}`
        : `${sign}${Math.round(v).toLocaleString('tr-TR')}`
    }

    const valueLabel = isShareMode ? 'Pay' : 'Poliçe'

    return (
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">Haftalık Karşılaştırma Detayı</p>

        <div className="overflow-x-auto">
          <table className="w-full text-xs">
            <thead>
              <tr className="border-b border-slate-100 dark:border-white/8">
                <th className="text-left pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Hafta</th>
                <th className="text-right pb-2 font-semibold uppercase tracking-wide" style={{ color: COLOR_A }}>A {valueLabel}</th>
                <th className="text-right pb-2 font-semibold uppercase tracking-wide" style={{ color: COLOR_A }}>A WoW</th>
                <th className="text-right pb-2 font-semibold uppercase tracking-wide" style={{ color: COLOR_B }}>B {valueLabel}</th>
                <th className="text-right pb-2 font-semibold uppercase tracking-wide" style={{ color: COLOR_B }}>B WoW</th>
                <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Fark</th>
              </tr>
            </thead>
            <tbody>
              {merged.map((row, i) => {
                const isLast = i === merged.length - 1
                const diffColor = row.diff > 0 ? COLOR_B : row.diff < 0 ? COLOR_A : '#94a3b8'

                return (
                  <tr
                    key={row.weekLabel || i}
                    className={`border-b border-slate-50 dark:border-white/4 ${isLast ? 'bg-slate-50 dark:bg-white/4' : ''}`}
                  >
                    <td className={`py-2 font-medium ${isLast ? 'text-slate-900 dark:text-white' : 'text-slate-600 dark:text-slate-300'}`}>
                      {row.weekLabel}
                    </td>
                    <td className={`py-2 text-right tabular-nums font-semibold ${isLast ? 'text-slate-900 dark:text-white' : 'text-slate-700 dark:text-slate-300'}`}>
                      {formatValue(row.valueA)}
                    </td>
                    <td className="py-2 text-right font-bold tabular-nums" style={{ color: getWowColor(row.a) }}>
                      {formatWow(row.a)}
                    </td>
                    <td className={`py-2 text-right tabular-nums font-semibold ${isLast ? 'text-slate-900 dark:text-white' : 'text-slate-700 dark:text-slate-300'}`}>
                      {formatValue(row.valueB)}
                    </td>
                    <td className="py-2 text-right font-bold tabular-nums" style={{ color: getWowColor(row.b) }}>
                      {formatWow(row.b)}
                    </td>
                    <td className="py-2 text-right font-bold tabular-nums" style={{ color: diffColor }}>
                      {formatDiff(row.diff)}
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>
    )
  }

  if (!Array.isArray(data) || data.length === 0) {
    return (
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">8 Haftalık Detay</p>
        <div className="flex items-center justify-center h-40 text-sm text-slate-400">
          Tablo verisi bulunamadı
        </div>
      </div>
    )
  }

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <p className="text-sm font-semibold text-slate-700 dark:text-slate-200 mb-4">8 Haftalık Detay</p>

      <div className="overflow-x-auto">
        <table className="w-full text-xs">
          <thead>
            <tr className="border-b border-slate-100 dark:border-white/8">
              <th className="text-left pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Hafta</th>
              <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Poliçe</th>
              <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">Pay</th>
              <th className="text-right pb-2 font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wide">WoW</th>
            </tr>
          </thead>
          <tbody>
            {data.map((row, i) => {
              const isLast = i === data.length - 1
              const prevRow = i > 0 ? data[i - 1] : null
              const isNewFromZero =
                (row.change === null || row.change === undefined) &&
                i > 0 &&
                prevRow?.share === 0 &&
                row.share > 0

              const wowColor = isNewFromZero
                ? '#059669'
                : row.change > 0 ? '#059669' : row.change < 0 ? '#e11d48' : '#94a3b8'

              return (
                <tr
                  key={row.weekLabel || i}
                  className={`border-b border-slate-50 dark:border-white/4 ${isLast ? 'bg-slate-50 dark:bg-white/4' : ''}`}
                >
                  <td className={`py-2 font-medium ${isLast ? 'text-slate-900 dark:text-white' : 'text-slate-600 dark:text-slate-300'}`}>
                    {row.weekLabel}
                  </td>
                  <td className={`py-2 text-right tabular-nums ${isLast ? 'font-bold text-slate-900 dark:text-white' : 'text-slate-500 dark:text-slate-400'}`}>
                    {row.segmentCount?.toLocaleString('tr-TR')}
                  </td>
                  <td className={`py-2 text-right tabular-nums font-semibold ${isLast ? 'text-slate-900 dark:text-white' : 'text-slate-700 dark:text-slate-300'}`}>
                    %{row.share?.toFixed(2)}
                  </td>
                  <td className="py-2 text-right font-bold tabular-nums" style={{ color: wowColor }}>
                    {isNewFromZero
                      ? 'Yeni ↑'
                      : row.change !== null && row.change !== undefined
                        ? `${row.change > 0 ? '+' : ''}${row.change.toFixed(2)}%`
                        : '—'
                    }
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
    </div>
  )
}