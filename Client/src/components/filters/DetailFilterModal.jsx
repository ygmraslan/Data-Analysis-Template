import { useState, useEffect, useMemo } from 'react'
import { HiFilter } from 'react-icons/hi'
import { Dropdown } from '../customSegment/Dropdown'
import { getFilterOptions } from '../../api/filtersApi'
import { EMPTY_FILTER } from '../../utils/detailFilter'

const ALL = ['insuredType', 'product', 'businessSource', 'vehicleType']

function count(f, enabled = ALL) {
  if (!f) return 0
  let n = 0
  if (enabled.includes('insuredType'))    n += f.insuredTypes?.length    || 0
  if (enabled.includes('product'))        n += f.productCodes?.length     || 0
  if (enabled.includes('businessSource')) n += f.businessSources?.length  || 0
  if (enabled.includes('vehicleType'))    n += f.vehicleTypes?.length     || 0
  return n
}

export default function DetailFilterModal({ productGroup, value, onChange, enabled = ALL }) {
  const [open, setOpen]       = useState(false)
  const [draft, setDraft]     = useState(value || EMPTY_FILTER)
  const [options, setOptions] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let alive = true
    getFilterOptions()
      .then(r => { if (alive) setOptions(r.data) })
      .catch(() => { if (alive) setOptions(null) })
      .finally(() => { if (alive) setLoading(false) })
    return () => { alive = false }
  }, [])

  useEffect(() => {
    if (open) { setDraft(value || EMPTY_FILTER); document.body.style.overflow = 'hidden' }
    else { document.body.style.overflow = '' }
    return () => { document.body.style.overflow = '' }
  }, [open]) 

   const productOptions = useMemo(() => {
    const list = options?.product?.[productGroup] || []
    return list.map(p => ({ value: p.code, label: `${p.code} — ${p.label}` }))
  }, [options, productGroup])

  const set   = (key, arr) => setDraft(d => ({ ...d, [key]: arr }))
  const show  = (k) => enabled.includes(k)
  const apply = () => { onChange(draft); setOpen(false) }

  const clearDraft = () => setDraft(d => ({
    insuredTypes:    show('insuredType')    ? [] : d.insuredTypes,
    productCodes:    show('product')        ? [] : d.productCodes,
    businessSources: show('businessSource') ? [] : d.businessSources,
    vehicleTypes:    show('vehicleType')    ? [] : d.vehicleTypes,
  }))

  const activeCount = count(value, enabled)

  return (
    <>
      {/* Tetikleyici buton (toggle'ın yanına) */}
      <button
        type="button"
        onClick={() => setOpen(true)}
        className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-semibold border transition-all
          ${activeCount > 0
            ? 'bg-emerald-50 dark:bg-emerald-500/20 border-emerald-300 dark:border-emerald-500/50 text-emerald-700 dark:text-emerald-300'
            : 'bg-white dark:bg-white/5 border-slate-200 dark:border-white/10 text-slate-600 dark:text-slate-300 hover:border-slate-300 dark:hover:border-white/20'
          }`}
      >
        <HiFilter className="w-3.5 h-3.5" />
        Filtrele
        {activeCount > 0 && (
          <span className="inline-flex items-center justify-center min-w-[16px] h-4 px-1 rounded-full bg-emerald-500 text-white text-[10px] font-bold">
            {activeCount}
          </span>
        )}
      </button>

      {/* Modal (üstten açılır) */}
      {open && (
        <div className="fixed inset-0 z-50 flex items-start justify-center p-4 pt-20">
          <div
            className="absolute inset-0 bg-black/40 dark:bg-black/60 backdrop-blur-sm"
            onClick={() => setOpen(false)}
          />
          <div className="relative w-full max-w-lg bg-white dark:bg-[#002147] rounded-2xl shadow-2xl border border-slate-200 dark:border-white/8 animate-in fade-in zoom-in-95 duration-200">
            {/* Header */}
            <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100 dark:border-white/10">
              <h3 className="text-base font-bold text-slate-800 dark:text-white" style={{ fontFamily: 'Montserrat, sans-serif' }}>
                Filtrele
              </h3>
              <button type="button" onClick={() => setOpen(false)}
                className="text-slate-400 hover:text-slate-600 dark:hover:text-slate-200">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                </svg>
              </button>
            </div>

            {/* Body — dropdownlar */}
            <div className="p-5 grid grid-cols-1 sm:grid-cols-2 gap-4">
              {show('insuredType') && (
                <Dropdown label="Sigortalı Türü" value={draft.insuredTypes}
                  options={options?.insuredType || []} onChange={v => set('insuredTypes', v)} loading={loading} />
              )}
              {show('product') && (
                <Dropdown label="Ürün" value={draft.productCodes}
                  options={productOptions} onChange={v => set('productCodes', v)} loading={loading} />
              )}
              {show('businessSource') && (
                <Dropdown label="İş Kaynağı" value={draft.businessSources}
                  options={options?.businessSource || []} onChange={v => set('businessSources', v)} loading={loading} />
              )}
              {show('vehicleType') && (
                <Dropdown label="Araç Tipi" value={draft.vehicleTypes}
                  options={options?.vehicleType || []} onChange={v => set('vehicleTypes', v)} loading={loading} />
              )}
            </div>

            {/* İş Kaynağı bu sayfada kapalıysa bilgi notu */}
            {!show('businessSource') && (
              <div className="px-5 pb-2 -mt-2">
                <p className="text-[11px] italic text-slate-400 dark:text-slate-500">
                  Bu sayfada iş kaynağı filtresi uygulanmamaktadır.
                </p>
              </div>
            )}

            {/* Footer */}
            <div className="flex items-center justify-between px-5 py-4 border-t border-slate-100 dark:border-white/10">
              <button type="button" onClick={clearDraft}
                className="text-xs font-medium text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200">
                Temizle
              </button>
              <div className="flex gap-2">
                <button type="button" onClick={() => setOpen(false)}
                  className="px-4 py-2 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-white/8">
                  İptal
                </button>
                <button type="button" onClick={apply}
                  className="px-4 py-2 rounded-lg text-sm font-semibold bg-slate-900 dark:bg-emerald-500 hover:bg-slate-700 dark:hover:bg-emerald-400 text-white">
                  Uygula
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  )
}