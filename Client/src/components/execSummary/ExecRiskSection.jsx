import { useState, useEffect } from 'react'
import { getExecRisk } from '../../api/execSummaryApi'
import { SkeletonBlock, SkeletonCard } from '../ui/Skeleton'

const RiskSkeleton = () => (
  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
    {[1, 2, 3, 4].map(i => (
      <SkeletonCard key={i}>
        <SkeletonBlock width="w-32" height="h-4" className="mb-2" />
        <SkeletonBlock width="w-full" height="h-3" className="mb-1" />
        <SkeletonBlock width="w-24" height="h-5" />
      </SkeletonCard>
    ))}
  </div>
)

// Risk seviyesi hesaplama
function getRiskLevel(policyCount, totalPolicies) {
  const ratio = totalPolicies > 0 ? (policyCount / totalPolicies) * 100 : 0
  if (ratio >= 5) return { level: 'Kritik', color: 'red' }
  if (ratio >= 2) return { level: 'Yüksek', color: 'amber' }
  if (ratio >= 1) return { level: 'Orta', color: 'yellow' }
  return { level: 'Düşük', color: 'emerald' }
}

// Dinamik açıklama oluştur
function generateDescription(segment) {
  const { name, policyCount, ratio } = segment
  
  // Premium marka + yaşlı araç
  if (name?.toLowerCase().includes('bmw') || name?.toLowerCase().includes('mercedes') || name?.toLowerCase().includes('audi')) {
    if (name?.includes('11-15') || name?.includes('yaş')) {
      return `Premium marka + yaşlı araç kombinasyonu. Yüksek parça maliyeti riski taşıyor.`
    }
  }
  
  // Volkswagen yoğunluğu
  if (name?.toLowerCase().includes('volkswagen') || name?.toLowerCase().includes('vw')) {
    return `Yüksek portföy yoğunluğu sistematik frekans etkisi yaratabilir.`
  }
  
  // Fiat + genç sürücü
  if (name?.toLowerCase().includes('fiat') && name?.toLowerCase().includes('genç')) {
    return `Yüksek kaza frekansı beklenen segment. Dikkatli fiyatlama gerektirir.`
  }
  
  // Basamak 0
  if (name?.toLowerCase().includes('basamak 0') || name?.includes('Basamak0')) {
    return `Hasar geçmişi bilinmeyen müşteriler. Loss ratio riski yüksek.`
  }
  
  // Genç sürücü
  if (name?.toLowerCase().includes('genç') || name?.includes('18-25')) {
    return `Genç sürücü segmenti yüksek frekans riski taşır.`
  }
  
  // Yaşlı araç
  if (name?.includes('11-15') || name?.includes('16+') || name?.toLowerCase().includes('yaşlı araç')) {
    return `Yaşlı araç segmenti artan hasar maliyeti riski içerir.`
  }
  
  return segment.description || `${policyCount?.toLocaleString('tr-TR')} poliçe ile dikkat gerektiren segment.`
}

export default function ExecRiskSection({ productGroup, startDate, endDate }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!startDate || !endDate) return
    setLoading(true)
    setError('')
    getExecRisk(productGroup, startDate, endDate)
      .then(res => setData(res.data))
      .catch(() => setError('Veri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [productGroup, startDate, endDate])

  if (!startDate || !endDate) return null
  if (loading) return <RiskSkeleton />
  if (error) return <div className="text-sm text-red-400 py-10 text-center">{error}</div>
  if (!data?.segments?.length) {
    return (
      <div className="bg-emerald-50 dark:bg-emerald-500/10 border border-emerald-200 dark:border-emerald-500/30 rounded-xl p-4 text-sm text-emerald-700 dark:text-emerald-300">
        ✓ Bu dönemde kritik risk segmenti tespit edilmedi.
      </div>
    )
  }

  const totalPolicies = data.totalPolicies || data.segments.reduce((sum, s) => sum + (s.policyCount || 0), 0)

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      {data.segments.slice(0, 4).map((segment, i) => {
        const { level, color } = getRiskLevel(segment.policyCount, totalPolicies)
        const ratio = totalPolicies > 0 ? ((segment.policyCount / totalPolicies) * 100).toFixed(2) : 0
        
        const colorClasses = {
          red: {
            border: 'border-l-red-500',
            bg: 'bg-red-50 dark:bg-red-500/10',
            text: 'text-red-600 dark:text-red-400',
            badge: 'bg-red-100 dark:bg-red-500/20'
          },
          amber: {
            border: 'border-l-amber-500',
            bg: 'bg-amber-50 dark:bg-amber-500/10',
            text: 'text-amber-600 dark:text-amber-400',
            badge: 'bg-amber-100 dark:bg-amber-500/20'
          },
          yellow: {
            border: 'border-l-yellow-500',
            bg: 'bg-yellow-50 dark:bg-yellow-500/10',
            text: 'text-yellow-600 dark:text-yellow-400',
            badge: 'bg-yellow-100 dark:bg-yellow-500/20'
          },
          emerald: {
            border: 'border-l-emerald-500',
            bg: 'bg-emerald-50 dark:bg-emerald-500/10',
            text: 'text-emerald-600 dark:text-emerald-400',
            badge: 'bg-emerald-100 dark:bg-emerald-500/20'
          }
        }
        
        const colors = colorClasses[color]
        
        return (
          <div
            key={i}
            className={`bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 ${colors.border} border-l-4 rounded-xl p-4`}
          >
            <div className="flex items-start justify-between mb-2">
              <p className="text-sm font-semibold text-slate-800 dark:text-white">
                {segment.name}
              </p>
              <span className={`text-[10px] font-medium px-2 py-0.5 rounded-full ${colors.badge} ${colors.text}`}>
                {level}
              </span>
            </div>
            
            <p className="text-xs text-slate-500 dark:text-slate-400 mb-3 leading-relaxed">
              {generateDescription(segment)}
            </p>
            
            <div className="flex items-center gap-3">
              <div className={`px-2 py-1 rounded ${colors.bg}`}>
                <span className={`text-xs font-mono font-semibold ${colors.text}`}>
                  {segment.policyCount?.toLocaleString('tr-TR')} poliçe
                </span>
              </div>
              <span className="text-xs text-slate-400">
                Portföyün %{ratio}'si
              </span>
            </div>
          </div>
        )
      })}
    </div>
  )
}