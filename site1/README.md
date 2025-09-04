# Sistema de Gestión de Órdenes - Backend

Este es el backend de un sistema de gestión de órdenes desarrollado con .NET 8.0. Proporciona una API RESTful para gestionar órdenes, clientes y productos.

## Características

- Gestión completa de órdenes (CRUD)
- Validación de datos
- Documentación automática con Swagger
- Configuración CORS para desarrollo y producción
- Manejo de errores centralizado
- Patrón Repository y Servicios
- Mapeo de objetos con AutoMapper

## Tecnologías

- .NET 8.0
- Entity Framework Core
- Dapper para consultas SQL personalizadas
- AutoMapper
- Swagger/OpenAPI
- SQL Server

## Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2019 o superior
- Visual Studio 2022 o Visual Studio Code

## Configuración Inicial

1. Clona el repositorio
2. Configura la cadena de conexión en `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=tu_servidor;Database=TuBaseDeDatos;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "AllowedOrigins": ["http://localhost:4200"]
   }
   ```
3. Ejecuta las migraciones:
   ```bash
   dotnet ef database update
   ```

## Ejecutando la Aplicación

1. Ejecuta la aplicación:
   ```bash
   dotnet run
   ```
2. Abre tu navegador en `https://localhost:5001` (o `http://localhost:5000` para HTTP)
3. Accede a la documentación de la API en `/swagger`

## Documentación de la API

### 1. Estructura de Datos (Modelos)

#### Orden (`Order`)
- `Id`: Identificador único de la orden
- `PersonId`: ID de la persona que realiza el pedido
- `Number`: Número secuencial de la orden
- `CreatedAt`, `UpdatedAt`: Fechas de creación y actualización
- `OrderDetails`: Lista de ítems en la orden

#### Detalle de Orden (`OrderDetail`)
- `Id`: Identificador único del detalle
- `OrderId`: ID de la orden a la que pertenece
- `ItemId`: ID del ítem solicitado
- `Quantity`: Cantidad solicitada
- `Price`: Precio unitario en el momento de la compra
- `Total`: Total (Quantity * Price)

### 2. Patrón de Diseño: Repositorio

La implementación sigue el patrón Repositorio para separar la lógica que recupera los datos de la capa de negocio.

#### Operaciones Principales:
- `CreateAsync`: Crea una nueva orden con sus detalles
- `GetByIdAsync`: Obtiene una orden por su ID con todos sus detalles
- `GetAllAsync`: Obtiene todas las órdenes con sus detalles
- `UpdateAsync`: Actualiza una orden existente
- `DeleteAsync`: Elimina una orden y sus detalles
- `GetNextOrderNumberAsync`: Genera el siguiente número de orden secuencial

### 3. Capa de Servicio

La capa de servicio implementa la lógica de negocio:
- Validación de datos de entrada
- Verificación de existencia de entidades relacionadas
- Cálculo de totales
- Manejo de transacciones
- Transformación entre entidades y DTOs

### 4. Endpoints API

#### Órdenes
- `GET /api/orders` - Obtener todas las órdenes con sus detalles
- `GET /api/orders/{id}` - Obtener una orden específica por ID
- `POST /api/orders` - Crear una nueva orden
  ```json
  {
    "personId": 1,
    "createdBy": 1,
    "orderDetails": [
      {
        "itemId": 1,
        "quantity": 2,
        "price": 10.50
      }
    ]
  }
  ```
- `PUT /api/orders/{id}` - Actualizar una orden existente
- `DELETE /api/orders/{id}` - Eliminar una orden

### 5. Configuración Técnica

- **CORS**: Configurado para permitir solicitudes desde dominios específicos
- **AutoMapper**: Para mapeo automático entre entidades y DTOs
- **Swagger**: Documentación interactiva de la API disponible en `/swagger`
- **Logging**: Registro de errores y eventos importantes

### 6. Buenas Prácticas Implementadas

- **Separación de Responsabilidades**:
  - Controladores: Manejo de solicitudes HTTP
  - Servicios: Lógica de negocio
  - Repositorios: Acceso a datos

- **Manejo de Errores**:
  - Try-catch en controladores
  - Respuestas HTTP apropiadas
  - Mensajes de error descriptivos

- **Seguridad**:
  - Validación de modelos
  - Mapeo explícito de DTOs
  - Protección contra inyección SQL

## Estructura del Proyecto

```
site1/
├── Controllers/       # Controladores de la API
│   └── OrdersController.cs  # Endpoints para gestión de órdenes
├── Data/              # Contexto de base de datos
│   └── AppDbContext.cs
├── DTOs/              # Objetos de Transferencia de Datos
│   ├── OrderCreateDto.cs
│   ├── OrderReadDto.cs
│   ├── OrderDetailCreateDto.cs
│   └── OrderDetailReadDto.cs
├── Mappings/          # Perfiles de AutoMapper
│   └── MappingProfile.cs
├── Models/            # Modelos de dominio
│   ├── Order.cs
│   ├── OrderDetail.cs
│   ├── Item.cs
│   └── Person.cs
├── Repositories/      # Implementación del patrón Repositorio
│   ├── IOrderRepository.cs
│   └── OrderRepository.cs
└── Services/          # Lógica de negocio
    ├── IOrderService.cs
    └── OrderService.cs
```

## Flujo de una Petición Típica

1. El cliente realiza una petición HTTP a un endpoint (ej: `POST /api/orders`)
2. El controlador valida el modelo de entrada
3. El servicio aplica la lógica de negocio (validaciones, cálculos)
4. El repositorio interactúa con la base de datos
5. Los resultados se mapean a DTOs
6. Se devuelve la respuesta al cliente con el código de estado apropiado