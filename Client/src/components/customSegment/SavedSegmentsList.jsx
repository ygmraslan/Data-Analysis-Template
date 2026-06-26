import { useState, useEffect, useCallback } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { getSegments, getSegmentById, deleteSegment, runSegment } from '../../api/customSegmentApi'
import ConfirmModal from '../ui/ConfirmModal'
import SegmentCard from './SegmentCard'
import { HiPlus, HiSearch } from 'react-icons/hi'

export default function SavedSegmentsList({ productGroup, search, onCreateClick, onCountChange }) {
  const { hasPermission } = usePermission()

  const [segments, setSegments] = useState([])
  const [loading, setLoading] = useState(true)
  const [expandedId, setExpandedId] = useState(null)
  const [runningId, setRunningId] = useState(null)
  const [results, setResults] = useState({})
  const [deleteModal, setDeleteModal] = useState({ open: false, segment: null })
  const [deleting, setDeleting] = useState(false)

  const loadSegments = useCallback(async () => {
    setLoading(true)
    try {
      const res = await getSegments(productGroup)
      const items = res.data.items || []
      setSegments(items)
      onCountChange?.(items.length)
    } catch (err) {
      console.error('Load segments error:', err)
    } finally {
      setLoading(false)
    }
  }, [productGroup, onCountChange])

  useEffect(() => {
    loadSegments()
  }, [loadSegments])

  const filtered = segments.filter(s =>
    s.name.toLowerCase().includes(search.toLowerCase())
  )

  const handleToggleExpand = async (segmentId) => {
    if (expandedId === segmentId) {
      setExpandedId(null)
      return
    }
    setExpandedId(segmentId)

    if (results[segmentId]) return

    try {
      const res = await getSegmentById(segmentId)
      const data = res.data
      if (data?.lastResult) {
        setResults(prev => ({
          ...prev,
          [segmentId]: {
            ...data.lastResult,
            weeklyData: data.lastResult.weeklyData || []
          }
        }))
      }
    } catch (err) {
      console.error('Load segment detail error:', err)
    }
  }

  const handleRun = async (segment, weekStart, weekEnd) => {
    if (!weekStart || !weekEnd) return
    setRunningId(segment.id)
    try {
      const res = await runSegment(segment.id, weekStart, weekEnd)
      setResults(prev => ({ ...prev, [segment.id]: res.data }))
      setSegments(prev => prev.map(s =>
        s.id === segment.id
          ? { ...s, lastResult: { ...res.data, createdByName: res.data.createdByName } }
          : s
      ))
    } catch (err) {
      console.error('Run segment error:', err)
      alert('Analiz sırasında hata oluştu')
    } finally {
      setRunningId(null)
    }
  }

  const handleDeleteClick = (segment) => {
    setDeleteModal({ open: true, segment })
  }

  const handleDeleteConfirm = async () => {
    if (!deleteModal.segment) return
    setDeleting(true)
    try {
      await deleteSegment(deleteModal.segment.id)
      const newSegments = segments.filter(s => s.id !== deleteModal.segment.id)
      setSegments(newSegments)
      onCountChange?.(newSegments.length)
      setDeleteModal({ open: false, segment: null })
    } catch (err) {
      console.error('Delete segment error:', err)
      alert('Silme sırasında hata oluştu')
    } finally {
      setDeleting(false)
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <div className="w-8 h-8 border-2 border-emerald-500 border-t-transparent rounded-full animate-spin" />
      </div>
    )
  }

  if (filtered.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-16 text-center">
        <div className="w-14 h-14 rounded-2xl bg-slate-100 dark:bg-white/5 flex items-center justify-center mb-3">
          <HiSearch className="w-7 h-7 text-slate-400" />
        </div>
        <p className="text-sm text-slate-500 dark:text-slate-400">
          {search ? 'Arama sonucu bulunamadı' : 'Henüz kayıtlı segment yok'}
        </p>
        {!search && hasPermission('CustomSegment.Create') && (
          <button
            onClick={onCreateClick}
            className="mt-4 flex items-center gap-1.5 px-4 py-2 text-sm font-medium rounded-lg bg-emerald-600 hover:bg-emerald-700 text-white transition-colors"
          >
            <HiPlus className="w-4 h-4" />
            İlk Segmenti Oluştur
          </button>
        )}
      </div>
    )
  }

  return (
    <>
      <div className="space-y-3">
        {filtered.map(segment => (
          <SegmentCard
            key={segment.id}
            segment={segment}
            isExpanded={expandedId === segment.id}
            onToggle={() => handleToggleExpand(segment.id)}
            onRun={(weekStart, weekEnd) => handleRun(segment, weekStart, weekEnd)}
            onDelete={() => handleDeleteClick(segment)}
            running={runningId === segment.id}
            result={results[segment.id]}
            hasRunPermission={hasPermission('CustomSegment.Run')}
            hasDeletePermission={hasPermission('CustomSegment.Delete')}
          />
        ))}
      </div>

      <ConfirmModal
        isOpen={deleteModal.open}
        onCancel={() => setDeleteModal({ open: false, segment: null })}
        onConfirm={handleDeleteConfirm}
        title="Segmenti Sil"
        description={`"${deleteModal.segment?.name}" segmentini silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`}
        confirmText="Sil"
        variant="danger"
        loading={deleting}
      />
    </>
  )
}