using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{
    public class ChangeLogBusiness
    {
        private readonly ChangeLogBusinessData _changeLogBusinessData;
        private readonly ILogger _logger;

        public ChangeLogBusiness(ChangeLogBusinessData changeLogBusinessData, ILogger logger)
        {
            _changeLogBusinessData = changeLogBusinessData;
            _logger = logger;
        }

        // Método para obtener todos los registros de cambios como DTOs
        public async Task<IEnumerable<ChangeLogBusinessDTO>> GetAllChangeLogBusinessAsync()
        {
            try
            {
                var changeLogBusinessList = await _changeLogBusinessData.GetAllAsync();
                var changeLogBusinessDTOList = new List<ChangeLogBusinessDTO>();

                foreach (var changeLogBusiness in changeLogBusinessList)
                {
                    changeLogBusinessDTOList.Add(new ChangeLogBusinessDTO
                    {
                        ChangeLogBusinessId = changeLogBusiness.ChangeLogBusinessId,
                        Name = changeLogBusiness.Name
                    });
                }

                return changeLogBusinessDTOList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de cambios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de cambios", ex);
            }
        }

        // Método para obtener un registro de cambio por ID como DTO
        public async Task<ChangeLogBusinessDTO> GetChangeLogBusinessByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de cambio con ID inválido: {ChangeLogBusinessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro debe ser mayor que cero");
            }

            try
            {
                var changeLogBusiness = await _changeLogBusinessData.GetByIdAsync(id);
                if (changeLogBusiness == null)
                {
                    _logger.LogInformation("No se encontró ningún registro con ID: {ChangeLogBusinessId}", id);
                    throw new EntityNotFoundException("ChangeLogBusiness", id);
                }

                return new ChangeLogBusinessDTO
                {
                    ChangeLogBusinessId = changeLogBusiness.ChangeLogBusinessId,
                    Name = changeLogBusiness.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de cambio con ID: {ChangeLogBusinessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro con ID {id}", ex);
            }
        }

        // Método para crear un registro de cambio desde un DTO
        public async Task<ChangeLogBusinessDTO> CreateChangeLogBusinessAsync(ChangeLogBusinessDTO changeLogBusinessDto)
        {
            try
            {
                ValidateChangeLogBusiness(changeLogBusinessDto);

                var changeLogBusiness = new ChangeLogBusiness
                {
                    Name = changeLogBusinessDto.Name,
                };

                var created = await _changeLogBusinessData.CreateAsync(changeLogBusiness);

                return new ChangeLogBusinessDTO
                {
                    ChangeLogBusinessId = created.ChangeLogBusinessId,
                    Name = created.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro de cambio: {Name}", changeLogBusinessDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateChangeLogBusiness(ChangeLogBusinessDTO changeLogBusinessDto)
        {
            if (changeLogBusinessDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto registro no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(changeLogBusinessDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del registro es obligatorio");
            }
        }
    }
}
