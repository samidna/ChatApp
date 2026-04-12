export interface User {
  id: string;
  username: string;
  email: string;
  avatarUrl?: string;
  isOnline: boolean;
  lastSeen: string;
}

export interface Room {
  id: string;
  name: string;
  isPrivate: boolean;
  createdAt: string;
  members: RoomMember[];
}

export interface RoomMember {
  userId: string;
  username: string;
  isAdmin: boolean;
}

export interface Message {
  id: string;
  content: string;
  fileUrl?: string;
  isRead: boolean;
  sentAt: string;
  senderId: string;
  senderUsername: string;
  roomId: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  username: string;
  email: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
}

export interface SendMessageDto {
  roomId: string;
  content: string;
  fileUrl?: string;
}

export interface UserResponseDto {
  id: string;
  username: string;
  email: string;
  isOnline: boolean;
}