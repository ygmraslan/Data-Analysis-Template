import { useState, useEffect, useCallback } from 'react'
import { usePermission } from '../../hooks/usePermission'
import { getComparisons, getComparisonById, deleteComparison, runComparison } from '../../api/customSegmentApi'
import ConfirmModal from '../ui/ConfirmModal'
import ComparisonCard from './ComparisonCard'
import { HiPlus, HiSearch } from 'react-icons/hi'

export default function SavedComparisonsList({ productGroup, search, onCreateClick, onCountChange }) {
  const { hasPermission } = usePermission()

  const [comparisons, setComparisons] = useState([])
  const [loading, setLoading] = useState(true)
  const [expandedId, setExpandedId] = useState(null)
  const [runningId, setRunningId] = useState(null)
  const [details, setDetails] = useState({})
  const [deleteModal, setDeleteModal] = useState({ open: false, comparison: null })
  const [deleting, setDeleting] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await getComparisons(productGroup)
      const items = res.data.items || []
      setComparisons(items)
      onCountChange?.(items.length)
    } catch (err) {
      console.error('Load comparisons error:', err)
    } finally {
      setLoading(false)
    }
  }, [productGroup, onCountChange])

  useEffect(() => {
    load()
  }, [load])

  const filtered = comparisons.filter(c =>
    c.name.toLowerCase().includes(search.toLowerCase())
  )

  const handleToggleExpand = async (id) => {
    if (expandedId === id) {
      setExpandedId(null)
      return
    }
    setExpandedId(id)

    if (details[id]) return

    try {
      const res = await getComparisonById(id)
      setDetails(prev => ({ ...prev, [id]: res.data }))
    } catch (err) {
      console.error('Load comparison detail error:', err)
    }
  }

  const handleRun = async (comparison, weekStart, weekEnd) => {
    if (!weekStart || !weekEnd) return
    setRunningId(comparison.id)
    try {
      const res = await runComparison(comparison.id, weekStart, weekEnd)
      const data = res.data

      setDetails(prev => ({ ...prev, [comparison.id]: data }))

      setComparisons(prev => prev.map(c =>
        c.id === comparison.id
          ? {
              ...c,
              weekStart: data.weekStart,
              weekEnd: data.weekEnd,
              segmentA: c.segmentA ? {
                ...c.segmentA,
                endShare: data.segmentA?.endShare ?? c.segmentA.endShare,
                change: data.segmentA?.change ?? c.segmentA.change
              } : c.segmentA,
              segmentB: c.segmentB ? {
                ...c.segmentB,
                endShare: data.segmentB?.endShare ?? c.segmentB.endShare,
                change: data.segmentB?.change ?? c.segmentB.change
              } : c.segmentB
            }
          : c
      ))
    } catch (err) {
      console.error('Run comparison error:', err)
      alert('Yeniden hesaplama sırasında hata oluştu')
    } finally {
      setRunningId(null)
    }
  }

  const handleDeleteClick = (comparison) => {
    setDeleteModal({ open: true, comparison })
  }

  const handleDeleteConfirm = async () => {
    if (!deleteModal.comparison) return
    setDeleting(true)
    try {
      await deleteComparison(deleteModal.comparison.id)
      const next = comparisons.filter(c => c.id !== deleteModal.comparison.id)
      setComparisons(next)
      onCountChange?.(next.length)
      setDeleteModal({ open: false, comparison: null })
    } catch (err) {
      console.error('Delete comparison error:', err)
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
          {search ? 'Arama sonucu bulunamadı' : 'Henüz kayıtlı karşılaştırma yok'}
        </p>
        {!search && hasPermission('CustomSegment.Create') && (
          <button
            onClick={onCreateClick}
            className="mt-4 flex items-center gap-1.5 px-4 py-2 text-sm font-medium rounded-lg bg-emerald-600 hover:bg-emerald-700 text-white transition-colors"
          >
            <HiPlus className="w-4 h-4" />
            İlk Karşılaştırmayı Oluştur
          </button>
        )}
      </div>
    )
  }

  return (
    <>
      <div className="space-y-3">
        {filtered.map(comparison => (
          <ComparisonCard
            key={comparison.id}
            comparison={comparison}
            isExpanded={expandedId === comparison.id}
            onToggle={() => handleToggleExpand(comparison.id)}
            onRun={(weekStart, weekEnd) => handleRun(comparison, weekStart, weekEnd)}
            onDelete={() => handleDeleteClick(comparison)}
            running={runningId === comparison.id}
            detail={details[comparison.id]}
            hasRunPermission={hasPermission('CustomSegment.Run')}
            hasDeletePermission={hasPermission('CustomSegment.Delete')}
          />
        ))}
      </div>

      <ConfirmModal
        isOpen={deleteModal.open}
        onCancel={() => setDeleteModal({ open: false, comparison: null })}
        onConfirm={handleDeleteConfirm}
        title="Karşılaştırmayı Sil"
        description={`"${deleteModal.comparison?.name}" karşılaştırmasını silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`}
        confirmText="Sil"
        variant="danger"
        loading={deleting}
      />
    </>
  )
}