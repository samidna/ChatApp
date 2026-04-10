import { useState, useEffect, useRef } from 'react';
import { useAuth } from '../context/AuthContext';
import { useSignalR } from '../hooks/useSignalR';
import { getRooms, createRoom } from '../api/rooms';
import { getMessages } from '../api/messages';
import { Room, Message } from '../types';

export default function ChatPage() {
  const { user, logout } = useAuth();
  const { messages, joinRoom, leaveRoom, sendMessage, sendTyping, setMessages } = useSignalR();
  const [rooms, setRooms] = useState<Room[]>([]);
  const [selectedRoom, setSelectedRoom] = useState<Room | null>(null);
  const [newMessage, setNewMessage] = useState('');
  const [newRoomName, setNewRoomName] = useState('');
  const [typingTimeout, setTypingTimeout] = useState<NodeJS.Timeout | null>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    loadRooms();
  }, []);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const loadRooms = async () => {
    try {
      const data = await getRooms();
      setRooms(data);
    } catch (err) {
      console.error('Otaqlar yüklənmədi:', err);
    }
  };

  const handleSelectRoom = async (room: Room) => {
    if (selectedRoom) await leaveRoom(selectedRoom.id);
    setSelectedRoom(room);
    setMessages([]);
    await joinRoom(room.id);
    const history = await getMessages(room.id);
    setMessages(history.reverse());
  };

  const handleSendMessage = async () => {
    if (!newMessage.trim() || !selectedRoom) return;
    await sendMessage(selectedRoom.id, newMessage);
    setNewMessage('');
  };

  const handleTyping = async () => {
    if (!selectedRoom) return;
    if (typingTimeout) clearTimeout(typingTimeout);
    await sendTyping(selectedRoom.id);
    const timeout = setTimeout(() => setTypingTimeout(null), 2000);
    setTypingTimeout(timeout);
  };

  const handleCreateRoom = async () => {
    if (!newRoomName.trim()) return;
    try {
      const room = await createRoom(newRoomName, false);
      setRooms((prev) => [...prev, room]);
      setNewRoomName('');
    } catch (err) {
      console.error('Otaq yaradılmadı:', err);
    }
  };

  return (
    <div style={styles.container}>
      {/* Sidebar */}
      <div style={styles.sidebar}>
        <div style={styles.sidebarHeader}>
          <span style={styles.username}>👤 {user?.username}</span>
          <button style={styles.logoutBtn} onClick={logout}>Çıx</button>
        </div>

        <div style={styles.createRoom}>
          <input
            style={styles.roomInput}
            placeholder="Otaq adı..."
            value={newRoomName}
            onChange={(e) => setNewRoomName(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleCreateRoom()}
          />
          <button style={styles.createBtn} onClick={handleCreateRoom}>+</button>
        </div>

        <div style={styles.roomList}>
          {rooms.map((room) => (
            <div
              key={room.id}
              style={{
                ...styles.roomItem,
                backgroundColor: selectedRoom?.id === room.id ? '#4f46e5' : 'transparent',
                color: selectedRoom?.id === room.id ? 'white' : '#333',
              }}
              onClick={() => handleSelectRoom(room)}
            >
              # {room.name}
            </div>
          ))}
        </div>
      </div>

      {/* Chat area */}
      <div style={styles.chatArea}>
        {selectedRoom ? (
          <>
            <div style={styles.chatHeader}>
              <h3 style={{ margin: 0 }}># {selectedRoom.name}</h3>
            </div>

            <div style={styles.messages}>
              {messages.map((msg: Message) => (
                <div
                  key={msg.id}
                  style={{
                    ...styles.message,
                    alignSelf: msg.senderId === user?.username ? 'flex-end' : 'flex-start',
                  }}
                >
                  <span style={styles.sender}>{msg.senderUsername}</span>
                  <div style={{
                    ...styles.bubble,
                    backgroundColor: msg.senderUsername === user?.username ? '#4f46e5' : '#f0f2f5',
                    color: msg.senderUsername === user?.username ? 'white' : '#333',
                  }}>
                    {msg.content}
                  </div>
                  <span style={styles.time}>
                    {new Date(msg.sentAt).toLocaleTimeString()}
                  </span>
                </div>
              ))}
              <div ref={messagesEndRef} />
            </div>

            <div style={styles.inputArea}>
              <input
                style={styles.messageInput}
                placeholder="Mesaj yaz..."
                value={newMessage}
                onChange={(e) => {
                  setNewMessage(e.target.value);
                  handleTyping();
                }}
                onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
              />
              <button style={styles.sendBtn} onClick={handleSendMessage}>
                Göndər
              </button>
            </div>
          </>
        ) : (
          <div style={styles.noRoom}>
            <p>Otaq seçin və ya yeni otaq yaradın</p>
          </div>
        )}
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  container: { display: 'flex', height: '100vh', fontFamily: 'sans-serif' },
  sidebar: { width: '260px', backgroundColor: '#1a1a2e', display: 'flex', flexDirection: 'column' },
  sidebarHeader: { padding: '1rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderBottom: '1px solid #333' },
  username: { color: 'white', fontWeight: 'bold' },
  logoutBtn: { backgroundColor: 'transparent', color: '#aaa', border: '1px solid #444', borderRadius: '6px', padding: '0.3rem 0.6rem', cursor: 'pointer' },
  createRoom: { padding: '0.75rem', display: 'flex', gap: '0.5rem' },
  roomInput: { flex: 1, padding: '0.5rem', borderRadius: '6px', border: 'none', fontSize: '0.9rem' },
  createBtn: { padding: '0.5rem 0.75rem', backgroundColor: '#4f46e5', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontSize: '1.2rem' },
  roomList: { flex: 1, overflowY: 'auto' },
  roomItem: { padding: '0.75rem 1rem', cursor: 'pointer', borderRadius: '6px', margin: '0.2rem 0.5rem', transition: 'background 0.2s' },
  chatArea: { flex: 1, display: 'flex', flexDirection: 'column' },
  chatHeader: { padding: '1rem', borderBottom: '1px solid #eee', backgroundColor: 'white' },
  messages: { flex: 1, overflowY: 'auto', padding: '1rem', display: 'flex', flexDirection: 'column', gap: '0.5rem' },
  message: { display: 'flex', flexDirection: 'column', maxWidth: '60%' },
  sender: { fontSize: '0.75rem', color: '#888', marginBottom: '0.2rem' },
  bubble: { padding: '0.6rem 1rem', borderRadius: '12px', fontSize: '0.95rem' },
  time: { fontSize: '0.7rem', color: '#aaa', marginTop: '0.2rem' },
  inputArea: { padding: '1rem', borderTop: '1px solid #eee', display: 'flex', gap: '0.5rem', backgroundColor: 'white' },
  messageInput: { flex: 1, padding: '0.75rem', borderRadius: '8px', border: '1px solid #ddd', fontSize: '1rem' },
  sendBtn: { padding: '0.75rem 1.5rem', backgroundColor: '#4f46e5', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer' },
  noRoom: { flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#aaa' },
};