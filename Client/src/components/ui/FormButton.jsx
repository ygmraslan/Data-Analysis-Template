export default function FormButton({ loading, loadingText, children, disabled, className = '', onClick, type = 'submit' }) {
  return (
    <button
      type={type}
      onClick={onClick}
      disabled={loading || disabled}
      className={`flex items-center justify-center gap-2 font-semibold transition-all
        disabled:opacity-50 disabled:cursor-not-allowed ${className}`}
    >
      {loading ? (
        <>
          <svg className="animate-spin" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M21 12a9 9 0 1 1-6.219-8.56" />
          </svg>
          {loadingText || 'Yükleniyor...'}
        </>
      ) : children}
    </button>
  )
}