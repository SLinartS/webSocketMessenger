import { useEffect, useState } from 'react';
import useWebSocket from '../hooks/useWebSocket';
import { Message } from '../types/Message';

const Messager = () => {
  const [messageList, setMessageList] = useState<Message[]>();
  const [message, setMessage] = useState<string>('');
  const [userName, setUserName] = useState<string>('defaultUserName');

  const { lastMessage, sendMessage, status } = useWebSocket();

  function sendSocketMessage() {
    console.log(message);
    if (status === 'open') {
      const data: Message = {
        author: userName,
        value: message,
      };

      sendMessage(JSON.stringify(data));
    }
  }

  useEffect(() => {
    setUserName(Date.now().toString());
  }, []);

  useEffect(() => {
    try {
      const allMessages = JSON.parse(lastMessage);
      setMessageList(allMessages);
    } catch (error) {}
  }, [lastMessage]);

  return (
    <div>
      <h1>Messager</h1>
      <div>
        {messageList?.map((messageItem) => (
          <div key={messageItem.value + messageItem.author + Date.now()}>
            <span>{messageItem.author}</span>
            <span>{messageItem.value}</span>
          </div>
        ))}
      </div>
      <textarea
        name="message-field"
        id=""
        cols={30}
        rows={10}
        value={message}
        onChange={(e) => setMessage(e.target.value)}
      />
      <button type="button" onClick={sendSocketMessage}>
        Отправить
      </button>
    </div>
  );
};

export default Messager;
