import { FormEvent, useEffect, useMemo, useState } from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {
  faBuilding,
  faCalendarDay,
  faClock,
  faFloppyDisk,
  faTriangleExclamation,
  faTruck,
  faUser,
  faXmark,
} from '@fortawesome/free-solid-svg-icons'
import '../Cadastros/CadastroListPage.css'
import './AgendamentosPage.css'
import { hasPermission, PermissionUser } from '../../security/permissions'

type AgendamentoMode = 'inbound' | 'outbound'

type AgendamentosPageProps = {
  mode: AgendamentoMode
}

type Agendamento = {
  id: number
  operacaoId: number
  statusId: number
  veiculoId: number
  veiculoPlaca: string
  tipoVeiculo: string
  transportadoraId: number
  transportadoraNome: string
  motoristaId: number
  motoristaNome: string
  motoristaCnh: string
  dataHoraAgendada: string
}

type Option = {
  id: number
  label: string
}

type HorarioOption = {
  dataHora: string
  horario: string
}

type ModalMode = 'create' | 'edit'

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

const pageConfig = {
  inbound: {
    title: 'Inbound',
    operacaoId: 1,
  },
  outbound: {
    title: 'Outbound',
    operacaoId: 2,
  },
}

const weekdays = ['Domingo', 'Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'Sábado']

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

function getStoredUser(): PermissionUser | null {
  const rawUser = localStorage.getItem('zyx.user')

  if (!rawUser) {
    return null
  }

  try {
    return JSON.parse(rawUser) as PermissionUser
  } catch {
    return null
  }
}

async function readErrorMessage(response: Response, fallback: string) {
  try {
    const data = (await response.json()) as { message?: string }
    return data.message ?? fallback
  } catch {
    return fallback
  }
}

function toDateKey(date: Date) {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')

  return `${year}-${month}-${day}`
}

function addDays(dateKey: string, days: number) {
  const date = new Date(`${dateKey}T00:00:00`)
  date.setDate(date.getDate() + days)
  return date
}

function formatDateLabel(date: Date) {
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  }).format(date)
}

function formatTime(value: string) {
  return new Intl.DateTimeFormat('pt-BR', {
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))
}

function getDatePart(value: string) {
  return toDateKey(new Date(value))
}

function getTimePart(value: string) {
  return formatTime(value)
}

function buildDateTime(date: string, time: string) {
  return `${date}T${time.length === 5 ? `${time}:00` : time}`
}

