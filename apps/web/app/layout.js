import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import SessionProvider from "@/contexts/SessionProvider";
import { Toaster } from "@/components/ui/sonner";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata = {
  title: "Full-Stack Skeleton Project Template",
  description: "Production-ready template for building full-stack applications with .NET 9 API backend and Next.js frontend, featuring built-in authentication and Google OAuth. Skip the boilerplate and start building features immediately.",
  keywords: ["Next.js", ".NET", "template", "authentication", "full-stack", "skeleton", "boilerplate"],
  authors: [{ name: "Skeleton Project" }],
  creator: "Skeleton Project Template",
  publisher: "Skeleton Project",
  openGraph: {
    title: "Full-Stack Skeleton Project Template",
    description: "Production-ready template with .NET 9 API backend and Next.js frontend, featuring built-in authentication and Google OAuth.",
    type: "website",
  },
  twitter: {
    card: "summary_large_image",
    title: "Full-Stack Skeleton Project Template",
    description: "Production-ready template with .NET 9 API backend and Next.js frontend, featuring built-in authentication and Google OAuth.",
  },
};

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <SessionProvider>
          <main>{children}</main>
          <Toaster />
        </SessionProvider>
      </body>
    </html>
  );
}
