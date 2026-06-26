import { useState } from 'react'

function EyeIcon({ open }) {
  return open ? (
    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
      <line x1="1" y1="1" x2="23" y2="23" />
    </svg>
  ) : (
    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
      <circle cx="12" cy="12" r="3" />
    </svg>
  )
}

export default function PasswordInput({ register, name, label, placeholder = '••••••••', error, size = 'md' }) {
  const [show, setShow] = useState(false)

  const inputClass = size === 'lg'
    ? 'w-full h-16 rounded-2xl pl-14 pr-14 text-base font-medium text-slate-900 bg-slate-50 outline-none transition-all border-2 border-slate-100 focus:border-emerald-400 focus:bg-white focus:shadow-[0_0_0_4px_rgba(52,211,153,0.08)]'
    : 'w-full h-10 px-3 pr-10 rounded-lg border text-sm transition-all bg-slate-50 border-slate-200 text-slate-800 placeholder-slate-300 dark:bg-white/6 dark:border-white/10 dark:text-white dark:placeholder-white/20 focus:outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-400/20'

  return (
    <div>
      {label && (
        <label className="block text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-1.5">
          {label}
        </label>
      )}
      <div className="relative">
        <input
          {...register(name)}
          type={show ? 'text' : 'password'}
          placeholder={placeholder}
          className={`${inputClass} ${size === 'lg' && error ? 'border-red-400' : ''}`}
        />
        <button
          type="button"
          onClick={() => setShow(!show)}
          className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600 dark:hover:text-slate-200"
        >
          <EyeIcon open={show} />
        </button>
      </div>
      {error && <p className="text-xs text-red-500 mt-1">{error.message}</p>}
    </div>
  )
}