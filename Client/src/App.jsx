import { RouterProvider } from 'react-router-dom'
import { useEffect } from 'react'
import router from './routes'
import useThemeStore from './store/themeStore'

function App() {
  const { toggleTheme } = useThemeStore()

  useEffect(() => {
    const saved = localStorage.getItem('theme')
    if (saved === 'dark') {
      toggleTheme()
    } else if (!saved) {
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
      if (prefersDark) toggleTheme()
    }
  }, [])

  return <RouterProvider router={router} />
}

export default App