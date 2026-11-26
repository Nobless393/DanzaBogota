// ui-components v2 — ConfirmModal, PageHeader, Pagination, Textarea added
import type { ReactNode, ButtonHTMLAttributes, InputHTMLAttributes, SelectHTMLAttributes, TextareaHTMLAttributes } from "react";

// ─── Alert ───────────────────────────────────────────────────────────────────
interface AlertProps {
  tone?: "error" | "success" | "info" | "warn" | "warning";
  children: ReactNode;
  className?: string;
}

export function Alert({ tone = "error", children, className = "" }: AlertProps) {
  const t = tone === "warning" ? "warn" : tone;
  return (
    <div className={`alert alert--${t} ${className}`} role="alert">
      {children}
    </div>
  );
}

// ─── Card ────────────────────────────────────────────────────────────────────
interface CardProps {
  children: ReactNode;
  className?: string;
  title?: string;
}

export function Card({ children, className = "", title }: CardProps) {
  return (
    <div className={`card ${className}`}>
      {title && <h3 className="card-title">{title}</h3>}
      {children}
    </div>
  );
}

// ─── PageLoader ──────────────────────────────────────────────────────────────
export function PageLoader() {
  return (
    <div className="page-loader" aria-label="Cargando…">
      <span className="spinner" />
    </div>
  );
}

// ─── StatusBadge ─────────────────────────────────────────────────────────────
const STATUS_LABELS: Record<string, string> = {
  Published: "Publicado",
  Draft: "Borrador",
  Cancelled: "Cancelado",
  Finished: "Finalizado",
  Pending: "Pendiente",
  Approved: "Aprobado",
  Rejected: "Rechazado",
};

interface StatusBadgeProps {
  status?: string;
  className?: string;
}

export function StatusBadge({ status = "", className = "" }: StatusBadgeProps) {
  const slug = status.toLowerCase();
  return (
    <span className={`status-badge status-badge--${slug} ${className}`}>
      {STATUS_LABELS[status] ?? status}
    </span>
  );
}

// ─── Button ──────────────────────────────────────────────────────────────────
interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  loading?: boolean;
  variant?: "primary" | "secondary" | "ghost" | "danger" | "success";
  children: ReactNode;
}

export function Button({ loading = false, variant = "primary", children, className = "", disabled, ...rest }: ButtonProps) {
  return (
    <button className={`btn btn-${variant} ${className}`} disabled={disabled || loading} {...rest}>
      {loading ? <span className="spinner spinner--sm" aria-hidden="true" /> : null}
      {children}
    </button>
  );
}

// ─── Input ───────────────────────────────────────────────────────────────────
interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  hint?: string;
}

export function Input({ label, error, hint, className = "", id, ...rest }: InputProps) {
  const inputId = id ?? (label ? label.toLowerCase().replace(/\s+/g, "-") : undefined);
  return (
    <div className={`field ${className}`}>
      {label && <label htmlFor={inputId} className="field-label">{label}</label>}
      <input id={inputId} className={`input ${error ? "input--error" : ""}`} {...rest} />
      {hint && <span className="field-hint">{hint}</span>}
      {error && <span className="field-error">{error}</span>}
    </div>
  );
}

// ─── Textarea ────────────────────────────────────────────────────────────────
interface TextareaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
}

export function Textarea({ label, error, className = "", id, ...rest }: TextareaProps) {
  const areaId = id ?? (label ? label.toLowerCase().replace(/\s+/g, "-") : undefined);
  return (
    <div className={`field ${className}`}>
      {label && <label htmlFor={areaId} className="field-label">{label}</label>}
      <textarea id={areaId} className={`textarea ${error ? "textarea--error" : ""}`} {...rest} />
      {error && <span className="field-error">{error}</span>}
    </div>
  );
}

// ─── EmptyState ──────────────────────────────────────────────────────────────
interface EmptyStateProps {
  title: string;
  message?: string;
  className?: string;
}

export function EmptyState({ title, message, className = "" }: EmptyStateProps) {
  return (
    <div className={`empty-state ${className}`}>
      <span className="empty-state-icon" aria-hidden="true">📭</span>
      <p className="empty-state-title">{title}</p>
      {message && <p className="empty-state-message">{message}</p>}
    </div>
  );
}

// ─── Select ──────────────────────────────────────────────────────────────────
interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  children: ReactNode;
}

export function Select({ label, error, className = "", id, children, ...rest }: SelectProps) {
  const selectId = id ?? (label ? label.toLowerCase().replace(/\s+/g, "-") : undefined);
  return (
    <div className={`field ${className}`}>
      {label && <label htmlFor={selectId} className="field-label">{label}</label>}
      <select id={selectId} className={`select ${error ? "select--error" : ""}`} {...rest}>
        {children}
      </select>
      {error && <span className="field-error">{error}</span>}
    </div>
  );
}

// ─── PageHeader ──────────────────────────────────────────────────────────────
interface PageHeaderProps {
  title: string;
  subtitle?: string;
  actions?: ReactNode;
}

export function PageHeader({ title, subtitle, actions }: PageHeaderProps) {
  return (
    <div className="page-header">
      <div className="page-header-text">
        <h1>{title}</h1>
        {subtitle && <p className="page-header-sub">{subtitle}</p>}
      </div>
      {actions && <div className="page-header-actions">{actions}</div>}
    </div>
  );
}

// ─── Pagination ──────────────────────────────────────────────────────────────
interface PaginationProps {
  page: number;
  totalPages: number;
  totalItems?: number;
  onChange: (page: number) => void;
}

export function Pagination({ page, totalPages, totalItems, onChange }: PaginationProps) {
  if (totalPages <= 1) return null;
  return (
    <div className="pagination">
      <button
        className="btn btn-ghost btn-sm"
        disabled={page <= 1}
        onClick={() => onChange(page - 1)}
      >
        ← Anterior
      </button>
      <span className="pagination-info">
        Página {page} de {totalPages}
        {totalItems !== undefined ? ` · ${totalItems} registros` : ""}
      </span>
      <button
        className="btn btn-ghost btn-sm"
        disabled={page >= totalPages}
        onClick={() => onChange(page + 1)}
      >
        Siguiente →
      </button>
    </div>
  );
}

// ─── ConfirmModal ────────────────────────────────────────────────────────────
interface ConfirmModalProps {
  open: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  tone?: "primary" | "danger";
  loading?: boolean;
  onConfirm: () => void;
  onClose: () => void;
}

export function ConfirmModal({
  open,
  title,
  message,
  confirmLabel = "Confirmar",
  tone = "primary",
  loading = false,
  onConfirm,
  onClose,
}: ConfirmModalProps) {
  if (!open) return null;
  return (
    <div className="modal-overlay" role="dialog" aria-modal="true" aria-labelledby="modal-title">
      <div className="modal">
        <h3 id="modal-title" className="modal-title">{title}</h3>
        <p className="modal-message">{message}</p>
        <div className="modal-actions">
          <button className="btn btn-ghost" onClick={onClose} disabled={loading}>
            Cancelar
          </button>
          <Button
            variant={tone === "danger" ? "danger" : "primary"}
            loading={loading}
            onClick={onConfirm}
          >
            {confirmLabel}
          </Button>
        </div>
      </div>
    </div>
  );
}
