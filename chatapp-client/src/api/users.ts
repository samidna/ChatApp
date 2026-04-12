import api from './axios';
import { UserResponseDto } from '../types';

export const searchUsers = async (username: string): Promise<UserResponseDto[]> => {
  const response = await api.get<UserResponseDto[]>(`/user/search?username=${username}`);
  return response.data;
};

export const addMember = async (roomId: string, userId: string): Promise<void> => {
  await api.post(`/room/${roomId}/members/${userId}`);
};