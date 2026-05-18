# 🏋️‍♂️ AthletaNet - Gestión Inteligente de Gimnasios

**AthletaNet** es una aplicación web desarrollada en **ASP.NET Core Razor Pages** diseñada para optimizar la administración, el control de acceso y el flujo de atención diaria dentro de un centro de acondicionamiento físico. 

Este proyecto fue desarrollado como aplicación práctica para el curso de **Estructuras de Datos** en el **ITM**, aplicando conceptos avanzados de colecciones lineales personalizadas y persistencia de datos.

---

## 🚀 Características Principales

* **Control de Acceso Unificado:** Autenticación y manejo de vistas dinámicas basadas en tres roles de usuario (Administrador, Instructor y Cliente).
* **Gestión de Turnos (Flujo FIFO):** Sistema de colas en tiempo real para simular la asignación de instructores y rutinas a los clientes en espera.
* **Persistencia Local:** Almacenamiento y lectura dinámica de la información a través de archivos planos **JSON**, eliminando dependencias de motores relacionales pesados.
* **Interfaz Moderna:** Diseño limpio, responsive y estilizado utilizando componentes de **Bootstrap** y la tipografía **Poppins**.

---

## 🛠️ Detalles de Ingeniería (Estructuras de Datos)

Para cumplir con las directivas del taller, la lógica del negocio evita por completo el uso de colecciones nativas de .NET (`List<T>`, `Queue<T>`, etc.) y se fundamenta en estructuras genéricas construidas desde cero utilizando nodos de memoria RAM:

* **`ListaEnlazada<T>`:** Utilizada para la gestión, actualización y borrado de catálogos de clientes e instructores.
* **`Cola<T>`:** Utilizada para modelar el flujo de turnos diarios bajo la premisa de que el primer cliente en llegar es el primero en ser atendido.

> 💡 *NOTA* El repositorio incluye los archivos JSON en la carpeta de datos (`bin/Debug/`) con registros de prueba ya cargados para facilitar la revisión inmediata del flujo del sistema.
