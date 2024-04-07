import { useCallback, useEffect, useMemo, useState } from 'react';
import { Message } from '../types/Message';

const useWebSocket = () => {
  const [socket, setSocket] = useState<WebSocket>();
  const [lastMessage, setLastMessage] = useState<Message[]>();
  const [status, setStatus] = useState<string>();

  const sendMessage = useCallback(
    (message: string) => {
      if (socket) {
        socket.send(message);
      }
    },
    [socket],
  );

  useEffect(() => {
    setSocket(new WebSocket(`ws://${process.env.REACT_APP_API_URL}/wsconnect`));
  }, []);

  useEffect(() => {
    if (socket) {
      if (socket.readyState === WebSocket.CLOSED) {
        setStatus('closed');
      }
      if (socket.readyState === WebSocket.OPEN) {
        setStatus('open');
      }
    }
  }, [socket, socket?.readyState]);

  useEffect(() => {
    if (socket) {
      socket.onmessage = (event: MessageEvent) => {
        setLastMessage(JSON.parse(event.data));
      };

      socket.onopen = () => {};

      socket.onclose = () => {
        console.log('Соединение закрыто');
      };
    }
  }, [socket]);

  return useMemo(
    () => ({
      lastMessage,
      sendMessage,
      status,
      socket,
    }),
    [lastMessage, sendMessage, socket, status],
  );
};

export default useWebSocket;
