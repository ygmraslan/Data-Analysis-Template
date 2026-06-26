import { useState } from 'react'

export function useExport(exportFn, fileName) {
  const [loading, setLoading] = useState(false)
  const [error, setError]     = useState('')

  const trigger = async (...args) => {
    setLoading(true)
    setError('')
    try {
      const response = await exportFn(...args)
      const url = URL.createObjectURL(response.data)
      const a   = document.createElement('a')
      a.href     = url
      a.download = fileName
      a.click()
      URL.revokeObjectURL(url)
    } catch {
      setError('Dosya indirilemedi.')
    } finally {
      setLoading(false)
    }
  }

  return { trigger, loading, error }
}