export const streamAgentResponse = async (prompt: string, onUpdate: (text: string) => void) => {
  const response = await fetch('https://localhost:7001/api/agent/chat-stream', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ prompt })
  });

  const reader = response.body?.getReader();
  const decoder = new TextDecoder();

  while (true) {
    const { done, value } = await reader!.read();
    if (done) break;
    
    // MAF sends 'ResponseUpdate' objects
    const chunk = decoder.decode(value);
    onUpdate(chunk); 
  }
};