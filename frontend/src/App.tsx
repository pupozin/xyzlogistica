import { useMemo, useState } from 'react'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import MainLayout from './layouts/MainLayout/MainLayout'
import AgendamentosPage from './pages/Agendamentos/AgendamentosPage'
import CadastroListPage from './pages/Cadastros/CadastroListPage'
import CheckInPage from './pages/CheckIn/CheckInPage'
import JanelaAgendamentosPage from './pages/Configuracoes/JanelaAgendamentosPage'
import InventarioPage from './pages/Inventario/InventarioPage'
import Login from './pages/Login/Login'
import OperacoesPage from './pages/Operacoes/OperacoesPage'
import RelatoriosPage from './pages/Relatorios/RelatoriosPage'

type UserInfo = {
  nome?: string
  email?: string
  perfilDescricao?: string
}

function getStoredUser() {
  const token = localStorage.getItem('zyx.token')
  const rawUser = localStorage.getItem('zyx.user')

  if (!token || !rawUser) {
    return null
  }

  try {
    return JSON.parse(rawUser) as UserInfo
  } catch {
    localStorage.removeItem('zyx.user')
    localStorage.removeItem('zyx.token')
    return null
  }
}

function App() {
  const initialUser = useMemo(() => getStoredUser(), [])
  const [user, setUser] = useState<UserInfo | null>(initialUser)

  function handleLogout() {
    localStorage.removeItem('zyx.user')
    localStorage.removeItem('zyx.token')
    setUser(null)
  }

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/checkin" element={<CheckInPage />} />

        {!user ? (
          <Route path="*" element={<Login onAuthenticated={setUser} />} />
        ) : (
          <Route element={<MainLayout user={user} onLogout={handleLogout} />}>
            <Route index element={<Navigate to="/agendamentos/inbound" replace />} />
            <Route path="/agendamentos/inbound" element={<AgendamentosPage mode="inbound" />} />
            <Route path="/agendamentos/outbound" element={<AgendamentosPage mode="outbound" />} />
            <Route path="/operacao/inbound" element={<OperacoesPage mode="inbound" />} />
            <Route path="/operacao/outbound" element={<OperacoesPage mode="outbound" />} />
            <Route path="/cadastros/transportadora" element={<CadastroListPage cadastro="transportadora" />} />
            <Route path="/cadastros/motorista" element={<CadastroListPage cadastro="motorista" />} />
            <Route path="/cadastros/veiculo" element={<CadastroListPage cadastro="veiculo" />} />
            <Route path="/cadastros/local" element={<CadastroListPage cadastro="local" />} />
            <Route path="/cadastros/produto" element={<CadastroListPage cadastro="produto" />} />
            <Route path="/cadastros/usuario" element={<CadastroListPage cadastro="usuario" />} />
            <Route path="/cadastros/perfil" element={<CadastroListPage cadastro="perfil" />} />
            <Route path="/configuracoes/janela-agendamentos" element={<JanelaAgendamentosPage />} />
            <Route path="/inventario" element={<InventarioPage />} />
            <Route path="/relatorios" element={<RelatoriosPage />} />
            <Route path="*" element={<Navigate to="/agendamentos/inbound" replace />} />
          </Route>
        )}
      </Routes>
    </BrowserRouter>
  )
}

export default App
