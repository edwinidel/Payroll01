# Manual de Usuario (Administradores y RRHH)

## 1. Acceso y seguridad
- Inicia sesión con tus credenciales corporativas. Si tu cuenta tiene segundo factor, ingresa el código del autenticador o canal configurado. 
	- Captura sugerida: pantalla de login con campos y botón "Iniciar sesión". Guarda como `img/login.png`.
- Si aparece el mensaje "Debe seleccionar una compañía", primero realiza el cambio de compañía (ver sección 2).

## 2. Selección de compañía
- Menú: Inicio → Cambiar Compañía.
	- Captura sugerida: menú principal mostrando la opción de cambiar compañía (`img/menu-cambiar-compania.png`).
- Selecciona la compañía a la que tienes acceso y confirma. La selección permanece en la sesión y filtra todo lo que ves/editarás.
	- Captura sugerida: modal/listado de compañías con el botón de confirmar (`img/selector-compania.png`).

## 3. Navegación principal
- Administración: empleados, planillas, clasificaciones.
- Configuración: bancos, estructuras de depósito, catálogos contables.
- Contabilidad: importación de cuentas contables.
- Administración SuperAdmin (solo rol): estructuras de depósito globales.
- Captura sugerida: pantalla de inicio con los accesos principales (`img/dashboard-principal.png`).

## 4. Configuración inicial recomendada
1) Crear bancos y, si aplica, bancos tránsito (Configuración → Bancos → Nuevo).
2) Definir grupos de pago, horarios y catálogos (empleados, proyectos, centros de costo).
3) Cargar conceptos de pago y asociar deducciones legales.
4) Crear o ajustar estructuras de depósito bancario (ACH/TXT/CSV) y asignar campos.
5) Importar cuentas contables en la compañía activa (si se usarán asientos/controles).

## 5. Bancos
- Ruta: Configuración → Bancos.
- Crear/editar: completa nombre, banco tránsito (opcional) y estado activo.
- Eliminar: marca el banco como inactivo/eliminado (baja lógica).
- Captura sugerida: listado de bancos (`img/bancos-listado.png`) y formulario de nuevo banco (`img/bancos-nuevo.png`).

## 6. Estructuras de depósito (ACH/TXT/CSV)
- Ruta: Administracion SuperAdmin → Estructuras de Depósito.
- Crear/editar: define tipo de depósito, tipo de archivo y nombre.
- Gestionar campos: añade campos disponibles, asigna compañía y ordena (puedes usar alta masiva y reordenar).
- Generar archivo: selecciona estructura y compañía; descarga con extensión acorde al formato (csv/txt/xlsx). Requiere datos bancarios de empleados.
- Capturas sugeridas: listado (`img/depositos-listado.png`), gestión de campos con orden (`img/depositos-campos.png`), confirmación de generación (`img/depositos-generar.png`).

## 7. Conceptos de pago y deducciones
- Ruta: Administración → Conceptos de Pago.
- Detalle: visualiza deducciones legales asociadas y asigna/quita deducciones.
- CRUD de conceptos según políticas internas (horas regulares, extras, recurrentes).
- Capturas sugeridas: detalle de concepto con deducciones asignadas (`img/conceptos-detalle.png`) y selector de deducciones (`img/conceptos-deducciones.png`).

## 8. Empleados
- Ruta: Administración → Empleados.
- Crear: exige compañía seleccionada; respeta el máximo de empleados por grupo de negocio.
- Campos clave: identificación, tipo de trabajador, posición, grupo de pago, datos bancarios, salario o destajo (si es destajo, seleccionar unidad de obra), horario, centro de costo y jerarquía (sucursal, departamento, proyecto, fase, actividad).
- Editar: valida pertenencia a la compañía; permite actualizar foto y datos de pago.
- Eliminar: revisar antes para no perder histórico.
- Capturas sugeridas: listado de empleados (`img/empleados-listado.png`), formulario de alta (`img/empleados-nuevo.png`), pestaña de datos de pago (`img/empleados-pago.png`).

## 9. Planillas
- Ruta: Administración → Planillas.
- Crear: estado inicial Draft; define grupo de pago y fecha de creación automática; se asocia a la compañía actual.
- Editar: ajusta grupo/estado mientras pertenezca a la compañía seleccionada.
- Eliminar: remueve la cabecera de planilla de la compañía.
- Consulta rápida de grupo de pago: la acción de obtención de empleados por grupo muestra lista y fechas de última nómina/ausencias/extra, útil para validar contratistas vs nómina fija.
- Capturas sugeridas: listado de planillas (`img/planillas-listado.png`) y formulario de creación (`img/planillas-nueva.png`).

## 10. Importar cuentas contables (Excel)
- Ruta: Contabilidad → Importar.
- Archivo permitido: .xlsx/.xls ≤ 10 MB con encabezado y al menos una fila de datos.
- Flujo: subir archivo → vista previa valida duplicados y estructura → guardar importación. Las cuentas válidas se crean activas en la compañía actual.
- Capturas sugeridas: pantalla de importación con selector de archivo (`img/cuentas-importar.png`) y vista previa con validaciones (`img/cuentas-preview.png`).

## 11. Roles y permisos
- Administrator: CRUD de bancos, conceptos, empleados, planillas y uso de estructuras de depósito asignadas.
- Administrator (SuperAdmin): crear/editar estructuras de depósito globales y sus campos.
- Empleado/Portal (planificado): acceso restringido a recibos, certificados, saldos y notificaciones.

## 12. Buenas prácticas operativas
- Antes de crear empleados, confirma bancos, grupos de pago, centros de costo y deducciones configuradas.
- Verifica siempre la compañía activa en la barra superior.
- Para destajo, selecciona tipo de unidad obligatoria; para nómina regular, limpia campos de destajo.
- Ajusta y prueba estructuras de depósito con un grupo pequeño antes de usar en producción.
- Importar cuentas: corrige errores en Excel y reintenta; el sistema no crea duplicados por compañía.

## 13. Soporte rápido (mensajes comunes)
- "Debe seleccionar una compañía": ve a Inicio → Cambiar Compañía.
- "No tiene permisos": revisa rol o acceso a la compañía.
- Error al generar depósito: revisa que la estructura tenga campos obligatorios, sin duplicados, y que la compañía tenga empleados con datos bancarios completos.

## 14. En desarrollo / próximos (roadmap)
- Portal del empleado: recibos, certificados, saldos de vacaciones/médicos, centro de notificaciones.
- Scheduler de notificaciones (contratos, documentos, licencias, cumpleaños).
- Integraciones de asistencia y exportaciones regulatorias (SIPE, e-Tax 03).
