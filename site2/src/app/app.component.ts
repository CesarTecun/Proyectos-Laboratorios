import { Component, HostListener, ViewChild, ElementRef, OnInit, Inject, PLATFORM_ID, DestroyRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { BehaviorSubject, Observable, of, fromEvent } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

// Importar servicios y modelos a través del barrel file
import { MessageService } from './services';

interface UserInfo {
  name: string;
  email: string;
  initials: string;
}

interface Notification {
  id: number;
  title: string;
  message: string;
  read: boolean;
  date: Date;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, 
    RouterOutlet, 
    RouterLink, 
    RouterLinkActive
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  // Menu states
  isMenuOpen = false;
  isProfileMenuOpen = false;
  
  // User info
  userInfo: UserInfo = {
    name: 'Admin User',
    email: 'admin@example.com',
    initials: 'AU'
  };
  
  // Notifications
  notifications: Notification[] = [];
  unreadNotifications = 0;
  
  // Loading state
  isLoading = false;
  
  // Screen size tracking
  isMobileView = false;
  private isBrowser: boolean;

  constructor(
    private router: Router,
    private messageService: MessageService,
    @Inject(PLATFORM_ID) platformId: Object,
    private destroyRef: DestroyRef
  ) { 
    this.isBrowser = isPlatformBrowser(platformId);
    if (this.isBrowser) {
      this.checkScreenSize();
    }
  }

  ngOnInit(): void {
    // Load notifications
    this.loadNotifications();

    // Setup resize observer only in browser
    if (this.isBrowser) {
      fromEvent(window, 'resize')
        .pipe(
          takeUntilDestroyed(this.destroyRef)
        )
        .subscribe(() => this.checkScreenSize());
    }
  }

  // Load notifications
  private loadNotifications(): void {
    // Mock notifications since we removed the auth service
    const mockNotifications: Notification[] = [
      { id: 1, title: 'Bienvenido', message: 'Bienvenido a OrderPro', read: true, date: new Date() },
      { id: 2, title: 'Actualización', message: 'Nueva versión disponible', read: false, date: new Date() }
    ];
    
    this.notifications = mockNotifications;
    this.unreadNotifications = mockNotifications.filter(n => !n.read).length;
  }

  // Toggle mobile menu
  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
    if (this.isMenuOpen) {
      this.isProfileMenuOpen = false;
    }
  }

  // Toggle profile menu
  toggleProfileMenu(): void {
    this.isProfileMenuOpen = !this.isProfileMenuOpen;
    if (this.isProfileMenuOpen) {
      this.isMenuOpen = false;
    }
  }

  // Close all menus
  closeAllMenus(): void {
    this.isMenuOpen = false;
    this.isProfileMenuOpen = false;
  }

  // Navigation
  navigateTo(route: string, event?: Event): void {
    event?.preventDefault();
    this.router.navigate([route]);
    this.closeAllMenus();
  }

  // Handle window resize
  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    if (this.isBrowser) {
      this.checkScreenSize();
      if (!this.isMobileView) {
        this.closeAllMenus();
      }
    }
  }

  // Check screen size and update mobile view flag
  private checkScreenSize(): void {
    if (this.isBrowser) {
      this.isMobileView = window.innerWidth < 768;
    }
  }

  // Handle escape key
  @HostListener('window:keydown.escape')
  onEscapeKey(): void {
    this.closeAllMenus();
  }

  // Simple logout that just navigates to home
  logout(): void {
    this.isLoading = true;
    // Simulate API call
    setTimeout(() => {
      this.router.navigate(['/']);
      this.isLoading = false;
    }, 500);
  }
}
