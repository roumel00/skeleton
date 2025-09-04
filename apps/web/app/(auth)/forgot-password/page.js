'use client';

import { useState, useEffect } from 'react';
import { useSession } from '@/contexts/SessionProvider';
import { Button } from '@/components/ui/button';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import Link from 'next/link';

export default function ForgotPasswordPage() {
  const { forgotPassword, isAuthenticated } = useSession();
  const [email, setEmail] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [isSuccess, setIsSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');
    setMessage('');

    const result = await forgotPassword(email);
    
    if (result.success) {
      setIsSuccess(true);
      setMessage(result.message || 'If an account with that email exists, a password reset link has been sent.');
    } else {
      setError(result.error);
    }
    
    setIsLoading(false);
  };

  if (isAuthenticated) {
    return null; // Will redirect to dashboard
  }

  if (isSuccess) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
        <Card className="max-w-md w-full">
          <CardHeader className="text-center">
            <CardTitle className="text-3xl font-extrabold text-green-600">
              Check Your Email
            </CardTitle>
            <CardDescription>
              We&apos;ve sent password reset instructions to your email address.
            </CardDescription>
          </CardHeader>
          
          <CardContent className="space-y-6">
            {message && (
              <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-md text-sm">
                {message}
              </div>
            )}
            
            <div className="text-center space-y-4">
              <p className="text-sm text-gray-600">
                Didn&apos;t receive an email? Check your spam folder or try again.
              </p>
              
              <div className="space-y-2">
                <Button
                  onClick={() => {
                    setIsSuccess(false);
                    setMessage('');
                    setEmail('');
                  }}
                  variant="outline"
                  className="w-full"
                >
                  Try Again
                </Button>
                
                <Link href="/login" className="block">
                  <Button variant="ghost" className="w-full">
                    Back to Login
                  </Button>
                </Link>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <Card className="max-w-md w-full">
        <CardHeader className="text-center">
          <CardTitle className="text-3xl font-extrabold">
            Forgot your password?
          </CardTitle>
          <CardDescription>
            Enter your email address and we&apos;ll send you a link to reset your password.
          </CardDescription>
        </CardHeader>
        
        <CardContent>
          <form className="space-y-6" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md text-sm">
                {error}
              </div>
            )}
            
            <div className="space-y-2">
              <Label htmlFor="email">Email address</Label>
              <Input
                id="email"
                name="email"
                type="email"
                autoComplete="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Enter your email"
              />
            </div>

            <Button
              type="submit"
              disabled={isLoading}
              className="w-full"
            >
              {isLoading ? 'Sending...' : 'Send Reset Link'}
            </Button>
            
            <div className="text-center">
              <Link href="/login" className="text-sm font-medium text-indigo-600 hover:text-indigo-500">
                Back to Login
              </Link>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
