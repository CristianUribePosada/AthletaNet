# AthletaNet - Gestión Inteligente de Gimnasios

**AthletaNet** es una aplicación web desarrollada en **ASP.NET Core Razor Pages** diseñada para optimizar la administración, el control de acceso y el flujo de atención diaria dentro de un centro de acondicionamiento físico. 

Este proyecto fue desarrollado como aplicación práctica para el curso de **Estructuras de Datos** en el **ITM**, aplicando conceptos avanzados de colecciones lineales personalizadas y persistencia de datos.

---

## Características Principales

* **Control de Acceso Unificado:** Autenticación y manejo de vistas dinámicas basadas en tres roles de usuario (Administrador, Instructor y Cliente).
* **Gestión de Turnos (Flujo FIFO):** Sistema de colas en tiempo real para simular la asignación de instructores y rutinas a los clientes en espera.
* **Persistencia Local:** Almacenamiento y lectura dinámica de la información a través de archivos planos **JSON**, eliminando dependencias de motores relacionales pesados.
* **Interfaz Moderna:** Diseño limpio, responsive y estilizado utilizando componentes de **Bootstrap** y la tipografía **Poppins**.

---

## Detalles de Ingeniería (Estructuras de Datos)

Para cumplir con las directivas del taller, la lógica del negocio evita por completo el uso de colecciones nativas de .NET (`List<T>`, `Queue<T>`, etc.) y se fundamenta en estructuras genéricas construidas desde cero utilizando nodos de memoria RAM:

* **`ListaEnlazada<T>`:** Utilizada para la gestión, actualización y borrado de catálogos de clientes e instructores.
* **`Cola<T>`:** Utilizada para modelar el flujo de turnos diarios bajo la premisa de que el primer cliente en llegar es el primero en ser atendido.

## INSTRUCCIONES DE USO DE ATHLETANET

**1. Credenciales de Acceso por Defecto**

Al arrancar la aplicación, el sistema lo dirigirá a la pantalla de Login por seguridad:

Administrador Único: admin / admin123

Clientes e Instructores: Se ingresa con el usuario y contraseña que usted mismo registre en la consola de administración.

---

**2. Flujo de Prueba Recomendado (Paso a Paso)**

* PASO A: Gestión de Personal y Atletas (Rol: Administrador)

Inicie sesión con la cuenta de Administrador (admin).

Registrar un Instructor: En el formulario inferior izquierdo, cree un coach (Ej: coach1). Note cómo se añade a la tabla usando la ListaEnlazada<T>.

Registrar un Cliente: En el formulario superior izquierdo, cree un atleta (Ej: atleta1) seleccionando un plan de membresía (Mensual, Trimestral o Anual). El sistema calculará la fecha de vencimiento automáticamente.

Cierre sesión.

* PASO B: Fila de Espera y Turno Único (Rol: Cliente)

Inicie sesión con la cuenta del atleta creado (atleta1).

El sistema validará que la membresía esté Activa.

En el selector, elija atenderse con el instructor creado (coach1) o seleccione "Entrenamiento Libre".

Presione Agendar Turno de Hoy. El turno se encolará de inmediato en la estructura Cola<T>.

Comprobación de un Turno por día: Intente presionar el botón de agendar nuevamente. El sistema bloqueará la acción informando que solo se permite un turno diario por atleta.

Cierre sesión.

* PASO C: Prescripción de Rutina y Desencolado FIFO (Rol: Instructor)

Inicie sesión con la cuenta del instructor (coach1).

En el panel izquierdo verá la Fila de Espera del Día organizada estrictamente por orden de llegada (Primero en Llegar, Primero en Ser Atendido).

En el formulario de atención aparecerá el atleta que va a la cabeza de la fila. Redacte la rutina (Ej: 4 series de prensa y sentadillas) y presione Finalizar Turno y Liberar Fila.

Esta acción asigna la rutina al historial del cliente y ejecuta internamente un .Desencolar(), retirándolo de la cola del día.

Cierre sesión.

* PASO D: Auditoría e Informes Administrativos (Rol: Administrador)

Regrese e inicie sesión como admin.

Diríjase a la parte inferior del panel. Podrá auditar la tabla de Auditoría de Turnos en Fila (Informe Global), la cual muestra de forma unificada el estado de todos los turnos del gimnasio (si están "En Espera" o con "Rutina Asignada").

---

3. Comportamiento de Acciones Especiales
Eliminación en Cascada (Clientes): Al presionar "Eliminar" en la lista de clientes, el sistema recorre la ListaEnlazada, remueve el nodo y purga por completo su membresía e historial de turnos del archivo plano .json.

Deshabilitar sin Borrar (Instructores): Siguiendo la restricción del taller, los instructores no se pueden borrar. Al presionar el botón de acción, se conmuta su bandera a Deshabilitado, lo que bloquea su acceso al portal pero conserva intactos sus registros históricos.
