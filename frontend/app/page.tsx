'use client';

import { useState, useEffect } from 'react';

const API_URL = process.env.NEXT_PUBLIC_API_URL
    ? `${process.env.NEXT_PUBLIC_API_URL}/api/prompts`
    : 'http://localhost:5089/api/prompts';

interface PromptJob {
  id: string;
  prompt: string;
  status: 'Pending' | 'Processing' | 'Completed' | 'Failed';
  result: string | null;
  errorMessage: string | null;
  createdAt: string;
}

export default function Home() {
  const [prompt, setPrompt] = useState('');
  const [jobs, setJobs] = useState<PromptJob[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchJobs = async () => {
    const res = await fetch(API_URL);
    const data = await res.json();
    setJobs(data);
  };

  useEffect(() => {
    fetchJobs();
    const interval = setInterval(fetchJobs, 3000);
    return () => clearInterval(interval);
  }, []);

  const handleSubmit = async () => {
    if (!prompt.trim()) return;
    setLoading(true);
    await fetch(API_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ prompt }),
    });
    setPrompt('');
    setLoading(false);
    fetchJobs();
  };

  const statusColor = (status: string) => {
    switch (status) {
      case 'Completed': return 'text-green-600';
      case 'Failed': return 'text-red-600';
      case 'Processing': return 'text-yellow-600';
      default: return 'text-gray-500';
    }
  };

  return (
      <main className="max-w-4xl mx-auto p-8">
        <h1 className="text-3xl font-bold mb-8">Prompt Processor</h1>

        <div className="mb-8 flex gap-4">
        <textarea
            className="flex-1 border rounded p-3 resize-none"
            rows={3}
            placeholder="Enter your prompt..."
            value={prompt}
            onChange={(e) => setPrompt(e.target.value)}
        />
          <button
              className="bg-blue-600 text-white px-6 py-3 rounded hover:bg-blue-700 disabled:opacity-50"
              onClick={handleSubmit}
              disabled={loading}
          >
            {loading ? 'Sending...' : 'Send'}
          </button>
        </div>

        <table className="w-full border-collapse">
          <thead>
          <tr className="bg-gray-100">
            <th className="border p-3 text-left">Prompt</th>
            <th className="border p-3 text-left">Status</th>
            <th className="border p-3 text-left">Result</th>
            <th className="border p-3 text-left">Created</th>
          </tr>
          </thead>
          <tbody>
          {jobs.map((job) => (
              <tr key={job.id} className="hover:bg-gray-50">
                <td className="border p-3 max-w-xs truncate">{job.prompt}</td>
                <td className={`border p-3 font-medium ${statusColor(job.status)}`}>{job.status}</td>
                <td className="border p-3 max-w-sm">{job.result ?? job.errorMessage ?? '—'}</td>
                <td className="border p-3 text-sm text-gray-500">
                  {new Date(job.createdAt).toLocaleTimeString()}
                </td>
              </tr>
          ))}
          </tbody>
        </table>
      </main>
  );
}