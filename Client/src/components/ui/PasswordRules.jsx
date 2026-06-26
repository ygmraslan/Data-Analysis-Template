import { passwordRules } from '../../utils/passwordSchema'

export default function PasswordRules({ value = '' }) {
  return (
    <div className="bg-slate-50 dark:bg-white/4 rounded-xl p-4 h-fit">
      <p className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-3">
        Şifre Gereksinimleri
      </p>
      <div className="space-y-2">
        {passwordRules.map((rule) => {
          const passed = rule.test(value)
          return (
            <div key={rule.label} className="flex items-center gap-2">
              <div className={`w-4 h-4 rounded-full flex items-center justify-center flex-shrink-0 transition-colors duration-200
                ${passed ? 'bg-emerald-500' : 'bg-slate-200 dark:bg-white/10'}`}>
                {passed && (
                  <svg width="8" height="8" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="3.5">
                    <polyline points="20 6 9 17 4 12" />
                  </svg>
                )}
              </div>
              <span className={`text-xs transition-colors duration-200
                ${passed ? 'text-emerald-600 dark:text-emerald-400' : 'text-slate-400 dark:text-slate-500'}`}>
                {rule.label}
              </span>
            </div>
          )
        })}
      </div>
    </div>
  )
}