import { useState } from 'react';
import type { KeyboardEvent } from 'react';

// 1. Define the shape of a Message
interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

function App() {
  const [input, setInput] = useState<string>('');
  
  // 2. Tell TypeScript this is an array of ChatMessage, not 'never'
  const [chat, setChat] = useState<ChatMessage[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  const sendMessage = async () => {
    if (!input.trim()) return;

    setLoading(true);
    const userMsg: ChatMessage = { role: 'user', content: input };
    
    // Update chat with user message
    setChat((prev) => [...prev, userMsg]);

    try {
      const response = await fetch('http://localhost:5160/api/chat/ask', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message: input })
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Server responded with ${response.status}: ${errorText}`);
      }

      const data = await response.json();
      // Use data.analysis as defined in your Results.Ok(...)
      const assistantMsg: ChatMessage = { 
        role: 'assistant', 
        content: data.output 
      };

      setChat((prev) => [...prev, assistantMsg]);
    } catch (error) {
      console.error("Failed to send message:", error);
    } finally {
      setLoading(false);
      setInput('');
    }
  };

  const handleKeyPress = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') sendMessage();
  };

  return (
    <div className="p-10 bg-slate-900 text-white min-h-screen">
      <h1 className="text-2xl mb-5">🏗️ AEC-Pulse AI Assistant</h1>
      
      <div className="space-y-4 mb-10 h-96 overflow-y-auto border-p-4 border-slate-700 bg-slate-800 p-4 rounded">
        {chat.map((m, i) => (
          <div key={i} className={m.role === 'user' ? 'text-blue-400' : 'text-green-400'}>
            <span className="font-bold">{m.role.toUpperCase()}: </span>
           <span className="whitespace-pre-wrap">{m.content}</span>
          </div>
        ))}
        {loading && <div className="text-gray-500 animate-pulse">Agent is thinking...</div>}
      </div>

      <div className="flex gap-2">
        <input 
          className="flex-1 p-2 bg-slate-700 rounded border border-slate-600 focus:outline-none focus:border-blue-500"
          value={input} 
          onChange={(e) => setInput(e.target.value)} 
          onKeyDown={handleKeyPress}
          placeholder="Ask about project margins or labor status..."
        />
        <button 
          onClick={sendMessage}
          disabled={loading}
          className="bg-blue-600 px-4 py-2 rounded disabled:bg-gray-600"
        >
          Send
        </button>
      </div>
    </div>
  );
}

export default App; // Ensure this export is at the bottom