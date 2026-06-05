import { FormEvent, useState } from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faEnvelope, faKey } from '@fortawesome/free-solid-svg-icons'
import './Login.css'
import logoUrl from '../../../images/logo-xyz.png'

type LoginResponse = {
  token: string
  nome: string
  email: string
}

type PrimeiroAcessoResponse = {
  email: string
  primeiroAcesso: boolean
}

type AuthView = 'login' | 'first-access-email' | 'set-password'

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

type LoginProps = {
  onAuthenticated: (user: LoginResponse) => void
}

function Login({ onAuthenticated }: LoginProps) {
  const [view, setView] = useState<AuthView>('login')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [firstAccessEmail, setFirstAccessEmail] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [message, setMessage] = useState('')

  async function handleLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsLoading(true)
    setMessage('')

    try {
      const response = await fetch(`${apiBaseUrl}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, senha: password }),
      })

      if (!response.ok) {
        throw new Error('E-mail ou senha invalidos.')
      }

      const data = (await response.json()) as LoginResponse
      localStorage.setItem('zyx.token', data.token)
      localStorage.setItem('zyx.user', JSON.stringify(data))
      onAuthenticated(data)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Nao foi possivel entrar.')
    } finally {
      setIsLoading(false)
    }
  }

  async function handleCheckFirstAccess(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsLoading(true)
    setMessage('')

    try {
      const query = new URLSearchParams({ email: firstAccessEmail })
      const response = await fetch(`${apiBaseUrl}/api/auth/primeiro-acesso?${query}`)

      if (!response.ok) {
        throw new Error('Usuario nao encontrado.')
      }

      const data = (await response.json()) as PrimeiroAcessoResponse

      if (!data.primeiroAcesso) {
        throw new Error('Este usuario ja possui senha cadastrada.')
      }

      setView('set-password')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Nao foi possivel verificar o e-mail.')
    } finally {
      setIsLoading(false)
    }
  }

  async function handleSetPassword(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setMessage('')

    if (newPassword !== confirmPassword) {
      setMessage('As senhas nao conferem.')
      return
    }

    setIsLoading(true)

    try {
      const response = await fetch(`${apiBaseUrl}/api/auth/primeiro-acesso`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: firstAccessEmail, senha: newPassword }),
      })

      if (!response.ok) {
        throw new Error('Nao foi possivel definir a senha.')
      }

      setEmail(firstAccessEmail)
      setPassword('')
      setNewPassword('')
      setConfirmPassword('')
      setView('login')
      setMessage('Senha cadastrada. Entre com sua nova senha.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Nao foi possivel definir a senha.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <main className="login-page">
      <section className="auth-shell">
        {view === 'login' && (
          <form className="auth-card" onSubmit={handleLogin}>
            <img className="auth-logo" src={logoUrl} alt="XYZ Logistica" />
            <header className="auth-header">
              <h1>Bem-vindo!</h1>
              <p>Acesse sua conta para continuar</p>
            </header>

            <label className="field">
              <span>Email</span>
              <div className="input-control">
                <FontAwesomeIcon className="input-icon" icon={faEnvelope} />
                <input
                  type="email"
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  placeholder="Digite seu e-mail"
                  autoComplete="email"
                  required
                />
              </div>
            </label>

            <label className="field">
              <span>Senha</span>
              <div className="input-control">
                <FontAwesomeIcon className="input-icon" icon={faKey} />
                <input
                  type="password"
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  placeholder="Digite sua senha"
                  autoComplete="current-password"
                  required
                />
              </div>
            </label>

            <button className="link-button" type="button" onClick={() => setView('first-access-email')}>
              Primeiro acesso?
            </button>

            <button className="primary-button" type="submit" disabled={isLoading}>
              {isLoading ? 'Entrando...' : 'Entrar'}
            </button>

            {message && <p className="form-message">{message}</p>}
          </form>
        )}

        {view === 'first-access-email' && (
          <form className="auth-card" onSubmit={handleCheckFirstAccess}>
            <img className="auth-logo" src={logoUrl} alt="XYZ Logistica" />
            <header className="auth-header">
              <h1>Primeiro acesso</h1>
              <p>Informe seu e-mail para continuar</p>
            </header>

            <label className="field">
              <span>Email</span>
              <div className="input-control">
                <FontAwesomeIcon className="input-icon" icon={faEnvelope} />
                <input
                  type="email"
                  value={firstAccessEmail}
                  onChange={(event) => setFirstAccessEmail(event.target.value)}
                  placeholder="Digite seu e-mail"
                  autoComplete="email"
                  required
                />
              </div>
            </label>

            <button className="primary-button" type="submit" disabled={isLoading}>
              {isLoading ? 'Verificando...' : 'Continuar'}
            </button>
            <button className="secondary-button" type="button" onClick={() => setView('login')}>
              Voltar
            </button>

            {message && <p className="form-message">{message}</p>}
          </form>
        )}

        {view === 'set-password' && (
          <form className="auth-card" onSubmit={handleSetPassword}>
            <img className="auth-logo" src={logoUrl} alt="XYZ Logistica" />
            <header className="auth-header">
              <h1>Definir senha</h1>
              <p>Cadastre sua senha de acesso</p>
            </header>

            <label className="field">
              <span>Nova senha</span>
              <div className="input-control">
                <FontAwesomeIcon className="input-icon" icon={faKey} />
                <input
                  type="password"
                  value={newPassword}
                  onChange={(event) => setNewPassword(event.target.value)}
                  placeholder="Digite sua senha"
                  autoComplete="new-password"
                  minLength={6}
                  required
                />
              </div>
            </label>

            <label className="field">
              <span>Confirmar senha</span>
              <div className="input-control">
                <FontAwesomeIcon className="input-icon" icon={faKey} />
                <input
                  type="password"
                  value={confirmPassword}
                  onChange={(event) => setConfirmPassword(event.target.value)}
                  placeholder="Digite novamente"
                  autoComplete="new-password"
                  minLength={6}
                  required
                />
              </div>
            </label>

            <button className="primary-button" type="submit" disabled={isLoading}>
              {isLoading ? 'Salvando...' : 'Salvar senha'}
            </button>
            <button className="secondary-button" type="button" onClick={() => setView('first-access-email')}>
              Voltar
            </button>

            {message && <p className="form-message">{message}</p>}
          </form>
        )}
      </section>
    </main>
  )
}

export default Login
