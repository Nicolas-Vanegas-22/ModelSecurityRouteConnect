using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{
    public class DestinationActivityBusiness
    {
        private readonly DestinationActivityBusinessData _destinationActivityBusinessData;
        private readonly ILogger _logger;

        public DestinationActivityBusiness(DestinationActivityBusinessData destinationActivityBusinessData, ILogger logger)
        {
            _destinationActivityBusinessData = destinationActivityBusinessData;
            _logger = logger;
        }

        // Método para obtener todas las actividades de destino como DTOs
        public async Task<IEnumerable<DestinationActivityBusinessDTO>> GetAllDestinationActivityBusinessAsync()
        {
            try
            {
                var destinationActivityBusinessList = await _destinationActivityBusinessData.GetAllAsync();
                var destinationActivityBusinessDTOList = new List<DestinationActivityBusinessDTO>();

                foreach (var destinationActivityBusiness in destinationActivityBusinessList)
                {
                    destinationActivityBusinessDTOList.Add(new DestinationActivityBusinessDTO
                    {
                        DestinationActivityBusinessId = destinationActivityBusiness.DestinationActivityBusinessId,
                        Name = destinationActivityBusiness.Name
                    });
                }

                return destinationActivityBusinessDTOList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las actividades de destino");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de actividades", ex);
            }
        }

        // Método para obtener una actividad de destino por ID como DTO
        public async Task<DestinationActivityBusinessDTO> GetDestinationActivityBusinessByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una actividad con ID inválido: {DestinationActivityBusinessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la actividad debe ser mayor que cero");
            }

            try
            {
                var destinationActivityBusiness = await _destinationActivityBusinessData.GetByIdAsync(id);
                if (destinationActivityBusiness == null)
                {
                    _logger.LogInformation("No se encontró ninguna actividad con ID: {DestinationActivityBusinessId}", id);
                    throw new EntityNotFoundException("DestinationActivityBusiness", id);
                }

                return new DestinationActivityBusinessDTO
                {
                    DestinationActivityBusinessId = destinationActivityBusiness.DestinationActivityBusinessId,
                    Name = destinationActivityBusiness.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la actividad con ID: {DestinationActivityBusinessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la actividad con ID {id}", ex);
            }
        }

        // Método para crear una actividad de destino desde un DTO
        public async Task<DestinationActivityBusinessDTO> CreateDestinationActivityBusinessAsync(DestinationActivityBusinessDTO destinationActivityBusinessDto)
        {
            try
            {
                ValidateDestinationActivityBusiness(destinationActivityBusinessDto);

                var destinationActivityBusiness = new DestinationActivityBusiness
                {
                    Name = destinationActivityBusinessDto.Name,
                };

                var created = await _destinationActivityBusinessData.CreateAsync(destinationActivityBusiness);

                return new DestinationActivityBusinessDTO
                {
                    DestinationActivityBusinessId = created.DestinationActivityBusinessId,
                    Name = created.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva actividad de destino: {Name}", destinationActivityBusinessDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la actividad", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateDestinationActivityBusiness(DestinationActivityBusinessDTO destinationActivityBusinessDto)
        {
            if (destinationActivityBusinessDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto actividad no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(destinationActivityBusinessDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una actividad con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la actividad es obligatorio");
            }
        }
    }
}

