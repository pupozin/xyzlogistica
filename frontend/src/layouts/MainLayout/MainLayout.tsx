import { useMemo, useState } from 'react'
import { NavLink, Outlet, useLocation } from 'react-router-dom'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {
  faBars,
  faBoxArchive,
  faCalendarDays,
  faChartSimple,
  faChevronRight,
  faGear,
  faRightFromBracket,
  faTruck,
  faUser,
  faUsers,
} from '@fortawesome/free-solid-svg-icons'
import './MainLayout.css'
import logoUrl from '../../../images/logo-xyz.png'

type MenuChild = {
  label: string
  path: string
}

type MenuGroup = {
  label: string
  icon: typeof faCalendarDays
  children: MenuChild[]
}

type UserInfo = {
  nome?: string
  email?: string
  perfilDescricao?: string
}

type MainLayoutProps = {
  user: UserInfo | null
  onLogout: () => void
}

const menuGroups: MenuGroup[] = [
  {
    label: 'Agendamentos',
    icon: faCalendarDays,
    children: [
      { label: 'Inbound', path: '/agendamentos/inbound' },
      { label: 'Outbound', path: '/agendamentos/outbound' },
    ],
  },
  {
    label: 'Operacao',
    icon: faTruck,
    children: [
      { label: 'Operacao Inbound', path: '/operacao/inbound' },
      { label: 'Operacao Outbound', path: '/operacao/outbound' },
    ],
  },
  {
    label: 'Cadastros',
    icon: faUsers,
    children: [
      { label: 'Transportadora', path: '/cadastros/transportadora' },
      { label: 'Motorista', path: '/cadastros/motorista' },
      { label: 'Veiculo', path: '/cadastros/veiculo' },
      { label: 'Local', path: '/cadastros/local' },
      { label: 'Produto', path: '/cadastros/produto' },
      { label: 'Usuario', path: '/cadastros/usuario' },
      { label: 'Perfil', path: '/cadastros/perfil' },
    ],
  },
  {
    label: 'Configuracoes',
    icon: faGear,
    children: [{ label: 'Janela agendamentos', path: '/configuracoes/janela-agendamentos' }],
  },
  {
    label: 'Inventario',
    icon: faBoxArchive,
    children: [{ label: 'Inventario', path: '/inventario' }],
  },
  {
    label: 'Relatorios',
    icon: faChartSimple,
    children: [{ label: 'Relatorios', path: '/relatorios' }],
  },
]

function MainLayout({ user, onLogout }: MainLayoutProps) {
  const [isMenuOpen, setIsMenuOpen] = useState(true)
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false)
  const location = useLocation()

  const currentGroupLabel = useMemo(() => {
    return menuGroups.find((group) =>
      group.children.some((child) => location.pathname === child.path),
    )?.label
  }, [location.pathname])

  const [expandedGroup, setExpandedGroup] = useState(currentGroupLabel ?? 'Agendamentos')

  function toggleGroup(label: string) {
    if (!isMenuOpen) {
      setIsMenuOpen(true)
      setExpandedGroup(label)
      return
    }

    setExpandedGroup((current) => (current === label ? '' : label))
  }

  function handleLogout() {
    setIsUserMenuOpen(false)
    onLogout()
  }

  return (
    <div className={`app-layout ${isMenuOpen ? 'menu-open' : 'menu-collapsed'}`}>
      <aside className="sidebar" aria-label="Menu lateral">
        <div className="sidebar-header">
          {isMenuOpen && <img src={logoUrl} alt="XYZ Logistica" className="sidebar-logo" />}
          <button className="menu-toggle" type="button" onClick={() => setIsMenuOpen((open) => !open)}>
            <FontAwesomeIcon icon={faBars} />
          </button>
        </div>

        <nav className="sidebar-nav">
          {menuGroups.map((group) => {
            const isActiveGroup = expandedGroup === group.label
            const isExpanded = isMenuOpen && expandedGroup === group.label

            return (
              <div className="menu-group" key={group.label}>
                <button
                  className={`menu-parent ${isActiveGroup ? 'active' : ''}`}
                  type="button"
                  onClick={() => toggleGroup(group.label)}
                  title={group.label}
                >
                  <FontAwesomeIcon className="menu-icon" icon={group.icon} />
                  {isMenuOpen && <span>{group.label}</span>}
                  {isMenuOpen && <FontAwesomeIcon className="menu-chevron" icon={faChevronRight} />}
                </button>

                {isExpanded && (
                  <div className="menu-children">
                    {group.children.map((child) => (
                      <NavLink
                        className={({ isActive }) => `menu-child ${isActive ? 'active' : ''}`}
                        key={child.path}
                        to={child.path}
                      >
                        {child.label}
                      </NavLink>
                    ))}
                  </div>
                )}
              </div>
            )
          })}
        </nav>
      </aside>

      <div className="content-area">
        <header className="topbar">
          <div className="user-badge">
            <button
              className="user-icon"
              type="button"
              onClick={() => setIsUserMenuOpen((open) => !open)}
              aria-label="Abrir menu do usuario"
              aria-expanded={isUserMenuOpen}
            >
              <FontAwesomeIcon icon={faUser} />
            </button>
            <span>{user?.nome ?? user?.email ?? 'Usuario'} - XYZ Logistica</span>

            {isUserMenuOpen && (
              <div className="user-menu" role="menu">
                <button className="user-menu-item" type="button" onClick={handleLogout} role="menuitem">
                  <FontAwesomeIcon icon={faRightFromBracket} />
                  Sair
                </button>
              </div>
            )}
          </div>
        </header>

        <main className="page-content">
          <Outlet />
        </main>
      </div>
    </div>
  )
}

export default MainLayout
