export type CadastroKey =
  | 'transportadora'
  | 'motorista'
  | 'veiculo'
  | 'local'
  | 'produto'
  | 'usuario'
  | 'perfil'

export type CadastroColumn = {
  header: string
  field: string
  type?: 'date' | 'boolean'
}

export type CadastroFormField = {
  name: string
  label: string
  type: 'text' | 'email' | 'select' | 'checkbox-list'
  placeholder?: string
  required?: boolean
  icon: 'building' | 'id-card' | 'phone' | 'mail' | 'truck' | 'box' | 'user' | 'shield' | 'list'
  optionsEndpoint?: string
  optionLabelField?: string
  optionValueField?: string
}

export type CadastroConfig = {
  key: CadastroKey
  title: string
  newButtonLabel: string
  saveButtonLabel: string
  endpoint: string
  searchPlaceholder: string
  buildQuery: (search: string) => URLSearchParams
  columns: CadastroColumn[]
  formFields: CadastroFormField[]
}

const emptyQuery = () => new URLSearchParams()

export const cadastroConfigs: Record<CadastroKey, CadastroConfig> = {
  transportadora: {
    key: 'transportadora',
    title: 'Transportadora',
    newButtonLabel: '+ Nova transportadora',
    saveButtonLabel: 'Salvar transportadora',
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
    formFields: [
      {
        name: 'nome',
        label: 'Nome da transportadora',
        type: 'text',
        placeholder: 'Digite o nome da transportadora',
        required: true,
        icon: 'building',
      },
      {
        name: 'cnpj',
        label: 'CNPJ',
        type: 'text',
        placeholder: '00.000.000/0000-00',
        required: true,
        icon: 'id-card',
      },
    ],
  },
  motorista: {
    key: 'motorista',
    title: 'Motorista',
    newButtonLabel: '+ Novo motorista',
    saveButtonLabel: 'Salvar motorista',
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
    formFields: [
      {
        name: 'nome',
        label: 'Nome do motorista',
        type: 'text',
        placeholder: 'Digite o nome do motorista',
        required: true,
        icon: 'user',
      },
      {
        name: 'cnh',
        label: 'CNH',
        type: 'text',
        placeholder: 'Digite a CNH',
        required: true,
        icon: 'id-card',
      },
      {
        name: 'telefone',
        label: 'Telefone',
        type: 'text',
        placeholder: '(00) 00000-0000',
        required: true,
        icon: 'phone',
      },
    ],
  },
  veiculo: {
    key: 'veiculo',
    title: 'Veículo',
    newButtonLabel: '+ Novo veículo',
    saveButtonLabel: 'Salvar veículo',
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
    formFields: [
      {
        name: 'placa',
        label: 'Placa',
        type: 'text',
        placeholder: 'Digite a placa',
        required: true,
        icon: 'truck',
      },
      {
        name: 'tipoVeiculo',
        label: 'Tipo do veículo',
        type: 'text',
        placeholder: 'Ex: Baú, Truck, Carreta',
        required: true,
        icon: 'truck',
      },
      {
        name: 'transportadoraId',
        label: 'Transportadora',
        type: 'select',
        placeholder: 'Selecione a transportadora',
        required: true,
        icon: 'building',
        optionsEndpoint: '/api/transportadoras',
        optionLabelField: 'nome',
        optionValueField: 'id',
      },
    ],
  },
  local: {
    key: 'local',
    title: 'Local',
    newButtonLabel: '+ Novo local',
    saveButtonLabel: 'Salvar local',
    endpoint: '/api/locais',
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
    formFields: [
      {
        name: 'descricao',
        label: 'Descrição',
        type: 'text',
        placeholder: 'Digite a descrição do local',
        required: true,
        icon: 'building',
      },
    ],
  },
  produto: {
    key: 'produto',
    title: 'Produto',
    newButtonLabel: '+ Novo produto',
    saveButtonLabel: 'Salvar produto',
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
    formFields: [
      {
        name: 'descricao',
        label: 'Descrição',
        type: 'text',
        placeholder: 'Digite a descrição do produto',
        required: true,
        icon: 'box',
      },
    ],
  },
  usuario: {
    key: 'usuario',
    title: 'Usuário',
    newButtonLabel: '+ Novo usuário',
    saveButtonLabel: 'Salvar usuário',
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
    formFields: [
      {
        name: 'nome',
        label: 'Nome do usuário',
        type: 'text',
        placeholder: 'Digite o nome do usuário',
        required: true,
        icon: 'user',
      },
      {
        name: 'email',
        label: 'E-mail',
        type: 'email',
        placeholder: 'exemplo@zyx.local',
        required: true,
        icon: 'mail',
      },
      {
        name: 'perfilId',
        label: 'Perfil',
        type: 'select',
        placeholder: 'Selecione o perfil',
        required: true,
        icon: 'shield',
        optionsEndpoint: '/api/perfis',
        optionLabelField: 'descricao',
        optionValueField: 'id',
      },
    ],
  },
  perfil: {
    key: 'perfil',
    title: 'Perfil',
    newButtonLabel: '+ Novo perfil',
    saveButtonLabel: 'Salvar perfil',
    endpoint: '/api/perfis',
    searchPlaceholder: 'Descrição',
    buildQuery: emptyQuery,
    columns: [
      { header: 'Descrição', field: 'descricao' },
      { header: 'Ativo', field: 'ativo', type: 'boolean' },
    ],
    formFields: [
      {
        name: 'descricao',
        label: 'Descrição',
        type: 'text',
        placeholder: 'Digite a descrição do perfil',
        required: true,
        icon: 'shield',
      },
      {
        name: 'permissaoIds',
        label: 'Permissões',
        type: 'checkbox-list',
        required: false,
        icon: 'list',
        optionsEndpoint: '/api/permissoes',
        optionLabelField: 'descricao',
        optionValueField: 'id',
      },
    ],
  },
}
