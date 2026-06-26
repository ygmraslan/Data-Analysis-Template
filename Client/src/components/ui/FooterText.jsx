export default function FooterText({ className = '' }) {
  return (
    <p className={`text-xs text-slate-400 dark:text-slate-600 text-center select-none ${className}`}>
      © 2026 DataAnalysis
    </p>
  )
}