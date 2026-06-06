import { FormEvent, useEffect, useState } from 'react'
import '../Cadastros/CadastroListPage.css'
import './JanelaAgendamentosPage.css'

type ConfiguracaoAgendamento = {
  id: number
  intervaloMinutos: number
  horaInicio: string
  horaFim: string
  ativo: boolean
  criadoEm: string
  atualizadoEm?: string | null
}

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

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

function toTimeInput(value: string) {
  return value.slice(0, 5)
}

function toTimeSpan(value: string) {
  return value.length === 5 ? `${value}:00` : value
}

function JanelaAgendamentosPage() {
  const [intervaloMinutos, setIntervaloMinutos] = useState('')
  const [horaInicio, setHoraInicio] = useState('')
  const [horaFim, setHoraFim] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [message, setMessage] = useState('')

  async function loadConfig() {
    setIsLoading(true)
    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/configuracao-agendamento/ativa`, {
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível carregar a janela de agendamentos.'))
      }

      const data = (await response.json()) as ConfiguracaoAgendamento
      setIntervaloMinutos(String(data.intervaloMinutos))
      setHoraInicio(toTimeInput(data.horaInicio))
      setHoraFim(toTimeInput(data.horaFim))
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar configuração.')
    } finally {
      setIsLoading(false)
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/configuracao-agendamento/ativa`, {
        method: 'PUT',
        headers: getAuthHeaders(true),
        body: JSON.stringify({
          intervaloMinutos: Number(intervaloMinutos),
          horaInicio: toTimeSpan(horaInicio),
          horaFim: toTimeSpan(horaFim),
        }),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível salvar a janela de agendamentos.'))
      }

      const data = (await response.json()) as ConfiguracaoAgendamento
      setIntervaloMinutos(String(data.intervaloMinutos))
      setHoraInicio(toTimeInput(data.horaInicio))
      setHoraFim(toTimeInput(data.horaFim))
      setMessage('Janela de agendamentos salva.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar configuração.')
    } finally {
      setIsSaving(false)
    }
  }

  useEffect(() => {
    void loadConfig()
  }, [])

  return (
    <section className="cadastro-page">
      <header className="cadastro-header">
        <div className="cadastro-title-actions">
          <h1>Janela de agendamentos</h1>
        </div>
      </header>

      <div className="cadastro-card">
        <form className="janela-form" onSubmit={(event) => void handleSubmit(event)}>
          <div className="janela-fields">
            <label className="janela-field">
              <span>Intervalo em minutos</span>
              <input
                type="number"
                min="1"
                max="1440"
                required
                value={intervaloMinutos}
                disabled={isLoading}
                onChange={(event) => setIntervaloMinutos(event.target.value)}
              />
            </label>

            <label className="janela-field">
              <span>Hora início</span>
              <input
                type="time"
                required
                value={horaInicio}
                disabled={isLoading}
                onChange={(event) => setHoraInicio(event.target.value)}
              />
            </label>

            <label className="janela-field">
              <span>Hora fim</span>
              <input
                type="time"
                required
                value={horaFim}
                disabled={isLoading}
                onChange={(event) => setHoraFim(event.target.value)}
              />
            </label>

            <div className="janela-actions">
              <button className="janela-save-button" type="submit" disabled={isLoading || isSaving}>
                {isSaving ? 'Salvando...' : 'Salvar alterações'}
              </button>
            </div>
          </div>
        </form>

        <footer className="cadastro-footer">
          {message && <span className="cadastro-message">{message}</span>}
        </footer>
      </div>
    </section>
  )
}

export default JanelaAgendamentosPage
