"use client"

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton,
} from "@/components/ui/sidebar"
import { useSession } from "@/contexts/SessionProvider"
import { Button } from "@/components/ui/button"
import { LogOut, Home, User, Settings } from "lucide-react"
import Link from "next/link"
import { Separator } from "@/components/ui/separator"

export function AppSidebar() {
  const { user, signOut } = useSession()

  return (
    <Sidebar>
      <SidebarHeader>
        <div className="p-4">
          <Link href="/dashboard" className="text-2xl font-bold text-foreground hover:text-primary transition-colors">
            Skeleton
          </Link>
        </div>
      </SidebarHeader>

      <div className="px-4">
        <Separator />
      </div>
      
      <SidebarContent>
        <SidebarGroup>
          <SidebarMenu className="py-4">

            <SidebarMenuItem>
              <SidebarMenuButton asChild>
                <a href="/dashboard" className="flex items-center gap-2">
                  <Home className="h-4 w-4" />
                  Dashboard
                </a>
              </SidebarMenuButton>
            </SidebarMenuItem>

          </SidebarMenu>
        </SidebarGroup>
      </SidebarContent>
        
      <div className="px-4">
        <Separator />
      </div>
        
      <SidebarFooter>
        <div className="p-4 space-y-3">
          {user && (
            <div className="text-sm text-gray-600 px-3">
              Hi, {user.firstName}
            </div>
          )}
          <Button 
            variant="destructive" 
            size="sm" 
            onClick={signOut}
            className="w-full justify-start gap-2"
          >
            <LogOut className="h-4 w-4" />
            Logout
          </Button>
        </div>
      </SidebarFooter>
    </Sidebar>
  )
}