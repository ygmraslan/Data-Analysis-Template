import { useState, useEffect, useRef } from 'react'
import { HiChevronDown, HiSearch, HiX } from 'react-icons/hi'

// ========================================
// DROPDOWN - Multi-select with search
// ========================================

export function Dropdown({ label, value, options, onChange, placeholder = 'Tümü', loading = false }) {
  const [open, setOpen] = useState(false)
  const [search, setSearch] = useState('')
  const ref = useRef(null)
  const inputRef = useRef(null)

  useEffect(() => {
    const handleClick = (e) => {
      if (ref.current && !ref.current.contains(e.target)) {
        setOpen(false)
        setSearch('')
      }
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  useEffect(() => {
    if (open && inputRef.current) {
      inputRef.current.focus()
    }
  }, [open])

  const displayValue = value && value.length > 0 
    ? (value.length === 1 ? value[0] : `${value.length} seçili`)
    : placeholder

  const filteredOptions = options.filter(opt => {
    const optLabel = typeof opt === 'string' ? opt : opt.label
    return optLabel.toLowerCase().includes(search.toLowerCase())
  })

  return (
    <div>
      {label && (
        <label className="block text-[11px] font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wide mb-1.5">
          {label}
        </label>
      )}
      <div ref={ref} className="relative">
        <button
          type="button"
          onClick={() => !loading && setOpen(!open)}
          disabled={loading}
          className={`w-full flex items-center justify-between gap-2 px-3 py-2.5 text-sm font-medium rounded-lg border transition-all
            ${loading 
              ? 'bg-slate-50 dark:bg-white/5 border-slate-200 dark:border-white/10 text-slate-400 cursor-wait'
              : open
                ? 'bg-emerald-50 dark:bg-emerald-500/20 border-emerald-300 dark:border-emerald-500/50 text-emerald-700 dark:text-emerald-300'
                : 'bg-white dark:bg-white/5 border-slate-200 dark:border-white/10 text-slate-700 dark:text-slate-200 hover:border-slate-300 dark:hover:border-white/20'
            }`}
        >
          <span className="truncate">{loading ? 'Yükleniyor...' : displayValue}</span>
          <HiChevronDown className={`w-4 h-4 flex-shrink-0 text-slate-400 transition-transform ${open ? 'rotate-180' : ''}`} />
        </button>

        {open && (
          <div className="absolute top-full left-0 right-0 mt-1 bg-white dark:bg-[#1a3a5c] border border-slate-200 dark:border-white/15 rounded-lg shadow-xl z-50 overflow-hidden">
            {/* Seçili olanlar */}
            {value && value.length > 0 && (
              <div className="p-2 border-b border-slate-100 dark:border-white/10 flex flex-wrap gap-1">
                {value.map(v => (
                  <span 
                    key={v} 
                    className="inline-flex items-center gap-1 px-1.5 py-0.5 text-[10px] bg-emerald-100 dark:bg-emerald-500/20 text-emerald-700 dark:text-emerald-300 rounded"
                  >
                    {v}
                    <HiX 
                      className="w-2.5 h-2.5 cursor-pointer hover:text-emerald-900 dark:hover:text-emerald-100" 
                      onClick={(e) => {
                        e.stopPropagation()
                        onChange(value.filter(x => x !== v))
                      }}
                    />
                  </span>
                ))}
              </div>
            )}

            {/* Arama */}
            <div className="p-2 border-b border-slate-100 dark:border-white/10">
              <div className="relative">
                <HiSearch className="absolute left-2 top-1/2 -translate-y-1/2 w-3 h-3 text-slate-400" />
                <input
                  ref={inputRef}
                  type="text"
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  placeholder="Ara..."
                  className="w-full pl-6 pr-2 py-1 text-xs bg-slate-50 dark:bg-white/5 border border-slate-200 dark:border-white/10 rounded text-slate-700 dark:text-slate-200 placeholder-slate-400 focus:outline-none focus:border-emerald-300"
                />
              </div>
            </div>

            {/* Options */}
            <div className="max-h-36 overflow-y-auto py-1">
              {filteredOptions.length > 0 ? (
                filteredOptions.map((opt) => {
                  const optValue = typeof opt === 'string' ? opt : opt.value
                  const optLabel = typeof opt === 'string' ? opt : opt.label
                  const isSelected = value?.includes(optValue)

                  return (
                    <button
                      key={optValue}
                      type="button"
                      onClick={() => {
                        if (isSelected) {
                          onChange(value.filter(v => v !== optValue))
                        } else {
                          onChange([...(value || []), optValue])
                        }
                      }}
                      className={`w-full text-left px-2.5 py-1 text-xs transition-colors flex items-center gap-2
                        ${isSelected
                          ? 'bg-emerald-50 dark:bg-emerald-500/20 text-emerald-600 dark:text-emerald-300 font-medium'
                          : 'text-slate-600 dark:text-slate-200 hover:bg-slate-50 dark:hover:bg-white/10'
                        }`}
                    >
                      <span className={`w-3 h-3 rounded border flex items-center justify-center ${isSelected ? 'bg-emerald-500 border-emerald-500' : 'border-slate-300 dark:border-slate-500'}`}>
                        {isSelected && <svg className="w-2 h-2 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={3}><path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" /></svg>}
                      </span>
                      {optLabel}
                    </button>
                  )
                })
              ) : (
                <div className="px-2.5 py-2 text-xs text-slate-400">Sonuç bulunamadı</div>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  )
}

// ========================================
// SINGLE SELECT DROPDOWN
// ========================================

export function SingleDropdown({ label, value, options, onChange, placeholder = 'Seçiniz', loading = false }) {
  const [open, setOpen] = useState(false)
  const ref = useRef(null)

  useEffect(() => {
    const handleClick = (e) => {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false)
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  const selected = options.find(o => o.value === value)

  return (
    <div>
      {label && (
        <label className="block text-[11px] font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wide mb-1.5">
          {label}
        </label>
      )}
      <div ref={ref} className="relative">
        <button
          type="button"
          onClick={() => !loading && setOpen(!open)}
          disabled={loading}
          className={`w-full flex items-center justify-between gap-2 px-3 py-2.5 text-sm font-medium rounded-lg border transition-all
            ${loading
              ? 'bg-slate-50 dark:bg-white/5 border-slate-200 dark:border-white/10 text-slate-400 cursor-wait'
              : open
                ? 'bg-emerald-50 dark:bg-emerald-500/20 border-emerald-300 dark:border-emerald-500/50 text-emerald-700 dark:text-emerald-300'
                : 'bg-white dark:bg-white/5 border-slate-200 dark:border-white/10 text-slate-700 dark:text-slate-200 hover:border-slate-300 dark:hover:border-white/20'
            }`}
        >
          <span className="truncate">{loading ? 'Yükleniyor...' : (selected?.label || placeholder)}</span>
          <HiChevronDown className={`w-4 h-4 flex-shrink-0 text-slate-400 transition-transform ${open ? 'rotate-180' : ''}`} />
        </button>

        {open && (
          <div className="absolute top-full left-0 right-0 mt-1 bg-white dark:bg-[#1a3a5c] border border-slate-200 dark:border-white/15 rounded-lg shadow-xl z-50 py-1 max-h-40 overflow-y-auto">
            {options.map((opt) => (
              <button
                key={opt.value}
                type="button"
                onClick={() => { onChange(opt.value); setOpen(false) }}
                className={`w-full text-left px-2.5 py-1 text-xs transition-colors
                  ${opt.value === value
                    ? 'bg-emerald-50 dark:bg-emerald-500/20 text-emerald-600 dark:text-emerald-300 font-medium'
                    : 'text-slate-600 dark:text-slate-200 hover:bg-slate-50 dark:hover:bg-white/10'
                  }`}
              >
                {opt.label}
              </button>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}