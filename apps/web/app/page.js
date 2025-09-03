import Navbar from "@/components/navbar";

export default function Home() {
  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      
      <main className="container mx-auto px-4 py-16 max-w-4xl">
        <div className="text-center space-y-8">
          <div className="space-y-4">
            <h1 className="text-4xl font-bold tracking-tight text-foreground">
              Skeleton Project
            </h1>
            <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
              A full-stack application template with Next.js frontend and .NET API backend
            </p>
          </div>

          <div className="bg-card border rounded-lg p-8 text-left space-y-6">
            <div>
              <h2 className="text-2xl font-semibold text-foreground mb-4">What's Included</h2>
              <div className="grid md:grid-cols-2 gap-6">
                <div className="space-y-3">
                  <h3 className="text-lg font-medium text-foreground">Frontend (Next.js)</h3>
                  <ul className="space-y-2 text-sm text-muted-foreground">
                    <li>• Modern React with Next.js 15</li>
                    <li>• ShadCN UI via Tailwind CSS for styling</li>
                    <li>• Authentication context</li>
                    <li>• Protected routes</li>
                    <li>• Responsive UI components</li>
                  </ul>
                </div>
                <div className="space-y-3">
                  <h3 className="text-lg font-medium text-foreground">Backend (.NET)</h3>
                  <ul className="space-y-2 text-sm text-muted-foreground">
                    <li>• .NET 9 Web API</li>
                    <li>• Entity Framework Core</li>
                    <li>• JWT Authentication</li>
                    <li>• User management</li>
                    <li>• Docker support</li>
                  </ul>
                </div>
              </div>
            </div>

            <div>
              <h2 className="text-2xl font-semibold text-foreground mb-4">Getting Started</h2>
              <div className="bg-muted rounded-md p-4 font-mono text-sm">
                <p className="text-foreground"># Start the backend</p>
                <p className="text-muted-foreground">cd apps/Api && dotnet run</p>
                <br />
                <p className="text-foreground"># Start the frontend</p>
                <p className="text-muted-foreground">cd apps/web && npm run dev</p>
              </div>
            </div>

            <div>
              <h2 className="text-2xl font-semibold text-foreground mb-4">Project Structure</h2>
              <div className="bg-muted rounded-md p-4 font-mono text-sm">
                <p className="text-foreground">skeleton-project/</p>
                <p className="text-muted-foreground">├── apps/</p>
                <p className="text-muted-foreground">│   ├── Api/          # .NET Backend</p>
                <p className="text-muted-foreground">│   └── web/          # Next.js Frontend</p>
                <p className="text-muted-foreground">└── docker-compose.yml</p>
              </div>
            </div>
          </div>

          <div className="text-center">
            <p className="text-sm text-muted-foreground">
              Ready to build something amazing? Start by exploring the code and customizing it for your needs.
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}
