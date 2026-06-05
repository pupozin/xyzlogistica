import { useEffect, useMemo, useState } from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faPen, faTrashCan } from '@fortawesome/free-solid-svg-icons'
import './CadastroListPage.css'
import { CadastroKey, cadastroConfigs } from './cadastroConfigs'

type CadastroListPageProps = {
  cadastro: CadastroKey
}

type GridRow = Record<string, unknown> & {
  id: number
}

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

function CadastroListPage({ cadastro }: CadastroListPageProps) {
  const config = cadastroConfigs[cadastro]
  const [rows, setRows] = useState<GridRow[]>([])
  const [search, setSearch] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [message, setMessage] = useState('')

  const visibleRows = useMemo(() => {
    if (cadastro !== 'perfil' || !search.trim()) {
      return rows
    }

    return rows.filter((row) =>
      String(row.descricao ?? '')
        .toLowerCase()
        .includes(search.trim().toLowerCase()),
    )
  }, [cadastro, rows, search])

  async function loadRows(searchTerm = search) {
    setIsLoading(true)
    setMessage('')

    try {
      const token = localStorage.getItem('zyx.token')
      const query = config.buildQuery(searchTerm.trim())
      const url = `${apiBaseUrl}${config.endpoint}${query.size ? `?${query}` : ''}`
      const response = await fetch(url, {
        headers: token ? { Authorization: `Bearer ${token}` } : undefined,
      })

      if (!response.ok) {
        throw new Error('Nao foi possivel carregar os registros.')
      }

      const data = (await response.json()) as GridRow[]
      setRows(data)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar registros.')
    } finally {
      setIsLoading(false)
    }
  }

  async function handleDelete(row: GridRow) {
    const confirmed = window.confirm('Deseja excluir este registro?')

    if (!confirmed) {
      return
    }

    try {
      const token = localStorage.getItem('zyx.token')
      const response = await fetch(`${apiBaseUrl}${config.endpoint}/${row.id}`, {
        method: 'DELETE',
        headers: token ? { Authorization: `Bearer ${token}` } : undefined,
      })

      if (!response.ok) {
        throw new Error('Nao foi possivel excluir o registro.')
      }

      await loadRows()
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir registro.')
    }
  }

  function formatValue(row: GridRow, field: string, type?: string) {
    const value = row[field]

    if (value === null || value === undefined || value === '') {
      return '-'
    }

    if (type === 'date') {
      return new Intl.DateTimeFormat('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(new Date(String(value)))
    }

    if (type === 'boolean') {
      return value ? 'Sim' : 'Não'
    }

    return String(value)
  }

  useEffect(() => {
    void loadRows('')
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [cadastro])

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      void loadRows(search)
    }, 350)

    return () => window.clearTimeout(timeoutId)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search])

  return (
    <section className="cadastro-page">
      <header className="cadastro-header">
        <div className="cadastro-title-actions">
          <h1>{config.title}</h1>
          <button className="cadastro-new-button" type="button">
            {config.newButtonLabel}
          </button>
        </div>

        <div className="cadastro-filter">
          <label htmlFor={`${cadastro}-search`}>Filtro:</label>
          <input
            id={`${cadastro}-search`}
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={config.searchPlaceholder}
          />
        </div>
      </header>

      <div className="cadastro-card">
        <div className="cadastro-table-wrap">
          <table className="cadastro-table">
            <thead>
              <tr>
                {config.columns.map((column) => (
                  <th key={column.field}>{column.header}</th>
                ))}
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {visibleRows.map((row) => (
                <tr key={row.id}>
                  {config.columns.map((column) => (
                    <td key={column.field}>{formatValue(row, column.field, column.type)}</td>
                  ))}
                  <td>
                    <div className="cadastro-actions">
                      <button className="edit-action" type="button">
                        <FontAwesomeIcon icon={faPen} />
                        Editar
                      </button>
                      <button className="delete-action" type="button" onClick={() => void handleDelete(row)}>
                        <FontAwesomeIcon icon={faTrashCan} />
                        Excluir
                      </button>
                    </div>
                  </td>
                </tr>
              ))}

              {!isLoading && visibleRows.length === 0 && (
                <tr>
                  <td className="empty-cell" colSpan={config.columns.length + 1}>
                    Nenhum registro encontrado.
                  </td>
                </tr>
              )}

              {isLoading && (
                <tr>
                  <td className="empty-cell" colSpan={config.columns.length + 1}>
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

export default CadastroListPage
