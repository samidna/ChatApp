import { useEffect, useRef, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { Message } from '../types';

export function useSignalR() {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [typingUsers, setTypingUsers] = useState<string[]>([]);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) return;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7202/hubs/chat', {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    newConnection.on('ReceiveMessage', (message: Message) => {
      setMessages((prev) => [...prev, message]);
    });

    newConnection.on('UserTyping', (userId: string) => {
      setTypingUsers((prev) => [...prev, userId]);
      setTimeout(() => {
        setTypingUsers((prev) => prev.filter((id) => id !== userId));
      }, 2000);
    });

    newConnection.on('UserOnline', (userId: string) => {
      console.log(`${userId} online oldu`);
    });

    newConnection.on('UserOffline', (userId: string) => {
      console.log(`${userId} offline oldu`);
    });

    newConnection
      .start()
      .then(() => {
        setConnection(newConnection);
        connectionRef.current = newConnection;
      })
      .catch((err) => console.error('SignalR bağlantı xətası:', err));

    return () => {
      newConnection.stop();
    };
  }, []);

  const joinRoom = async (roomId: string) => {
    if (connectionRef.current) {
      await connectionRef.current.invoke('JoinRoom', roomId);
    }
  };

  const leaveRoom = async (roomId: string) => {
    if (connectionRef.current) {
      await connectionRef.current.invoke('LeaveRoom', roomId);
    }
  };

  const sendMessage = async (roomId: string, content: string) => {
    if (connectionRef.current) {
      await connectionRef.current.invoke('SendMessage', { roomId, content });
    }
  };

  const sendTyping = async (roomId: string) => {
    if (connectionRef.current) {
      await connectionRef.current.invoke('Typing', roomId);
    }
  };

  return {
    connection,
    messages,
    typingUsers,
    joinRoom,
    leaveRoom,
    sendMessage,
    sendTyping,
    setMessages,
  };
}