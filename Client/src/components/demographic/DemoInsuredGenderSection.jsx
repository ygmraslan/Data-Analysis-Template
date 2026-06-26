import { useState, useEffect } from 'react'
import { PieChart, Pie, Cell, ResponsiveContainer } from 'recharts'
import * as demoApi from '../../api/demoApi'
import { getLastWeekRange } from '../../utils/formatDate'
import { SkeletonBlock, SkeletonCard } from '../ui/Skeleton'

const Skeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
    {[1, 2].map(i => (
      <SkeletonCard key={i} className="h-52" />
    ))}
  </div>
)

const COLORS = {
  gercek: '#059669',
  tuzel:  '#2563eb', 
  erkek:  '#1d4ed8',
  kadin:  '#7c3aed',
  diger:  '#64748b',
}

const getColor = (label) => {
  const l = label?.toUpperCase() || ''
  if (l.includes('GERCEK') || l.includes('GERÇEK')) return COLORS.gercek
  if (l.includes('TUZEL') || l.includes('TÜZEL')) return COLORS.tuzel
  if (l.includes('ERKEK') || l === 'E') return COLORS.erkek
  if (l.includes('KADIN') || l === 'K') return COLORS.kadin
  return COLORS.diger
}

const getDisplayLabel = (label) => {
  const l = label?.toUpperCase() || ''
  if (l.includes('GERCEK') || l.includes('GERÇEK')) return 'Gerçek Kişi'
  if (l.includes('TUZEL') || l.includes('TÜZEL')) return 'Tüzel Kişi'
  if (l.includes('ERKEK') || l === 'E') return 'Erkek'
  if (l.includes('KADIN') || l === 'K') return 'Kadın'
  if (l.includes('BİLİNMİYOR') || l.includes('BILINMIYOR') || l === '') return 'Bilinmiyor'
  return label
}

const formatCount = n => Number(n || 0).toLocaleString('tr-TR')

function ChartSection({ title, data, weekRange }) {
  if (!data || data.length === 0) return null

  const total = data.reduce((sum, d) => sum + (d.policyCount || 0), 0)

  const pieData = data.map(d => ({
    name: getDisplayLabel(d.label),
    value: d.policyCount || 0,
    color: getColor(d.label),
    ratio: d.ratio,
    wow: d.woW,
  }))

  return (
    <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5">
      <div className="flex items-center justify-between mb-4">
        <p className="text-sm font-semibold text-slate-700 dark:text-slate-200">{title}</p>
        <p className="text-xs text-slate-400">{weekRange}</p>
      </div>
      
      <div className="flex items-center gap-8">
        {/* Donut Chart */}
        <div className="relative flex-shrink-0" style={{ width: 120, height: 120 }}>
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie 
                data={pieData} 
                dataKey="value" 
                cx="50%" 
                cy="50%"
                innerRadius={38} 
                outerRadius={55} 
                paddingAngle={2} 
                strokeWidth={0}
              >
                {pieData.map((entry, i) => (
                  <Cell key={i} fill={entry.color} />
                ))}
              </Pie>
            </PieChart>
          </ResponsiveContainer>
          <div className="absolute inset-0 flex flex-col items-center justify-center">
            <p className="text-base font-bold text-slate-800 dark:text-white">{formatCount(total)}</p>
            <p className="text-[9px] text-slate-400">poliçe</p>
          </div>
        </div>

        {/* Detay listesi */}
        <div className="flex-1 space-y-2.5">
          {pieData.map((item) => (
            <div key={item.name} className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <div className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: item.color }} />
                <span className="text-sm text-slate-700 dark:text-slate-200">{item.name}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <span className="text-slate-500 dark:text-slate-400">{formatCount(item.value)}</span>
                <span className="font-semibold text-slate-700 dark:text-white w-14 text-right">%{item.ratio?.toFixed(1)}</span>
                {item.wow !== null && item.wow !== undefined ? (
                  <span className={`w-14 text-right font-medium ${item.wow >= 0 ? 'text-emerald-600 dark:text-emerald-400' : 'text-red-600 dark:text-red-400'}`}>
                    {item.wow > 0 ? '+' : ''}{item.wow?.toFixed(1)}%
                  </span>
                ) : (
                  <span className="w-14 text-right text-slate-300">—</span>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

export default function DemoInsuredGenderSection({ productGroup, filter }) {
  const [insuredData, setInsuredData] = useState([])
  const [genderData, setGenderData] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const weekRange = getLastWeekRange()

  useEffect(() => {
    setLoading(true)
    setError('')
    Promise.all([
      demoApi.getDemoInsuredType(productGroup, filter),
      demoApi.getDemoGender(productGroup, filter),
    ])
      .then(([insuredRes, genderRes]) => {
        setInsuredData(insuredRes.data || [])
        setGenderData(genderRes.data || [])
      })
      .catch(() => setError('Sigortalı türü / cinsiyet verisi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, filter])

  if (loading) return <Skeleton />
  if (error) return <div className="flex items-center justify-center py-10 text-sm text-red-400">{error}</div>

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <ChartSection title="Sigortalı Türü Dağılımı" data={insuredData} weekRange={weekRange} />
      <ChartSection title="Cinsiyet Dağılımı" data={genderData} weekRange={weekRange} />
    </div>
  )
}