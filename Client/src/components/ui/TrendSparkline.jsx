import {
  ResponsiveContainer,
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
} from 'recharts'

function fmtWeekRange(str) {
  if (!str) return ''
  const d = new Date(str)
  const end = new Date(str)
  end.setDate(end.getDate() + 6)
  const f = x => `${String(x.getDate()).padStart(2,'0')}.${String(x.getMonth()+1).padStart(2,'0')}`
  return `${f(d)}-${f(end)}`
}

function calcSlope(values) {
  const n = values.length
  if (n < 2) return 0
  const xMean = (n - 1) / 2
  const yMean = values.reduce((s, v) => s + v, 0) / n
  let num = 0, den = 0
  values.forEach((y, x) => {
    num += (x - xMean) * (y - yMean)
    den += (x - xMean) ** 2
  })
  return den === 0 ? 0 : num / den
}

const CustomTooltip = ({ active, payload, label, suffix }) => {
  if (!active || !payload?.length) return null
  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/10 rounded-lg px-3 py-2 shadow-lg text-xs">
      <p className="text-slate-500 dark:text-slate-400 mb-1.5">{label}</p>
      {payload.map(p => (
        <div key={p.dataKey} className="flex items-center gap-2 mb-0.5 last:mb-0">
          <span className="w-2 h-2 rounded-full flex-shrink-0" style={{ background: p.color }} />
          <span className="text-slate-600 dark:text-slate-300">{p.name}</span>
          <span className="font-semibold text-slate-900 dark:text-white ml-auto pl-3">
            {suffix}{p.value}
          </span>
        </div>
      ))}
    </div>
  )
}

export default function TrendSparkline({ data = [], series = [], suffix = '%' }) {
  if (!data.length || !series.length) return null

  const chartData = data.map(d => ({
    ...d,
    _label: fmtWeekRange(d.weekStart),
  }))

  return (
    <div className="w-full">
      <ResponsiveContainer width="100%" height={180}>
        <LineChart data={chartData} margin={{ top: 16, right: 30, bottom: 8, left: 8 }}>
          <CartesianGrid
            strokeDasharray="3 3"
            stroke="rgba(148,163,184,0.12)"
            vertical={false}
          />
          <XAxis
            dataKey="_label"
            tick={{ fontSize: 10, fill: '#94a3b8' }}
            tickLine={false}
            axisLine={false}
            interval="preserveStartEnd"
            dy={5}
          />
          <YAxis
            tick={{ fontSize: 10, fill: '#94a3b8' }}
            tickLine={false}
            axisLine={false}
            tickFormatter={v => `${suffix}${v}`}
            width={45}
            dx={-5}
          />
          <Tooltip content={<CustomTooltip suffix={suffix} />} />
          {series.map(s => (
            <Line
              key={s.key}
              type="monotone"
              dataKey={s.key}
              name={s.label}
              stroke={s.color}
              strokeWidth={2}
              strokeDasharray={s.dashed ? '6 3' : undefined}
              dot={{ r: 3, fill: s.color, strokeWidth: 0 }}
              activeDot={{ r: 5, fill: s.color, strokeWidth: 2, stroke: '#fff' }}
            />
          ))}
        </LineChart>
      </ResponsiveContainer>

      <div className="flex flex-wrap gap-x-5 gap-y-1 mt-1.5 px-1">
        {series.map(s => {
          const values = data.map(d => Number(d[s.key] ?? 0))
          const slope  = calcSlope(values)
          const dir    = slope >  0.005 ? '↑' : slope < -0.005 ? '↓' : '→'
          const clr    = slope >  0.005 ? '#ef4444' : slope < -0.005 ? '#10b981' : '#94a3b8'
          return (
            <span key={s.key} className="text-xs" style={{ color: clr }}>
              {s.label} eğim: {slope >= 0 ? '+' : ''}{slope.toFixed(2)} p/hafta {dir}
            </span>
          )
        })}
      </div>
    </div>
  )
}