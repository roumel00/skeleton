import Navbar from "@/components/navbar";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import Link from "next/link";

const Indentation = (count) => <>{Array.from({ length: count }, (_, i) => <span key={i}>&nbsp;</span>)}</>;

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
              <Button variant="outline" size="lg" asChild>
                <Link href="/signup">Try Demo</Link>
              </Button>
            </div>
          </div>

          {/* Features Grid */}
          <div className="grid md:grid-cols-3 gap-8 text-left">
            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-blue-500/10 rounded-lg flex items-center justify-center mb-2">
                  <span className="text-2xl">🔐</span>
                </div>
                <CardTitle className="text-xl">Authentication System</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2 text-sm text-muted-foreground">
                  <li>• JWT with HTTP-only cookies</li>
                  <li>• User registration & login</li>
                  <li>• Password reset via email</li>
                  <li>• Google OAuth integration</li>
                  <li>• Session management</li>
                </ul>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-green-500/10 rounded-lg flex items-center justify-center mb-2">
                  <span className="text-2xl">🎨</span>
                </div>
                <CardTitle className="text-xl">Modern Frontend</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2 text-sm text-muted-foreground">
                  <li>• Next.js 15 with App Router</li>
                  <li>• shadcn/ui components</li>
                  <li>• Responsive design</li>
                  <li>• Protected routes</li>
                </ul>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-purple-500/10 rounded-lg flex items-center justify-center mb-2">
                  <span className="text-2xl">⚡</span>
                </div>
                <CardTitle className="text-xl">Robust Backend</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2 text-sm text-muted-foreground">
                  <li>• .NET 9 Web API</li>
                  <li>• Entity Framework Core</li>
                  <li>• PostgreSQL database</li>
                  <li>• Docker containerization</li>
                  <li>• Email service (Resend)</li>
                </ul>
              </CardContent>
            </Card>
          </div>

          {/* Quick Start Section */}
          <Card>
            <CardHeader className="text-center">
              <CardTitle className="text-3xl">Quick Start</CardTitle>
              <p className="text-muted-foreground">Get up and running in minutes with Docker</p>
            </CardHeader>
            <CardContent>
              <div className="grid md:grid-cols-2 gap-8">
                <div className="space-y-4">
                  <h3 className="text-lg font-medium text-foreground">1. Clone & Setup</h3>
                  <div className="bg-muted rounded-lg p-4 font-mono text-sm">
                    <p className="text-green-600"># Clone the repository</p>
                    <p className="text-muted-foreground">git clone https://github.com/roumel00/skeleton.git</p>
                    <p className="text-muted-foreground">cd skeleton-project</p>
                    <br />
                    <p className="text-green-600"># Setup environment</p>
                    <p className="text-muted-foreground">cp .env.example .env</p>
                    <p className="text-green-600"># Run the fly secrets set for all the variables</p>
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
            </CardContent>
          </Card>

          {/* Project Structure */}
          <Card>
            <CardHeader className="text-center">
              <CardTitle className="text-2xl">Project Structure</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="bg-muted rounded-lg p-6 font-mono text-sm max-w-2xl mx-auto text-left">
                <p className="text-foreground">skeleton-project/</p>
                <p className="text-muted-foreground">├── apps/</p>
                <p className="text-muted-foreground">│{Indentation(4)}├── Api/{Indentation(23)}<span className="text-blue-600"># .NET 9 Web API</span></p>
                <p className="text-muted-foreground">│{Indentation(4)}│{Indentation(4)}├── Controllers/{Indentation(10)}<span className="text-green-600"># API endpoints</span></p>
                <p className="text-muted-foreground">│{Indentation(4)}│{Indentation(4)}├── Data/{Indentation(17)}<span className="text-green-600"># Database context</span></p>
                <p className="text-muted-foreground">│{Indentation(4)}│{Indentation(4)}├── Entities/{Indentation(13)}<span className="text-green-600"># Database entities</span></p>
                <p className="text-muted-foreground">│{Indentation(4)}│{Indentation(4)}├── Migrations/{Indentation(11)}<span className="text-green-600"># Database migrations</span></p>
                <p className="text-muted-foreground">│{Indentation(4)}│{Indentation(4)}├── Models/{Indentation(15)}<span className="text-green-600"># DTOs and request/response models</span></p>
                <p className="text-muted-foreground">│{Indentation(4)}│{Indentation(4)}├── Services/{Indentation(13)}<span className="text-green-600"># Business logic</span></p>
                <p className="text-muted-foreground">│{Indentation(4)}│{Indentation(4)}└── Program.cs{Indentation(12)}<span className="text-green-600"># Application configuration</span></p>
                <p className="text-muted-foreground">│{Indentation(4)}└── web/{Indentation(23)}<span className="text-blue-600"># Next.js Frontend</span></p>
                <p className="text-muted-foreground">│{Indentation(10)}├── app/{Indentation(18)}<span className="text-green-600"># App Router pages</span></p>
                <p className="text-muted-foreground">│{Indentation(10)}├── components/{Indentation(11)}<span className="text-green-600"># UI components</span></p>
                <p className="text-muted-foreground">│{Indentation(10)}└── contexts/{Indentation(13)}<span className="text-green-600"># React contexts</span></p>
                <p className="text-muted-foreground">├── docker-compose.yml{Indentation(14)}<span className="text-purple-600"># Multi-container setup</span></p>
                <p className="text-muted-foreground">└── .env{Indentation(28)}<span className="text-purple-600"># Environment variables</span></p>
              </div>
            </CardContent>
          </Card>

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
