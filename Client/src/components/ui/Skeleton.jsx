export function SkeletonBlock({ width = 'w-full', height = 'h-4', rounded = 'rounded-md', className = '' }) {
  return (
    <div className={`${width} ${height} ${rounded} bg-slate-200 dark:bg-white/8 animate-pulse ${className}`} />
  )
}

export function SkeletonCard({ children, className = '' }) {
  return (
    <div className={`bg-white dark:bg-[#0d1f3c] border border-slate-200 dark:border-white/7 rounded-xl p-5 ${className}`}>
      {children}
    </div>
  )
}

export function SkeletonRow({ children, className = '' }) {
  return (
    <div className={`flex items-center gap-3 ${className}`}>
      {children}
    </div>
  )
}

export function SkeletonChart({ height = 180 }) {
  const bars = [45, 65, 50, 80, 60, 75, 55, 70]
  return (
    <div className="w-full flex items-end gap-2 px-2" style={{ height }}>
      {bars.map((h, i) => (
        <div
          key={i}
          className="flex-1 bg-slate-200 dark:bg-white/8 rounded-t-md animate-pulse"
          style={{
            height: `${h}%`,
            animationDelay: `${i * 80}ms`
          }}
        />
      ))}
    </div>
  )
}