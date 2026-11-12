# Clinica Veterinaria API

API REST en .NET 10 con arquitectura por capas, DTOs y AutoMapper. Modela propietarios y mascotas en una relación 1:N.

## Proyectos

- `ClinicaVeterinaria.Entidades`: entidades y DTOs.
- `ClinicaVeterinaria.Datos`: DbContext y repositorios.
- `ClinicaVeterinaria.Negocio`: servicios, reglas y MappingProfile.
- `ClinicaVeterinaria.Api`: controladores, Swagger y composición de dependencias.

El script SQL se encuentra en `Database/ClinicaVeterinariaDb.sql`. Para evidencias locales reproducibles, `UseInMemoryDatabase` está habilitado; para SQL Server, cambie a `false` y ejecute el script.
