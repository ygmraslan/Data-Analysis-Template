export default function PageTitle({ icon, title, subtitle, action }) {
  return (
    <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 pb-1 border-b border-slate-200 dark:border-white/8">
      <div className="flex items-center gap-3">
        {icon && (
          <div className="w-8 h-8 rounded-lg bg-emerald-500/10 dark:bg-emerald-500/20 flex items-center justify-center flex-shrink-0 text-emerald-500">
            {icon}
          </div>
        )}
        <div>
          <h1 className="text-base font-bold text-slate-800 dark:text-white">{title}</h1>
          {subtitle && <p className="text-xs text-slate-400 dark:text-slate-500">{subtitle}</p>}
        </div>
      </div>
      {action && <div className="w-full sm:w-auto">{action}</div>}
    </div>
  )
}