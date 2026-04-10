import api from './axios';
import { Message, SendMessageDto } from '../types';

export const getMessages = async (roomId: string, page = 1): Promise<Message[]> => {
  const response = await api.get<Message[]>(`/message/${roomId}?page=${page}&pageSize=20`);
  return response.data;
};

export const sendMessage = async (dto: SendMessageDto): Promise<Message> => {
  const response = await api.post<Message>('/message', dto);
  return response.data;
};

export const deleteMessage = async (messageId: string): Promise<void> => {
  await api.delete(`/message/${messageId}`);
};