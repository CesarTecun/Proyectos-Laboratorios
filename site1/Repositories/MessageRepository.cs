using MessageApi.Data;
using MessageApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageApi.Repositories;

/// <summary>
/// Implementación del repositorio para la entidad Message.
/// Maneja las operaciones de base de datos para los mensajes del sistema.
/// </summary>
public class MessageRepository : IMessageRepository, IDisposable
{
    private bool _disposed = false;
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de mensajes.
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    public MessageRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Agrega un nuevo mensaje al sistema de forma asíncrona.
    /// </summary>
    /// <param name="message">Contenido del mensaje a agregar</param>
    /// <returns>El mensaje creado con su ID generado</returns>
    public async Task<Message> AddMessageAsync(string message)
    {
        var entity = new Message
        {
            MessageText = message ?? throw new ArgumentNullException(nameof(message)),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _context.Messages.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Obtiene todos los mensajes del sistema de forma asíncrona.
    /// Los mensajes se ordenan por su ID de forma ascendente.
    /// </summary>
    /// <returns>Colección de todos los mensajes</returns>
    public async Task<IEnumerable<Message>> GetAllMessagesAsync()
    {
        return await _context.Messages
            .OrderBy(m => m.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un mensaje por su ID de forma asíncrona.
    /// </summary>
    /// <param name="id">ID del mensaje a buscar</param>
    /// <returns>El mensaje encontrado o null si no existe</returns>
    public async Task<Message?> GetMessageByIdAsync(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    /// <summary>
    /// Actualiza un mensaje existente de forma asíncrona.
    /// </summary>
    /// <param name="message">Objeto Message con los datos actualizados</param>
    /// <returns>El mensaje actualizado o null si no se encontró el mensaje</returns>
    public async Task<Message?> UpdateMessageAsync(Message message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        var existing = await _context.Messages.FindAsync(message.Id);
        if (existing == null) return null;

        existing.MessageText = message.MessageText;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    /// <summary>
    /// Elimina un mensaje por su ID de forma asíncrona.
    /// </summary>
    /// <param name="id">ID del mensaje a eliminar</param>
    /// <returns>True si la eliminación fue exitosa, false si el mensaje no existe</returns>
    public async Task<bool> DeleteMessageAsync(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message == null) return false;

        _context.Messages.Remove(message);
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

