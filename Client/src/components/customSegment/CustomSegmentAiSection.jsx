import { DeepSeekLogo, GeminiLogo, GptLogo } from '../../assets/logos'

// ========================================
// SABITLER
// ========================================

const MODEL_CONFIG = {
  deepseek: { key: 'deepseek', name: 'DeepSeek', Logo: DeepSeekLogo },
  gemini: { key: 'gemini', name: 'Gemini', Logo: GeminiLogo },
  gpt: { key: 'gpt', name: 'GPT', Logo: GptLogo }
}

const MONTHS = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']

const formatDateRange = (startDate, endDate) => {
  if (!startDate || !endDate) return ''
  const start = new Date(startDate)
  const end = new Date(endDate)
  return `${start.getDate()} ${MONTHS[start.getMonth()]} - ${end.getDate()} ${MONTHS[end.getMonth()]} ${end.getFullYear()}`
}

// ========================================
// MODEL CARD COMPONENT
// ========================================

function ModelCard({ config, comment, loading, error }) {
  const Logo = config.Logo

  // Loading
  if (loading) {
    return (
      <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden h-[250px]">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-purple-200 dark:border-purple-500/30">
          <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-purple-500 to-violet-600 flex items-center justify-center text-white">
            <Logo className="w-5 h-5" />
          </div>
          <div className="flex-1">
            <div className="text-sm font-semibold text-slate-700 dark:text-slate-300">{config.name}</div>
            <div className="text-xs text-slate-500 dark:text-slate-400">Analiz hazırlanıyor...</div>
          </div>
          <div className="w-5 h-5 border-2 border-purple-300 border-t-purple-600 rounded-full animate-spin" />
        </div>
        <div className="p-4 space-y-2">
          <div className="h-4 bg-slate-100 dark:bg-white/5 rounded animate-pulse" />
          <div className="h-4 bg-slate-100 dark:bg-white/5 rounded animate-pulse w-4/5" />
          <div className="h-4 bg-slate-100 dark:bg-white/5 rounded animate-pulse w-3/5" />
        </div>
      </div>
    )
  }

  // Error
  if (error) {
    return (
      <div className="bg-white dark:bg-white/5 border border-red-200 dark:border-red-500/30 rounded-xl overflow-hidden h-[250px]">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-red-200 dark:border-red-500/30">
          <div className="w-8 h-8 rounded-lg bg-red-100 dark:bg-red-500/20 flex items-center justify-center text-red-600">
            <Logo className="w-5 h-5" />
          </div>
          <div className="flex-1">
            <div className="text-sm font-semibold text-red-700 dark:text-red-300">{config.name}</div>
            <div className="text-xs text-red-500 dark:text-red-400">Hata oluştu</div>
          </div>
        </div>
        <div className="p-4">
          <p className="text-xs text-slate-600 dark:text-slate-400">{error}</p>
        </div>
      </div>
    )
  }

  // No comment yet
  if (!comment) {
    return (
      <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden h-[250px]">
        <div className="flex items-center gap-3 px-4 py-3 border-b border-purple-200 dark:border-purple-500/30">
          <div className="w-8 h-8 rounded-lg bg-slate-100 dark:bg-white/10 flex items-center justify-center text-slate-400">
            <Logo className="w-5 h-5" />
          </div>
          <div className="text-sm font-semibold text-slate-500 dark:text-slate-400">{config.name}</div>
        </div>
        <div className="p-4 flex items-center justify-center min-h-[80px]">
          <p className="text-xs text-slate-400 dark:text-slate-500">Henüz analiz yapılmadı</p>
        </div>
      </div>
    )
  }

  // Success - show comment
  return (
    <div className="bg-white dark:bg-white/5 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden flex flex-col h-[250px]">
      <div className="flex items-center gap-3 px-4 py-3 border-b border-purple-200 dark:border-purple-500/30">
        <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-purple-500 to-violet-600 flex items-center justify-center text-white shadow-lg shadow-purple-500/25">
          <Logo className="w-5 h-5" />
        </div>
        <div className="text-sm font-semibold text-slate-700 dark:text-slate-300">{config.name}</div>
      </div>
      <div className="p-4 overflow-y-auto flex-1">
        <p className="text-xs text-slate-600 dark:text-slate-300 leading-relaxed whitespace-pre-line">
          {comment}
        </p>
      </div>
    </div>
  )
}

// ========================================
// MAIN COMPONENT
// ========================================

export default function CustomSegmentAiSection({ 
  weekStart,
  weekEnd,
  aiComments,  // { deepseek: { comment, loading, error }, gemini: {...}, gpt: {...} }
  title = 'Yapay Zeka Değerlendirmesi',
  subtitle = null,
}) {
  // Veri yoksa gösterme
  if (!aiComments) return null

  // Subtitle override gelmediyse hafta aralığı gösterilsin
  const subtitleText = subtitle ?? (weekStart && weekEnd ? `${formatDateRange(weekStart, weekEnd)} dönemi` : null)

  return (
    <div className="bg-gradient-to-br from-purple-50 via-violet-50 to-fuchsia-50 dark:from-purple-500/10 dark:via-violet-500/10 dark:to-fuchsia-500/10 border border-purple-200 dark:border-purple-500/30 rounded-xl overflow-hidden">
      {/* Header */}
      <div className="px-5 py-4 border-b border-purple-200 dark:border-purple-500/30">
        <h3 className="text-sm font-semibold text-purple-800 dark:text-purple-200">
          {title}
        </h3>
        {subtitleText && (
          <p className="text-xs text-purple-600 dark:text-purple-400 mt-0.5">
            {subtitleText}
          </p>
        )}
      </div>

      {/* 3 Model Kartları */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-4 p-4">
        {Object.entries(MODEL_CONFIG).map(([key, config]) => (
          <ModelCard
            key={key}
            config={config}
            comment={aiComments[key]?.comment}
            loading={aiComments[key]?.loading}
            error={aiComments[key]?.error}
          />
        ))}
      </div>
    </div>
  )
}