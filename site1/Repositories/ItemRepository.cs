using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MessageApi.Models;
using MessageApi.Data;

namespace MessageApi.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad Item.
    /// Maneja las operaciones de base de datos para los ítems en el sistema.
    /// </summary>
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de ítems.
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public ItemRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Obtiene todos los ítems de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de ítems</returns>
        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _context.Items.ToListAsync();
        }

        /// <summary>
        /// Obtiene un ítem por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del ítem a buscar</param>
        /// <returns>El ítem encontrado o null si no existe</returns>
        public async Task<Item?> GetItemByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");

            return await _context.Items.FindAsync(id);
        }

        /// <summary>
        /// Crea un nuevo ítem en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="item">Objeto Item con los datos del ítem a crear</param>
        /// <returns>El ítem creado con su ID generado</returns>
        public async Task<Item> CreateItemAsync(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// Actualiza un ítem existente en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="item">Objeto Item con los datos actualizados</param>
        /// <returns>True si la actualización fue exitosa, false si el ítem no existe</returns>
        public async Task<bool> UpdateItemAsync(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var existingItem = await _context.Items.FindAsync(item.Id);
            if (existingItem == null)
                return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina un ítem de la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del ítem a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false si el ítem no existe</returns>
        public async Task<bool> DeleteItemAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");

            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
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
