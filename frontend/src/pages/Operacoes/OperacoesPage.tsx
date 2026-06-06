import { FormEvent, useEffect, useMemo, useState } from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {
  faArrowRight,
  faBox,
  faCheck,
  faFloppyDisk,
  faPen,
  faTrashCan,
  faTruck,
  faWarehouse,
  faXmark,
} from '@fortawesome/free-solid-svg-icons'
import '../Cadastros/CadastroListPage.css'
import './OperacoesPage.css'

type OperacaoMode = 'inbound' | 'outbound'

type OperacoesPageProps = {
  mode: OperacaoMode
}

type Agendamento = {
  id: number
  statusId: number
  statusDescricao: string
  veiculoPlaca: string
  transportadoraNome: string
  motoristaNome: string
  dataHoraAgendada: string
  dataHoraChegada?: string | null
  dataHoraDoca?: string | null
  dataHoraFinalizado?: string | null
}

type OperacaoAba = {
  statusId: number
  statusDescricao: string
  titulo: string
  quantidade: number
  agendamentos: Agendamento[]
}

type Option = {
  id: number
  label: string
}

type OperacaoItem = {
  id: number
  agendamentoId: number
  produtoId: number
  produtoDescricao: string
  quantidade: number
}

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

const pageConfig = {
  inbound: 'Operação Inbound',
  outbound: 'Operação Outbound',
}

const statusIcons = {
  2: faTruck,
  3: faWarehouse,
  4: faCheck,
}

function getAuthHeaders(includeJson = false) {
  const token = localStorage.getItem('zyx.token')
  const headers = new Headers()

  if (includeJson) {
    headers.set('Content-Type', 'application/json')
  }

  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  return headers
}

async function readErrorMessage(response: Response, fallback: string) {
  try {
    const data = (await response.json()) as { message?: string }
    return data.message ?? fallback
  } catch {
    return fallback
  }
}

function formatDateTime(value?: string | null) {
  if (!value) {
    return '-'
  }

  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))
}

