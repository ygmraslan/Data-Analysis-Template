import { useState, useEffect, useCallback } from 'react'
import { getExecAiDeepSeek, getExecAiGemini, getExecAiGpt } from '../../api/execSummaryApi'
import { DeepSeekLogo, GeminiLogo, GptLogo } from '../../assets/logos'
import { HiRefresh } from 'react-icons/hi'

const MODEL_CONFIG = {
  deepseek: { 
    key: 'deepseek', 
    name: 'DeepSeek', 
    Logo: DeepSeekLogo,
    fetchFn: getExecAiDeepSeek
  },
  gemini: { 
    key: 'gemini', 
    name: 'Gemini', 
    Logo: GeminiLogo,
    fetchFn: getExecAiGemini
  },
  gpt: { 
    key: 'gpt', 
    name: 'GPT', 
    Logo: GptLogo,
    fetchFn: getExecAiGpt
  }
}

const MONTHS = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık']

const getMetricBgColor = (level) => {
  switch (level) {
    case 'danger': return 'bg-red-50 dark:bg-red-500/10'
    case 'warning': return 'bg-amber-50 dark:bg-amber-500/10'
    case 'success': return 'bg-emerald-50 dark:bg-emerald-500/10'
    default: return 'bg-slate-100 dark:bg-white/5'
  }
}

const getMetricTextColor = (level) => {
  switch (level) {
    case 'danger': return 'text-red-600 dark:text-red-400'
    case 'warning': return 'text-amber-600 dark:text-amber-400'
    case 'success': return 'text-emerald-600 dark:text-emerald-400'
    default: return 'text-slate-800 dark:text-slate-200'
  }
}

const formatDateRange = (startDate, endDate) => {
  if (!startDate || !endDate) return ''
  const start = new Date(startDate)
  const end = new Date(endDate)
  return `${start.getDate()} - ${end.getDate()} ${MONTHS[end.getMonth()]} ${end.getFullYear()}`
}


