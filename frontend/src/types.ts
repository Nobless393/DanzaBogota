// ─── Auth ────────────────────────────────────────────────────────────────────
export type Role = "Admin" | "Gestor" | "Grupo";

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword?: string;
  documentType?: string;
  documentNumber?: string | null;
  role?: string;
}

export interface AuthResponseDto {
  success: boolean;
  token?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  roles?: string[];
  errorMessage?: string;
}

// ─── Users ───────────────────────────────────────────────────────────────────
export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
}

// ─── Events ──────────────────────────────────────────────────────────────────
export interface EventDto {
  id: number;
  name?: string;
  title?: string;
  description?: string;
  startDate: string;
  endDate: string;
  location?: string;
  imageUrl?: string;
  photoUrl?: string;
  resourceFileUrl?: string;
  eventTypeId?: number;
  eventTypeName?: string;
  eventTypeIds?: number[];
  categoryIds?: number[];
  localityId?: number;
  localityName?: string;
  locationName?: string;
  address?: string;
  capacity?: number;
  maxParticipants?: number;
  maxParticipantsPerGroup?: number;
  status?: string;
  createdByName?: string;
}

export interface CreateEventDto {
  name: string;
  description?: string;
  startDate: string;
  endDate: string;
  location?: string;
  photoUrl?: string;
  eventTypeId: number;
  localityId?: number;
  capacity?: number;
}

// ─── Groups ──────────────────────────────────────────────────────────────────
export interface GroupDto {
  id: number;
  name: string;
  description?: string;
  photoUrl?: string;
  managerId?: string;
  categoryId?: number;
  categoryName?: string;
  localityId?: number;
  localityName?: string;
  phone?: string;
  email?: string;
  directorName?: string;
}

// ─── Inscriptions ────────────────────────────────────────────────────────────
export interface InscriptionDto {
  id: number;
  eventId: number;
  eventName?: string;
  eventTitle?: string;
  groupId: number;
  groupName?: string;
  status?: string;
  registeredAt?: string;
  participantCount?: number;
}

// ─── Participants ────────────────────────────────────────────────────────────
export interface ParticipantDto {
  id: number;
  firstName: string;
  lastName: string;
  documentNumber?: string;
  birthDate?: string;
  groupId?: number;
}

// ─── Categories ──────────────────────────────────────────────────────────────
export interface CategoryDto {
  id: number;
  name: string;
  description?: string;
  minAge?: number;
  maxAge?: number;
}

// ─── Localities ──────────────────────────────────────────────────────────────
export interface LocalityDto {
  id: number;
  name: string;
}

// ─── Event Types ─────────────────────────────────────────────────────────────
export interface EventTypeDto {
  id: number;
  name: string;
  description?: string;
  isActive?: boolean;
}

// ─── Paginated ───────────────────────────────────────────────────────────────
export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiResponse<T = unknown> {
  success: boolean;
  data?: T;
  errorMessage?: string;
  errors?: string[];
}
