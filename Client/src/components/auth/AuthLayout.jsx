import AuthLogo from './AuthLogo'

export default function AuthLayout({ children, maxWidth = 'max-w-2xl' }) {
  return (
    <div className="min-h-screen flex items-center justify-center p-6 bg-gradient-to-b from-slate-800 to-slate-950">
      <div className={`w-full ${maxWidth} bg-white rounded-3xl shadow-2xl p-10`}>
        <AuthLogo />
        {children}
      </div>
    </div>
  )
}