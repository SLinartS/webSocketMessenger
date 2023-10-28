import { useCallback, useEffect, useMemo, useState } from 'react';

const useWebSocket = () => {
  const [socket, setSocket] = useState<WebSocket>();
  const [lastMessage, setLastMessage] = useState<string>();
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
    setSocket(new WebSocket('ws://localhost:5104/wsconnect'));
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
        setLastMessage(event.data);
      };

      socket.onopen = () => {
        // console.log('Соединение открыто');
        // setTimeout(() => {
        //   socket.send(`{"author": "${Date.now()}", "value": "Test"}`);
        // }, 3000);
        // setTimeout(() => {
        //   socket.send('hello test2');
        // }, 6000);
      };

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
    }),
    [lastMessage, sendMessage, status],
  );
};

export default useWebSocket;
