import api from './axios';
import { AuthResponse, LoginDto, RegisterDto } from '../types';

export const login = async (dto: LoginDto): Promise<AuthResponse> => {
  const response = await api.post<AuthResponse>('/auth/login', dto);
  return response.data;
};

export const register = async (dto: RegisterDto): Promise<AuthResponse> => {
  const response = await api.post<AuthResponse>('/auth/register', dto);
  return response.data;
};