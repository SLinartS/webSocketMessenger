import { useEffect, useState } from 'react';
import useWebSocket from '../hooks/useWebSocket';
import { Message } from '../types/Message';
import './styles.scss';

const Messenger = () => {
  const [messageList, setMessageList] = useState<Message[]>();
  const [message, setMessage] = useState<string>('');
  const [isModalWindowView, setIsModalWindowView] = useState<boolean>(true);
  const [userName, setUserName] = useState<string>('');

  const { lastMessage, sendMessage, status } = useWebSocket();

  function sendSocketMessage() {
    if (status === 'open') {
      const data: Message = {
        id: '',
        author: userName,
        value: message,
        type: 'message',
      };

      sendMessage(JSON.stringify(data));
    }
  }

  function clearChat() {
    if (status === 'open') {
      const data: Message = {
        id: '',
        author: null,
        value: null,
        type: 'action',
        action: 'clearChat',
      };

      sendMessage(JSON.stringify(data));
    }
  }

  function EnterToChat() {
    if (userName.length > 3) {
      setIsModalWindowView(false);
    }
  }

  useEffect(() => {
    try {
      setMessageList(lastMessage);
    } catch (error) {}
  }, [lastMessage]);

  return (
    <div className="messenger">
      <h1>Messenger</h1>
      <div className="messenger__text-area-segment">
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
        <button type="button" onClick={clearChat}>
          Очистить
        </button>
      </div>
      <div>
        {messageList?.map((messageItem) => (
          <div key={messageItem.id}>
            <p className="message">
              {' '}
              <span>{messageItem.author}</span>
              :&nbsp;
              <span>{messageItem.value}</span>
            </p>
          </div>
        ))}
      </div>
      <div
        className={`modalWindow ${!isModalWindowView && 'modalWindow--closed'}`}
      >
        {' '}
        <div className="modalWindow__content">
          <label htmlFor="userName">Введи имя пользователя</label>
          <input
            type="text"
            name="userName"
            id="userName"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
          />
          <button type="button" onClick={EnterToChat}>
            Начать
          </button>
        </div>
        <div className="modalWindow__background"></div>
      </div>
    </div>
  );
};

export default Messenger;
