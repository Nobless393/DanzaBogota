import client from "./client";
import type { UserDto } from "../types";

interface ApiWrapper<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface ChangeRoleDto {
  userId: string;
  newRole: string;
}

export const usersApi = {
  /** GET /api/User — solo Admin */
  async getAll(): Promise<UserDto[]> {
    const { data } = await client.get<ApiWrapper<UserDto[]>>("/User");
    return data.data ?? [];
  },

  /** GET /api/User/me */
  async getMe(): Promise<UserDto> {
    const { data } = await client.get<ApiWrapper<UserDto>>("/User/me");
    return data.data;
  },

  /** PATCH /api/User/change-role — solo Admin */
  async changeRole(payload: ChangeRoleDto): Promise<string> {
    const { data } = await client.patch<ApiWrapper<string>>("/User/change-role", payload);
    return data.message ?? "Rol actualizado.";
  },
};
