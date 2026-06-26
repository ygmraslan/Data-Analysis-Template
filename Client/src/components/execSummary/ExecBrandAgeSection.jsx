import { useState, useEffect } from 'react'
import { getExecBrandAge } from '../../api/execSummaryApi'
import { SkeletonBlock, SkeletonCard } from '../ui/Skeleton'

const BrandAgeSkeleton = () => (
  <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
    <SkeletonCard>
      <SkeletonBlock width="w-40" height="h-4" className="mb-4" />
      <div className="space-y-2">
        {[1,2,3,4,5,6,7,8,9,10].map(i => <SkeletonBlock key={i} width="w-full" height="h-5" />)}
      </div>
    </SkeletonCard>
    <SkeletonCard>
      <SkeletonBlock width="w-48" height="h-4" className="mb-4" />
      <div className="space-y-2">
        {[1,2,3,4,5,6,7,8,9,10].map(i => <SkeletonBlock key={i} width="w-full" height="h-6" />)}
      </div>
    </SkeletonCard>
  </div>
)

// Heatmap renk fonksiyonu: Yeşil → Sarı → Kırmızı
function heatColor(ratio) {
  const green = [99, 190, 123]
  const yellow = [255, 235, 132]
  const red = [248, 105, 107]
  
  let r, g, b
  if (ratio <= 0.5) {
    const t = ratio * 2
    r = Math.round(green[0] + (yellow[0] - green[0]) * t)
    g = Math.round(green[1] + (yellow[1] - green[1]) * t)
    b = Math.round(green[2] + (yellow[2] - green[2]) * t)
  } else {
    const t = (ratio - 0.5) * 2
    r = Math.round(yellow[0] + (red[0] - yellow[0]) * t)
    g = Math.round(yellow[1] + (red[1] - yellow[1]) * t)
    b = Math.round(yellow[2] + (red[2] - yellow[2]) * t)
  }
  return `rgb(${r},${g},${b})`
}

// Tarih formatla - "30 Mar — 5 Nis 2026"
function formatDateRange(startStr, endStr) {
  if (!startStr || !endStr) return ''
  const months = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']
  
  // ISO format veya YYYYMMDD format
  const parseDate = (str) => {
    if (!str) return null
    // ISO format: "2026-03-30T00:00:00+03:00"
    if (str.includes('T') || str.includes('-')) {
      const d = new Date(str)
      return isNaN(d.getTime()) ? null : d
    }
    // YYYYMMDD format: "20260330"
    if (str.length === 8) {
      const year = parseInt(str.substring(0, 4))
      const month = parseInt(str.substring(4, 6)) - 1
      const day = parseInt(str.substring(6, 8))
      return new Date(year, month, day)
    }
    return null
  }
  
  const start = parseDate(startStr)
  const end = parseDate(endStr)
  if (!start || !end) return ''
  
  return `${start.getDate()} ${months[start.getMonth()]} — ${end.getDate()} ${months[end.getMonth()]} ${end.getFullYear()}`
}

// Premium marka kontrolü
const PREMIUM_BRANDS = ['BMW', 'MERCEDES', 'AUDI', 'PORSCHE', 'LAND ROVER', 'JAGUAR', 'VOLVO', 'LEXUS', 'INFINITI']
const isPremium = (brand) => {
  if (!brand) return false
  const upper = brand.toUpperCase()
  return PREMIUM_BRANDS.some(p => upper.includes(p))
}

// Yaş grubu kolonları (API field isimleri)
const AGE_GROUPS = [
  { key: 'age0To2', label: '0-2' },
  { key: 'age3To5', label: '3-5' },
  { key: 'age6To10', label: '6-10' },
  { key: 'age11To15', label: '11-15' },
  { key: 'age16Plus', label: '16+' },
]

