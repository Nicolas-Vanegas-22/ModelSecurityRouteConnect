using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class FormModule
    {
        private readonly FormModuleData _formModuleData;
        private readonly ILogger _logger;

        public FormModule(FormModuleData formModuleData, ILogger logger)
        {
            _formModuleData = formModuleData;
            _logger = logger;
        }

        // Obtener todos los módulos de formulario
        public async Task<IEnumerable<FormModuleDTO>> GetAllAsync()
        {
            try
            {
                var modules = await _formModuleData.GetAllAsync();
                return modules.Select(m => new FormModuleDTO
                {
                    FormModuleId = m.FormModuleId,
                    Name = m.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos de formulario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los módulos", ex);
            }
        }

        // Obtener uno por ID
        public async Task<FormModuleDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID del módulo debe ser mayor que cero");
            }

            try
            {
                var module = await _formModuleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró el módulo: {FormModuleId}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return new FormModuleDTO
                {
                    FormModuleId = module.FormModuleId,
                    Name = module.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
            }
        }

        // Crear nuevo módulo
        public async Task<FormModuleDTO> CreateAsync(FormModuleDTO dto)
        {
            try
            {
                Validate(dto);

                var entity = new Entity.Model.FormModule
                {
                    Name = dto.Name
                };

                var created = await _formModuleData.CreateAsync(entity);

                return new FormModuleDTO
                {
                    FormModuleId = created.FormModuleId,
                    Name = created.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear módulo: {Name}", dto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
            }
        }

        // Validación
        private void Validate(FormModuleDTO dto)
        {
            if (dto == null)
                throw new ValidationException("El objeto módulo no puede ser nulo");

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Intento de crear módulo con Name vacío");
                throw new ValidationException("Name", "El Name del módulo es obligatorio");
            }
        }
    }
}
