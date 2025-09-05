using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MessageApi.Data;
using MessageApi.Models;

namespace MessageApi.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad Order.
    /// Maneja las operaciones de base de datos para las órdenes del sistema,
    /// incluyendo la gestión de transacciones para mantener la integridad de los datos.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private bool _disposed = false;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de órdenes.
        /// </summary>
        /// <param name="context">Contexto de base de datos de Entity Framework</param>
        /// <param name="configuration">Configuración de la aplicación para acceder a la cadena de conexión</param>
        public OrderRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Crea y devuelve una nueva conexión a la base de datos.
        /// </summary>
        /// <returns>Nueva instancia de SqlConnection configurada con la cadena de conexión</returns>
        private SqlConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'");
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Crea una nueva orden en la base de datos de forma asíncrona.
        /// Incluye la creación de la orden principal y sus detalles en una transacción.
        /// </summary>
        /// <param name="order">Objeto Order con los datos de la orden a crear</param>
        /// <returns>La orden creada con su ID generado</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el parámetro order es nulo</exception>
        public async Task<Order> CreateAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            
            if (order.Number == 0)
            {
                order.Number = await GetNextOrderNumberAsync();
            }

            using var connection = CreateConnection();
            await connection.OpenAsync();
            
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            
            try
            {
                const string orderSql = @"
                    INSERT INTO Orders (Number, PersonId, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                    OUTPUT INSERTED.Id
                    VALUES (@Number, @PersonId, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                // Ejecutar la inserción de la orden y obtener el ID generado
                var orderId = await connection.ExecuteScalarAsync<int>(orderSql, order, transaction: transaction);
                order.Id = orderId;

                // Insertar los detalles de la orden
                foreach (var detail in order.OrderDetails)
                {
                    detail.OrderId = orderId;
                    detail.CreatedAt = DateTime.UtcNow;
                    detail.UpdatedAt = DateTime.UtcNow;
                    detail.Total = detail.Quantity * detail.Price;
                    
                    const string detailSql = @"
                        INSERT INTO OrderDetails (OrderId, ItemId, Quantity, Price, Total, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES (@OrderId, @ItemId, @Quantity, @Price, @Total, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                    await connection.ExecuteAsync(detailSql, detail, transaction: transaction);
                }

                await transaction.CommitAsync();
                return order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al crear la orden en la base de datos", ex);
            }
        }

        /// <summary>
        /// Obtiene una orden por su ID de forma asíncrona, incluyendo la información
        /// de la persona asociada y los detalles de la orden con sus productos.
        /// </summary>
        /// <param name="id">ID de la orden a buscar</param>
        /// <returns>La orden encontrada o null si no existe</returns>
        public async Task<Order?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            
            const string orderSql = @"
                SELECT o.*, p.FirstName + ' ' + p.LastName as PersonName
                FROM Orders o
                INNER JOIN Persons p ON o.PersonId = p.Id
                WHERE o.Id = @Id;";
                
            const string detailsSql = @"
                SELECT od.*, i.Name as ItemName
                FROM OrderDetails od
                INNER JOIN Items i ON od.ItemId = i.Id
                WHERE od.OrderId = @OrderId;";

            try
            {
                var order = await connection.QueryFirstOrDefaultAsync<Order>(orderSql, new { Id = id });
                
                if (order != null)
                {
                    var details = await connection.QueryAsync<OrderDetail>(detailsSql, new { OrderId = id });
                    order.OrderDetails = details.ToList();
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la orden de la base de datos", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las órdenes de forma asíncrona, incluyendo la información
        /// de la persona asociada y los detalles de la orden con sus productos.
        /// </summary>
        /// <returns>Lista de órdenes encontradas</returns>
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            
            const string orderSql = @"
                SELECT o.*, p.FirstName + ' ' + p.LastName as PersonName
                FROM Orders o
                INNER JOIN Persons p ON o.PersonId = p.Id
                ORDER BY o.CreatedAt DESC;";
                
            const string detailsSql = @"
                SELECT od.*, i.Name as ItemName
                FROM OrderDetails od
                INNER JOIN Items i ON od.ItemId = i.Id
                WHERE od.OrderId IN (SELECT Id FROM Orders);";

            try
            {
                var orders = (await connection.QueryAsync<Order>(orderSql)).ToList();
                
                if (orders.Any())
                {
                    var allDetails = await connection.QueryAsync<OrderDetail>(detailsSql);
                    var detailsLookup = allDetails.ToLookup(d => d.OrderId);
                    
                    foreach (var order in orders)
                    {
                        order.OrderDetails = detailsLookup[order.Id].ToList();
                    }
                }

                return orders;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las órdenes de la base de datos", ex);
            }
        }

        /// <summary>
        /// Actualiza una orden existente en la base de datos de forma asíncrona.
        /// Incluye la actualización de la orden principal y el reemplazo de sus detalles.
        /// </summary>
        /// <param name="order">Objeto Order con los datos actualizados</param>
        /// <returns>True si la actualización fue exitosa, false si la orden no existe</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el parámetro order es nulo</exception>
        public async Task<bool> UpdateAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            order.UpdatedAt = DateTime.UtcNow;
            
            using var connection = CreateConnection();
            await connection.OpenAsync();
            
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            
            try
            {
                const string orderSql = @"
                    UPDATE Orders 
                    SET PersonId = @PersonId, 
                        UpdatedBy = @UpdatedBy, 
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
                    
                const string deleteDetailsSql = "DELETE FROM OrderDetails WHERE OrderId = @OrderId";

                // Actualizar la orden
                var affectedRows = await connection.ExecuteAsync(orderSql, order, transaction: transaction);
                if (affectedRows == 0) 
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Eliminar detalles existentes
                await connection.ExecuteAsync(deleteDetailsSql, new { OrderId = order.Id }, transaction: transaction);

                // Insertar los nuevos detalles
                foreach (var detail in order.OrderDetails)
                {
                    detail.OrderId = order.Id;
                    detail.UpdatedAt = DateTime.UtcNow;
                    detail.Total = detail.Quantity * detail.Price;
                    
                    const string insertDetailSql = @"
                        INSERT INTO OrderDetails (OrderId, ItemId, Quantity, Price, Total, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES (@OrderId, @ItemId, @Quantity, @Price, @Total, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                    await connection.ExecuteAsync(insertDetailSql, detail, transaction: transaction);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al actualizar la orden en la base de datos", ex);
            }
        }

        /// <summary>
        /// Elimina una orden por su ID de forma asíncrona.
        /// Nota: La eliminación en cascada de los detalles de la orden está configurada en la base de datos.
        /// </summary>
        /// <param name="id">ID de la orden a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false si la orden no existe</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            
            try
            {
                // Eliminar detalles primero por restricciones de clave foránea
                const string deleteDetailsSql = "DELETE FROM OrderDetails WHERE OrderId = @OrderId";
                await connection.ExecuteAsync(deleteDetailsSql, new { OrderId = id }, transaction: transaction);
                
                // Luego eliminar la orden
                const string deleteOrderSql = "DELETE FROM Orders WHERE Id = @Id";
                var affectedRows = await connection.ExecuteAsync(deleteOrderSql, new { Id = id }, transaction: transaction);
                
                if (affectedRows > 0)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                
                await transaction.RollbackAsync();
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al eliminar la orden de la base de datos", ex);
            }
        }

        /// <summary>
        /// Obtiene el siguiente número de orden disponible de forma asíncrona.
        /// Calcula el siguiente número como el máximo número de orden existente + 1.
        /// Si no hay órdenes, devuelve 1.
        /// </summary>
        /// <returns>El siguiente número de orden disponible</returns>
        public async Task<int> GetNextOrderNumberAsync()
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            
            const string sql = @"
                SELECT ISNULL(MAX(Number), 0) + 1 
                FROM Orders;";
                
            try
            {
                return await connection.ExecuteScalarAsync<int>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el siguiente número de orden", ex);
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

