import { useState, useEffect, useCallback } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { getOptions, calculateDrift, saveSegment, saveComparison, getCalculateAiComment, getCompareAiComment } from '../../api/customSegmentApi'
import { getAvailableWeeks, getCurrentWeek } from '../../api/execSummaryApi'
import PageTitle from '../../components/ui/PageTitle'
import AlertMessage from '../../components/ui/AlertMessage'
import CustomSegmentFilters from '../../components/customSegment/CustomSegmentFilters'
import SingleSegmentResults from '../../components/customSegment/SingleSegmentResults'
import CompareSegmentResults from '../../components/customSegment/CompareSegmentResults'
import SaveSegmentModal from '../../components/customSegment/SaveSegmentModal'
import ConfirmModal from '../../components/ui/ConfirmModal'
import { HiFolder } from 'react-icons/hi'
import { useNavigate } from 'react-router-dom'
const INITIAL_FILTERS = {
  brands: [],
  insuredAges: [],
  insuredTypes: [],
  genders: [],
  vehicleAges: [],
  vehicleValues: []
}

const INITIAL_AI_COMMENTS = {
  deepseek: { comment: null, loading: false, error: null },
  gemini: { comment: null, loading: false, error: null },
  gpt: { comment: null, loading: false, error: null }
}
export default function CustomSegment() {
  const { hasPermission } = usePermission()
  const navigate = useNavigate()
  const currentYear = new Date().getFullYear()
  const currentMonth = new Date().getMonth() + 1
  const [productGroup, setProductGroup] = useState('KASKO')
  const [mode, setMode] = useState('single') // 'single' veya 'compare'
  const [options, setOptions] = useState(null)
  const [optionsLoading, setOptionsLoading] = useState(true)
  const [year, setYear] = useState(currentYear)
  const [month, setMonth] = useState(currentMonth)
  const [weeks, setWeeks] = useState([])
  const [selectedWeek, setSelectedWeek] = useState(null)
  const [weeksLoading, setWeeksLoading] = useState(true)
  const [initialLoading, setInitialLoading] = useState(true)
  const [filters, setFilters] = useState(INITIAL_FILTERS)
  const [filtersB, setFiltersB] = useState(INITIAL_FILTERS)
  const [result, setResult] = useState(null)
  const [compareResult, setCompareResult] = useState(null) // { resultA, resultB }
  const [calculating, setCalculating] = useState(false)
  const [saving, setSaving] = useState(false)
  const [aiComments, setAiComments] = useState(INITIAL_AI_COMMENTS)
  const [compareAiComments, setCompareAiComments] = useState(INITIAL_AI_COMMENTS)
  const [showSaveModal, setShowSaveModal] = useState(false)
  const [saveName, setSaveName] = useState('')
  const [showSuccessModal, setShowSuccessModal] = useState(false)
  const [pageError, setPageError] = useState(null)
  useEffect(() => {
    setOptionsLoading(true)
    getOptions(productGroup)
      .then(res => setOptions(res.data))
      .catch(err => console.error('Options error:', err))
      .finally(() => setOptionsLoading(false))
  }, [productGroup])

  useEffect(() => {
    setInitialLoading(true)
    getCurrentWeek()
      .then(res => {
        const week = res.data
        setYear(week.year)
        setMonth(week.month)
        setSelectedWeek(week)
        return getAvailableWeeks(week.year, week.month)
      })
      .then(res => setWeeks(res.data || []))
      .catch(err => console.error('Week error:', err))
      .finally(() => {
        setWeeksLoading(false)
        setInitialLoading(false)
      })
  }, [])

  useEffect(() => {
    if (initialLoading) return

    setWeeksLoading(true)
    getAvailableWeeks(year, month)
      .then(res => {
        const weekList = res.data || []
        setWeeks(weekList)
        if (weekList.length > 0) {
          setSelectedWeek(weekList[weekList.length - 1])
        } else {
          setSelectedWeek(null)
        }
      })
      .catch(err => console.error('Weeks error:', err))
      .finally(() => setWeeksLoading(false))
  }, [year, month, initialLoading])
  useEffect(() => {
    setFilters(INITIAL_FILTERS)
    setFiltersB(INITIAL_FILTERS)
    setResult(null)
    setCompareResult(null)
    setAiComments(INITIAL_AI_COMMENTS)
    setCompareAiComments(INITIAL_AI_COMMENTS)
    setPageError(null)
  }, [productGroup])
  useEffect(() => {
    setFilters(INITIAL_FILTERS)
    setFiltersB(INITIAL_FILTERS)
    setResult(null)
    setCompareResult(null)
    setAiComments(INITIAL_AI_COMMENTS)
    setCompareAiComments(INITIAL_AI_COMMENTS)
    setPageError(null)
  }, [mode])
  const fetchAiComment = useCallback(async (modelKey, driftResult) => {
    setAiComments(prev => ({
      ...prev,
      [modelKey]: { comment: null, loading: true, error: null }
    }))

    try {
      const payload = {
        productGroup,
        weekStart: selectedWeek.startDate,
        weekEnd: selectedWeek.endDate,
        filters,
        result: {
          totalPolicy: driftResult.totalPolicy,
          segmentCount: driftResult.segmentCount,
          startShare: driftResult.startShare,
          endShare: driftResult.endShare,
          change: driftResult.change,
          growthMultiple: driftResult.growthMultiple,
          weeklyData: driftResult.weeklyData
        }
      }
      const res = await getCalculateAiComment(modelKey, payload)
      setAiComments(prev => ({
        ...prev,
        [modelKey]: { comment: res.data.comment, loading: false, error: null }
      }))
    } catch (err) {
      console.error(`${modelKey} AI error:`, err)
      setAiComments(prev => ({
        ...prev,
        [modelKey]: { comment: null, loading: false, error: err.response?.data?.message || err.message || 'Hata oluştu' }
      }))
    }
  }, [productGroup, selectedWeek, filters])
  const fetchCompareAiComment = useCallback(async (modelKey, resultA, resultB) => {
    setCompareAiComments(prev => ({
      ...prev,
      [modelKey]: { comment: null, loading: true, error: null }
    }))

    try {
      const payload = {
        productGroup,
        weekStart: selectedWeek.startDate,
        weekEnd: selectedWeek.endDate,
        filtersA: filters,
        resultA: {
          totalPolicy: resultA.totalPolicy,
          segmentCount: resultA.segmentCount,
          startShare: resultA.startShare,
          endShare: resultA.endShare,
          change: resultA.change,
          growthMultiple: resultA.growthMultiple,
          weeklyData: resultA.weeklyData
        },
        filtersB,
        resultB: {
          totalPolicy: resultB.totalPolicy,
          segmentCount: resultB.segmentCount,
          startShare: resultB.startShare,
          endShare: resultB.endShare,
          change: resultB.change,
          growthMultiple: resultB.growthMultiple,
          weeklyData: resultB.weeklyData
        }
      }

      const res = await getCompareAiComment(modelKey, payload)
      setCompareAiComments(prev => ({
        ...prev,
        [modelKey]: { comment: res.data.comment, loading: false, error: null }
      }))
    } catch (err) {
      console.error(`${modelKey} compare AI error:`, err)
      setCompareAiComments(prev => ({
        ...prev,
        [modelKey]: { comment: null, loading: false, error: err.response?.data?.message || err.message || 'Hata oluştu' }
      }))
    }
  }, [productGroup, selectedWeek, filters, filtersB])

  const handleCalculate = useCallback(async () => {
    setPageError(null)

    if (!selectedWeek) {
      setPageError('Lütfen hafta seçin')
      return
    }

    const hasFilter = (f) =>
      f.brands?.length > 0 ||
      f.insuredAges?.length > 0 ||
      f.insuredTypes?.length > 0 ||
      f.genders?.length > 0 ||
      f.vehicleAges?.length > 0 ||
      f.vehicleValues?.length > 0
    if (mode === 'compare') {
      if (!hasFilter(filters)) {
        setPageError('Segment A için en az bir filtre seçin')
        return
      }
      if (!hasFilter(filtersB)) {
        setPageError('Segment B için en az bir filtre seçin')
        return
      }

      setCalculating(true)
      setCompareResult(null)
      setCompareAiComments(INITIAL_AI_COMMENTS)

      try {
        const baseRequest = {
          productGroup,
          weekStart: selectedWeek.startDate,
          weekEnd: selectedWeek.endDate
        }
        const [resA, resB] = await Promise.all([
          calculateDrift({ ...baseRequest, filters }),
          calculateDrift({ ...baseRequest, filters: filtersB })
        ])

        const compare = { resultA: resA.data, resultB: resB.data }
        setCompareResult(compare)
        fetchCompareAiComment('deepseek', resA.data, resB.data)
        fetchCompareAiComment('gemini', resA.data, resB.data)
        fetchCompareAiComment('gpt', resA.data, resB.data)

      } catch (err) {
        console.error('Compare calculate error:', err)
        setPageError('Karşılaştırma sırasında hata oluştu')
      } finally {
        setCalculating(false)
      }
      return
    }
    if (!hasFilter(filters)) {
      setPageError('En az bir filtre seçin')
      return
    }

    setCalculating(true)
    setAiComments(INITIAL_AI_COMMENTS)

    try {
      const payload = {
        productGroup,
        weekStart: selectedWeek.startDate,
        weekEnd: selectedWeek.endDate,
        filters
      }
      const res = await calculateDrift(payload)
      const driftResult = res.data
      setResult(driftResult)

      fetchAiComment('deepseek', driftResult)
      fetchAiComment('gemini', driftResult)
      fetchAiComment('gpt', driftResult)

    } catch (err) {
      console.error('Calculate error:', err)
      setPageError('Hesaplama sırasında hata oluştu')
    } finally {
      setCalculating(false)
    }
  }, [mode, selectedWeek, filters, filtersB, productGroup, fetchAiComment, fetchCompareAiComment])

  const [saveError, setSaveError] = useState(null)

  const openSaveModal = () => {
    setSaveName('')
    setSaveError(null)
    setShowSaveModal(true)
  }

  const handleSave = useCallback(async () => {
    setSaveError(null)

    if (!saveName.trim()) {
      setSaveError('Lütfen bir isim girin')
      return
    }

    setSaving(true)
    try {
      if (mode === 'compare') {
        if (!compareResult) {
          setSaveError('Önce karşılaştırma hesaplayın')
          setSaving(false)
          return
        }

        await saveComparison({
          name: saveName.trim(),
          productGroup,
          weekStart: selectedWeek.startDate,
          weekEnd: selectedWeek.endDate,
          segmentA: {
            filters,
            result: {
              totalPolicy: compareResult.resultA.totalPolicy,
              segmentCount: compareResult.resultA.segmentCount,
              startShare: compareResult.resultA.startShare,
              endShare: compareResult.resultA.endShare,
              change: compareResult.resultA.change,
              growthMultiple: compareResult.resultA.growthMultiple,
              weeklyData: compareResult.resultA.weeklyData
            }
          },
          segmentB: {
            filters: filtersB,
            result: {
              totalPolicy: compareResult.resultB.totalPolicy,
              segmentCount: compareResult.resultB.segmentCount,
              startShare: compareResult.resultB.startShare,
              endShare: compareResult.resultB.endShare,
              change: compareResult.resultB.change,
              growthMultiple: compareResult.resultB.growthMultiple,
              weeklyData: compareResult.resultB.weeklyData
            }
          },
          aiComments: {
            deepSeek: compareAiComments.deepseek?.comment || null,
            gemini: compareAiComments.gemini?.comment || null,
            gpt: compareAiComments.gpt?.comment || null
          }
        })

        setShowSaveModal(false)
        setSaveName('')
        setShowSuccessModal(true)
        return
      }
      if (!result) {
        setSaveError('Önce drift hesaplayın')
        setSaving(false)
        return
      }

      await saveSegment({
        name: saveName.trim(),
        productGroup,
        weekStart: selectedWeek.startDate,
        weekEnd: selectedWeek.endDate,
        filters,
        result: {
          totalPolicy: result.totalPolicy,
          segmentCount: result.segmentCount,
          startShare: result.startShare,
          endShare: result.endShare,
          change: result.change,
          growthMultiple: result.growthMultiple,
          weeklyData: result.weeklyData
        },
        aiComments: {
          deepSeek: aiComments.deepseek?.comment || null,
          gemini: aiComments.gemini?.comment || null,
          gpt: aiComments.gpt?.comment || null
        }
      })
      setShowSaveModal(false)
      setSaveName('')
      setShowSuccessModal(true)
    } catch (err) {
      console.error('Save error:', err)
      setSaveError(err.response?.data?.message || 'Kaydetme sırasında hata oluştu')
    } finally {
      setSaving(false)
    }
  }, [mode, result, compareResult, saveName, productGroup, selectedWeek, filters, filtersB, aiComments])
  const metrics = result ? {
    startShare: result.startShare,
    endShare: result.endShare,
    change: result.change,
    growth: result.growthMultiple
  } : null

  const chartData = result?.weeklyData?.map(w => ({
    weekLabel: w.weekLabel,
    share: w.segmentShare,
    segmentCount: w.segmentCount,
    totalCount: w.totalPolicy,
    change: w.woW
  })) || []

  const tableData = result?.weeklyData?.map(w => ({
    weekLabel: w.weekLabel,
    totalCount: w.totalPolicy,
    segmentCount: w.segmentCount,
    share: w.segmentShare,
    change: w.woW
  })) || []
  const buildSeriesData = (res) => (res?.weeklyData || []).map(w => ({
    weekLabel: w.weekLabel,
    share: w.segmentShare,
    segmentCount: w.segmentCount,
    totalCount: w.totalPolicy,
    change: w.woW
  }))

  const chartDataA = buildSeriesData(compareResult?.resultA)
  const chartDataB = buildSeriesData(compareResult?.resultB)
  const tableDataA = chartDataA
  const tableDataB = chartDataB
  const productGroupTabs = (
    <div className="flex items-center gap-1 bg-slate-100 dark:bg-white/8 rounded-lg p-1">
      {['KASKO', 'TRAFIK'].map(pg => (
        <button
          key={pg}
          onClick={() => setProductGroup(pg)}
          className={`px-5 py-1.5 rounded-md text-xs font-semibold transition-all ${
            productGroup === pg
              ? 'bg-white dark:bg-white/15 text-slate-800 dark:text-white shadow-sm'
              : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
          }`}
        >
          {pg === 'TRAFIK' ? 'TRAFİK' : pg}
        </button>
      ))}
    </div>
  )

  return (
    <div className="space-y-6">
      {/* Header */}
      <PageTitle
        title="Özel Segment Drift Analizi"
        action={
          <div className="flex items-center gap-3">
            {productGroupTabs}
            
            {hasPermission('CustomSegment.View') && (
              <button
                onClick={() => navigate('/custom-segment/saved')}
                className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg border border-amber-300 dark:border-amber-500/30 text-amber-700 dark:text-amber-400 hover:bg-amber-50 dark:hover:bg-amber-500/10 transition-colors"
              >
                <HiFolder className="w-3.5 h-3.5" />
                Kayıtlı Segmentler
              </button>
            )}
          </div>
        }
      />

      {/* Sayfa-üstü uyarı */}
      {pageError && (
        <AlertMessage type="error" message={pageError} />
      )}

      {/* Filtre Kartı */}
      <CustomSegmentFilters
        options={options}
        optionsLoading={optionsLoading}
        filters={filters}
        onChange={setFilters}
        filtersB={filtersB}
        onChangeB={setFiltersB}
        mode={mode}
        onModeChange={setMode}
        year={year}
        month={month}
        weeks={weeks}
        selectedWeek={selectedWeek}
        onYearChange={setYear}
        onMonthChange={setMonth}
        onWeekChange={setSelectedWeek}
        weeksLoading={weeksLoading}
        onCalculate={handleCalculate}
        calculating={calculating}
      />

      {mode === 'single' && result && (
        <SingleSegmentResults
          metrics={metrics}
          chartData={chartData}
          tableData={tableData}
          aiComments={aiComments}
          weekStart={selectedWeek?.startDate}
          weekEnd={selectedWeek?.endDate}
          canSave={hasPermission('CustomSegment.Create')}
          onSave={openSaveModal}
        />
      )}

      {mode === 'compare' && compareResult && (
        <CompareSegmentResults
          resultA={compareResult.resultA}
          resultB={compareResult.resultB}
          chartDataA={chartDataA}
          chartDataB={chartDataB}
          tableDataA={tableDataA}
          tableDataB={tableDataB}
          aiComments={compareAiComments}
          canSave={hasPermission('CustomSegment.Create')}
          onSave={openSaveModal}
        />
      )}

      <SaveSegmentModal
        isOpen={showSaveModal}
        mode={mode}
        saveName={saveName}
        setSaveName={setSaveName}
        saveError={saveError}
        setSaveError={setSaveError}
        saving={saving}
        onSave={handleSave}
        onClose={() => setShowSaveModal(false)}
      />

      <ConfirmModal
        isOpen={showSuccessModal}
        title={mode === 'compare' ? 'Karşılaştırma Kaydedildi' : 'Segment Kaydedildi'}
        description={mode === 'compare'
          ? 'Karşılaştırmanız başarıyla kaydedildi. Ne yapmak istersiniz?'
          : 'Segmentiniz başarıyla kaydedildi. Ne yapmak istersiniz?'}
        confirmText="Kayıtlılara Git"
        cancelText="Kapat"
        variant="primary"
        onCancel={() => setShowSuccessModal(false)}
        onConfirm={() => {
          setShowSuccessModal(false)
          navigate('/custom-segment/saved')
        }}
      />
    </div>
  )
}