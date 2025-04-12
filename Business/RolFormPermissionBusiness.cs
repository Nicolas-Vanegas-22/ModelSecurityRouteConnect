using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{
    public class RolFormPermissionBusiness
    {
        private readonly RolFormPermissionBusinessData _rolFormPermissionBusinessData;
        private readonly ILogger _logger;

        public RolFormPermissionBusiness(RolFormPermissionBusinessData rolFormPermissionBusinessData, ILogger logger)
        {
            _rolFormPermissionBusinessData = rolFormPermissionBusinessData;
            _logger = logger;
        }

        // Método para obtener todos los permisos de formulario por rol como DTOs
        public async Task<IEnumerable<RolFormPermissionBusinessDTO>> GetAllRolFormPermissionBusinessAsync()
        {
            try
            {
                var rolFormPermissionBusinessList = await _rolFormPermissionBusinessData.GetAllAsync();
                var rolFormPermissionBusinessDTOList = new List<RolFormPermissionBusinessDTO>();

                foreach (var rolFormPermissionBusiness in rolFormPermissionBusinessList)
                {
                    rolFormPermissionBusinessDTOList.Add(new RolFormPermissionBusinessDTO
                    {
                        RolFormPermissionBusinessId = rolFormPermissionBusiness.RolFormPermissionBusinessId,
                        Name = rolFormPermissionBusiness.Name
                    });
                }

                return rolFormPermissionBusinessDTOList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de formulario por rol");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permisos", ex);
            }
        }

        // Método para obtener un permiso por rol y formulario por ID como DTO
        public async Task<RolFormPermissionBusinessDTO> GetRolFormPermissionBusinessByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permiso con ID inválido: {RolFormPermissionBusinessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }

            try
            {
                var rolFormPermissionBusiness = await _rolFormPermissionBusinessData.GetByIdAsync(id);
                if (rolFormPermissionBusiness == null)
                {
                    _logger.LogInformation("No se encontró ningún permiso con ID: {RolFormPermissionBusinessId}", id);
                    throw new EntityNotFoundException("RolFormPermissionBusiness", id);
                }

                return new RolFormPermissionBusinessDTO
                {
                    RolFormPermissionBusinessId = rolFormPermissionBusiness.RolFormPermissionBusinessId,
                    Name = rolFormPermissionBusiness.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {RolFormPermissionBusinessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permiso con ID {id}", ex);
            }
        }

        // Método para crear un permiso desde un DTO
        public async Task<RolFormPermissionBusinessDTO> CreateRolFormPermissionBusinessAsync(RolFormPermissionBusinessDTO rolFormPermissionBusinessDto)
        {
            try
            {
                ValidateRolFormPermissionBusiness(rolFormPermissionBusinessDto);

                var rolFormPermissionBusiness = new RolFormPermissionBusiness
                {
                    Name = rolFormPermissionBusinessDto.Name,
                };

                var rolFormPermissionBusinessCreado = await _rolFormPermissionBusinessData.CreateAsync(rolFormPermissionBusiness);

                return new RolFormPermissionBusinessDTO
                {
                    RolFormPermissionBusinessId = rolFormPermissionBusinessCreado.RolFormPermissionBusinessId,
                    Name = rolFormPermissionBusinessCreado.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo permiso de formulario por rol: {Name}", rolFormPermissionBusinessDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRolFormPermissionBusiness(RolFormPermissionBusinessDTO rolFormPermissionBusinessDto)
        {
            if (rolFormPermissionBusinessDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto permiso no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(rolFormPermissionBusinessDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un permiso con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del permiso es obligatorio");
            }
        }
    }
}

