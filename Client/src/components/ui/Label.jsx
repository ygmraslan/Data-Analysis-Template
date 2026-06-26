export default function Label({ children, className = '' }) {
  return (
    <label className={`block text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-1.5 ${className}`}>
      {children}
    </label>
  )
}