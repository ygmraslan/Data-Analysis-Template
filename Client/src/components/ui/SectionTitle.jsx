export default function SectionTitle({ icon, children }) {
  return (
    <div className="px-6 py-4 border-b border-slate-100 dark:border-white/8">
      <div className="flex items-center gap-2">
        {icon && <span className="text-slate-400">{icon}</span>}
        <span className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
          {children}
        </span>
      </div>
    </div>
  )
}