export interface Message {
  id: string;
  author: string;
  value: string;
  type: 'connection' | 'message' | 'action';
  action?: string
}
