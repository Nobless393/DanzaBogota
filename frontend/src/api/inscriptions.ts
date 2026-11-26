import client from "./client";
import type { InscriptionDto, ParticipantDto } from "../types";

interface ApiWrapper<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface CreateInscriptionDto {
  eventId: number;
  groupId: number;
}

export interface ReviewInscriptionDto {
  inscriptionId: number;
  approve: boolean;
  notes?: string;
}

export interface UpdateParticipantsDto {
  participants: Partial<ParticipantDto>[];
}

export interface InscriptionStatsDto {
  totalInscriptions?: number;
  approved?: number;
  pending?: number;
  rejected?: number;
}

export const inscriptionsApi = {
  /** POST /api/Inscription — solo rol Grupo */
  async create(payload: CreateInscriptionDto): Promise<string> {
    const { data } = await client.post<ApiWrapper<string>>("/Inscription", payload);
    return data.message ?? "Inscripción enviada.";
  },

  /** GET /api/Inscription/my-inscriptions — rol Grupo */
  async getMyInscriptions(): Promise<InscriptionDto[]> {
    const { data } = await client.get<ApiWrapper<InscriptionDto[]>>("/Inscription/my-inscriptions");
    return data.data ?? [];
  },

  /** GET /api/Inscription/event/{eventId} — Admin/Gestor */
  async getByEvent(eventId: number): Promise<InscriptionDto[]> {
    const { data } = await client.get<ApiWrapper<InscriptionDto[]>>(`/Inscription/event/${eventId}`);
    return data.data ?? [];
  },

  /** PATCH /api/Inscription/review — Admin/Gestor */
  async review(payload: ReviewInscriptionDto): Promise<string> {
    const { data } = await client.patch<ApiWrapper<string>>("/Inscription/review", payload);
    return data.message ?? "Revisión registrada.";
  },

  /** PATCH /api/Inscription/{id}/cancel — Grupo */
  async cancel(id: number): Promise<string> {
    const { data } = await client.patch<ApiWrapper<string>>(`/Inscription/${id}/cancel`);
    return data.message ?? "Inscripción cancelada.";
  },

  /** GET /api/Inscription/{id}/participants */
  async getParticipants(id: number): Promise<ParticipantDto[]> {
    const { data } = await client.get<ApiWrapper<ParticipantDto[]>>(`/Inscription/${id}/participants`);
    return data.data ?? [];
  },

  /** PUT /api/Inscription/{id}/participants */
  async updateParticipants(id: number, payload: UpdateParticipantsDto): Promise<string> {
    const { data } = await client.put<ApiWrapper<string>>(`/Inscription/${id}/participants`, payload);
    return data.message ?? "Participantes actualizados.";
  },

  /** GET /api/Inscription/stats — Admin/Gestor */
  async getStats(): Promise<InscriptionStatsDto> {
    const { data } = await client.get<ApiWrapper<InscriptionStatsDto>>("/Inscription/stats");
    return data.data ?? {};
  },
};
