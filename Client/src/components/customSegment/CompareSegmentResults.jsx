import { useState } from 'react'
import SectionLabel from '../ui/SectionLabel'
import CompareMetricsSection from './CompareMetricsSection'
import CustomSegmentChart from './CustomSegmentChart'
import CustomSegmentTable from './CustomSegmentTable'
import CustomSegmentAiSection from './CustomSegmentAiSection'
import SaveResultButton from './SaveResultButton'

function ViewModeToggle({ value, onChange }) {
  const options = [
    { key: 'share', label: 'Pay' },
    { key: 'count', label: 'Poliçe Sayısı' }
  ]
  return (
    <div className="flex items-center bg-slate-100 dark:bg-white/8 rounded-lg p-0.5">
      {options.map(opt => (
        <button
          key={opt.key}
          onClick={() => onChange(opt.key)}
          className={`px-3 py-1.5 rounded-md text-xs font-medium transition-all ${
            value === opt.key
              ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
              : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
          }`}
        >
          {opt.label}
        </button>
      ))}
    </div>
  )
}

export default function CompareSegmentResults({
  resultA,
  resultB,
  chartDataA,
  chartDataB,
  tableDataA,
  tableDataB,
  aiComments,
  canSave,
  onSave
}) {
  const [viewMode, setViewMode] = useState('share')

  return (
    <>
      <div>
        <div className="flex items-center justify-between mb-3">
          <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
            Drift Metrikleri
          </p>
          <ViewModeToggle value={viewMode} onChange={setViewMode} />
        </div>
        <CompareMetricsSection resultA={resultA} resultB={resultB} viewMode={viewMode} />
      </div>

      <div>
        <SectionLabel>Haftalık Trend</SectionLabel>
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <CustomSegmentChart dataA={chartDataA} dataB={chartDataB} viewMode={viewMode} />
          <CustomSegmentTable dataA={tableDataA} dataB={tableDataB} viewMode={viewMode} />
        </div>
      </div>

      <div>
        <SectionLabel>AI Analiz Yorumları</SectionLabel>
        <CustomSegmentAiSection
          title="Karşılaştırma Değerlendirmesi"
          subtitle="Segment A vs Segment B"
          aiComments={aiComments}
        />
      </div>

      {canSave && <SaveResultButton label="Karşılaştırmayı Kaydet" onClick={onSave} />}
    </>
  )
}