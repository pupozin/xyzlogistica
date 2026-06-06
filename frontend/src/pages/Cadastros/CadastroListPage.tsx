import { FormEvent, useEffect, useMemo, useState } from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {
  faBox,
  faBuilding,
  faEnvelope,
  faFloppyDisk,
  faIdCard,
  faListCheck,
  faPen,
  faPhone,
  faShieldHalved,
  faTrashCan,
  faTriangleExclamation,
  faTruck,
  faUser,
  faXmark,
} from '@fortawesome/free-solid-svg-icons'
import './CadastroListPage.css'
import { CadastroFormField, CadastroKey, cadastroConfigs } from './cadastroConfigs'

type CadastroListPageProps = {
  cadastro: CadastroKey
}

type GridRow = Record<string, unknown> & {
  id: number
}

type FormValue = string | number[]
type FormValues = Record<string, FormValue>

type SelectOption = {
  id: number
  label: string
}

type ModalMode = 'create' | 'edit'

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5271'

const fieldIcons = {
  building: faBuilding,
  'id-card': faIdCard,
  phone: faPhone,
  mail: faEnvelope,
  truck: faTruck,
  box: faBox,
  user: faUser,
  shield: faShieldHalved,
  list: faListCheck,
}

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

function buildInitialValues(fields: CadastroFormField[], row?: GridRow): FormValues {
  return fields.reduce<FormValues>((values, field) => {
    if (field.type === 'checkbox-list') {
      values[field.name] = Array.isArray(row?.[field.name]) ? (row?.[field.name] as number[]) : []
      return values
    }

    const value = row?.[field.name]
    values[field.name] = value === null || value === undefined ? '' : String(value)
    return values
  }, {})
}

function formatCnpj(value: string) {
  return value
    .replace(/\D/g, '')
    .slice(0, 14)
    .replace(/^(\d{2})(\d)/, '$1.$2')
    .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
    .replace(/\.(\d{3})(\d)/, '.$1/$2')
    .replace(/(\d{4})(\d)/, '$1-$2')
}

