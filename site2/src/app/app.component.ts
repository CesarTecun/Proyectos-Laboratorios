import { Component, HostListener, OnInit, Inject, PLATFORM_ID, DestroyRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { fromEvent, debounceTime } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

/**
 * Servicios de la aplicación
 */
import { MessageService } from './services';

/**
 * Interfaz para la información del usuario
 */
interface UserInfo {
  name: string;        // Nombre completo del usuario
  initials: string;    // Iniciales del usuario para el avatar
}

/**
 * Interfaz para las notificaciones
 */
interface Notification {
  id: number;         // Identificador único
  title: string;      // Título de la notificación
  message: string;    // Contenido del mensaje
  read: boolean;      // Estado de lectura
  date: Date;         // Fecha de la notificación
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
  
  // Información del usuario
  userInfo: UserInfo = {
    name: 'Admin User',
    initials: 'AU'
  };
  
  // Notifications
  notifications: Notification[] = [];
  unreadNotifications = 0;
  
  // Loading state
  isLoading = false;
  
  // Control de tamaño de pantalla
  isMobileView = false;
  private readonly isBrowser: boolean;

  /**
   * Constructor del componente
   * @param router Servicio de enrutamiento
   * @param messageService Servicio de mensajes
   * @param platformId Identificador de la plataforma
   * @param destroyRef Referencia para la destrucción del componente
   */
  constructor(
    private readonly router: Router,
    private readonly messageService: MessageService,
    @Inject(PLATFORM_ID) platformId: Object,
    private readonly destroyRef: DestroyRef
  ) { 
    this.isBrowser = isPlatformBrowser(platformId);
  }

  /**
   * Inicialización del componente
   */
  ngOnInit(): void {
    this.cargarNotificaciones();
    this.configurarObservableRedimension();
  }

  /**
   * Configura el observable para detectar cambios en el tamaño de la ventana
   * con un debounce para mejorar el rendimiento
   */
  private configurarObservableRedimension(): void {
    if (!this.isBrowser) return;

    fromEvent(window, 'resize')
      .pipe(
        debounceTime(100), // Espera 100ms después del último evento
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.actualizarVistaMovil());
    
    // Verificar tamaño inicial
    this.actualizarVistaMovil();
  }

  /**
   * Carga las notificaciones del usuario
   * TODO: Reemplazar con llamada real al servicio
   */
  private cargarNotificaciones(): void {
    // Datos de ejemplo - Reemplazar con llamada al servicio
    const notificacionesEjemplo: Notification[] = [
      { 
        id: 1, 
        title: 'Bienvenido', 
        message: 'Bienvenido a OrderPro', 
        read: true, 
        date: new Date() 
      },
      { 
        id: 2, 
        title: 'Actualización', 
        message: 'Nueva versión disponible', 
        read: false, 
        date: new Date() 
      }
    ];
    
    this.notifications = notificacionesEjemplo;
    this.actualizarContadorNoLeidos();
  }

  /**
   * Actualiza el contador de notificaciones no leídas
   */
  private actualizarContadorNoLeidos(): void {
    this.unreadNotifications = this.notifications.filter(n => !n.read).length;
  }

  /**
   * Alterna el menú principal
   */
  alternarMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
    if (this.isMenuOpen) {
      this.isProfileMenuOpen = false;
    }
  }

  /**
   * Alterna el menú de perfil
   */
  alternarMenuPerfil(): void {
    this.isProfileMenuOpen = !this.isProfileMenuOpen;
    if (this.isProfileMenuOpen) {
      this.isMenuOpen = false;
    }
  }

  /**
   * Cierra todos los menús desplegables
   */
  private cerrarTodosLosMenus(): void {
    this.isMenuOpen = false;
    this.isProfileMenuOpen = false;
  }

  /**
   * Navega a una ruta específica
   * @param ruta Ruta de destino
   * @param evento Evento del DOM (opcional)
   */
  navegarA(ruta: string, evento?: Event): void {
    evento?.preventDefault();
    this.router.navigate([ruta])
      .catch(error => {
        console.error('Error al navegar:', error);
        this.messageService.showError('No se pudo cargar la página solicitada');
      });
    this.cerrarTodosLosMenus();
  }

  /**
   * Manejador del evento de redimensión de ventana
   * @param event Evento de redimensión
   */
  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    // La lógica de redimensión ahora está en configurarObservableRedimension
  }

  /**
   * Actualiza el estado de la vista móvil basado en el ancho de la ventana
   */
  private actualizarVistaMovil(): void {
    if (!this.isBrowser) return;
    
    const nuevoEstado = window.innerWidth < 768;
    
    // Solo actualizar si cambió el estado
    if (this.isMobileView !== nuevoEstado) {
      this.isMobileView = nuevoEstado;
      
      // Cerrar menús al cambiar a escritorio
      if (!this.isMobileView) {
        this.cerrarTodosLosMenus();
      }
    }
  }

  /**
   * Maneja la tecla Escape para cerrar menús
   */
  @HostListener('window:keydown.escape')
  onTeclaEscape(): void {
    this.cerrarTodosLosMenus();
  }

  /**
   * Cierra la sesión del usuario
   * TODO: Implementar lógica real de cierre de sesión
   */
  cerrarSesion(): void {
    this.isLoading = true;
    
    // Simular llamada a la API
    // TODO: Reemplazar con llamada real al servicio de autenticación
    setTimeout(() => {
      this.router.navigate(['/auth/login'])
        .finally(() => {
          this.isLoading = false;
        });
    }, 500);
  }
}
