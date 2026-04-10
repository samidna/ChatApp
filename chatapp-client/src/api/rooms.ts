import api from './axios';
import { Room } from '../types';

export const getRooms = async (): Promise<Room[]> => {
  const response = await api.get<Room[]>('/room');
  return response.data;
};

export const createRoom = async (name: string, isPrivate: boolean): Promise<Room> => {
  const response = await api.post<Room>('/room', { name, isPrivate });
  return response.data;
};

export const deleteRoom = async (roomId: string): Promise<void> => {
  await api.delete(`/room/${roomId}`);
};