function ModelCard({ modelKey, config, data, loading, error, onRefresh, refreshing }) {
  const Logo = config.Logo
  const { generalStatus, findings, recommendations, criticalCount, highCount, mediumCount } = data || {}

  if (loading) {
    return (
      <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden h-[550px]">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-purple-200 dark:border-purple-500/30">
          <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-purple-500 to-violet-600 flex items-center justify-center text-white animate-pulse">
            <Logo className="w-5 h-5" />
          </div>
          <div className="flex-1">
            <div className="text-sm font-semibold text-slate-700 dark:text-slate-300">{config.name}</div>
            <div className="text-xs text-slate-500 dark:text-slate-400">Analiz hazırlanıyor, bu işlem 1-2 dakika sürebilir...</div>
          </div>
          <div className="w-5 h-5 border-2 border-purple-300 border-t-purple-600 rounded-full animate-spin" />
        </div>
        <div className="p-4 space-y-3">
          <div className="h-16 bg-slate-100 dark:bg-white/5 rounded-lg animate-pulse" />
          <div className="h-12 bg-slate-100 dark:bg-white/5 rounded-lg animate-pulse" />
          <div className="h-12 bg-slate-100 dark:bg-white/5 rounded-lg animate-pulse" />
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="bg-white dark:bg-white/5 border border-red-200 dark:border-red-500/30 rounded-xl overflow-hidden h-[550px]">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-red-200 dark:border-red-500/30">
          <div className="w-8 h-8 rounded-lg bg-red-100 dark:bg-red-500/20 flex items-center justify-center text-red-600 dark:text-red-400">
            <Logo className="w-5 h-5" />
          </div>
          <div className="flex-1">
            <div className="text-sm font-semibold text-red-700 dark:text-red-300">{config.name}</div>
            <div className="text-xs text-red-500 dark:text-red-400">Hata oluştu</div>
          </div>
        </div>
        <div className="p-4 flex flex-col items-center justify-center h-full">
          <span className="text-2xl mb-2">⚠️</span>
          <p className="text-xs text-slate-600 dark:text-slate-400 mb-3 text-center">{error}</p>
          <button
            onClick={() => onRefresh(modelKey)}
            disabled={refreshing}
            className="flex items-center gap-1.5 px-3 py-1.5 text-xs bg-red-100 dark:bg-red-500/20 text-red-600 dark:text-red-400 rounded-lg hover:bg-red-200 transition-colors disabled:opacity-50"
          >
            <HiRefresh className={`w-3.5 h-3.5 ${refreshing ? 'animate-spin' : ''}`} />
            Tekrar Dene
          </button>
        </div>
      </div>
    )
  }

  if (!data) {
    return (
      <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden h-[550px]">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-purple-200 dark:border-purple-500/30">
          <div className="w-8 h-8 rounded-lg bg-slate-100 dark:bg-white/10 flex items-center justify-center text-slate-400">
            <Logo className="w-5 h-5" />
          </div>
          <div className="text-sm font-semibold text-slate-700 dark:text-slate-300">{config.name}</div>
        </div>
        <div className="p-4 flex items-center justify-center h-full">
          <p className="text-sm text-slate-500 dark:text-slate-400">Veri bulunamadı</p>
        </div>
      </div>
    )
  }

  return (
    <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden h-[550px] flex flex-col">
      {/* Header */}
      <div className="flex items-center justify-between px-4 py-3 border-b border-purple-200 dark:border-purple-500/30">
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-purple-500 to-violet-600 flex items-center justify-center text-white shadow-lg shadow-purple-500/25">
            <Logo className="w-5 h-5" />
          </div>
          <span className="text-sm font-semibold text-slate-700 dark:text-slate-300">{config.name}</span>
        </div>
        <button
          onClick={() => onRefresh(modelKey)}
          disabled={refreshing}
          className="w-7 h-7 flex items-center justify-center rounded-lg bg-slate-100 dark:bg-white/10 text-slate-500 dark:text-slate-400 hover:bg-slate-200 dark:hover:bg-white/20 transition-colors disabled:opacity-50"
          title="Yenile"
        >
          <HiRefresh className={`w-4 h-4 ${refreshing ? 'animate-spin' : ''}`} />
        </button>
      </div>

      {/* Risk Badge'leri */}
      <div className="flex flex-wrap items-center gap-2 px-4 py-2.5 border-b border-purple-200 dark:border-purple-500/30 bg-purple-50/50 dark:bg-purple-500/5">
        {criticalCount > 0 && (
          <span className="inline-flex items-center gap-1.5 px-2.5 py-1 text-[11px] font-semibold rounded-md bg-red-50 dark:bg-red-500/10 border border-red-200 dark:border-red-500/30 text-red-600 dark:text-red-400">
            <span className="w-1.5 h-1.5 rounded-full bg-red-500" />
            {criticalCount} Kritik Bulgu
          </span>
        )}
        {highCount > 0 && (
          <span className="inline-flex items-center gap-1.5 px-2.5 py-1 text-[11px] font-semibold rounded-md bg-orange-50 dark:bg-orange-500/10 border border-orange-200 dark:border-orange-500/30 text-orange-600 dark:text-orange-400">
            <span className="w-1.5 h-1.5 rounded-full bg-orange-500" />
            {highCount} Yüksek Risk
          </span>
        )}
        {mediumCount > 0 && (
          <span className="inline-flex items-center gap-1.5 px-2.5 py-1 text-[11px] font-semibold rounded-md bg-amber-50 dark:bg-amber-500/10 border border-amber-200 dark:border-amber-500/30 text-amber-600 dark:text-amber-400">
            <span className="w-1.5 h-1.5 rounded-full bg-amber-500" />
            {mediumCount} Orta Risk
          </span>
        )}
      </div>

      {/* İçerik - Scroll */}
      <div className="flex-1 p-4 space-y-4 overflow-y-auto">
        {/* Genel Durum */}
        {generalStatus && (
          <div>
            <div className="flex items-center gap-2 mb-2">
              <span className="text-[10px] font-bold uppercase tracking-wide text-purple-600 dark:text-purple-400">Genel Durum</span>
              <span className="flex-1 h-px bg-gradient-to-r from-purple-200 to-transparent dark:from-purple-500/30" />
            </div>
            <div className="bg-purple-50/50 dark:bg-purple-500/5 border border-purple-200 dark:border-purple-500/30 rounded-lg p-3">
              <p className="text-[11px] text-slate-700 dark:text-slate-300 leading-relaxed mb-3">
                {generalStatus.summary}
              </p>
              {generalStatus.metrics?.length > 0 && (
                <div className="flex flex-wrap gap-1.5">
                  {generalStatus.metrics.map((metric, idx) => (
                    <div key={idx} className={`flex items-center gap-1.5 px-2 py-1 rounded text-[10px] ${getMetricBgColor(metric.level)}`}>
                      <span className="text-slate-500 dark:text-slate-400">{metric.label}:</span>
                      <span className={`font-bold ${getMetricTextColor(metric.level)}`}>{metric.value}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        )}

        {/* Kritik Risk Bulguları */}
        {findings?.length > 0 && (
          <div>
            <div className="flex items-center gap-2 mb-2">
              <span className="text-[10px] font-bold uppercase tracking-wide text-purple-600 dark:text-purple-400">Kritik Risk Bulguları</span>
              <span className="flex-1 h-px bg-gradient-to-r from-purple-200 to-transparent dark:from-purple-500/30" />
            </div>
            <div className="space-y-2">
              {findings.map((finding, idx) => (
                <div 
                  key={idx} 
                  className={`flex gap-3 p-3 bg-purple-50/50 dark:bg-purple-500/5 rounded-lg border-l-[3px] ${
                    finding.level === 'critical' ? 'border-l-red-500' :
                    finding.level === 'high' ? 'border-l-orange-500' : 'border-l-amber-500'
                  }`}
                >
                  <div className={`w-6 h-6 rounded-md flex items-center justify-center flex-shrink-0 text-sm ${
                    finding.level === 'critical' ? 'bg-red-50 dark:bg-red-500/10' :
                    finding.level === 'high' ? 'bg-orange-50 dark:bg-orange-500/10' : 'bg-amber-50 dark:bg-amber-500/10'
                  }`}>
                    {finding.level === 'critical' ? '🔴' : finding.level === 'high' ? '🟠' : '🟡'}
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="text-[11px] font-semibold text-slate-800 dark:text-slate-200 mb-0.5">
                      {finding.title}
                    </div>
                    <div className="text-[10px] text-slate-600 dark:text-slate-400 leading-relaxed">
                      {finding.description}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Kısa Vadeli Öneriler */}
        {recommendations?.length > 0 && (
          <div>
            <div className="flex items-center gap-2 mb-2">
              <span className="text-[10px] font-bold uppercase tracking-wide text-purple-600 dark:text-purple-400">Kısa Vadeli Öneriler</span>
              <span className="flex-1 h-px bg-gradient-to-r from-purple-200 to-transparent dark:from-purple-500/30" />
            </div>
            <div className="space-y-2">
              {recommendations.map((rec, idx) => (
                <div key={idx} className="p-3 bg-purple-50/50 dark:bg-purple-500/5 border border-purple-200 dark:border-purple-500/30 rounded-lg">
                  <div className="flex items-center gap-2 mb-1">
                    <div className="w-6 h-6 bg-blue-50 dark:bg-blue-500/10 rounded-md flex items-center justify-center text-sm">
                      {rec.icon || '⚡'}
                    </div>
                    <span className="text-[11px] font-semibold text-slate-800 dark:text-slate-200">
                      {rec.title}
                    </span>
                  </div>
                  <p className="text-[10px] text-slate-600 dark:text-slate-400 leading-relaxed pl-8">
                    {rec.description}
                  </p>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  )
}

export default function ExecAiSection({ productGroup, startDate, endDate }) {
  
  const [models, setModels] = useState({
    deepseek: { data: null, loading: false, error: null },
    gemini: { data: null, loading: false, error: null },
    gpt: { data: null, loading: false, error: null }
  })
  const [refreshingModel, setRefreshingModel] = useState(null)

  
  const fetchModel = useCallback(async (modelKey, forceRefresh = false) => {
    if (!startDate || !endDate) return

    const config = MODEL_CONFIG[modelKey]
    if (!config) return

    setModels(prev => ({
      ...prev,
      [modelKey]: { ...prev[modelKey], loading: true, error: null }
    }))

    try {
      const res = await config.fetchFn(productGroup, startDate, endDate, forceRefresh)
      const responseData = res.data?.data?.data || res.data?.data || res.data
      setModels(prev => ({
        ...prev,
        [modelKey]: { data: responseData, loading: false, error: null }
      }))
    } catch (err) {
      console.error(`[ExecAi:${config.name}] Hata detayı:`, {
        message: err.message,
        code: err.code,
        status: err.response?.status,
        statusText: err.response?.statusText,
        responseData: err.response?.data,
        isTimeout: err.code === 'ECONNABORTED' || err.message?.includes('timeout'),
        isNetwork: err.code === 'ERR_NETWORK' || err.message === 'Network Error',
        config: {
          url: err.config?.url,
          timeout: err.config?.timeout,
          method: err.config?.method
        }
      })
      const isTimeout = err.code === 'ECONNABORTED' || err.message?.toLowerCase().includes('timeout')
      const isNetwork = err.code === 'ERR_NETWORK' || err.message === 'Network Error'

      let userMessage
      if (isTimeout) {
        userMessage = 'Analiz çok uzun sürdü. Sayfayı yenilediğinizde tamamlanmış sonuç görünebilir.'
      } else if (isNetwork) {
        userMessage = 'Bağlantı hatası. İnternet bağlantınızı kontrol edip tekrar deneyin.'
      } else if (err.response?.status >= 500) {
        userMessage = 'Sunucu hatası. Lütfen tekrar deneyin.'
      } else {
        userMessage = err.response?.data?.message || err.message || 'Bir hata oluştu'
      }

      setModels(prev => ({
        ...prev,
        [modelKey]: { data: null, loading: false, error: userMessage }
      }))
    }
  }, [productGroup, startDate, endDate])

  const fetchAllModels = useCallback(() => {
    Object.keys(MODEL_CONFIG).forEach(key => fetchModel(key, false))
  }, [fetchModel])

  const handleRefresh = useCallback(async (modelKey) => {
    setRefreshingModel(modelKey)
    await fetchModel(modelKey, true)
    setRefreshingModel(null)
  }, [fetchModel])

  const handleRefreshAll = useCallback(async () => {
    setRefreshingModel('all')
    await Promise.all(Object.keys(MODEL_CONFIG).map(key => fetchModel(key, true)))
    setRefreshingModel(null)
  }, [fetchModel])

  
  useEffect(() => {
    if (startDate && endDate) {
      fetchAllModels()
    }
  }, [productGroup, startDate, endDate, fetchAllModels])

  if (!startDate || !endDate) {
    return (
      <div className="bg-gradient-to-br from-purple-50 to-violet-50 dark:from-purple-500/10 dark:to-violet-500/10 border border-purple-200 dark:border-purple-500/30 rounded-xl p-5">
        <div className="flex items-center gap-3 text-purple-600 dark:text-purple-400">
          <div className="w-9 h-9 rounded-lg bg-gradient-to-br from-purple-500 to-violet-600 flex items-center justify-center text-white">
            ✦
          </div>
          <span className="text-sm">Analiz için hafta seçin</span>
        </div>
      </div>
    )
  }

  return (
    <div className="bg-gradient-to-br from-purple-50 via-violet-50 to-fuchsia-50 dark:from-purple-500/10 dark:via-violet-500/10 dark:to-fuchsia-500/10 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden">
      {/* Ana Header */}
      <div className="flex items-center justify-between px-5 py-4 border-b border-purple-200 dark:border-purple-500/30">
        <div>
          <h2 className="text-base font-semibold text-purple-800 dark:text-purple-200">
            Yapay Zeka Destekli Değerlendirme
          </h2>
          <p className="text-sm text-purple-600 dark:text-purple-400">
            {formatDateRange(startDate, endDate)}
          </p>
        </div>
        <button
          onClick={handleRefreshAll}
          disabled={refreshingModel === 'all'}
          className="flex items-center gap-2 px-4 py-2 text-sm bg-purple-100 dark:bg-purple-500/20 text-purple-700 dark:text-purple-300 rounded-lg hover:bg-purple-200 dark:hover:bg-purple-500/30 transition-colors disabled:opacity-50"
        >
          <HiRefresh className={`w-4 h-4 ${refreshingModel === 'all' ? 'animate-spin' : ''}`} />
          Tümünü Yenile
        </button>
      </div>

      {/* 3 Model Kartları */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-4 p-4">
        {Object.entries(MODEL_CONFIG).map(([key, config]) => (
          <ModelCard
            key={key}
            modelKey={key}
            config={config}
            data={models[key].data}
            loading={models[key].loading}
            error={models[key].error}
            onRefresh={handleRefresh}
            refreshing={refreshingModel === key || refreshingModel === 'all'}
          />
        ))}
      </div>
    </div>
  )
}