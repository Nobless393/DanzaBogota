import client from "./client";
import type { CategoryDto, LocalityDto, EventTypeDto } from "../types";

interface ApiWrapper<T> {
  data: T;
  message: string;
  success: boolean;
}

export const catalogApi = {
  async getCategories(): Promise<CategoryDto[]> {
    const { data } = await client.get<ApiWrapper<CategoryDto[]>>("/Category");
    return data.data;
  },

  async getLocalities(): Promise<LocalityDto[]> {
    const { data } = await client.get<ApiWrapper<LocalityDto[]>>("/Locality");
    return data.data;
  },

  async getEventTypes(): Promise<EventTypeDto[]> {
    const { data } = await client.get<ApiWrapper<EventTypeDto[]>>("/EventType");
    return data.data;
  },
};