export default function ExecBrandAgeSection({ productGroup, startDate, endDate }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!startDate || !endDate) return
    setLoading(true)
    setError('')
    getExecBrandAge(productGroup, startDate, endDate)
      .then(res => setData(res.data))
      .catch(() => setError('Veri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, startDate, endDate])

  if (!startDate || !endDate) return null
  if (loading) return <BrandAgeSkeleton />
  if (error) return <div className="text-sm text-red-400 py-10 text-center">{error}</div>
  
  // API'den gelen matrix
  const matrix = data?.matrix || []
  if (!matrix.length) return <div className="text-sm text-slate-400 py-10 text-center">Veri bulunamadı</div>
  
  // Toplam poliçe
  const totalPolicies = matrix.reduce((sum, row) => sum + (row.total || 0), 0)
  
  // Premium marka oranı (manuel kontrol)
  const premiumCount = matrix.filter(row => row.isPremiumBrand || isPremium(row.brand)).reduce((sum, row) => sum + (row.total || 0), 0)
  const premiumRatio = totalPolicies > 0 ? ((premiumCount / totalPolicies) * 100).toFixed(1) : '0'
  
  // En yüksek poliçe sayısı (bar genişliği için)
  const maxCount = Math.max(...matrix.map(row => row.total || 0))
  
  // Matrix için min/max değerler (heatmap renklendirmesi için)
  const allValues = matrix.flatMap(row => 
    AGE_GROUPS.map(ag => row[ag.key] || 0)
  ).filter(v => v > 0)
  const matrixMin = allValues.length > 0 ? Math.min(...allValues) : 0
  const matrixMax = allValues.length > 0 ? Math.max(...allValues) : 1
  
  // Heatmap oranı hesapla
  const getHeatRatio = (value) => {
    if (!value || value <= 0) return null
    if (matrixMax === matrixMin) return 0.5
    return (value - matrixMin) / (matrixMax - matrixMin)
  }
  
  // Dinamik özet cümlesi
  const generateSummary = () => {
    const ratio = parseFloat(premiumRatio)
    if (ratio >= 25) {
      return `Premium marka yoğunluğu %${premiumRatio} ile yüksek seviyede. Parça maliyeti ve hasar bedeli baskısı oluşturabilir.`
    } else if (ratio >= 15) {
      return `Premium marka oranı %${premiumRatio}. Ortalama araç bedeli ve yedek parça maliyetlerini artırıyor.`
    } else if (ratio > 0) {
      return `Premium marka oranı %${premiumRatio}. Portföyde standart segment ağırlıklı dağılım mevcut.`
    } else {
      return `Portföyde premium marka bulunmuyor. Standart segment ağırlıklı dağılım.`
    }
  }
  
  // Premium kontrolü helper
  const checkPremium = (row) => row.isPremiumBrand || isPremium(row.brand)

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
      {/* Sol: Top 10 Marka Dağılımı */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 flex flex-col">
        <div className="mb-4">
          <p className="text-sm font-semibold text-slate-800 dark:text-white">Top 10 Marka Dağılımı</p>
          <p className="text-xs text-slate-400 mt-0.5">{formatDateRange(startDate, endDate)}</p>
        </div>
        
        {/* Marka listesi */}
        <div className="space-y-3 flex-1">
          {matrix.slice(0, 10).map((row, i) => {
            const ratio = maxCount > 0 ? (row.total / maxCount) * 100 : 0
            const pct = totalPolicies > 0 ? ((row.total / totalPolicies) * 100).toFixed(1) : 0
            const premium = checkPremium(row)
            
            return (
              <div key={i} className="flex items-center gap-4">
                <div className={`w-32 text-xs truncate flex-shrink-0 ${premium ? 'text-red-600 dark:text-red-400 font-semibold' : 'text-slate-700 dark:text-slate-300'}`}>
                  {row.brand}
                </div>
                <div className="flex-1 h-3 bg-slate-100 dark:bg-white/10 rounded-full overflow-hidden">
                  <div
                    className={`h-full rounded-full transition-all ${premium ? 'bg-red-500' : 'bg-emerald-500'}`}
                    style={{ width: `${ratio}%` }}
                  />
                </div>
                <div className="w-24 text-right text-xs flex-shrink-0">
                  <span className="font-semibold text-slate-800 dark:text-white">{row.total?.toLocaleString('tr-TR')}</span>
                  <span className="ml-1.5 text-slate-400">%{pct}</span>
                </div>
              </div>
            )
          })}
        </div>
        
        {/* Özet cümlesi - ALTTA */}
        <div className={`mt-4 px-3 py-2 rounded-lg ${
          parseFloat(premiumRatio) >= 25 
            ? 'bg-red-50 dark:bg-red-500/10' 
            : parseFloat(premiumRatio) >= 15 
              ? 'bg-amber-50 dark:bg-amber-500/10' 
              : 'bg-slate-50 dark:bg-white/5'
        }`}>
          <p className={`text-xs ${
            parseFloat(premiumRatio) >= 25 
              ? 'text-red-600 dark:text-red-300' 
              : parseFloat(premiumRatio) >= 15 
                ? 'text-amber-600 dark:text-amber-300' 
                : 'text-slate-500 dark:text-slate-400'
          }`}>
            {generateSummary()}
          </p>
        </div>
      </div>

      {/* Sağ: Marka × Araç Yaşı Matrisi */}
      <div className="bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 flex flex-col">
        <div className="flex items-center justify-between mb-1">
          <p className="text-sm font-semibold text-slate-800 dark:text-white">Marka × Araç Yaşı Matrisi</p>
          {/* Renk skalası - sağ üstte */}
          <div className="flex items-center gap-1.5">
            <span className="text-[10px] text-slate-400">Düşük</span>
            <div className="flex gap-0.5">
              {[0, 0.25, 0.5, 0.75, 1].map((r, i) => (
                <div key={i} className="w-4 h-2.5 rounded-sm" style={{ background: heatColor(r) }} />
              ))}
            </div>
            <span className="text-[10px] text-slate-400">Yüksek</span>
          </div>
        </div>
        <p className="text-xs text-slate-400 mb-3">Renk yoğunluğu poliçe sayısını gösterir</p>
        
        {/* Heatmap Tablosu */}
        <div className="flex-1">
          <table className="w-full text-xs">
            <thead>
              <tr className="border-b border-slate-100 dark:border-white/6">
                <th className="text-left py-2 text-slate-500 dark:text-slate-400 font-medium w-28">Marka</th>
                {AGE_GROUPS.map(ag => (
                  <th 
                    key={ag.key} 
                    className={`text-center py-2 font-medium ${
                      ag.label === '11-15' ? 'text-red-500' : 'text-slate-500 dark:text-slate-400'
                    }`}
                  >
                    {ag.label}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {matrix.slice(0, 10).map((row, i) => {
                const premium = checkPremium(row)
                return (
                  <tr key={i} className={`border-b border-slate-50 dark:border-white/4 ${premium ? 'bg-red-50/30 dark:bg-red-500/5' : ''}`}>
                    <td className={`py-1.5 ${premium ? 'text-red-600 dark:text-red-400 font-medium' : 'text-slate-700 dark:text-slate-300'}`}>
                      {row.brand}
                    </td>
                    {AGE_GROUPS.map(ag => {
                      const value = row[ag.key] || 0
                      const ratio = getHeatRatio(value)
                      const bg = ratio !== null ? heatColor(ratio) : 'transparent'
                      
                      return (
                        <td key={ag.key} className="py-1 px-1 text-center">
                          <div 
                            className="rounded px-1.5 py-1 text-[11px] font-medium"
                            style={{ 
                              background: bg,
                              color: ratio !== null ? '#1a1a1a' : '#94a3b8'
                            }}
                          >
                            {value > 0 ? value : '—'}
                          </div>
                        </td>
                      )
                    })}
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}