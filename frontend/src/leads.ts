export type LeadStatus = 'New' | 'InProgress' | 'Success' | 'Rejected'

export type Lead = {
  id: string
  name: string
  phone: string
  comment: string | null
  source: string
  status: LeadStatus
  createdAt: string
  updatedAt: string
}

export type CreateLeadPayload = {
  name: string
  phone: string
  comment?: string
  source: string
}

export const STATUS_OPTIONS: { value: LeadStatus | 'all'; label: string }[] = [
  { value: 'all', label: 'Все' },
  { value: 'New', label: 'Новая' },
  { value: 'InProgress', label: 'В работе' },
  { value: 'Success', label: 'Успешно' },
  { value: 'Rejected', label: 'Отказ' }
]

export const STATUS_LABEL: Record<LeadStatus, string> = {
  New: 'Новая',
  InProgress: 'В работе',
  Success: 'Успешно',
  Rejected: 'Отказ'
}

export const SOURCE_OPTIONS = ['Сайт', 'Телефон', 'Telegram', 'WhatsApp', 'Реклама', 'Партнёр', 'CRM']

async function readError(response: Response): Promise<string> {
  try {
    const body = await response.json()
    if (body?.error) return body.error
    if (body?.title) return body.title
    if (body?.errors) {
      return Object.values(body.errors).flat().join(' ')
    }
  } catch {
    /* ignore */
  }
  return `Ошибка ${response.status}`
}

export async function fetchLeads(status: LeadStatus | 'all'): Promise<Lead[]> {
  const query = status === 'all' ? '' : `?status=${status}`
  const response = await fetch(`/api/leads${query}`)
  if (!response.ok) throw new Error(await readError(response))
  return response.json()
}

export async function createLead(payload: CreateLeadPayload): Promise<Lead> {
  const response = await fetch('/api/leads', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  })
  if (!response.ok) throw new Error(await readError(response))
  return response.json()
}

export async function updateLeadStatus(id: string, status: LeadStatus): Promise<Lead> {
  const response = await fetch(`/api/leads/${id}/status`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ status })
  })
  if (!response.ok) throw new Error(await readError(response))
  return response.json()
}
