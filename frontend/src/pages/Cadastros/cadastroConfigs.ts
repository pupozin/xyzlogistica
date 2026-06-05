export type CadastroKey =
  | 'transportadora'
  | 'motorista'
  | 'veiculo'
  | 'produto'
  | 'usuario'
  | 'perfil'

export type CadastroColumn = {
  header: string
  field: string
  type?: 'date' | 'boolean'
}

export type CadastroConfig = {
  key: CadastroKey
  title: string
  newButtonLabel: string
  endpoint: string
  searchPlaceholder: string
  buildQuery: (search: string) => URLSearchParams
  columns: CadastroColumn[]
}

const emptyQuery = () => new URLSearchParams()

export const cadastroConfigs: Record<CadastroKey, CadastroConfig> = {
  transportadora: {
    key: 'transportadora',
    title: 'Transportadora',
    newButtonLabel: '+ Nova transportadora',
    endpoint: '/api/transportadoras',
    searchPlaceholder: 'Nome ou CNPJ',
    buildQuery: (search) => {
      const query = new URLSearchParams()
      if (search) {
        query.set('nome', search)
        query.set('cnpj', search)
      }
      return query
    },
    columns: [
      { header: 'Nome', field: 'nome' },
      { header: 'CNPJ', field: 'cnpj' },
      { header: 'Criado em', field: 'criadoEm', type: 'date' },
      { header: 'Atualizado em', field: 'atualizadoEm', type: 'date' },
    ],
  },
  motorista: {
    key: 'motorista',
    title: 'Motorista',
    newButtonLabel: '+ Novo motorista',
    endpoint: '/api/motoristas',
    searchPlaceholder: 'Nome ou CNH',
    buildQuery: (search) => {
      const query = new URLSearchParams()
      if (search) {
        query.set('nome', search)
        query.set('cnh', search)
      }
      return query
    },
    columns: [
      { header: 'Nome', field: 'nome' },
      { header: 'CNH', field: 'cnh' },
      { header: 'Telefone', field: 'telefone' },
      { header: 'Criado em', field: 'criadoEm', type: 'date' },
      { header: 'Atualizado em', field: 'atualizadoEm', type: 'date' },
    ],
  },
  veiculo: {
    key: 'veiculo',
    title: 'Veículo',
    newButtonLabel: '+ Novo veículo',
    endpoint: '/api/veiculos',
    searchPlaceholder: 'Placa ou tipo',
    buildQuery: (search) => {
      const query = new URLSearchParams()
      if (search) {
        query.set('placa', search)
        query.set('tipoVeiculo', search)
      }
      return query
    },
    columns: [
      { header: 'Placa', field: 'placa' },
      { header: 'Tipo', field: 'tipoVeiculo' },
      { header: 'Transportadora', field: 'transportadoraNome' },
      { header: 'Criado em', field: 'criadoEm', type: 'date' },
      { header: 'Atualizado em', field: 'atualizadoEm', type: 'date' },
    ],
  },
  produto: {
    key: 'produto',
    title: 'Produto',
    newButtonLabel: '+ Novo produto',
    endpoint: '/api/produtos',
    searchPlaceholder: 'Descrição',
    buildQuery: (search) => {
      const query = new URLSearchParams()
      if (search) {
        query.set('descricao', search)
      }
      return query
    },
    columns: [
      { header: 'Descrição', field: 'descricao' },
      { header: 'Criado em', field: 'criadoEm', type: 'date' },
      { header: 'Atualizado em', field: 'atualizadoEm', type: 'date' },
    ],
  },
  usuario: {
    key: 'usuario',
    title: 'Usuário',
    newButtonLabel: '+ Novo usuário',
    endpoint: '/api/usuarios',
    searchPlaceholder: 'Nome ou e-mail',
    buildQuery: (search) => {
      const query = new URLSearchParams()
      if (search) {
        query.set('nome', search)
        query.set('email', search)
      }
      return query
    },
    columns: [
      { header: 'Nome', field: 'nome' },
      { header: 'E-mail', field: 'email' },
      { header: 'Perfil', field: 'perfilDescricao' },
      { header: 'Primeiro acesso', field: 'primeiroAcesso', type: 'boolean' },
      { header: 'Criado em', field: 'criadoEm', type: 'date' },
    ],
  },
  perfil: {
    key: 'perfil',
    title: 'Perfil',
    newButtonLabel: '+ Novo perfil',
    endpoint: '/api/perfis',
    searchPlaceholder: 'Descrição',
    buildQuery: emptyQuery,
    columns: [
      { header: 'Descrição', field: 'descricao' },
      { header: 'Ativo', field: 'ativo', type: 'boolean' },
    ],
  },
}
