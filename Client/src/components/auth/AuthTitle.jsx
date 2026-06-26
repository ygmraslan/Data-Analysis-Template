export default function AuthTitle({ children, subtitle, className = '' }) {
  return (
    <div className={`mb-8 ${className}`}>
      <h2 className="text-4xl font-bold text-slate-900 tracking-tight mb-2">
        {children}
      </h2>
      {subtitle && (
        <p className="text-sm text-slate-500">{subtitle}</p>
      )}
    </div>
  )
}