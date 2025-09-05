export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  // Agrega más propiedades según sea necesario
}

export interface LoginCredentials {
  email: string;
  password: string;
  rememberMe?: boolean;
}
