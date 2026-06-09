export type Permission = {
  codigo: string
}

export type PermissionUser = {
  permissoes?: Permission[]
}

export function hasPermission(user: PermissionUser | null | undefined, permission: string) {
  return Boolean(user?.permissoes?.some((item) => item.codigo === permission))
}

export const cadastroPermissions = {
  transportadora: {
    view: 'transportadoras.visualizar',
    create: 'transportadoras.criar',
    edit: 'transportadoras.editar',
    delete: 'transportadoras.excluir',
  },
  motorista: {
    view: 'motoristas.visualizar',
    create: 'motoristas.criar',
    edit: 'motoristas.editar',
    delete: 'motoristas.excluir',
  },
  veiculo: {
    view: 'veiculos.visualizar',
    create: 'veiculos.criar',
    edit: 'veiculos.editar',
    delete: 'veiculos.excluir',
  },
  local: {
    view: 'locais.visualizar',
    create: 'locais.criar',
    edit: 'locais.editar',
    delete: 'locais.excluir',
  },
  produto: {
    view: 'produtos.visualizar',
    create: 'produtos.criar',
    edit: 'produtos.editar',
    delete: 'produtos.excluir',
  },
  usuario: {
    view: 'usuarios.visualizar',
    create: 'usuarios.criar',
    edit: 'usuarios.editar',
    delete: 'usuarios.excluir',
  },
  perfil: {
    view: 'perfis.visualizar',
    create: 'perfis.editar_permissoes',
    edit: 'perfis.editar_permissoes',
    delete: 'perfis.editar_permissoes',
  },
} as const
