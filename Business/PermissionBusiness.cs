using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los permission del sistema.
    /// </summary>

    public class PermissionBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger _logger;

        public PermissionBusiness(PermissionData permissionData, ILogger logger)
        {
            _permissionData = permissionData;
            _logger = logger;
        }

        // Método para obtener todos los Permission como DTOs
        public async Task<IEnumerable<PermissionDto>> GetAllPermissionAsync()
        {
            try
            {
                var permissions = await _permissionData.GetAllAsync();
                var permissionDTOs = new List<PermissionDto>();

                foreach (var person in persons)
                {
                    personDTOs.Add(new PersonDto
                    {
                        PersonsId = person.PersonId,
                        LastName = person.LastName,
                        FirstName = person.FirstName
                    });
                }

                return personDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los persons");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de persons", ex);
            }
        }

        // Método para obtener un persons por ID como DTO
        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un person con ID inválido: {personId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del person debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ningún Person con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return new PersonDto
                {
                    PersonsId = person.PersonId,
                    LastName = person.LastName,
                    FirstName = person.FirstName
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Person con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un Person desde un DTO
        public async Task<PersonDto> CreatePersonAsync(PersonDto PersonDto)
        {
            try
            {
                ValidatPerson(PersonDto);

                var person = new Person
                {
                    PersonId = PersonDto.PersonsId,
                    LastName = PersonDto.LastName,
                    FirstName = PersonDto.FirstName
                };

                var personCreado = await _personData.CreateAsync(person);

                return new PersonDto
                {
                    PersonsId = personCreado.PersonId,
                    LastName = personCreado.LastName,
                    FirstName = personCreado.FirstName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo user: {PersonNombre}", PersonDto?.PersonName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Person", ex);
            }
        }

        // Método para validar el Persons
        private void ValidatPerson(PersonDto PersonDto)
        {
            if (PersonDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Person no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace((string?)PersonDto.PersonName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Person con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del Person es obligatorio");
            }
        }
    }
}