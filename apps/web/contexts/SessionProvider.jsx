'use client';

import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { useRouter } from 'next/navigation';
import { Skeleton } from '@/components/ui/skeleton';

const SessionContext = createContext(undefined);

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:3030';

export default function SessionProvider({ children }) {
  const router = useRouter();
  const [user, setUser] = useState(null);
  const [status, setStatus] = useState('loading');

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const res = await fetch(API_BASE_URL + '/api/auth/me', { credentials: 'include', cache: 'no-store' });
        if (cancelled) return;
        if (res.ok) {
          const data = await res.json();
          const u = data?.user ?? null;
          setUser(u);
          setStatus(u ? 'authenticated' : 'unauthenticated');
        } else {
          setUser(null);
          setStatus('unauthenticated');
        }
      } catch {
        if (!cancelled) {
          setUser(null);
          setStatus('unauthenticated');
        }
      }
    })();
    return () => { cancelled = true; };
  }, []);

  const refresh = async () => {
    setStatus('loading');
    try {
      const res = await fetch(API_BASE_URL + '/api/auth/me', { credentials: 'include', cache: 'no-store' });
      if (res.ok) {
        const data = await res.json();
        const u = data?.user ?? null;
        setUser(u);
        setStatus(u ? 'authenticated' : 'unauthenticated');
      } else {
        setUser(null);
        setStatus('unauthenticated');
      }
    } catch {
      setUser(null);
      setStatus('unauthenticated');
    }
  };

  const signOut = async () => {
    try {
      await fetch(API_BASE_URL + '/api/auth/logout', { method: 'POST', credentials: 'include' });
    } finally {
      setUser(null);
      setStatus('unauthenticated');
      router.replace("/");
    }
  };

  const signIn = async (email, password) => {
    try {
      const res = await fetch(API_BASE_URL + '/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ email, password }),
      });
      
      if (res.ok) {
        const data = await res.json();
        setUser(data.user);
        setStatus('authenticated');
        router.replace('/dashboard');
        return { success: true };
      } else {
        const error = await res.json();
        return { success: false, error: error.message || 'Login failed' };
      }
    } catch (error) {
      return { success: false, error: 'Network error' };
    }
  };

  const signUp = async (email, password, firstName, lastName) => {
    try {
      const res = await fetch(API_BASE_URL + '/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ 
          email, 
          firstName, 
          lastName, 
          password, 
          confirmPassword: password 
        }),
      });
      
      if (res.ok) {
        const data = await res.json();
        setUser(data.user);
        setStatus('authenticated');
        router.replace('/dashboard');
        return { success: true };
      } else {
        const error = await res.json();
        return { success: false, error: error.message || 'Registration failed' };
      }
    } catch (error) {
      return { success: false, error: 'Network error' };
    }
  };

  const value = useMemo(() => ({
    user,
    status,
    isAuthenticated: status === 'authenticated',
    refresh,
    signOut,
    signIn,
    signUp,
  }), [user, status, refresh, signOut, signIn, signUp]);

  if (status === 'loading') {
    return (
      <div className="min-h-screen grid place-items-center">
        <div className="space-y-4 w-full max-w-md">
          <Skeleton className="h-12 w-full" />
          <Skeleton className="h-4 w-3/4" />
          <Skeleton className="h-4 w-1/2" />
        </div>
      </div>
    );
  }

  return <SessionContext.Provider value={value}>{children}</SessionContext.Provider>;
}

export function useSession() {
  const ctx = useContext(SessionContext);
  if (!ctx) throw new Error('useSession must be used within a SessionProvider');
  return ctx;
}
