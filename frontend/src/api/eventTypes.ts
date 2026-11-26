import client from "./client";
import type { EventTypeDto } from "../types";

interface ApiWrapper<T> {
  data: T;
  message: string;
  success: boolean;
}

export const eventTypesApi = {
  /** GET /api/EventType — público */
  async getAll(): Promise<EventTypeDto[]> {
    const { data } = await client.get<ApiWrapper<EventTypeDto[]>>("/EventType");
    return data.data ?? [];
  },

  /** GET /api/EventType/{id} — público */
  async getById(id: number): Promise<EventTypeDto> {
    const { data } = await client.get<ApiWrapper<EventTypeDto>>(`/EventType/${id}`);
    return data.data;
  },

  /** POST /api/EventType — solo Admin */
  async create(payload: Omit<EventTypeDto, "id">): Promise<number> {
    const { data } = await client.post<ApiWrapper<number>>("/EventType", payload);
    return data.data;
  },

  /** PUT /api/EventType — solo Admin */
  async update(payload: EventTypeDto): Promise<number> {
    const { data } = await client.put<ApiWrapper<number>>("/EventType", payload);
    return data.data;
  },

  /** DELETE /api/EventType/{id} — solo Admin */
  async delete(id: number): Promise<void> {
    await client.delete(`/EventType/${id}`);
  },
};