function CadastroListPage({ cadastro }: CadastroListPageProps) {
  const config = cadastroConfigs[cadastro]
  const [rows, setRows] = useState<GridRow[]>([])
  const [search, setSearch] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [message, setMessage] = useState('')
  const [modalMode, setModalMode] = useState<ModalMode | null>(null)
  const [editingRow, setEditingRow] = useState<GridRow | null>(null)
  const [formValues, setFormValues] = useState<FormValues>(() => buildInitialValues(config.formFields))
  const [fieldOptions, setFieldOptions] = useState<Record<string, SelectOption[]>>({})
  const [deleteRow, setDeleteRow] = useState<GridRow | null>(null)

  const visibleRows = useMemo(() => {
    if (cadastro !== 'perfil' || !search.trim()) {
      return rows
    }

    return rows.filter((row) =>
      String(row.descricao ?? '')
        .toLowerCase()
        .includes(search.trim().toLowerCase()),
    )
  }, [cadastro, rows, search])

  async function loadRows(searchTerm = search) {
    setIsLoading(true)
    setMessage('')

    try {
      const query = config.buildQuery(searchTerm.trim())
      const url = `${apiBaseUrl}${config.endpoint}${query.size ? `?${query}` : ''}`
      const response = await fetch(url, {
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível carregar os registros.'))
      }

      const data = (await response.json()) as GridRow[]
      setRows(data)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar registros.')
    } finally {
      setIsLoading(false)
    }
  }

  async function loadFieldOptions() {
    const optionFields = config.formFields.filter((field) => field.optionsEndpoint)

    if (optionFields.length === 0) {
      setFieldOptions({})
      return
    }

    const loadedOptions = await Promise.all(
      optionFields.map(async (field) => {
        const response = await fetch(`${apiBaseUrl}${field.optionsEndpoint}`, {
          headers: getAuthHeaders(),
        })

        if (!response.ok) {
          throw new Error(`Não foi possível carregar ${field.label.toLowerCase()}.`)
        }

        const data = (await response.json()) as GridRow[]
        const labelField = field.optionLabelField ?? 'descricao'
        const valueField = field.optionValueField ?? 'id'

        return [
          field.name,
          data.map((item) => ({
            id: Number(item[valueField]),
            label: String(item[labelField] ?? item[valueField]),
          })),
        ] as const
      }),
    )

    setFieldOptions(Object.fromEntries(loadedOptions))
  }

  async function loadPerfilPermissoes(perfilId: number) {
    const response = await fetch(`${apiBaseUrl}/api/perfis/${perfilId}/permissoes`, {
      headers: getAuthHeaders(),
    })

    if (!response.ok) {
      throw new Error('Não foi possível carregar as permissões do perfil.')
    }

    const data = (await response.json()) as GridRow[]
    return data.map((permissao) => Number(permissao.id))
  }

  async function openCreateModal() {
    setMessage('')
    setEditingRow(null)
    setFormValues(buildInitialValues(config.formFields))
    setModalMode('create')

    try {
      await loadFieldOptions()
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar opções.')
    }
  }

  async function openEditModal(row: GridRow) {
    setMessage('')
    setEditingRow(row)
    setModalMode('edit')

    try {
      await loadFieldOptions()

      const response = await fetch(`${apiBaseUrl}${config.endpoint}/${row.id}`, {
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível carregar o registro.'))
      }

      const data = (await response.json()) as GridRow
      const values = buildInitialValues(config.formFields, data)

      if (cadastro === 'transportadora') {
        values.cnpj = formatCnpj(String(values.cnpj ?? ''))
      }

      if (cadastro === 'perfil') {
        values.permissaoIds = await loadPerfilPermissoes(row.id)
      }

      setEditingRow(data)
      setFormValues(values)
    } catch (error) {
      setModalMode(null)
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar registro.')
    }
  }

  function closeFormModal(force = false) {
    if (isSaving && !force) {
      return
    }

    setModalMode(null)
    setEditingRow(null)
    setFormValues(buildInitialValues(config.formFields))
  }

  function updateField(field: CadastroFormField, value: string) {
    const nextValue = cadastro === 'transportadora' && field.name === 'cnpj' ? formatCnpj(value) : value

    setFormValues((current) => ({
      ...current,
      [field.name]: nextValue,
    }))
  }

  function toggleCheckboxField(field: CadastroFormField, optionId: number) {
    setFormValues((current) => {
      const selectedIds = Array.isArray(current[field.name]) ? (current[field.name] as number[]) : []
      const nextIds = selectedIds.includes(optionId)
        ? selectedIds.filter((id) => id !== optionId)
        : [...selectedIds, optionId]

      return {
        ...current,
        [field.name]: nextIds,
      }
    })
  }

  function buildPayload() {
    return config.formFields.reduce<Record<string, string | number | number[]>>((payload, field) => {
      const value = formValues[field.name]

      if (field.type === 'checkbox-list') {
        payload[field.name] = Array.isArray(value) ? value : []
        return payload
      }

      if (field.type === 'select') {
        payload[field.name] = Number(value)
        return payload
      }

      payload[field.name] = String(value ?? '').trim()
      return payload
    }, {})
  }

  async function handleSave(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!modalMode) {
      return
    }

    setIsSaving(true)
    setMessage('')

    try {
      const response = await fetch(
        `${apiBaseUrl}${config.endpoint}${modalMode === 'edit' && editingRow ? `/${editingRow.id}` : ''}`,
        {
          method: modalMode === 'edit' ? 'PUT' : 'POST',
          headers: getAuthHeaders(true),
          body: JSON.stringify(buildPayload()),
        },
      )

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível salvar o registro.'))
      }

      closeFormModal(true)
      await loadRows()
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar registro.')
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDelete() {
    if (!deleteRow) {
      return
    }

    try {
      const response = await fetch(`${apiBaseUrl}${config.endpoint}/${deleteRow.id}`, {
        method: 'DELETE',
        headers: getAuthHeaders(),
      })

      if (!response.ok) {
        throw new Error(await readErrorMessage(response, 'Não foi possível excluir o registro.'))
      }

      setDeleteRow(null)
      await loadRows()
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir registro.')
    }
  }

  function formatValue(row: GridRow, field: string, type?: string) {
    const value = row[field]

    if (value === null || value === undefined || value === '') {
      return '-'
    }

    if (type === 'date') {
      return new Intl.DateTimeFormat('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(new Date(String(value)))
    }

    if (type === 'boolean') {
      return value ? 'Sim' : 'Não'
    }

    return String(value)
  }

  useEffect(() => {
    setSearch('')
    setRows([])
    setMessage('')
    setModalMode(null)
    setDeleteRow(null)
    void loadRows('')
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [cadastro])

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      void loadRows(search)
    }, 350)

    return () => window.clearTimeout(timeoutId)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search])

  return (
    <section className="cadastro-page">
      <header className="cadastro-header">
        <div className="cadastro-title-actions">
          <h1>{config.title}</h1>
          <button className="cadastro-new-button" type="button" onClick={() => void openCreateModal()}>
            {config.newButtonLabel}
          </button>
        </div>

        <div className="cadastro-filter">
          <label htmlFor={`${cadastro}-search`}>Filtro:</label>
          <input
            id={`${cadastro}-search`}
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={config.searchPlaceholder}
          />
        </div>
      </header>

      <div className="cadastro-card">
        <div className="cadastro-table-wrap">
          <table className="cadastro-table">
            <thead>
              <tr>
                {config.columns.map((column) => (
                  <th key={column.field}>{column.header}</th>
                ))}
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {visibleRows.map((row) => (
                <tr key={row.id}>
                  {config.columns.map((column) => (
                    <td key={column.field}>{formatValue(row, column.field, column.type)}</td>
                  ))}
                  <td>
                    <div className="cadastro-actions">
                      <button className="edit-action" type="button" onClick={() => void openEditModal(row)}>
                        <FontAwesomeIcon icon={faPen} />
                      </button>
                      <button className="delete-action" type="button" onClick={() => setDeleteRow(row)}>
                        <FontAwesomeIcon icon={faTrashCan} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}

              {!isLoading && visibleRows.length === 0 && (
                <tr>
                  <td className="empty-cell" colSpan={config.columns.length + 1}>
                    Nenhum registro encontrado.
                  </td>
                </tr>
              )}

              {isLoading && (
                <tr>
                  <td className="empty-cell" colSpan={config.columns.length + 1}>
                    Carregando registros...
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        <footer className="cadastro-footer">
          {message && <span className="cadastro-message">{message}</span>}
          <div className="pagination">
            <button type="button">‹</button>
            <span>1</span>
            <button type="button">›</button>
          </div>
        </footer>
      </div>

      {modalMode && (
        <div className="cadastro-modal-backdrop">
          <form className="cadastro-modal" onSubmit={(event) => void handleSave(event)}>
            <button className="modal-close-button" type="button" onClick={() => closeFormModal()} aria-label="Fechar">
              <FontAwesomeIcon icon={faXmark} />
            </button>

            <header className="modal-header">
              <div className="modal-icon">
                <FontAwesomeIcon icon={fieldIcons[config.formFields[0]?.icon ?? 'building']} />
              </div>
              <div>
                <h2>{config.title}</h2>
                <p>
                  {modalMode === 'create'
                    ? 'Preencha as informações para cadastrar.'
                    : 'Atualize as informações do cadastro.'}
                </p>
              </div>
            </header>

            <div className="modal-fields">
              {config.formFields.map((field) => {
                const value = formValues[field.name]

                return (
                  <label className="modal-field" key={field.name}>
                    <span>
                      {field.label}
                      {field.required && <strong> *</strong>}
                    </span>

                    {field.type === 'checkbox-list' ? (
                      <div className="checkbox-list">
                        {(fieldOptions[field.name] ?? []).map((option) => {
                          const selectedIds = Array.isArray(value) ? value : []

                          return (
                            <label className="checkbox-option" key={option.id}>
                              <input
                                type="checkbox"
                                checked={selectedIds.includes(option.id)}
                                onChange={() => toggleCheckboxField(field, option.id)}
                              />
                              <span>{option.label}</span>
                            </label>
                          )
                        })}
                      </div>
                    ) : (
                      <div className="input-shell">
                        <FontAwesomeIcon icon={fieldIcons[field.icon]} />
                        {field.type === 'select' ? (
                          <select
                            value={String(value ?? '')}
                            required={field.required}
                            onChange={(event) => updateField(field, event.target.value)}
                          >
                            <option value="">{field.placeholder ?? 'Selecione'}</option>
                            {(fieldOptions[field.name] ?? []).map((option) => (
                              <option key={option.id} value={option.id}>
                                {option.label}
                              </option>
                            ))}
                          </select>
                        ) : (
                          <input
                            type={field.type}
                            value={String(value ?? '')}
                            required={field.required}
                            placeholder={field.placeholder}
                            maxLength={cadastro === 'transportadora' && field.name === 'cnpj' ? 18 : undefined}
                            onChange={(event) => updateField(field, event.target.value)}
                          />
                        )}
                      </div>
                    )}
                  </label>
                )
              })}
            </div>

            <footer className="modal-actions">
              <button className="modal-cancel-button" type="button" onClick={() => closeFormModal()}>
                Cancelar
              </button>
              <button className="modal-save-button" type="submit" disabled={isSaving}>
                <FontAwesomeIcon icon={faFloppyDisk} />
                {isSaving ? 'Salvando...' : config.saveButtonLabel}
              </button>
            </footer>
          </form>
        </div>
      )}

      {deleteRow && (
        <div className="cadastro-modal-backdrop">
          <div className="delete-modal" role="dialog" aria-modal="true">
            <button className="modal-close-button" type="button" onClick={() => setDeleteRow(null)} aria-label="Fechar">
              <FontAwesomeIcon icon={faXmark} />
            </button>

            <div className="delete-icon">
              <FontAwesomeIcon icon={faTriangleExclamation} />
            </div>
            <h2>Excluir {config.title.toLowerCase()}</h2>
            <p>Tem certeza que deseja excluir este registro?</p>

            <footer className="modal-actions">
              <button className="modal-cancel-button" type="button" onClick={() => setDeleteRow(null)}>
                Cancelar
              </button>
              <button className="modal-delete-button" type="button" onClick={() => void handleDelete()}>
                <FontAwesomeIcon icon={faTrashCan} />
                Excluir
              </button>
            </footer>
          </div>
        </div>
      )}
    </section>
  )
}

export default CadastroListPage