function OperacoesPage({ mode }: OperacoesPageProps) {
  const [abas, setAbas] = useState<OperacaoAba[]>([])
  const [activeStatusId, setActiveStatusId] = useState(2)
  const [search, setSearch] = useState('')
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [selectedAgendamento, setSelectedAgendamento] = useState<Agendamento | null>(null)
  const [docaModalOpen, setDocaModalOpen] = useState(false)
  const [localId, setLocalId] = useState('')
  const [locais, setLocais] = useState<Option[]>([])
  const [itemsModalOpen, setItemsModalOpen] = useState(false)
  const [produtos, setProdutos] = useState<Option[]>([])
  const [items, setItems] = useState<OperacaoItem[]>([])
  const [produtoId, setProdutoId] = useState('')
  const [quantidade, setQuantidade] = useState('')
  const [isSaving, setIsSaving] = useState(false)
  const [toastMessage, setToastMessage] = useState('')

  const activeAba = useMemo(() => {
    return abas.find((aba) => aba.statusId === activeStatusId) ?? abas[0]
  }, [abas, activeStatusId])

  function showToast(messageText: string) {
    setToastMessage(messageText)
  }

  async function loadAbas(searchTerm = search) {
    setIsLoading(true)
    setMessage('')

    try {
      const query = new URLSearchParams()

      if (searchTerm.trim()) {
        query.set('busca', searchTerm.trim())
      }

      const response = await fetch(`${apiBaseUrl}/api/operacoes/${mode}/abas${query.size ? `?${query}` : ''}`, {
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível carregar a operação.'))
      }

      const data = (await response.json()) as OperacaoAba[]
      setAbas(data)

      if (!data.some((aba) => aba.statusId === activeStatusId)) {
        setActiveStatusId(data[0]?.statusId ?? 2)
      }
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar operação.')
    } finally {
      setIsLoading(false)
    }
  }

  async function loadLocais() {
    const response = await fetch(`${apiBaseUrl}/api/locais`, {
      headers: getAuthHeaders(),
    })

    if (!response.ok) {
      throw new Error(await readErrorMessage(response, 'Não foi possível carregar as docas.'))
    }

    const data = (await response.json()) as Array<{ id: number; descricao: string }>
    setLocais(data.map((item) => ({ id: item.id, label: item.descricao })))
  }

  async function loadProdutos() {
    const response = await fetch(`${apiBaseUrl}/api/produtos`, {
      headers: getAuthHeaders(),
    })

    if (!response.ok) {
      throw new Error(await readErrorMessage(response, 'Não foi possível carregar os produtos.'))
    }

    const data = (await response.json()) as Array<{ id: number; descricao: string }>
    setProdutos(data.map((item) => ({ id: item.id, label: item.descricao })))
  }

  async function loadItems(agendamentoId: number) {
    const response = await fetch(`${apiBaseUrl}/api/agendamentos/${agendamentoId}/itens`, {
      headers: getAuthHeaders(),
    })

    if (!response.ok) {
      throw new Error(await readErrorMessage(response, 'Não foi possível carregar os itens.'))
    }

    setItems((await response.json()) as OperacaoItem[])
  }

  async function openDocaModal(agendamento: Agendamento) {
    setSelectedAgendamento(agendamento)
    setLocalId('')
    setDocaModalOpen(true)
    setMessage('')

    try {
      await loadLocais()
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar docas.')
    }
  }

  async function openItemsModal(agendamento: Agendamento) {
    setSelectedAgendamento(agendamento)
    setProdutoId('')
    setQuantidade('')
    setItemsModalOpen(true)
    setMessage('')

    try {
      await Promise.all([loadProdutos(), loadItems(agendamento.id)])
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar itens.')
    }
  }

  async function handleEnviarDoca(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!selectedAgendamento) {
      return
    }

    setIsSaving(true)
    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/agendamentos/${selectedAgendamento.id}/enviar-doca`, {
        method: 'PUT',
        headers: getAuthHeaders(true),
        body: JSON.stringify({ localId: Number(localId) }),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível enviar para doca.'))
      }

      setDocaModalOpen(false)
      setSelectedAgendamento(null)
      await loadAbas()
      setActiveStatusId(3)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao enviar para doca.')
    } finally {
      setIsSaving(false)
    }
  }

  async function handleFinalizar(agendamento: Agendamento) {
    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/agendamentos/${agendamento.id}/finalizar`, {
        method: 'PUT',
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível finalizar.'))
      }

      await loadAbas()
      setActiveStatusId(4)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao finalizar.')
    }
  }

  async function handleAddItem(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!selectedAgendamento) {
      return
    }

    setIsSaving(true)
    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/agendamentos/${selectedAgendamento.id}/itens`, {
        method: 'POST',
        headers: getAuthHeaders(true),
        body: JSON.stringify({
          produtoId: Number(produtoId),
          quantidade: Number(quantidade),
        }),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível adicionar o item.'))
      }

      setProdutoId('')
      setQuantidade('')
      await loadItems(selectedAgendamento.id)
    } catch (error) {
      showToast(error instanceof Error ? error.message : 'Erro ao adicionar item.')
    } finally {
      setIsSaving(false)
    }
  }

  async function handleUpdateItem(item: OperacaoItem, nextQuantidade: string) {
    if (!selectedAgendamento) {
      return
    }

    const response = await fetch(`${apiBaseUrl}/api/agendamentos/${selectedAgendamento.id}/itens/${item.id}`, {
      method: 'PUT',
      headers: getAuthHeaders(true),
      body: JSON.stringify({ quantidade: Number(nextQuantidade) }),
    })

    if (!response.ok) {
      setMessage(await readErrorMessage(response, 'Não foi possível atualizar o item.'))
      return
    }

    await loadItems(selectedAgendamento.id)
  }

  async function handleDeleteItem(item: OperacaoItem) {
    if (!selectedAgendamento) {
      return
    }

    const response = await fetch(`${apiBaseUrl}/api/agendamentos/${selectedAgendamento.id}/itens/${item.id}`, {
      method: 'DELETE',
      headers: getAuthHeaders(),
    })

    if (!response.ok) {
      setMessage(await readErrorMessage(response, 'Não foi possível remover o item.'))
      return
    }

    await loadItems(selectedAgendamento.id)
  }

  useEffect(() => {
    void loadAbas('')
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [mode])

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      void loadAbas(search)
    }, 350)

    return () => window.clearTimeout(timeoutId)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search])

  useEffect(() => {
    if (!toastMessage) {
      return undefined
    }

    const timeoutId = window.setTimeout(() => setToastMessage(''), 3000)
    return () => window.clearTimeout(timeoutId)
  }, [toastMessage])

  return (
    <section className="operacao-page">
      <header className="operacao-header">
        <h1>{pageConfig[mode]}</h1>

        <div className="operacao-filter">
          <label htmlFor={`${mode}-operacao-search`}>Filtro:</label>
          <input
            id={`${mode}-operacao-search`}
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Agenda, Placa ou Transportadora"
          />
        </div>
      </header>

      <div className="operacao-tabs">
        {abas.map((aba) => (
          <button
            className={`operacao-tab ${aba.statusId === activeStatusId ? 'active' : ''}`}
            key={aba.statusId}
            type="button"
            onClick={() => setActiveStatusId(aba.statusId)}
          >
            <span className="operacao-tab-icon">
              <FontAwesomeIcon icon={statusIcons[aba.statusId as keyof typeof statusIcons] ?? faTruck} />
            </span>
            <span className="operacao-tab-text">
              <strong>{aba.titulo}</strong>
              <span>{aba.quantidade}</span>
            </span>
          </button>
        ))}
      </div>

      <div className="cadastro-card">
        <div className="cadastro-table-wrap">
          <table className="cadastro-table">
            <thead>
              <tr>
                <th>Agenda</th>
                <th>Status</th>
                <th>Agendada</th>
                <th>Chegada</th>
                <th>Motorista</th>
                <th>Transportadora</th>
                <th>Placa</th>
                {activeStatusId !== 4 && <th>Ações</th>}
              </tr>
            </thead>
            <tbody>
              {(activeAba?.agendamentos ?? []).map((agendamento) => (
                <tr key={agendamento.id}>
                  <td>{String(agendamento.id).padStart(4, '0')}</td>
                  <td>
                    <span className="status-pill">{agendamento.statusDescricao}</span>
                  </td>
                  <td>{formatDateTime(agendamento.dataHoraAgendada)}</td>
                  <td>{formatDateTime(agendamento.dataHoraChegada)}</td>
                  <td>{agendamento.motoristaNome}</td>
                  <td>{agendamento.transportadoraNome}</td>
                  <td>{agendamento.veiculoPlaca}</td>
                  {activeStatusId !== 4 && (
                    <td>
                      <div className="operacao-actions">
                        {activeStatusId === 3 && (
                          <button
                            className="operacao-action-button neutral"
                            type="button"
                            onClick={() => void openItemsModal(agendamento)}
                            aria-label="Editar itens"
                          >
                            <FontAwesomeIcon icon={faPen} />
                          </button>
                        )}
                        <button
                          className="operacao-action-button"
                          type="button"
                          onClick={() =>
                            activeStatusId === 2 ? void openDocaModal(agendamento) : void handleFinalizar(agendamento)
                          }
                          aria-label={activeStatusId === 2 ? 'Enviar para doca' : 'Finalizar'}
                        >
                          <FontAwesomeIcon icon={activeStatusId === 2 ? faArrowRight : faCheck} />
                        </button>
                      </div>
                    </td>
                  )}
                </tr>
              ))}

              {!isLoading && (activeAba?.agendamentos ?? []).length === 0 && (
                <tr>
                  <td className="empty-cell" colSpan={activeStatusId === 4 ? 7 : 8}>
                    Nenhum registro encontrado.
                  </td>
                </tr>
              )}

              {isLoading && (
                <tr>
                  <td className="empty-cell" colSpan={activeStatusId === 4 ? 7 : 8}>
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

      {docaModalOpen && (
        <div className="cadastro-modal-backdrop">
          <form className="cadastro-modal" onSubmit={(event) => void handleEnviarDoca(event)}>
            <button className="modal-close-button" type="button" onClick={() => setDocaModalOpen(false)}>
              <FontAwesomeIcon icon={faXmark} />
            </button>

            <header className="modal-header">
              <div className="modal-icon">
                <FontAwesomeIcon icon={faWarehouse} />
              </div>
              <div>
                <h2>Enviar para doca</h2>
                <p>Informe a doca para continuar a operação.</p>
              </div>
            </header>

            <label className="modal-field">
              <span>Doca *</span>
              <div className="input-shell">
                <FontAwesomeIcon icon={faWarehouse} />
                <select required value={localId} onChange={(event) => setLocalId(event.target.value)}>
                  <option value="">Selecione a doca</option>
                  {locais.map((local) => (
                    <option key={local.id} value={local.id}>
                      {local.label}
                    </option>
                  ))}
                </select>
              </div>
            </label>

            <footer className="modal-actions">
              <button className="modal-cancel-button" type="button" onClick={() => setDocaModalOpen(false)}>
                Cancelar
              </button>
              <button className="modal-save-button" type="submit" disabled={isSaving}>
                <FontAwesomeIcon icon={faArrowRight} />
                {isSaving ? 'Enviando...' : 'Enviar para doca'}
              </button>
            </footer>
          </form>
        </div>
      )}

      {itemsModalOpen && selectedAgendamento && (
        <div className="cadastro-modal-backdrop">
          <div className="cadastro-modal">
            <button className="modal-close-button" type="button" onClick={() => setItemsModalOpen(false)}>
              <FontAwesomeIcon icon={faXmark} />
            </button>

            <header className="modal-header">
              <div className="modal-icon">
                <FontAwesomeIcon icon={faBox} />
              </div>
              <div>
                <h2>Itens da operação</h2>
                <p>Adicione ou ajuste os produtos movimentados.</p>
              </div>
            </header>

            <form className="agenda-modal-grid" onSubmit={(event) => void handleAddItem(event)}>
              <label className="modal-field">
                <span>Produto *</span>
                <div className="input-shell">
                  <FontAwesomeIcon icon={faBox} />
                  <select required value={produtoId} onChange={(event) => setProdutoId(event.target.value)}>
                    <option value="">Selecione o produto</option>
                    {produtos.map((produto) => (
                      <option key={produto.id} value={produto.id}>
                        {produto.label}
                      </option>
                    ))}
                  </select>
                </div>
              </label>

              <label className="modal-field">
                <span>Quantidade *</span>
                <div className="input-shell">
                  <FontAwesomeIcon icon={faBox} />
                  <input
                    required
                    type="number"
                    min="0.001"
                    step="0.001"
                    value={quantidade}
                    onChange={(event) => setQuantidade(event.target.value)}
                    placeholder="0"
                  />
                </div>
              </label>

              <button className="modal-save-button operacao-add-item-button" type="submit" disabled={isSaving}>
                <FontAwesomeIcon icon={faFloppyDisk} />
                Adicionar item
              </button>
            </form>

            <div className="operacao-items-list">
              {items.map((item) => (
                <div className="operacao-item-row" key={item.id}>
                  <span>{item.produtoDescricao}</span>
                  <input
                    key={`${item.id}-${item.quantidade}`}
                    type="number"
                    min="0.001"
                    step="0.001"
                    defaultValue={item.quantidade}
                    onBlur={(event) => void handleUpdateItem(item, event.target.value)}
                  />
                  <button
                    className="operacao-item-remove"
                    type="button"
                    onClick={() => void handleDeleteItem(item)}
                    aria-label="Remover item"
                  >
                    <FontAwesomeIcon icon={faTrashCan} />
                  </button>
                </div>
              ))}

              {items.length === 0 && <span className="empty-cell">Nenhum item adicionado.</span>}
            </div>
          </div>
        </div>
      )}

      {toastMessage && (
        <div className="operacao-toast" role="status" aria-live="polite">
          <span>{toastMessage}</span>
        </div>
      )}
    </section>
  )
}

export default OperacoesPage
