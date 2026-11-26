import client from "./client";
import type { GroupDto } from "../types";

interface ApiWrapper<T> {
  data: T;
  message: string;
  success: boolean;
}

export const groupsApi = {
  /** GET /api/Group — Admin */
  async getAll(): Promise<GroupDto[]> {
    const { data } = await client.get<ApiWrapper<GroupDto[]>>("/Group");
    return data.data;
  },

  /** GET /api/Group/my-groups — Grupo */
  async getMyGroups(): Promise<GroupDto[]> {
    const { data } = await client.get<ApiWrapper<GroupDto[]>>("/Group/my-groups");
    return data.data;
  },

  /** GET /api/Group/{id} */
  async getById(id: number): Promise<GroupDto> {
    const { data } = await client.get<ApiWrapper<GroupDto>>(`/Group/${id}`);
    return data.data;
  },

  /** POST /api/Group — backend retorna int (el id creado) */
  async create(payload: Partial<GroupDto>): Promise<number> {
    const { data } = await client.post<ApiWrapper<number>>("/Group", payload);
    return data.data;
  },

  /** PUT /api/Group — backend recibe UpdateGroupDto en el body (no en la URL) */
  async update(id: number, payload: Partial<GroupDto>): Promise<number> {
    const { data } = await client.put<ApiWrapper<number>>("/Group", { ...payload, id });
    return data.data;
  },

  /** DELETE /api/Group/{id} */
  async delete(id: number): Promise<void> {
    await client.delete(`/Group/${id}`);
  },
};
