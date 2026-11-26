import client from "./client";

interface ApiWrapper<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface EventCommentDto {
  id: number;
  eventId: number;
  userId?: number;
  authorName?: string;
  content: string;
  createdAt?: string;
  likesCount?: number;
  liked?: boolean;
  isOwn?: boolean;
}

export interface CreateEventCommentDto {
  content: string;
}

export const commentsApi = {
  /** GET /api/Event/{eventId}/comments */
  async getByEvent(eventId: number, sort = "recent"): Promise<EventCommentDto[]> {
    const { data } = await client.get<ApiWrapper<EventCommentDto[]>>(
      `/Event/${eventId}/comments`,
      { params: { sort } }
    );
    return data.data ?? [];
  },

  /** POST /api/Event/{eventId}/comments */
  async create(eventId: number, payload: CreateEventCommentDto): Promise<EventCommentDto | null> {
    const { data } = await client.post<ApiWrapper<EventCommentDto | null>>(
      `/Event/${eventId}/comments`,
      payload
    );
    return data.data;
  },

  /** DELETE /api/Event/{eventId}/comments/{commentId} */
  async delete(eventId: number, commentId: number): Promise<void> {
    await client.delete(`/Event/${eventId}/comments/${commentId}`);
  },

  /** POST /api/Event/{eventId}/comments/{commentId}/like */
  async toggleLike(eventId: number, commentId: number): Promise<{ likesCount: number; liked: boolean }> {
    const { data } = await client.post<ApiWrapper<{ likesCount: number; liked: boolean }>>(
      `/Event/${eventId}/comments/${commentId}/like`
    );
    return data.data;
  },
};
