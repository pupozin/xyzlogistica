import { ChangeEvent, FormEvent, KeyboardEvent, useEffect, useRef, useState } from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {
  faArrowRight,
  faCheck,
  faClock,
  faIdCard,
  faPhone,
  faRotateLeft,
  faShieldHalved,
} from '@fortawesome/free-solid-svg-icons'
import './CheckInPage.css'
import logoUrl from '../../../images/logo-xyz.png'

type CheckInStep = 1 | 2 | 3

type CheckInCodigoResponse = {
  agendamentoId: number
  motoristaNome: string
  telefoneMascarado: string
  expiraEm: string
  codigoDesenvolvimento?: string | null
}

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

async function readErrorMessage(response: Response, fallback: string) {
  try {
    const data = (await response.json()) as { message?: string }
    return data.message ?? fallback
  } catch {
    return fallback
  }
}

function onlyDigits(value: string) {
  return value.replace(/\D/g, '').slice(0, 11)
}

function StepHeader({ step }: { step: CheckInStep }) {
  const progressWidth = step === 1 ? '0px' : step === 2 ? 'calc((100% - 130px) / 2)' : 'calc(100% - 130px)'

  return (
    <div className="checkin-steps">
      <span className="checkin-step-line" />
      <span className="checkin-step-progress" style={{ width: progressWidth }} />

      {[
        { id: 1, label: 'Identificação' },
        { id: 2, label: 'Confirmação' },
        { id: 3, label: 'Conclusão' },
      ].map((item) => {
        const isDone = step > item.id
        const isActive = step === item.id

        return (
          <div className={`checkin-step ${isDone ? 'done' : ''} ${isActive ? 'active' : ''}`} key={item.id}>
            <span className="checkin-step-number">
              {isDone ? <FontAwesomeIcon icon={faCheck} /> : item.id}
            </span>
            <span>{item.label}</span>
          </div>
        )
      })}
    </div>
  )
}

