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
import { cadastroPermissions, hasPermission, Permission } from './security/permissions'

type UserInfo = {
  nome?: string
  email?: string
  perfilDescricao?: string
  permissoes?: Permission[]
}

type ProtectedRouteProps = {
  user: UserInfo | null
  permission: string
  children: JSX.Element
}

const routePermissions = [
  { path: '/agendamentos/inbound', permission: 'agendamentos.visualizar' },
  { path: '/agendamentos/outbound', permission: 'agendamentos.visualizar' },
  { path: '/operacao/inbound', permission: 'operacoes.visualizar' },
  { path: '/operacao/outbound', permission: 'operacoes.visualizar' },
  { path: '/cadastros/transportadora', permission: cadastroPermissions.transportadora.view },
  { path: '/cadastros/motorista', permission: cadastroPermissions.motorista.view },
  { path: '/cadastros/veiculo', permission: cadastroPermissions.veiculo.view },
  { path: '/cadastros/local', permission: cadastroPermissions.local.view },
  { path: '/cadastros/produto', permission: cadastroPermissions.produto.view },
  { path: '/cadastros/usuario', permission: cadastroPermissions.usuario.view },
  { path: '/cadastros/perfil', permission: cadastroPermissions.perfil.view },
  { path: '/configuracoes/janela-agendamentos', permission: 'configuracoes.visualizar' },
  { path: '/inventario', permission: 'inventario.visualizar' },
  { path: '/relatorios', permission: 'relatorios.visualizar' },
]

function getFirstAllowedPath(user: UserInfo | null) {
  return routePermissions.find((route) => hasPermission(user, route.permission))?.path ?? '/checkin'
}

function ProtectedRoute({ user, permission, children }: ProtectedRouteProps) {
  if (!hasPermission(user, permission)) {
    return <Navigate to={getFirstAllowedPath(user)} replace />
  }

  return children
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
            <Route index element={<Navigate to={getFirstAllowedPath(user)} replace />} />
            <Route path="/agendamentos/inbound" element={<ProtectedRoute user={user} permission="agendamentos.visualizar"><AgendamentosPage mode="inbound" /></ProtectedRoute>} />
            <Route path="/agendamentos/outbound" element={<ProtectedRoute user={user} permission="agendamentos.visualizar"><AgendamentosPage mode="outbound" /></ProtectedRoute>} />
            <Route path="/operacao/inbound" element={<ProtectedRoute user={user} permission="operacoes.visualizar"><OperacoesPage mode="inbound" /></ProtectedRoute>} />
            <Route path="/operacao/outbound" element={<ProtectedRoute user={user} permission="operacoes.visualizar"><OperacoesPage mode="outbound" /></ProtectedRoute>} />
            <Route path="/cadastros/transportadora" element={<ProtectedRoute user={user} permission={cadastroPermissions.transportadora.view}><CadastroListPage cadastro="transportadora" /></ProtectedRoute>} />
            <Route path="/cadastros/motorista" element={<ProtectedRoute user={user} permission={cadastroPermissions.motorista.view}><CadastroListPage cadastro="motorista" /></ProtectedRoute>} />
            <Route path="/cadastros/veiculo" element={<ProtectedRoute user={user} permission={cadastroPermissions.veiculo.view}><CadastroListPage cadastro="veiculo" /></ProtectedRoute>} />
            <Route path="/cadastros/local" element={<ProtectedRoute user={user} permission={cadastroPermissions.local.view}><CadastroListPage cadastro="local" /></ProtectedRoute>} />
            <Route path="/cadastros/produto" element={<ProtectedRoute user={user} permission={cadastroPermissions.produto.view}><CadastroListPage cadastro="produto" /></ProtectedRoute>} />
            <Route path="/cadastros/usuario" element={<ProtectedRoute user={user} permission={cadastroPermissions.usuario.view}><CadastroListPage cadastro="usuario" /></ProtectedRoute>} />
            <Route path="/cadastros/perfil" element={<ProtectedRoute user={user} permission={cadastroPermissions.perfil.view}><CadastroListPage cadastro="perfil" /></ProtectedRoute>} />
            <Route path="/configuracoes/janela-agendamentos" element={<ProtectedRoute user={user} permission="configuracoes.visualizar"><JanelaAgendamentosPage /></ProtectedRoute>} />
            <Route path="/inventario" element={<ProtectedRoute user={user} permission="inventario.visualizar"><InventarioPage /></ProtectedRoute>} />
            <Route path="/relatorios" element={<ProtectedRoute user={user} permission="relatorios.visualizar"><RelatoriosPage /></ProtectedRoute>} />
            <Route path="*" element={<Navigate to={getFirstAllowedPath(user)} replace />} />
          </Route>
        )}
      </Routes>
    </BrowserRouter>
  )
}

export default App
