import client from "./client";
import type { EventDto } from "../types";

interface ApiWrapper<T> {
  data: T;
  message: string;
  success: boolean;
}

interface PaginatedData<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface EventQueryParams {
  page?: number;
  pageSize?: number;
  status?: string;
  localityId?: number;
  eventTypeId?: number;
  search?: string;
}

export interface PagedEvents {
  items: EventDto[];
  totalItems: number;
  totalPages: number;
  page: number;
  pageSize: number;
}

export const eventsApi = {
  async getPublished(params?: EventQueryParams): Promise<PagedEvents> {
    const { data } = await client.get<ApiWrapper<PaginatedData<EventDto>>>("/Event/published", { params });
    const d = data.data;
    return {
      items: d.items ?? [],
      totalItems: d.totalItems ?? 0,
      totalPages: d.totalPages ?? 1,
      page: d.page ?? 1,
      pageSize: d.pageSize ?? 10,
    };
  },

  async getAll(params?: EventQueryParams): Promise<PagedEvents> {
    const { data } = await client.get<ApiWrapper<PaginatedData<EventDto>>>("/Event", { params });
    const d = data.data;
    return {
      items: d.items ?? [],
      totalItems: d.totalItems ?? 0,
      totalPages: d.totalPages ?? 1,
      page: d.page ?? 1,
      pageSize: d.pageSize ?? 10,
    };
  },

  async getById(id: number): Promise<EventDto> {
    const { data } = await client.get<ApiWrapper<EventDto>>(`/Event/${id}`);
    return data.data;
  },

  async create(payload: Partial<EventDto>): Promise<number> {
    const { data } = await client.post<ApiWrapper<number>>("/Event", payload);
    return data.data;
  },

  async update(payload: Partial<EventDto> & { id: number }): Promise<number> {
    const { data } = await client.put<ApiWrapper<number>>("/Event", payload);
    return data.data;
  },

  async publish(id: number): Promise<string> {
    const { data } = await client.patch<ApiWrapper<string>>(`/Event/${id}/publish`);
    return data.message ?? "Evento publicado correctamente.";
  },

  async cancel(id: number): Promise<string> {
    const { data } = await client.patch<ApiWrapper<string>>(`/Event/${id}/cancel`);
    return data.message ?? "Evento cancelado.";
  },

  async remove(id: number): Promise<string> {
    const { data } = await client.delete<ApiWrapper<string>>(`/Event/${id}`);
    return data.message ?? "Evento eliminado.";
  },

  async delete(id: number): Promise<void> {
    await client.delete(`/Event/${id}`);
  },
};
