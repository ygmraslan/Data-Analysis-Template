export default function SectionLabel({ icon, number, children }) {
  return (
    <div className="flex items-center gap-2.5 mb-4">
      {number && (
        <span className="text-[10px] font-mono text-slate-400 dark:text-slate-500 border border-slate-200 dark:border-white/10 rounded px-1.5 py-0.5 bg-white dark:bg-white/5">
          {number}
        </span>
      )}
      <h2 className="text-base font-semibold text-slate-800 dark:text-white" style={{ fontFamily: "'Fraunces', Georgia, serif" }}>
        {icon && <span className="mr-1.5">{icon}</span>}
        {children}
      </h2>
    </div>
  )
}