namespace AthletaNet.DataStructures
{
    // El nodo es el eslabón de la cadena. Guarda el dato y apunta al siguiente.
    public class Nodo<T>
    {
        public T Data { get; set; }
        public Nodo<T>? Siguiente { get; set; }

        public Nodo(T data)
        {
            Data = data;
            Siguiente = null;
        }
    }

    // Nuestra propia estructura de lista dinámica
    public class ListaEnlazada<T>
    {
        private Nodo<T>? cabeza;
        public int Contado { get; private set; } = 0;

        // Agregar un elemento al final de la lista
        public void Agregar(T item)
        {
            Nodo<T> nuevoNodo = new Nodo<T>(item);
            if (cabeza == null)
            {
                cabeza = nuevoNodo;
            }
            else
            {
                Nodo<T> actual = cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
            Contado++;
        }

        // Método para poder eliminar un nodo específico de la lista (crucial para borrar clientes)
        public bool Eliminar(System.Func<T, bool> predicado)
        {
            if (cabeza == null) return false;

            // Si el elemento a eliminar es la cabeza
            if (predicado(cabeza.Data))
            {
                cabeza = cabeza.Siguiente;
                Contado--;
                return true;
            }

            Nodo<T> actual = cabeza;
            while (actual.Siguiente != null)
            {
                if (predicado(actual.Siguiente.Data))
                {
                    actual.Siguiente = actual.Siguiente.Siguiente;
                    Contado--;
                    return true;
                }
                actual = actual.Siguiente;
            }

            return false;
        }

        // Esto nos permite recorrer nuestra lista usando un "foreach" en la web
        public System.Collections.Generic.IEnumerable<T> ObtenerTodos()
        {
            Nodo<T>? actual = cabeza;
            while (actual != null)
            {
                yield return actual.Data;
                actual = actual.Siguiente;
            }
        }

        public void Limpiar()
        {
            cabeza = null;
            Contado = 0;
        }
    }
}