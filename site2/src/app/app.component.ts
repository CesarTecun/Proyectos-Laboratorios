import { Component, OnInit, HostListener, ViewChild, ElementRef, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { filter, map, tap } from 'rxjs/operators';

// Importar servicios y modelos a través del barrel file
import { MessageService, AuthService } from './services';

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
  title = 'OrderPro - Sistema de Gestión';
  
  // Estado del menú
  isUserMenuOpen = false;
  isMobileMenuOpen = false;
  isNotificationsOpen = false;
  
  // Datos del usuario
  userName$: BehaviorSubject<string> = new BehaviorSubject<string>('Usuario');
  userEmail$: BehaviorSubject<string> = new BehaviorSubject<string>('usuario@ejemplo.com');
  userInitials$: Observable<string>;
  
  // Notificaciones
  unreadNotifications = 3; // Ejemplo: reemplazar con datos reales
  
  // Estado de carga
  isLoading = false;
  
  // Referencias a elementos del DOM
  @ViewChild('profileMenu') profileMenu!: ElementRef<HTMLElement>;

  constructor(
    private router: Router,
    @Inject(AuthService) private authService: AuthService,
    private messageService: MessageService
  ) {
    // Cerrar menús al navegar (para móviles)
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.closeAllMenus();
      window.scrollTo(0, 0);
    });
    
    // Iniciales del usuario para el avatar
    this.userInitials$ = this.userName$.pipe(
      map(name => {
        if (!name) return 'U';
        const parts = name.split(' ');
        if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
        return `${parts[0].charAt(0)}${parts[parts.length - 1].charAt(0)}`.toUpperCase();
      })
    );
  }

  ngOnInit(): void {
    this.loadUserData();
    this.checkNotifications();
  }
  
  private loadUserData(): void {
    // Cargar datos del usuario autenticado
    const user = this.authService.getCurrentUser();
    if (user) {
      this.userName$.next(user.name || 'Usuario');
      this.userEmail$.next(user.email || 'usuario@ejemplo.com');
    } else {
      // Si no hay usuario, redirigir al login
      this.router.navigate(['/login']);
    }
  }
  
  private checkNotifications(): void {
    // Aquí iría la lógica para verificar notificaciones
    // Ejemplo:
    // this.notificationService.getUnreadCount().subscribe(count => {
    //   this.unreadNotifications = count;
    // });
  }
  
  // Manejo de menús
  toggleMobileMenu(event?: Event): void {
    event?.preventDefault();
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    
    // Cerrar otros menús si es necesario
    if (this.isMobileMenuOpen) {
      this.isUserMenuOpen = false;
      this.isNotificationsOpen = false;
    }
    
    // Bloquear el scroll cuando el menú móvil está abierto
    document.body.style.overflow = this.isMobileMenuOpen ? 'hidden' : '';
  }
  
  toggleUserMenu(event?: Event): void {
    event?.stopPropagation();
    this.isUserMenuOpen = !this.isUserMenuOpen;
    if (this.isUserMenuOpen) {
      this.isNotificationsOpen = false;
    } 
    this.isMobileMenuOpen = false;
  }
  
  toggleNotifications(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.isNotificationsOpen = !this.isNotificationsOpen;
    
    if (this.isNotificationsOpen) {
      this.isUserMenuOpen = false;
      this.isMobileMenuOpen = false;
      // Aquí podrías marcar notificaciones como leídas
      // this.notificationService.markAsRead();
      this.unreadNotifications = 0;
    }
  }
  
  closeAllMenus(): void {
    this.isMobileMenuOpen = false;
    this.isUserMenuOpen = false;
    this.isNotificationsOpen = false;
    document.body.style.overflow = '';
  }
  
  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
    document.body.style.overflow = '';
  }
  
  // Cerrar menús al hacer clic fuera
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    // Close menus if clicked outside
    if (!this.profileMenu?.nativeElement) return;
    
    const target = event.target as Node;
    const profileMenuElement = this.profileMenu.nativeElement;
    const isClickInside = profileMenuElement.contains(target);
    
    if (!isClickInside) {
      this.closeAllMenus();
    }
  }
  
  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    // Cerrar menú móvil si la pantalla es grande
    if (window.innerWidth >= 768) {
      this.closeAllMenus();
    }
  }
  
  @HostListener('window:keydown.escape')
  onEscapeKey(): void {
    this.closeAllMenus();
  }
  
  // Navegación
  navigateTo(route: string, event?: Event): void {
    event?.preventDefault();
    this.router.navigate([route]);
    this.closeAllMenus();
  }
  
  // Autenticación
  logout(): void {
    this.isLoading = true;
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']);
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Error al cerrar sesión:', error);
        // Mostrar mensaje de error genérico si el servicio de mensajes no está disponible
        alert('Error al cerrar sesión. Intente nuevamente.');
        this.isLoading = false;
      }
    });
  }
}
