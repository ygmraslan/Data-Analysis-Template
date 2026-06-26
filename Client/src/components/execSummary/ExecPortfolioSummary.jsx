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

function SummarySection({ icon, title, items, colorScheme }) {
  const colors = {
    purple: {
      bg: 'bg-purple-50 dark:bg-purple-500/10',
      border: 'border-purple-200 dark:border-purple-500/30',
      title: 'text-purple-700 dark:text-purple-300',
      bullet: 'text-purple-500',
      iconBg: 'bg-purple-100 dark:bg-purple-500/20'
    },
    red: {
      bg: 'bg-red-50 dark:bg-red-500/10',
      border: 'border-red-200 dark:border-red-500/30',
      title: 'text-red-700 dark:text-red-300',
      bullet: 'text-red-500',
      iconBg: 'bg-red-100 dark:bg-red-500/20'
    },
    emerald: {
      bg: 'bg-emerald-50 dark:bg-emerald-500/10',
      border: 'border-emerald-200 dark:border-emerald-500/30',
      title: 'text-emerald-700 dark:text-emerald-300',
      bullet: 'text-emerald-500',
      iconBg: 'bg-emerald-100 dark:bg-emerald-500/20'
    }
  }
  
  const c = colors[colorScheme]

  return (
    <div className={`${c.bg} border ${c.border} rounded-lg p-3`}>
      <div className="flex items-center gap-2 mb-2.5">
        <div className={`w-6 h-6 ${c.iconBg} rounded-md flex items-center justify-center text-sm`}>
          {icon}
        </div>
        <span className={`text-[11px] font-bold uppercase tracking-wide ${c.title}`}>
          {title}
        </span>
      </div>
      {items.length > 0 ? (
        <ul className="space-y-1.5">
          {items.map((item, i) => (
            <li key={i} className="flex items-start gap-2 text-[11px] text-slate-600 dark:text-slate-400 leading-relaxed">
              <span className={`${c.bullet} flex-shrink-0 mt-0.5`}>•</span>
              <span>{item}</span>
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-[11px] text-slate-400 italic">Veri bulunamadı</p>
      )}
    </div>
  )
}

function ModelPortfolioCard({ modelKey, config, data, loading, error, onRefresh, refreshing }) {
  const Logo = config.Logo
  const summary = data?.portfolioSummary
  const characteristics = summary?.characteristics || []
  const riskAreas = summary?.riskAreas || []
  const positiveFactors = summary?.positiveFactors || []

  if (loading) {
    return (
      <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-purple-200 dark:border-purple-500/30">
          <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-purple-500 to-violet-600 flex items-center justify-center text-white animate-pulse">
            <Logo className="w-5 h-5" />
          </div>
          <div className="flex-1">
            <div className="text-sm font-semibold text-slate-700 dark:text-slate-300">{config.name}</div>
            <div className="text-xs text-slate-500 dark:text-slate-400">Yükleniyor...</div>
          </div>
          <div className="w-5 h-5 border-2 border-purple-300 border-t-purple-600 rounded-full animate-spin" />
        </div>
        <div className="p-3 space-y-3">
          {[1, 2, 3].map(i => (
            <div key={i} className="bg-slate-50 dark:bg-white/5 rounded-lg p-3 space-y-2">
              <div className="flex items-center gap-2">
                <div className="w-6 h-6 bg-slate-200 dark:bg-white/10 rounded-md animate-pulse" />
                <div className="w-20 h-3 bg-slate-200 dark:bg-white/10 rounded animate-pulse" />
              </div>
              <div className="space-y-1.5">
                <div className="w-full h-2.5 bg-slate-100 dark:bg-white/5 rounded animate-pulse" />
                <div className="w-4/5 h-2.5 bg-slate-100 dark:bg-white/5 rounded animate-pulse" />
              </div>
            </div>
          ))}
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="bg-white dark:bg-white/5 border border-red-200 dark:border-red-500/30 rounded-xl overflow-hidden">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-red-200 dark:border-red-500/30">
          <div className="w-8 h-8 rounded-lg bg-red-100 dark:bg-red-500/20 flex items-center justify-center text-red-600 dark:text-red-400">
            <Logo className="w-5 h-5" />
          </div>
          <div className="flex-1">
            <div className="text-sm font-semibold text-red-700 dark:text-red-300">{config.name}</div>
            <div className="text-xs text-red-500 dark:text-red-400">Hata oluştu</div>
          </div>
        </div>
        <div className="p-4 flex flex-col items-center justify-center py-8">
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

  if (!data || !summary) {
    return (
      <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-purple-200 dark:border-purple-500/30">
          <div className="w-8 h-8 rounded-lg bg-slate-100 dark:bg-white/10 flex items-center justify-center text-slate-400">
            <Logo className="w-5 h-5" />
          </div>
          <div className="text-sm font-semibold text-slate-700 dark:text-slate-300">{config.name}</div>
        </div>
        <div className="p-4 flex items-center justify-center py-8">
          <p className="text-sm text-slate-500 dark:text-slate-400">Veri bulunamadı</p>
        </div>
      </div>
    )
  }

  return (
    <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden flex flex-col">
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

      {/* İçerik - 3 Renkli Bölüm */}
      <div className="p-3 space-y-3">
        <SummarySection
          icon="🔍"
          title="Portföy Karakteri"
          items={characteristics}
          colorScheme="purple"
        />
        <SummarySection
          icon="⚠️"
          title="Risk Alanları"
          items={riskAreas}
          colorScheme="red"
        />
        <SummarySection
          icon="✅"
          title="Olumlu Faktörler"
          items={positiveFactors}
          colorScheme="emerald"
        />
      </div>
    </div>
  )
}

export default function ExecPortfolioSummary({ productGroup, startDate, endDate }) {

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
      console.error(`${config.name} fetch error:`, err)
      setModels(prev => ({
        ...prev,
        [modelKey]: { data: null, loading: false, error: err.message || 'Bir hata oluştu' }
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

  
  useEffect(() => {
    if (startDate && endDate) {
      fetchAllModels()
    }
  }, [productGroup, startDate, endDate, fetchAllModels])


  if (!startDate || !endDate) {
    return null
  }

  return (
    <div className="grid grid-cols-1 xl:grid-cols-3 gap-4">
      {Object.entries(MODEL_CONFIG).map(([key, config]) => (
        <ModelPortfolioCard
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
  )
}