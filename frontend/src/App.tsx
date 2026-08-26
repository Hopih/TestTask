import { useEffect, useMemo, useState, type FormEvent } from 'react'
import {
  createLead,
  fetchLeads,
  STATUS_LABEL,
  STATUS_OPTIONS,
  SOURCE_OPTIONS,
  updateLeadStatus,
  type Lead,
  type LeadStatus
} from './leads'

export default function App() {
  const [leads, setLeads] = useState<Lead[]>([])
  const [filter, setFilter] = useState<LeadStatus | 'all'>('all')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [notice, setNotice] = useState<string | null>(null)

  const load = async () => {
    setLoading(true)
    setError(null)
    try {
      setLeads(await fetchLeads('all'))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Не удалось загрузить заявки')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void load()
  }, [])

  const counts = useMemo(() => {
    const result: Record<string, number> = { all: leads.length }
    for (const lead of leads) {
      result[lead.status] = (result[lead.status] ?? 0) + 1
    }
    return result
  }, [leads])

  const visible = filter === 'all' ? leads : leads.filter((lead) => lead.status === filter)

  return (
    <div className="page">
      <header className="masthead">
        <div>
          <p className="eyebrow">Внутренний инструмент команды</p>
          <h1>Стол заявок</h1>
          <p className="lede">
            Принять обращение, поставить статус и не потерять его между сменами.
          </p>
        </div>
        <div className="stamp">TestTask</div>
      </header>

      {error && <div className="banner error">{error}</div>}
      {notice && <div className="banner ok">{notice}</div>}

      <main className="layout">
        <LeadForm
          onCreated={async () => {
            setNotice('Заявка сохранена')
            await load()
            window.setTimeout(() => setNotice(null), 2500)
          }}
          onError={setError}
        />
        <section className="board">
          <div className="board-head">
            <h2>Все заявки</h2>
            <div className="filters" role="tablist" aria-label="Фильтр по статусу">
              {STATUS_OPTIONS.map((option) => (
                <button
                  key={option.value}
                  type="button"
                  role="tab"
                  aria-selected={filter === option.value}
                  className={filter === option.value ? 'chip active' : 'chip'}
                  onClick={() => setFilter(option.value)}
                >
                  {option.label}
                  <span className="count">{counts[option.value] ?? 0}</span>
                </button>
              ))}
            </div>
          </div>

          {loading ? (
            <p className="muted">Загружаем…</p>
          ) : visible.length === 0 ? (
            <p className="empty">Пока пусто. Создайте первую заявку слева.</p>
          ) : (
            <ul className="cards">
              {visible.map((lead) => (
                <LeadCard
                  key={lead.id}
                  lead={lead}
                  onStatus={async (status) => {
                    try {
                      const updated = await updateLeadStatus(lead.id, status)
                      setLeads((current) =>
                        current.map((item) => (item.id === updated.id ? updated : item))
                      )
                    } catch (err) {
                      setError(err instanceof Error ? err.message : 'Не удалось сменить статус')
                    }
                  }}
                />
              ))}
            </ul>
          )}
        </section>
      </main>
    </div>
  )
}

function LeadForm({
  onCreated,
  onError
}: {
  onCreated: () => Promise<void>
  onError: (message: string | null) => void
}) {
  const [name, setName] = useState('')
  const [phone, setPhone] = useState('')
  const [source, setSource] = useState('Сайт')
  const [comment, setComment] = useState('')
  const [saving, setSaving] = useState(false)

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setSaving(true)
    onError(null)
    try {
      await createLead({
        name,
        phone,
        source,
        comment: comment.trim() || undefined
      })
      setName('')
      setPhone('')
      setComment('')
      setSource('Сайт')
      await onCreated()
    } catch (err) {
      onError(err instanceof Error ? err.message : 'Не удалось создать заявку')
    } finally {
      setSaving(false)
    }
  }

  return (
    <form className="panel" onSubmit={submit}>
      <h2>Новая заявка</h2>
      <label>
        Имя
        <input value={name} onChange={(e) => setName(e.target.value)} required maxLength={120} />
      </label>
      <label>
        Телефон
        <input
          value={phone}
          onChange={(e) => setPhone(e.target.value)}
          required
          minLength={5}
          maxLength={30}
          inputMode="tel"
          placeholder="+7 999 123-45-67"
        />
      </label>
      <label>
        Источник
        <input
          list="sources"
          value={source}
          onChange={(e) => setSource(e.target.value)}
          required
          maxLength={80}
        />
        <datalist id="sources">
          {SOURCE_OPTIONS.map((item) => (
            <option key={item} value={item} />
          ))}
        </datalist>
      </label>
      <label>
        Комментарий
        <textarea
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          maxLength={2000}
          rows={4}
          placeholder="Что нужно сделать и когда удобно звонить"
        />
      </label>
      <button type="submit" disabled={saving}>
        {saving ? 'Сохраняем…' : 'Создать заявку'}
      </button>
    </form>
  )
}

function LeadCard({
  lead,
  onStatus
}: {
  lead: Lead
  onStatus: (status: LeadStatus) => Promise<void>
}) {
  return (
    <li className="card">
      <div className="card-top">
        <strong>{lead.name}</strong>
        <span className={`badge status-${lead.status}`}>{STATUS_LABEL[lead.status]}</span>
      </div>
      <p className="phone">{lead.phone}</p>
      {lead.comment && <p className="comment">{lead.comment}</p>}
      <dl className="meta">
        <div>
          <dt>Источник</dt>
          <dd>{lead.source}</dd>
        </div>
        <div>
          <dt>Создана</dt>
          <dd>{formatDate(lead.createdAt)}</dd>
        </div>
      </dl>
      <label className="status-edit">
        Статус
        <select
          value={lead.status}
          onChange={(event) => void onStatus(event.target.value as LeadStatus)}
        >
          {(Object.keys(STATUS_LABEL) as LeadStatus[]).map((status) => (
            <option key={status} value={status}>
              {STATUS_LABEL[status]}
            </option>
          ))}
        </select>
      </label>
    </li>
  )
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('ru-RU', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(value))
}
