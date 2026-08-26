import { useEffect, useId, useRef, useState } from 'react'
import { SOURCE_OPTIONS } from './leads'

type SourceSelectProps = {
  value: string
  onChange: (value: string) => void
}

export function SourceSelect({ value, onChange }: SourceSelectProps) {
  const [open, setOpen] = useState(false)
  const rootRef = useRef<HTMLDivElement>(null)
  const listId = useId()

  useEffect(() => {
    if (!open) return

    const onPointerDown = (event: PointerEvent) => {
      if (!rootRef.current?.contains(event.target as Node)) {
        setOpen(false)
      }
    }
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') setOpen(false)
    }

    document.addEventListener('pointerdown', onPointerDown)
    document.addEventListener('keydown', onKeyDown)
    return () => {
      document.removeEventListener('pointerdown', onPointerDown)
      document.removeEventListener('keydown', onKeyDown)
    }
  }, [open])

  return (
    <div className={`source-select${open ? ' open' : ''}`} ref={rootRef}>
      <button
        type="button"
        className="source-trigger"
        aria-haspopup="listbox"
        aria-expanded={open}
        aria-controls={listId}
        onClick={() => setOpen((current) => !current)}
      >
        <span>{value}</span>
        <svg viewBox="0 0 16 16" aria-hidden="true" className="source-chevron">
          <path
            d="M3.2 5.6 8 10.4l4.8-4.8"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.6"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </button>
      {open && (
        <ul className="source-menu" role="listbox" id={listId} aria-label="Источник заявки">
          {SOURCE_OPTIONS.map((option) => (
            <li key={option} role="option" aria-selected={option === value}>
              <button
                type="button"
                className={option === value ? 'source-option active' : 'source-option'}
                onClick={() => {
                  onChange(option)
                  setOpen(false)
                }}
              >
                {option}
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
