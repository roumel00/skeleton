import Navbar from "@/components/navbar";
import { Button } from "@/components/ui/button";
import Link from "next/link";

export default function Home() {
  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      
      <main className="container mx-auto px-4 py-16 max-w-6xl">
        <div className="text-center space-y-12">
          {/* Hero Section */}
          <div className="space-y-6">
            <div className="inline-flex items-center px-3 py-1 rounded-full bg-primary/10 text-primary text-sm font-medium">
              🚀 Production-Ready Template
            </div>
            <h1 className="text-5xl md:text-6xl font-bold tracking-tight text-foreground">
              Full-Stack Skeleton Project
            </h1>
            <p className="text-xl md:text-2xl text-muted-foreground max-w-3xl mx-auto leading-relaxed">
              A complete template with .NET 9 API backend and Next.js frontend, featuring built-in authentication and Google OAuth. 
              <span className="text-foreground font-medium"> Skip the boilerplate and start building features immediately.</span>
            </p>
            <div className="flex gap-4 justify-center flex-wrap">
              <Button asChild size="lg">
                <Link href="/login">Get Started</Link>
              </Button>
              <Button variant="outline" size="lg" asChild>
                <Link href="/signup">Try Demo</Link>
              </Button>
            </div>
          </div>

          {/* Features Grid */}
          <div className="grid md:grid-cols-3 gap-8 text-left">
            <div className="bg-card border rounded-xl p-6 space-y-4">
              <div className="w-12 h-12 bg-blue-500/10 rounded-lg flex items-center justify-center">
                <span className="text-2xl">🔐</span>
              </div>
              <h3 className="text-xl font-semibold text-foreground">Authentication System</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li>• JWT with HTTP-only cookies</li>
                <li>• User registration & login</li>
                <li>• Password reset via email</li>
                <li>• Google OAuth integration</li>
                <li>• Session management</li>
              </ul>
            </div>

            <div className="bg-card border rounded-xl p-6 space-y-4">
              <div className="w-12 h-12 bg-green-500/10 rounded-lg flex items-center justify-center">
                <span className="text-2xl">🎨</span>
              </div>
              <h3 className="text-xl font-semibold text-foreground">Modern Frontend</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li>• Next.js 15 with App Router</li>
                <li>• shadcn/ui components</li>
                <li>• Responsive design</li>
                <li>• Form validation (Zod)</li>
                <li>• Protected routes</li>
              </ul>
            </div>

            <div className="bg-card border rounded-xl p-6 space-y-4">
              <div className="w-12 h-12 bg-purple-500/10 rounded-lg flex items-center justify-center">
                <span className="text-2xl">⚡</span>
              </div>
              <h3 className="text-xl font-semibold text-foreground">Robust Backend</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li>• .NET 9 Web API</li>
                <li>• Entity Framework Core</li>
                <li>• PostgreSQL database</li>
                <li>• Docker containerization</li>
                <li>• Email service (Resend)</li>
              </ul>
            </div>
          </div>

          {/* Quick Start Section */}
          <div className="bg-card border rounded-xl p-8 text-left space-y-6">
            <div className="text-center">
              <h2 className="text-3xl font-semibold text-foreground mb-2">Quick Start</h2>
              <p className="text-muted-foreground">Get up and running in minutes with Docker</p>
            </div>
            
            <div className="grid md:grid-cols-2 gap-8">
              <div className="space-y-4">
                <h3 className="text-lg font-medium text-foreground">1. Clone & Setup</h3>
                <div className="bg-muted rounded-lg p-4 font-mono text-sm">
                  <p className="text-green-600"># Clone the repository</p>
                  <p className="text-muted-foreground">git clone your-repo.git</p>
                  <p className="text-muted-foreground">cd skeleton-project</p>
                  <br />
                  <p className="text-green-600"># Setup environment</p>
                  <p className="text-muted-foreground">cp .env.example .env</p>
                </div>
              </div>

              <div className="space-y-4">
                <h3 className="text-lg font-medium text-foreground">2. Start Services</h3>
                <div className="bg-muted rounded-lg p-4 font-mono text-sm">
                  <p className="text-green-600"># Start everything with Docker</p>
                  <p className="text-muted-foreground">docker compose up --build</p>
                  <br />
                  <p className="text-green-600"># Access your apps</p>
                  <p className="text-muted-foreground">Frontend: localhost:3000</p>
                  <p className="text-muted-foreground">API: localhost:3030</p>
                </div>
              </div>
            </div>
          </div>

          {/* Project Structure */}
          <div className="bg-card border rounded-xl p-8 text-left">
            <h2 className="text-2xl font-semibold text-foreground mb-6 text-center">Project Structure</h2>
            <div className="bg-muted rounded-lg p-6 font-mono text-sm max-w-2xl mx-auto">
              <p className="text-foreground">skeleton-project/</p>
              <p className="text-muted-foreground">├── apps/</p>
              <p className="text-muted-foreground">│   ├── Api/              <span className="text-blue-600"># .NET 9 Web API</span></p>
              <p className="text-muted-foreground">│   │   ├── Controllers/  <span className="text-green-600"># API endpoints</span></p>
              <p className="text-muted-foreground">│   │   ├── Services/     <span className="text-green-600"># Business logic</span></p>
              <p className="text-muted-foreground">│   │   └── Data/         <span className="text-green-600"># Database context</span></p>
              <p className="text-muted-foreground">│   └── web/              <span className="text-blue-600"># Next.js Frontend</span></p>
              <p className="text-muted-foreground">│       ├── app/          <span className="text-green-600"># App Router pages</span></p>
              <p className="text-muted-foreground">│       ├── components/   <span className="text-green-600"># UI components</span></p>
              <p className="text-muted-foreground">│       └── contexts/     <span className="text-green-600"># React contexts</span></p>
              <p className="text-muted-foreground">├── docker-compose.yml    <span className="text-purple-600"># Multi-container setup</span></p>
              <p className="text-muted-foreground">└── .env                  <span className="text-purple-600"># Environment variables</span></p>
            </div>
          </div>

          {/* Call to Action */}
          <div className="text-center space-y-4">
            <h2 className="text-2xl font-semibold text-foreground">Ready to Build Something Amazing?</h2>
            <p className="text-muted-foreground max-w-2xl mx-auto">
              This template includes everything you need to start your next project. Authentication, database, 
              modern UI components, and deployment-ready configuration.
            </p>
            <div className="flex gap-4 justify-center flex-wrap">
              <Button asChild>
                <Link href="/signup">Create Account</Link>
              </Button>
              <Button variant="outline" asChild>
                <Link href="/login">Sign In</Link>
              </Button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
