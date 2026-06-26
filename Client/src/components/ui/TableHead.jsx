export default function TableHead({ children, className = '' }) {
  return (
    <th className={`px-6 py-3.5 text-left text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider ${className}`}>
      {children}
    </th>
  )
}