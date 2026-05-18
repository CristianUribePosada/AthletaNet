namespace AthletaNet.DataStructures
{
    public class Cola<T>
    {
        private Nodo<T>? primero;
        private Nodo<T>? ultimo;
        public int Contado { get; private set; } = 0;

        // Encolar: Inserta un nuevo elemento al final de la fila
        public void Encolar(T item)
        {
            Nodo<T> nuevoNodo = new Nodo<T>(item);
            if (ultimo == null)
            {
                primero = ultimo = nuevoNodo;
            }
            else
            {
                ultimo.Siguiente = nuevoNodo;
                ultimo = nuevoNodo;
            }
            Contado++;
        }

        // Desencolar: Saca y retorna el primer elemento de la fila (el que más lleva esperando)
        public T Desencolar()
        {
            if (primero == null)
                throw new System.InvalidOperationException("La cola está vacía.");

            T valor = primero.Data;
            primero = primero.Siguiente;

            if (primero == null)
                ultimo = null;

            Contado--;
            return valor;
        }

        // Permite revisar quién está al principio de la fila sin sacarlo de ella
        public T? MirrorPrimero()
        {
            if (primero == null) return default;
            return primero.Data;
        }

        // Convierte la cola en una secuencia almacenable o iterable para las vistas HTML
        public System.Collections.Generic.IEnumerable<T> ObtenerTodos()
        {
            Nodo<T>? actual = primero;
            while (actual != null)
            {
                yield return actual.Data;
                actual = actual.Siguiente;
            }
        }
    }
}