using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{
    public class ChangeLog
    {
        private readonly ChangeLogData _changeLogData;
        private readonly ILogger _logger;

        public ChangeLog(ChangeLogData changeLogData, ILogger logger)
        {
            _changeLogData = changeLogData;
            _logger = logger;
        }

        // Obtener todos los registros
        public async Task<IEnumerable<ChangeLogDTO>> GetAllAsync()
        {
            try
            {
                var logs = await _changeLogData.GetAllAsync();
                return logs.Select(l => new ChangeLogDTO
                {
                    ChangeLogId = l.ChangeLogId,
                    Description = l.Description,
                    ChangeDate = l.ChangeDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los cambios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los logs de cambios", ex);
            }
        }

        // Obtener por ID
        public async Task<ChangeLogDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {ChangeLogId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var log = await _changeLogData.GetByIdAsync(id);
                if (log == null)
                {
                    _logger.LogInformation("Cambio no encontrado: {ChangeLogId}", id);
                    throw new EntityNotFoundException("ChangeLog", id);
                }

                return new ChangeLogDTO
                {
                    ChangeLogId = log.ChangeLogId,
                    Description = log.Description,
                    ChangeDate = log.ChangeDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cambio con ID: {ChangeLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el log con ID {id}", ex);
            }
        }

        // Crear nuevo cambio
        public async Task<ChangeLogDTO> CreateAsync(ChangeLogDTO dto)
        {
            try
            {
                Validate(dto);

                var entity = new Entity.Model.ChangeLog
                {
                    Description = dto.Description,
                    ChangeDate = dto.ChangeDate
                };

                var created = await _changeLogData.CreateAsync(entity);

                return new ChangeLogDTO
                {
                    ChangeLogId = created.ChangeLogId,
                    Description = created.Description,
                    ChangeDate = created.ChangeDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear log de cambio: {Description}", dto?.Description ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro de cambio", ex);
            }
        }

        // Validación
        private void Validate(ChangeLogDTO dto)
        {
            if (dto == null)
                throw new ValidationException("El objeto cambio no puede ser nulo");

            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                _logger.LogWarning("Intento de crear cambio con descripción vacía");
                throw new ValidationException("Description", "La descripción del cambio es obligatoria");
            }

            if (dto.ChangeDate == default)
            {
                _logger.LogWarning("Intento de crear cambio sin fecha válida");
                throw new ValidationException("ChangeDate", "La fecha del cambio es obligatoria");
            }
        }
    }
}
