export default function SectionLabel({ icon, children }) {
  return (
    <p className="flex items-center gap-1.5 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-3">
      {icon && <span>{icon}</span>}
      {children}
    </p>
  )
}