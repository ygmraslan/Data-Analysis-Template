import { PieChart as RechartsPie, Pie, Cell, Tooltip, ResponsiveContainer } from 'recharts'
import { getLastWeekRange } from '../../utils/formatDate'

const COLORS = [
  '#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6',
  '#06B6D4', '#F97316', '#EC4899', '#84CC16', '#64748B',
]

const CustomTooltip = ({ active, payload }) => {
  if (!active || !payload?.length) return null
  const { name, value } = payload[0]
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg text-xs">
      <span className="text-slate-700 dark:text-slate-200 font-semibold">{name}</span>
      <span className="text-slate-500 dark:text-slate-400 ml-2">%{value}</span>
    </div>
  )
}

export default function PieChart({ items = [], title, topN = 5 }) {
  if (!items.length) return null

  const weekRange = getLastWeekRange()

  let display = items
  if (items.length > topN) {
    const top  = items.slice(0, topN)
    const rest = items.slice(topN).reduce((s, i) => s + Number(i.share), 0)
    display = [...top, { label: 'Diğerleri', share: Math.round(rest * 10) / 10 }]
  }

  return (
    <div>
    {title && (
      <div className="mb-4">
        <p className="text-sm font-semibold text-slate-800 dark:text-white">{title}</p>
        <p className="text-xs text-slate-400 dark:text-slate-500 mt-0.5">{weekRange}</p>
      </div>
    )}

      <div className="flex items-center gap-4">
        {/* Donut */}
        <div className="flex-shrink-0 w-[130px] h-[130px]">
          <ResponsiveContainer width="100%" height="100%">
            <RechartsPie>
              <Pie
                data={display}
                dataKey="share"
                nameKey="label"
                cx="50%"
                cy="50%"
                innerRadius="62%"
                outerRadius="90%"
                paddingAngle={2}
                startAngle={90}
                endAngle={-270}
              >
                {display.map((_, i) => (
                  <Cell key={i} fill={COLORS[i % COLORS.length]} stroke="none" />
                ))}
              </Pie>
              <Tooltip content={<CustomTooltip />} />
            </RechartsPie>
          </ResponsiveContainer>
        </div>

        {/* Legend — oranlar burada */}
        <div className="flex-1 min-w-0 flex flex-col gap-1.5">
          {display.map((item, i) => (
            <div key={item.label} className="flex items-center justify-between gap-2">
              <div className="flex items-center gap-2 min-w-0">
                <div
                  className="w-2.5 h-2.5 rounded-sm flex-shrink-0"
                  style={{ background: COLORS[i % COLORS.length] }}
                />
                <span className="text-xs text-slate-600 dark:text-slate-300 truncate">
                  {item.label}
                </span>
              </div>
              <span className="text-xs font-semibold text-slate-800 dark:text-white flex-shrink-0">
                %{item.share}
              </span>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}