import { FormEvent, useMemo, useState } from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {
  faBoxesStacked,
  faChartLine,
  faDownload,
  faFileExcel,
  faGaugeHigh,
  faListCheck,
  faTruckRampBox,
} from '@fortawesome/free-solid-svg-icons'
import '../Cadastros/CadastroListPage.css'
import './RelatoriosPage.css'

type ReportKey =
  | 'agendamentos-geral'
  | 'estoque'
  | 'agendas-finalizadas'
  | 'movimentacao-estoque'
  | 'cargas-recebidas-enviadas'
  | 'performance-operacional'

type ReportConfig = {
  key: ReportKey
  title: string
  description: string
  icon: typeof faFileExcel
  usesDateRange: boolean
  usesOperation: boolean
  usesStatus?: boolean
  usesProduct?: boolean
}

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

const reports: ReportConfig[] = [
  {
    key: 'agendamentos-geral',
    title: 'Agendamentos geral',
    description: 'Lista completa das agendas no periodo, com status, operacao, motorista, placa e doca.',
    icon: faListCheck,
    usesDateRange: true,
    usesOperation: true,
    usesStatus: true,
  },
  {
    key: 'estoque',
    title: 'Estoque atual',
    description: 'Posicao atual do inventario por produto, com quantidade e ultima atualizacao.',
    icon: faBoxesStacked,
    usesDateRange: false,
    usesOperation: false,
    usesProduct: true,
  },
  {
    key: 'agendas-finalizadas',
    title: 'Agendas finalizadas',
    description: 'Agendas concluidas no periodo, com tempos de operacao e tempo em doca.',
    icon: faFileExcel,
    usesDateRange: true,
    usesOperation: true,
  },
  {
    key: 'movimentacao-estoque',
    title: 'Movimentacao de estoque',
    description: 'Quantidades recebidas e enviadas por produto em agendas finalizadas no periodo.',
    icon: faChartLine,
    usesDateRange: true,
    usesOperation: true,
    usesProduct: true,
  },
  {
    key: 'cargas-recebidas-enviadas',
    title: 'Cargas recebidas e enviadas',
    description: 'Resumo diario de cargas inbound recebidas e outbound enviadas.',
    icon: faTruckRampBox,
    usesDateRange: true,
    usesOperation: true,
  },
  {
    key: 'performance-operacional',
    title: 'Performance operacional',
    description: 'Indicadores de espera, tempo em doca e tempo total de operacao.',
    icon: faGaugeHigh,
    usesDateRange: true,
    usesOperation: true,
  },
]

const statusOptions = [
  { value: '', label: 'Todos' },
  { value: '1', label: 'Agendada' },
  { value: '2', label: 'Check-in realizado' },
  { value: '3', label: 'Em doca' },
  { value: '4', label: 'Finalizada' },
  { value: '5', label: 'Cancelada' },
]

function today() {
  return new Date().toISOString().slice(0, 10)
}

function getAuthHeaders() {
  const token = localStorage.getItem('zyx.token')
  const headers = new Headers()

  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  return headers
}

async function readErrorMessage(response: Response) {
  try {
    const data = (await response.json()) as { message?: string }
    return data.message ?? 'Nao foi possivel extrair o relatorio.'
  } catch {
    return 'Nao foi possivel extrair o relatorio.'
  }
}

function getFileName(response: Response, fallback: string) {
  const contentDisposition = response.headers.get('content-disposition')
  const match = contentDisposition?.match(/filename="?([^"]+)"?/i)
  return match?.[1] ?? `${fallback}.xlsx`
}

function RelatoriosPage() {
  const defaultDate = useMemo(() => today(), [])
  const [dataInicio, setDataInicio] = useState(defaultDate)
  const [dataFim, setDataFim] = useState(defaultDate)
  const [operacaoId, setOperacaoId] = useState('')
  const [statusId, setStatusId] = useState('')
  const [produto, setProduto] = useState('')
  const [loadingReport, setLoadingReport] = useState<ReportKey | null>(null)
  const [message, setMessage] = useState('')

  async function handleExport(event: FormEvent<HTMLFormElement>, report: ReportConfig) {
    event.preventDefault()
    setLoadingReport(report.key)
    setMessage('')

    try {
      const params = new URLSearchParams({
        dataInicio,
        dataFim,
      })

      if (report.usesOperation && operacaoId) {
        params.set('operacaoId', operacaoId)
      }

      if (report.usesStatus && statusId) {
        params.set('statusId', statusId)
      }

      if (report.usesProduct && produto.trim()) {
        params.set('produto', produto.trim())
      }

      const response = await fetch(`${apiBaseUrl}/api/relatorios/${report.key}/excel?${params}`, {
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response))
      }

      const blob = await response.blob()
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = getFileName(response, report.key)
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao extrair relatorio.')
    } finally {
      setLoadingReport(null)
    }
  }

  return (
    <section className="cadastro-page relatorios-page">
      <header className="cadastro-header">
        <div className="cadastro-title-actions">
          <h1>Relatorios</h1>
        </div>
      </header>

      <div className="relatorios-filter-band">
        <label>
          <span>Data inicial</span>
          <input type="date" value={dataInicio} onChange={(event) => setDataInicio(event.target.value)} />
        </label>

        <label>
          <span>Data final</span>
          <input type="date" value={dataFim} onChange={(event) => setDataFim(event.target.value)} />
        </label>

        <label>
          <span>Operacao</span>
          <select value={operacaoId} onChange={(event) => setOperacaoId(event.target.value)}>
            <option value="">Todas</option>
            <option value="1">Inbound</option>
            <option value="2">Outbound</option>
          </select>
        </label>

        <label>
          <span>Status</span>
          <select value={statusId} onChange={(event) => setStatusId(event.target.value)}>
            {statusOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </label>

        <label>
          <span>Produto</span>
          <input value={produto} onChange={(event) => setProduto(event.target.value)} placeholder="Nome do produto" />
        </label>
      </div>

      {message && <span className="cadastro-message">{message}</span>}

      <div className="relatorios-grid">
        {reports.map((report) => (
          <form className="relatorio-card" key={report.key} onSubmit={(event) => void handleExport(event, report)}>
            <span className="relatorio-icon">
              <FontAwesomeIcon icon={report.icon} />
            </span>

            <div className="relatorio-card-content">
              <h2>{report.title}</h2>
              <p>{report.description}</p>
            </div>

            <button className="relatorio-export-button" type="submit" disabled={loadingReport === report.key}>
              <FontAwesomeIcon icon={faDownload} />
              {loadingReport === report.key ? 'Extraindo...' : 'Extrair Excel'}
            </button>
          </form>
        ))}
      </div>
    </section>
  )
}

export default RelatoriosPage
