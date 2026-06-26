export default function AuthLogo() {
  return (
    <div className="flex items-center gap-3 mb-8">
      <div className="w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0 bg-emerald-400">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#fff" strokeWidth="2.5">
          <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5" />
        </svg>
      </div>
      <span className="text-2xl font-black text-slate-900">
        Data<span className="text-emerald-400">Analysis</span>
      </span>
    </div>
  )
}