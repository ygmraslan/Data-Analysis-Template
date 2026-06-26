import SectionLabel from '../ui/SectionLabel'
import CustomSegmentMetrics from './CustomSegmentMetrics'
import CustomSegmentChart from './CustomSegmentChart'
import CustomSegmentTable from './CustomSegmentTable'
import CustomSegmentAiSection from './CustomSegmentAiSection'
import SaveResultButton from './SaveResultButton'

export default function SingleSegmentResults({
  metrics,
  chartData,
  tableData,
  aiComments,
  weekStart,
  weekEnd,
  canSave,
  onSave
}) {
  return (
    <>
      <div>
        <SectionLabel>Drift Metrikleri</SectionLabel>
        <CustomSegmentMetrics data={metrics} />
      </div>

      <div>
        <SectionLabel>Haftalık Trend</SectionLabel>
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <CustomSegmentChart data={chartData} />
          <CustomSegmentTable data={tableData} />
        </div>
      </div>

      <div>
        <SectionLabel>AI Analiz Yorumları</SectionLabel>
        <CustomSegmentAiSection
          weekStart={weekStart}
          weekEnd={weekEnd}
          aiComments={aiComments}
        />
      </div>

      {canSave && <SaveResultButton label="Segmenti Kaydet" onClick={onSave} />}
    </>
  )
}