function AgendamentosPage({ mode }: AgendamentosPageProps) {
  const config = pageConfig[mode]
  const user = useMemo(() => getStoredUser(), [])
  const canCreate = hasPermission(user, 'agendamentos.criar')
  const canEdit = hasPermission(user, 'agendamentos.editar')
  const canCancel = hasPermission(user, 'agendamentos.cancelar')
  const [startDate, setStartDate] = useState(() => toDateKey(new Date()))
  const [agendamentos, setAgendamentos] = useState<Agendamento[]>([])
  const [transportadoras, setTransportadoras] = useState<Option[]>([])
  const [motoristas, setMotoristas] = useState<Option[]>([])
  const [veiculos, setVeiculos] = useState<Option[]>([])
  const [horarios, setHorarios] = useState<HorarioOption[]>([])
  const [modalMode, setModalMode] = useState<ModalMode | null>(null)
  const [editingAgendamento, setEditingAgendamento] = useState<Agendamento | null>(null)
  const [cancelAgendamento, setCancelAgendamento] = useState<Agendamento | null>(null)
  const [transportadoraId, setTransportadoraId] = useState('')
  const [veiculoId, setVeiculoId] = useState('')
  const [motoristaId, setMotoristaId] = useState('')
  const [agendaDate, setAgendaDate] = useState(() => toDateKey(new Date()))
  const [agendaTime, setAgendaTime] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [message, setMessage] = useState('')

  const days = useMemo(() => Array.from({ length: 7 }, (_, index) => addDays(startDate, index)), [startDate])

  const agendamentosByDay = useMemo(() => {
    const map = new Map<string, Agendamento[]>()

    days.forEach((day) => map.set(toDateKey(day), []))

    agendamentos.forEach((agendamento) => {
      const key = getDatePart(agendamento.dataHoraAgendada)
      const list = map.get(key)

      if (list) {
        list.push(agendamento)
      }
    })

    map.forEach((list) => {
      list.sort((first, second) => {
        return new Date(first.dataHoraAgendada).getTime() - new Date(second.dataHoraAgendada).getTime()
      })
    })

    return map
  }, [agendamentos, days])

  async function loadAgendamentos(date = startDate) {
    setIsLoading(true)
    setMessage('')

    try {
      const query = new URLSearchParams({
        data: date,
        operacaoId: String(config.operacaoId),
      })

      const response = await fetch(`${apiBaseUrl}/api/agendamentos?${query}`, {
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível carregar os agendamentos.'))
      }

      setAgendamentos((await response.json()) as Agendamento[])
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar agendamentos.')
    } finally {
      setIsLoading(false)
    }
  }

  async function loadBaseOptions(current?: Agendamento | null) {
    const [transportadorasResponse, motoristasResponse] = await Promise.all([
      fetch(`${apiBaseUrl}/api/transportadoras`, { headers: getAuthHeaders() }),
      fetch(`${apiBaseUrl}/api/agendamentos/motoristas-disponiveis`, { headers: getAuthHeaders() }),
    ])

    if (!transportadorasResponse.ok || !motoristasResponse.ok) {
      throw new Error('Não foi possível carregar as opções do agendamento.')
    }

    const transportadorasData = (await transportadorasResponse.json()) as Array<{ id: number; nome: string }>
    const motoristasData = (await motoristasResponse.json()) as Array<{ id: number; nome: string; cnh: string }>
    const motoristaOptions = motoristasData.map((item) => ({ id: item.id, label: `${item.nome} - ${item.cnh}` }))

    if (current) {
      const currentOption = {
        id: current.motoristaId,
        label: `${current.motoristaNome} - ${current.motoristaCnh}`,
      }

      if (!motoristaOptions.some((option) => option.id === currentOption.id)) {
        motoristaOptions.unshift(currentOption)
      }
    }

    setTransportadoras(transportadorasData.map((item) => ({ id: item.id, label: item.nome })))
    setMotoristas(motoristaOptions)
  }

  async function loadVeiculos(selectedTransportadoraId: string, current?: Agendamento | null) {
    if (!selectedTransportadoraId) {
      setVeiculos([])
      return
    }

    const query = new URLSearchParams({ transportadoraId: selectedTransportadoraId })
    const response = await fetch(`${apiBaseUrl}/api/agendamentos/veiculos-disponiveis?${query}`, {
      headers: getAuthHeaders(),
    })

    if (!response.ok) {
      throw new Error(await readErrorMessage(response, 'Não foi possível carregar os veículos.'))
    }

    const data = (await response.json()) as Array<{ id: number; placa: string; tipoVeiculo: string }>
    const options = data.map((item) => ({ id: item.id, label: `${item.placa} - ${item.tipoVeiculo}` }))

    if (current && String(current.transportadoraId) === selectedTransportadoraId) {
      const currentOption = {
        id: current.veiculoId,
        label: `${current.veiculoPlaca} - ${current.tipoVeiculo}`,
      }

      if (!options.some((option) => option.id === currentOption.id)) {
        options.unshift(currentOption)
      }
    }

    setVeiculos(options)
  }

  async function loadHorarios(date: string, current?: Agendamento | null) {
    if (!date) {
      setHorarios([])
      return
    }

    const query = new URLSearchParams({ data: date })
    const response = await fetch(`${apiBaseUrl}/api/agendamentos/horarios-disponiveis?${query}`, {
      headers: getAuthHeaders(),
    })

    if (!response.ok) {
      throw new Error(await readErrorMessage(response, 'Não foi possível carregar os horários.'))
    }

    const data = (await response.json()) as HorarioOption[]
    const options = [...data]

    if (current && getDatePart(current.dataHoraAgendada) === date) {
      const currentOption = {
        dataHora: current.dataHoraAgendada,
        horario: getTimePart(current.dataHoraAgendada),
      }

      if (!options.some((option) => option.horario === currentOption.horario)) {
        options.unshift(currentOption)
      }
    }

    options.sort((first, second) => first.horario.localeCompare(second.horario))
    setHorarios(options)
  }

  async function openCreateModal() {
    if (!canCreate) {
      return
    }

    setMessage('')
    setEditingAgendamento(null)
    setTransportadoraId('')
    setVeiculoId('')
    setMotoristaId('')
    setAgendaDate(startDate)
    setAgendaTime('')
    setVeiculos([])
    setHorarios([])
    setModalMode('create')

    try {
      await loadBaseOptions()
      await loadHorarios(startDate)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar opções.')
    }
  }

  async function openEditModal(agendamento: Agendamento) {
    if (!canEdit) {
      return
    }

    setMessage('')
    setModalMode('edit')

    try {
      const response = await fetch(`${apiBaseUrl}/api/agendamentos/${agendamento.id}`, {
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível carregar o agendamento.'))
      }

      const data = (await response.json()) as Agendamento
      const date = getDatePart(data.dataHoraAgendada)

      setEditingAgendamento(data)
      setTransportadoraId(String(data.transportadoraId))
      setVeiculoId(String(data.veiculoId))
      setMotoristaId(String(data.motoristaId))
      setAgendaDate(date)
      setAgendaTime(getTimePart(data.dataHoraAgendada))

      await loadBaseOptions(data)
      await Promise.all([loadVeiculos(String(data.transportadoraId), data), loadHorarios(date, data)])
    } catch (error) {
      setModalMode(null)
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar agendamento.')
    }
  }

  function closeModal() {
    if (isSaving) {
      return
    }

    setModalMode(null)
    setEditingAgendamento(null)
  }

  async function handleTransportadoraChange(value: string) {
    setTransportadoraId(value)
    setVeiculoId('')

    try {
      await loadVeiculos(value, editingAgendamento)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar veículos.')
    }
  }

  async function handleAgendaDateChange(value: string) {
    setAgendaDate(value)
    setAgendaTime('')

    try {
      await loadHorarios(value, editingAgendamento)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar horários.')
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (modalMode === 'edit' && editingAgendamento?.statusId !== 1) {
      return
    }

    setIsSaving(true)
    setMessage('')

    try {
      const response = await fetch(
        `${apiBaseUrl}/api/agendamentos${modalMode === 'edit' && editingAgendamento ? `/${editingAgendamento.id}` : ''}`,
        {
          method: modalMode === 'edit' ? 'PUT' : 'POST',
          headers: getAuthHeaders(true),
          body: JSON.stringify({
            operacaoId: config.operacaoId,
            transportadoraId: Number(transportadoraId),
            veiculoId: Number(veiculoId),
            motoristaId: Number(motoristaId),
            dataHoraAgendada: buildDateTime(agendaDate, agendaTime),
          }),
        },
      )

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível salvar o agendamento.'))
      }

      setModalMode(null)
      setEditingAgendamento(null)
      await loadAgendamentos()
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar agendamento.')
    } finally {
      setIsSaving(false)
    }
  }

  async function handleCancelAgendamento() {
    if (!cancelAgendamento || !canCancel) {
      return
    }

    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/agendamentos/${cancelAgendamento.id}/cancelar`, {
        method: 'PUT',
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível cancelar a agenda.'))
      }

      setCancelAgendamento(null)
      await loadAgendamentos()
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao cancelar agenda.')
    }
  }

  useEffect(() => {
    void loadAgendamentos(startDate)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [startDate, mode])

  const canEditAgendamento = modalMode === 'create' || editingAgendamento?.statusId === 1

  return (
    <section className="cadastro-page">
      <header className="cadastro-header">
        <div className="cadastro-title-actions">
          <h1>{config.title}</h1>
          {canCreate && (
            <button className="cadastro-new-button" type="button" onClick={() => void openCreateModal()}>
              + Nova agenda
            </button>
          )}
        </div>

        <div className="agendamento-date-filter">
          <label htmlFor={`${mode}-start-date`}>Data:</label>
          <input
            id={`${mode}-start-date`}
            type="date"
            value={startDate}
            onChange={(event) => setStartDate(event.target.value)}
          />
        </div>
      </header>

      <div className="agenda-board">
        {days.map((day) => {
          const key = toDateKey(day)
          const dayAgendamentos = agendamentosByDay.get(key) ?? []

          return (
            <section className="agenda-day" key={key}>
              <header className="agenda-day-header">
                <strong>{formatDateLabel(day)}</strong>
                <span>{weekdays[day.getDay()]}</span>
              </header>

              <div className="agenda-day-list">
                {dayAgendamentos.map((agendamento) => (
                  <button
                    className={`agenda-note ${agendamento.statusId === 4 ? 'agenda-note-finished' : 'agenda-note-open'}`}
                    key={agendamento.id}
                    type="button"
                    onClick={() => void openEditModal(agendamento)}
                  >
                    {canCancel && agendamento.statusId !== 4 && (
                      <span
                        className="agenda-cancel-button"
                        role="button"
                        tabIndex={0}
                        aria-label="Cancelar agenda"
                        onClick={(event) => {
                          event.stopPropagation()
                          setCancelAgendamento(agendamento)
                        }}
                        onKeyDown={(event) => {
                          if (event.key === 'Enter' || event.key === ' ') {
                            event.preventDefault()
                            event.stopPropagation()
                            setCancelAgendamento(agendamento)
                          }
                        }}
                      >
                        <FontAwesomeIcon icon={faXmark} />
                      </span>
                    )}
                    <span className="agenda-note-title">
                      <FontAwesomeIcon icon={faTruck} />
                      {agendamento.transportadoraNome} - {String(agendamento.id).padStart(4, '0')}
                    </span>
                    <span className="agenda-note-time">
                      <FontAwesomeIcon icon={faClock} />
                      {formatTime(agendamento.dataHoraAgendada)}
                    </span>
                  </button>
                ))}
              </div>
            </section>
          )
        })}
      </div>

      {message && <span className="cadastro-message agenda-message">{message}</span>}
      {isLoading && <span className="cadastro-message agenda-message">Carregando agendamentos...</span>}

      {modalMode && (
        <div className="cadastro-modal-backdrop">
          <form className="cadastro-modal" onSubmit={(event) => void handleSubmit(event)}>
            <button className="modal-close-button" type="button" onClick={closeModal} aria-label="Fechar">
              <FontAwesomeIcon icon={faXmark} />
            </button>

            <header className="modal-header">
              <div className="modal-icon">
                <FontAwesomeIcon icon={faCalendarDay} />
              </div>
              <div>
                <h2>Agendamento {config.title}</h2>
                <p>
                  {modalMode === 'create'
                    ? 'Preencha as informações para cadastrar.'
                    : 'Atualize as informações do agendamento.'}
                </p>
              </div>
            </header>

            <div className="agenda-modal-grid">
              <label className="modal-field">
                <span>Transportadora *</span>
                <div className="input-shell">
                  <FontAwesomeIcon icon={faBuilding} />
                  <select
                    required
                    disabled={!canEditAgendamento}
                    value={transportadoraId}
                    onChange={(event) => void handleTransportadoraChange(event.target.value)}
                  >
                    <option value="">Selecione a transportadora</option>
                    {transportadoras.map((option) => (
                      <option key={option.id} value={option.id}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                </div>
              </label>

              <label className="modal-field">
                <span>Veículo *</span>
                <div className="input-shell">
                  <FontAwesomeIcon icon={faTruck} />
                  <select
                    required
                    disabled={!canEditAgendamento}
                    value={veiculoId}
                    onChange={(event) => setVeiculoId(event.target.value)}
                  >
                    <option value="">
                      {transportadoraId ? 'Selecione o veículo' : 'Selecione a transportadora primeiro'}
                    </option>
                    {veiculos.map((option) => (
                      <option key={option.id} value={option.id}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                </div>
              </label>

              <label className="modal-field">
                <span>Motorista *</span>
                <div className="input-shell">
                  <FontAwesomeIcon icon={faUser} />
                  <select
                    required
                    disabled={!canEditAgendamento}
                    value={motoristaId}
                    onChange={(event) => setMotoristaId(event.target.value)}
                  >
                    <option value="">Selecione o motorista</option>
                    {motoristas.map((option) => (
                      <option key={option.id} value={option.id}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                </div>
              </label>

              <label className="modal-field">
                <span>Data *</span>
                <div className="input-shell">
                  <FontAwesomeIcon icon={faCalendarDay} />
                  <input
                    required
                    disabled={!canEditAgendamento}
                    type="date"
                    value={agendaDate}
                    onChange={(event) => void handleAgendaDateChange(event.target.value)}
                  />
                </div>
              </label>

              <label className="modal-field">
                <span>Horário *</span>
                <div className="input-shell">
                  <FontAwesomeIcon icon={faClock} />
                  <select
                    required
                    disabled={!canEditAgendamento}
                    value={agendaTime}
                    onChange={(event) => setAgendaTime(event.target.value)}
                  >
                    <option value="">Selecione o horário</option>
                    {horarios.map((option) => (
                      <option key={option.horario} value={option.horario}>
                        {option.horario}
                      </option>
                    ))}
                  </select>
                </div>
              </label>
            </div>

            <footer className="modal-actions">
              <button className="modal-cancel-button" type="button" onClick={closeModal}>
                Cancelar
              </button>
              {canEditAgendamento && (
                <button className="modal-save-button" type="submit" disabled={isSaving}>
                  <FontAwesomeIcon icon={faFloppyDisk} />
                  {isSaving ? 'Salvando...' : 'Salvar agendamento'}
                </button>
              )}
            </footer>
          </form>
        </div>
      )}

      {cancelAgendamento && (
        <div className="cadastro-modal-backdrop">
          <div className="delete-modal" role="dialog" aria-modal="true">
            <button
              className="modal-close-button"
              type="button"
              onClick={() => setCancelAgendamento(null)}
              aria-label="Fechar"
            >
              <FontAwesomeIcon icon={faXmark} />
            </button>

            <div className="delete-icon">
              <FontAwesomeIcon icon={faTriangleExclamation} />
            </div>
            <h2>Cancelar agenda</h2>
            <p>Deseja cancelar agenda?</p>

            <footer className="modal-actions">
              <button className="modal-cancel-button" type="button" onClick={() => setCancelAgendamento(null)}>
                Voltar
              </button>
              <button className="modal-delete-button" type="button" onClick={() => void handleCancelAgendamento()}>
                <FontAwesomeIcon icon={faXmark} />
                Cancelar agenda
              </button>
            </footer>
          </div>
        </div>
      )}
    </section>
  )
}

export default AgendamentosPage
