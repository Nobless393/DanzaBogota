import client from "./client";
import type { AuthResponseDto, LoginDto, RegisterDto } from "../types";

// La API devuelve { data: AuthResponseDto, message: string, success: bool }
// Axios extrae el body en `response.data`, así que tenemos que ir un nivel más adentro
interface ApiWrapper<T> {
  data: T;
  message: string;
  success: boolean;
}

export const authApi = {
  async login(payload: LoginDto): Promise<AuthResponseDto> {
    const { data } = await client.post<ApiWrapper<AuthResponseDto>>("/auth/login", payload);
    return data.data;
  },

  async register(payload: RegisterDto): Promise<AuthResponseDto> {
    const { data } = await client.post<ApiWrapper<AuthResponseDto>>("/auth/register", payload);
    return data.data;
  },
};
