# Rotar credenciales de base de datos (Guía)

Pasos recomendados para rotar la contraseña expuesta y crear una cuenta de aplicación con permisos mínimos.

1) Rotar la contraseña `sa` (si corresponde)

   - Conéctese al servidor SQL con una cuenta que tenga permisos de administrador.
   - Ejecute (cambiar `NewStrong!Passw0rd` por una contraseña fuerte y única):

   ```sql
   ALTER LOGIN [sa] WITH PASSWORD = 'NewStrong!Passw0rd';
   ```

   - Nota: si `sa` no debe usarse por seguridad, considere deshabilitarla después de crear y verificar el acceso con la cuenta de aplicación.

2) Crear una cuenta de aplicación con permisos mínimos para la base de datos `2FA`

   ```sql
   -- Crear login a nivel servidor
   CREATE LOGIN app_user WITH PASSWORD = 'AppStrongPass!23';

   -- Crear usuario en la base de datos y asignar roles de lectura/escritura
   USE [2FA];
   CREATE USER app_user FOR LOGIN app_user;
   ALTER ROLE db_datareader ADD MEMBER app_user;
   ALTER ROLE db_datawriter ADD MEMBER app_user;

   -- Opcional: conceder permisos más restrictivos sólo a esquemas o procedimientos necesarios
   -- GRANT EXECUTE ON SCHEMA::dbo TO app_user;
   ```

3) Actualizar la configuración de la aplicación

   - No poner la nueva contraseña en `appsettings.json`. En su lugar utilice:
     - Variables de entorno (ej. `ConnectionStrings__DefaultConnection`) o
     - `dotnet user-secrets` para desarrollo local
     - Un gestor de secretos en producción (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault)

   - Ejemplo para exportar en macOS/Linux:

   ```bash
   export ConnectionStrings__DefaultConnection="Server=YOUR_SERVER;Database=2FA;User Id=app_user;Password=AppStrongPass!23;"
   ```

4) Rotación coordinada

   - Rotee la contraseña en el servidor y actualice la configuración en los hosts/servicios que usen la conexión.
   - Verifique que la aplicación funciona con la nueva cuenta antes de deshabilitar `sa`.

5) Comunicación y seguimiento

   - Notifique a los miembros del equipo sobre la rotación y pida que actualicen cualquier copia local.
   - Revise logs de acceso a la base de datos para detectar accesos no autorizados antes/después de la rotación.

6) Pasos posteriores a la purga del repo

   - Después de limpiar el historial Git (ver `scripts/purge_git_history.sh`), todos los colaboradores deben clonar de nuevo el repositorio.
