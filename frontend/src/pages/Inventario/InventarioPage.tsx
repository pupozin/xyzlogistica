import { useEffect, useState } from 'react'
import '../Cadastros/CadastroListPage.css'

type InventarioItem = {
  produtoId: number
  produtoDescricao: string
  quantidadeAtual: number
  dataUltimaAtualizacao: string
}

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

function getAuthHeaders() {
  const token = localStorage.getItem('zyx.token')
  const headers = new Headers()

  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  return headers
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))
}

function formatQuantity(value: number) {
  return new Intl.NumberFormat('pt-BR', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 3,
  }).format(value)
}

function InventarioPage() {
  const [items, setItems] = useState<InventarioItem[]>([])
  const [search, setSearch] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [message, setMessage] = useState('')

  async function loadItems(searchTerm = search) {
    setIsLoading(true)
    setMessage('')

    try {
      const query = new URLSearchParams()

      if (searchTerm.trim()) {
        query.set('produto', searchTerm.trim())
      }

      const response = await fetch(`${apiBaseUrl}/api/inventario${query.size ? `?${query}` : ''}`, {
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error('Não foi possível carregar o inventário.')
      }

      setItems((await response.json()) as InventarioItem[])
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar inventário.')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    void loadItems('')
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      void loadItems(search)
    }, 350)

    return () => window.clearTimeout(timeoutId)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search])

  return (
    <section className="cadastro-page">
      <header className="cadastro-header">
        <div className="cadastro-title-actions">
          <h1>Inventário</h1>
        </div>

        <div className="cadastro-filter">
          <label htmlFor="inventario-search">Filtro:</label>
          <input
            id="inventario-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Produto"
          />
        </div>
      </header>

      <div className="cadastro-card">
        <div className="cadastro-table-wrap">
          <table className="cadastro-table">
            <thead>
              <tr>
                <th>Produto</th>
                <th>Quantidade atual</th>
                <th>Última atualização</th>
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.produtoId}>
                  <td>{item.produtoDescricao}</td>
                  <td>{formatQuantity(item.quantidadeAtual)}</td>
                  <td>{formatDate(item.dataUltimaAtualizacao)}</td>
                </tr>
              ))}

              {!isLoading && items.length === 0 && (
                <tr>
                  <td className="empty-cell" colSpan={3}>
                    Nenhum registro encontrado.
                  </td>
                </tr>
              )}

              {isLoading && (
                <tr>
                  <td className="empty-cell" colSpan={3}>
                    Carregando registros...
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        <footer className="cadastro-footer">
          {message && <span className="cadastro-message">{message}</span>}
          <div className="pagination">
            <button type="button">‹</button>
            <span>1</span>
            <button type="button">›</button>
          </div>
        </footer>
      </div>
    </section>
  )
}

export default InventarioPage
