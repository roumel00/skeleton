'use client';

import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { useSession } from '@/contexts/SessionProvider';
import { Skeleton } from '@/components/ui/skeleton';

export default function Navbar({}) {
  const { status, signOut } = useSession();

  return (
    <header className="border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="w-full px-4 py-4 flex items-center justify-between">
        <div className="flex items-center">
          <Link href="/" className="text-xl font-bold text-foreground">
            Skeleton
          </Link>
        </div>
        
        <div className="flex items-center gap-3">
          {status === 'loading' ? (
            <>
              <Skeleton className="h-9 w-16" />
              <Skeleton className="h-9 w-20" />
            </>
          ) : status === 'authenticated' ? (
            <>
              <Button asChild variant="ghost" size="sm" className="text-sm font-medium">
                <Link href="/dashboard">Dashboard</Link>
              </Button>
              <Button 
                variant="outline" 
                size="sm" 
                className="text-sm font-medium"
                onClick={signOut}
              >
                Sign Out
              </Button>
            </>
          ) : (
            <>
            <Button asChild variant="ghost" size="sm" className="text-sm font-medium">
              <Link href="/login">Login</Link>
            </Button>
            <Button asChild variant="default" size="sm" className="text-sm font-medium">
              <Link href="/signup">Signup</Link>
            </Button>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