function CheckInPage() {
  const [step, setStep] = useState<CheckInStep>(1)
  const [cnh, setCnh] = useState('')
  const [code, setCode] = useState<string[]>(Array(6).fill(''))
  const [checkInInfo, setCheckInInfo] = useState<CheckInCodigoResponse | null>(null)
  const [secondsLeft, setSecondsLeft] = useState(45)
  const [isLoading, setIsLoading] = useState(false)
  const [message, setMessage] = useState('')
  const codeRefs = useRef<Array<HTMLInputElement | null>>([])

  async function requestCode(event?: FormEvent<HTMLFormElement>) {
    event?.preventDefault()
    setIsLoading(true)
    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/checkin/solicitar-codigo`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ cnh }),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível solicitar o código.'))
      }

      const data = (await response.json()) as CheckInCodigoResponse
      setCheckInInfo(data)
      setCode(Array(6).fill(''))
      setSecondsLeft(45)
      setStep(2)
      window.setTimeout(() => codeRefs.current[0]?.focus(), 100)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao solicitar código.')
    } finally {
      setIsLoading(false)
    }
  }

  async function confirmCode(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsLoading(true)
    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/checkin/confirmar`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ cnh, codigo: code.join('') }),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível confirmar o check-in.'))
      }

      setStep(3)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao confirmar check-in.')
    } finally {
      setIsLoading(false)
    }
  }

  function handleCodeChange(index: number, event: ChangeEvent<HTMLInputElement>) {
    const digit = event.target.value.replace(/\D/g, '').slice(-1)
    const nextCode = [...code]
    nextCode[index] = digit
    setCode(nextCode)

    if (digit && index < 5) {
      codeRefs.current[index + 1]?.focus()
    }
  }

  function handleCodeKeyDown(index: number, event: KeyboardEvent<HTMLInputElement>) {
    if (event.key === 'Backspace' && !code[index] && index > 0) {
      codeRefs.current[index - 1]?.focus()
    }
  }

  function resetFlow() {
    setStep(1)
    setCnh('')
    setCode(Array(6).fill(''))
    setCheckInInfo(null)
    setSecondsLeft(45)
    setMessage('')
  }

  useEffect(() => {
    if (step !== 2) {
      return undefined
    }

    const intervalId = window.setInterval(() => {
      setSecondsLeft((current) => Math.max(0, current - 1))
    }, 1000)

    return () => window.clearInterval(intervalId)
  }, [step])

  useEffect(() => {
    if (step !== 3) {
      return undefined
    }

    const timeoutId = window.setTimeout(resetFlow, 5000)
    return () => window.clearTimeout(timeoutId)
  }, [step])

  const isCodeComplete = code.every(Boolean)

  return (
    <main className="checkin-page">
      <section className="checkin-tablet">
        <header className="checkin-brand">
          <img src={logoUrl} alt="XYZ Logística" />
        </header>

        <div className="checkin-body">
          <StepHeader step={step} />

          {step === 1 && (
            <>
              <form className="checkin-card" onSubmit={(event) => void requestCode(event)}>
                <div>
                  <h1>Olá, Motorista!</h1>
                  <p>Realize seu check-in para iniciar a operação.</p>
                </div>

                <label className="checkin-field">
                  <span>
                    CNH <strong>*</strong>
                  </span>
                  <div className="checkin-input-shell">
                    <FontAwesomeIcon icon={faIdCard} />
                    <input
                      inputMode="numeric"
                      value={cnh}
                      onChange={(event) => setCnh(onlyDigits(event.target.value))}
                      placeholder="Digite o número da sua CNH"
                      required
                    />
                  </div>
                  <span className="checkin-hint">Informe apenas os números, sem pontos ou traços.</span>
                </label>

                <button className="checkin-primary" type="submit" disabled={isLoading || cnh.length === 0}>
                  Confirmar
                  <FontAwesomeIcon icon={faArrowRight} />
                </button>
              </form>

              <div className="checkin-secure">
                <span className="checkin-secure-icon">
                  <FontAwesomeIcon icon={faShieldHalved} />
                </span>
                <div>
                  <strong>Seus dados estão seguros</strong>
                  <span>Utilizamos criptografia para proteger suas informações.</span>
                </div>
              </div>
            </>
          )}

          {step === 2 && (
            <>
              <form className="checkin-card" onSubmit={(event) => void confirmCode(event)}>
                <div>
                  <h1>Confirmação</h1>
                  <p>Informe o código de 6 dígitos enviado ao seu celular</p>
                </div>

                <div className="code-inputs">
                  {code.map((digit, index) => (
                    <input
                      // eslint-disable-next-line react/no-array-index-key
                      key={index}
                      ref={(element) => {
                        codeRefs.current[index] = element
                      }}
                      inputMode="numeric"
                      maxLength={1}
                      value={digit}
                      onChange={(event) => handleCodeChange(index, event)}
                      onKeyDown={(event) => handleCodeKeyDown(index, event)}
                    />
                  ))}
                </div>

                <div className="checkin-timer">
                  <FontAwesomeIcon icon={faClock} />
                  <span>
                    Código enviado. Reenviar em <strong>00:{String(secondsLeft).padStart(2, '0')}</strong>
                  </span>
                </div>

                {checkInInfo?.codigoDesenvolvimento && (
                  <div className="checkin-dev-code">
                    Codigo de teste: <strong>{checkInInfo.codigoDesenvolvimento}</strong>
                  </div>
                )}

                <button className="checkin-primary" type="submit" disabled={isLoading || !isCodeComplete}>
                  Confirmar
                </button>

                <button
                  className="checkin-secondary"
                  type="button"
                  disabled={isLoading || secondsLeft > 0}
                  onClick={() => void requestCode()}
                >
                  <FontAwesomeIcon icon={faRotateLeft} />
                  Reenviar código
                </button>
              </form>

              <div className="checkin-phone-card">
                <span className="checkin-phone-icon">
                  <FontAwesomeIcon icon={faPhone} />
                </span>
                <div>
                  <strong>Código enviado para</strong>
                  <span>{checkInInfo?.telefoneMascarado ?? '(**) *****-****'}</span>
                </div>
                <button type="button" onClick={resetFlow}>
                  Alterar
                </button>
              </div>
            </>
          )}

          {step === 3 && (
            <div className="success-content">
              <span className="success-icon">
                <FontAwesomeIcon icon={faCheck} />
              </span>
              <h1>Check-in realizado com sucesso!</h1>
              <p>Você já pode seguir para a próxima etapa da operação.</p>
            </div>
          )}

          {message && <span className="checkin-message">{message}</span>}
        </div>
      </section>
    </main>
  )
}

export default CheckInPage
