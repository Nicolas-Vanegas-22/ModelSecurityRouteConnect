using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{
    public class FormModuleBusiness
    {
        private readonly FormModuleBusinessData _formModuleBusinessData;
        private readonly ILogger _logger;

        public FormModuleBusiness(FormModuleBusinessData formModuleBusinessData, ILogger logger)
        {
            _formModuleBusinessData = formModuleBusinessData;
            _logger = logger;
        }

        // Método para obtener todos los módulos de formulario como DTOs
        public async Task<IEnumerable<FormModuleBusinessDTO>> GetAllFormModuleBusinessAsync()
        {
            try
            {
                var formModuleBusinessList = await _formModuleBusinessData.GetAllAsync();
                var formModuleBusinessDTOList = new List<FormModuleBusinessDTO>();

                foreach (var formModuleBusiness in formModuleBusinessList)
                {
                    formModuleBusinessDTOList.Add(new FormModuleBusinessDTO
                    {
                        FormModuleBusinessId = formModuleBusiness.FormModuleBusinessId,
                        Name = formModuleBusiness.Name
                    });
                }

                return formModuleBusinessDTOList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos de formulario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos", ex);
            }
        }

        // Método para obtener un módulo por ID como DTO
        public async Task<FormModuleBusinessDTO> GetFormModuleBusinessByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo con ID inválido: {FormModuleBusinessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del módulo debe ser mayor que cero");
            }

            try
            {
                var formModuleBusiness = await _formModuleBusinessData.GetByIdAsync(id);
                if (formModuleBusiness == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo con ID: {FormModuleBusinessId}", id);
                    throw new EntityNotFoundException("FormModuleBusiness", id);
                }

                return new FormModuleBusinessDTO
                {
                    FormModuleBusinessId = formModuleBusiness.FormModuleBusinessId,
                    Name = formModuleBusiness.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {FormModuleBusinessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
            }
        }

        // Método para crear un módulo desde un DTO
        public async Task<FormModuleBusinessDTO> CreateFormModuleBusinessAsync(FormModuleBusinessDTO formModuleBusinessDto)
        {
            try
            {
                ValidateFormModuleBusiness(formModuleBusinessDto);

                var formModuleBusiness = new FormModuleBusiness
                {
                    Name = formModuleBusinessDto.Name,
                };

                var formModuleBusinessCreado = await _formModuleBusinessData.CreateAsync(formModuleBusiness);

                return new FormModuleBusinessDTO
                {
                    FormModuleBusinessId = formModuleBusinessCreado.FormModuleBusinessId,
                    Name = formModuleBusinessCreado.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo módulo de formulario: {Name}", formModuleBusinessDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateFormModuleBusiness(FormModuleBusinessDTO formModuleBusinessDto)
        {
            if (formModuleBusinessDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto módulo no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(formModuleBusinessDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del módulo es obligatorio");
            }
        }
    }
}
