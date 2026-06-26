import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { usePermission } from '../../hooks/usePermission'
import PageTitle from '../../components/ui/PageTitle'
import SavedSegmentsList from '../../components/customSegment/SavedSegmentsList'
import SavedComparisonsList from '../../components/customSegment/SavedComparisonsList'
import { HiPlus } from 'react-icons/hi'

const PRODUCT_GROUPS = [
  { value: 'KASKO', label: 'KASKO' },
  { value: 'TRAFIK', label: 'TRAFİK' }
]

const TABS = [
  { key: 'segments', label: 'Segmentler' },
  { key: 'comparisons', label: 'Karşılaştırmalar' }
]

const PageIcon = (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z" />
  </svg>
)

export default function SavedSegments() {
  const { hasPermission } = usePermission()
  const navigate = useNavigate()

  const [productGroup, setProductGroup] = useState('KASKO')
  const [activeTab, setActiveTab] = useState('segments')
  const [search, setSearch] = useState('')
  const [segmentCount, setSegmentCount] = useState(null)
  const [comparisonCount, setComparisonCount] = useState(null)

  const handleTabChange = (key) => {
    setActiveTab(key)
    setSearch('')
  }

  const handleProductGroupChange = (pg) => {
    setProductGroup(pg)
    setSearch('')
  }

  const handleGoToCreate = () => {
    navigate('/custom-segment')
  }

  const renderCount = (n) => n === null ? '' : ` (${n})`
  const newButtonLabel = activeTab === 'segments' ? 'Yeni Segment' : 'Yeni Karşılaştırma'
  const searchPlaceholder = activeTab === 'segments'
    ? 'Segment adı ara...'
    : 'Karşılaştırma adı ara...'

  const headerAction = (
    <div className="flex items-center gap-2">
      <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
        {TABS.map(t => (
          <button
            key={t.key}
            onClick={() => handleTabChange(t.key)}
            className={`px-4 py-1.5 rounded-md text-xs font-semibold transition-all ${
              activeTab === t.key
                ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
            }`}
          >
            {t.label}{t.key === 'segments' ? renderCount(segmentCount) : renderCount(comparisonCount)}
          </button>
        ))}
      </div>

      {hasPermission('CustomSegment.Create') && (
        <button
          onClick={handleGoToCreate}
          className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-semibold rounded-lg bg-emerald-600 hover:bg-emerald-700 text-white transition-colors"
        >
          <HiPlus className="w-4 h-4" />
          {newButtonLabel}
        </button>
      )}
    </div>
  )

  return (
    <div className="space-y-4 md:space-y-5">
      <PageTitle icon={PageIcon} title="Kayıtlı Veriler" action={headerAction} />

      <div className="bg-white dark:bg-[#002147] rounded-2xl border border-slate-200 dark:border-white/8 overflow-hidden">
        <div className="flex flex-col md:flex-row items-start md:items-center gap-3 px-4 md:px-6 py-4 border-b border-slate-100 dark:border-white/8">
          <div className="relative w-full md:flex-1">
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder={searchPlaceholder}
              className="w-full h-10 md:h-9 pl-9 pr-4 rounded-lg border text-sm transition-all bg-slate-50 border-slate-200 text-slate-800 placeholder-slate-300 dark:bg-white/6 dark:border-white/10 dark:text-white dark:placeholder-white/20 focus:outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-400/20"
            />
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
              className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none">
              <circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
          </div>

          <div className="flex w-full md:w-auto items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1 overflow-x-auto">
            {PRODUCT_GROUPS.map(pg => (
              <button
                key={pg.value}
                onClick={() => handleProductGroupChange(pg.value)}
                className={`flex-1 md:flex-none whitespace-nowrap px-3 py-2 md:py-1.5 rounded-md text-sm md:text-xs font-semibold transition-all ${
                  productGroup === pg.value
                    ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
                    : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
                }`}
              >
                {pg.label}
              </button>
            ))}
          </div>
        </div>

        <div className="p-4 md:p-6">
          {activeTab === 'segments' && (
            <SavedSegmentsList
              productGroup={productGroup}
              search={search}
              onCreateClick={handleGoToCreate}
              onCountChange={setSegmentCount}
            />
          )}

          {activeTab === 'comparisons' && (
            <SavedComparisonsList
              productGroup={productGroup}
              search={search}
              onCreateClick={handleGoToCreate}
              onCountChange={setComparisonCount}
            />
          )}
        </div>
      </div>
    </div>
  )
}