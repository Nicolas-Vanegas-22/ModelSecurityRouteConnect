using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{

    public class DestinationBusiness
    {
        private readonly DestinationData _destinationData;
        private readonly ILogger _logger;

        public DestinationBusiness(DestinationData destinationData, ILogger logger)
        {
            _destinationData = destinationData;
            _logger = logger;
        }

        // Método para obtener todos los destinos comoo DTOs
        public async Task<IEnumerable<DestinationDTO>> GetAllDestinationsAsync()
        {
            try
            {
                var destinations = await _destinationData.GetAllAsync();
                var destinationsDTO = new List<DestinationDTO>();

                foreach (var destination in destinations)
                {
                    destinationsDTO.Add(new DestinationDTO
                    {
                        DestinationId = destination.DestinationId,
                        Name = destination.Name
                    });
                }

                return destinationsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los destinos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de destinos", ex);
            }
        }

        // Método para obtener un destino por ID como DTO
        public async Task<DestinationDTO> GetDestinationByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un destino con ID inválido: {DestinationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del destino debe ser mayor que cero");
            }

            try
            {
                var destination = await _destinationData.GetByIdAsync(id);
                if (destination == null)
                {
                    _logger.LogInformation("No se encontró ningún destino con ID: {DestinationId}", id);
                    throw new EntityNotFoundException("Destination", id);
                }

                return new DestinationDTO
                {
                    DestinationId = destination.DestinationId,
                    Name = destination.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el destino con ID: {DestinationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el destino con ID {id}", ex);
            }
        }

        // Método para crear un destino desde un DTO
        public async Task<DestinationDTO> CreateDestinationAsync(DestinationDTO DestinationDto)
        {
            try
            {
                ValidateDestination(DestinationDto);

                var destination = new Destination
                {
                    Name = DestinationDto.Name,
                };

                var destinationCreado = await _destinationData.CreateAsync(destination);

                return new DestinationDTO
                {
                    DestinationId = destinationCreado.DestinationId,
                    Name = destinationCreado.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo destino: {Name}", DestinationDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateDestination(DestinationDTO DestinationDto)
        {
            if (DestinationDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto destino no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(DestinationDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un destino con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del destino es obligatorio");
            }
        }
    }
}
