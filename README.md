\# AD01 - Comercio Multibodega



\## Aplicaciones Distribuidas

\*\*Tema:\*\* Diseño de Arquitecturas Distribuidas



\## Descripción

Sistema de comercio y distribución multibodega desarrollado

con una arquitectura de tres microservicios REST independientes.



Cada microservicio utiliza ASP.NET Core Web API y una

base de datos propia en SQL Server.



\## Tecnologías

\- C# y .NET 10

\- ASP.NET Core Web API

\- Entity Framework Core

\- SQL Server

\- Swagger

\- Postman

\- Git y GitHub



\## Arquitectura del sistema



\### 1. Catalogo.Api

Base de datos: CatalogoDB



Tablas:

\- Categoria

\- Marca

\- Producto



Permite administrar categorías, marcas y productos.



URL local: https://localhost:7144/swagger



\### 2. Inventario.Api

Base de datos: InventarioDB



Tablas:

\- Bodega

\- Existencia

\- MovimientoInventario

\- ReservaStock



Permite administrar bodegas, controlar existencias,

registrar entradas y salidas y gestionar reservas.



URL local: https://localhost:7111/swagger



\### 3. Pedidos.Api

Base de datos: PedidosDB



Tablas:

\- Cliente

\- Pedido

\- DetallePedido



Permite administrar clientes, registrar pedidos,

agregar detalles y calcular sus totales.



URL local: https://localhost:7034/swagger



\## Operaciones REST

Las APIs implementan las operaciones correspondientes:



\- GET: consultar registros.

\- POST: registrar información.

\- PUT: actualizar información.

\- DELETE: eliminar o cancelar registros.



Se utilizan respuestas HTTP como

200, 201, 204, 400, 404 y 409.



\## Instalación y ejecución

1\. Abrir ComercioMultibodega.sln en Visual Studio.

2\. Restaurar los paquetes NuGet.

3\. Crear las tres bases de datos en SQL Server.

4\. Crear sus tablas y relaciones.

5\. Configurar las cadenas de conexión locales.

6\. Compilar los tres proyectos.

7\. Ejecutar las APIs y abrir Swagger.



\## Configuración de seguridad

Las contraseñas de SQL Server no se incluyen

en el repositorio.



Cada proyecto utiliza su archivo local

appsettings.Development.json para configurar

la cadena de conexión.



\## Pruebas

Las APIs se prueban mediante Swagger y Postman.



Las pruebas incluyen:

\- Consultas GET.

\- Creación de registros POST.

\- Actualizaciones PUT.

\- Eliminaciones DELETE.

\- Validaciones y respuestas de error.



\## Consideraciones de arquitectura

Cada microservicio administra su propia base de datos.



Los identificadores de productos entre servicios

son referencias lógicas.



La validación automática de referencias entre

microservicios y la sincronización completa de

pedidos con inventario quedan pendientes.



\## Autor

Estudiante: COMPLETAR

Carrera: Desarrollo de Software

Asignatura: Aplicaciones Distribuidas

Actividad: AD01

