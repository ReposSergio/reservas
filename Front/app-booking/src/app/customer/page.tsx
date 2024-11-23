"use client";
import { useEffect, useState } from "react";
import axios from "axios";

// Actualizamos la interfaz para que el tipo sea correcto según la respuesta de Postman
interface Reservation {
  $id: string;
  $values: any[]; // Arreglo vacío, pero puede cambiar si se añaden reservas
}

interface Customer {
  id: number;
  email: string;
  name: string;
  passwordHash: string;
  salt: string;
  reservations: Reservation; // Ajustamos la propiedad de reservas
}

const CustomerList = () => {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // Estado para edición
  const [editingCustomer, setEditingCustomer] = useState<Customer | null>(null);
  const [email, setEmail] = useState<string>("");
  const [name, setName] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  const API_BASE_URL = "http://localhost:5000/api/clients";

  // Modificamos la función para trabajar con el formato de la respuesta
  const fetchCustomers = async () => {
    setLoading(true);
    try {
      const response = await axios.get(API_BASE_URL);
      
      // La respuesta está dentro de la propiedad "$values"
      setCustomers(response.data.$values);
    } catch (err) {
      setError("Error al cargar los clientes. Inténtalo de nuevo.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCustomers();
  }, []);

  const handleDelete = async (id: number) => {
    try {
      await axios.delete(`${API_BASE_URL}/${id}`);
      setCustomers(customers.filter((customer) => customer.id !== id));
    } catch (err) {
      setError("Error al eliminar el cliente. Inténtalo de nuevo.");
    }
  };

  const handleEdit = (customer: Customer) => {
    setEditingCustomer(customer);
    setEmail(customer.email);
    setName(customer.name);
    setPassword(""); // Limpiar la contraseña al editar
  };

  const handleUpdate = async () => {
    if (!editingCustomer) return;

    try {
      const updatedCustomer = { ...editingCustomer, email, name, password };
      await axios.put(`${API_BASE_URL}/${editingCustomer.id}`, updatedCustomer);

      setCustomers(
        customers.map((customer) =>
          customer.id === editingCustomer.id ? updatedCustomer : customer
        )
      );

      // Resetear el formulario de edición
      setEditingCustomer(null);
      setEmail("");
      setName("");
      setPassword("");
    } catch (err) {
      setError("Error al actualizar el cliente. Inténtalo de nuevo.");
    }
  };

  if (loading) return <p className="text-center text-gray-500">Cargando clientes...</p>;
  if (error) return <p className="text-center text-red-500">{error}</p>;

  return (
    <div className="p-4">
      <h1 className="text-xl font-bold mb-4 text-center">Clientes Disponibles</h1>
      {customers.length > 0 ? (
        <ul className="space-y-4">
          {customers.map((customer) => (
            <li
              key={customer.id}
              className="bg-white shadow-md rounded-lg p-4 border border-gray-200"
            >
              <h2 className="text-lg font-semibold">{customer.name}</h2>
              <p className="text-gray-600">{customer.email}</p>
              <div className="mt-4 flex space-x-2">
                <button
                  onClick={() => handleEdit(customer)}
                  className="bg-blue-500 text-white py-1 px-4 rounded hover:bg-blue-600"
                >
                  Editar
                </button>
                <button
                  onClick={() => handleDelete(customer.id)}
                  className="bg-red-500 text-white py-1 px-4 rounded hover:bg-red-600"
                >
                  Eliminar
                </button>
              </div>
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-center text-gray-500">No hay clientes disponibles.</p>
      )}

      {editingCustomer && (
        <div className="mt-8 p-4 bg-gray-100 rounded-lg">
          <h2 className="text-lg font-semibold mb-4">Editar Cliente</h2>
          <form
            onSubmit={(e) => {
              e.preventDefault();
              handleUpdate();
            }}
          >
            <div className="mb-4">
              <label htmlFor="email" className="block text-gray-700 font-medium mb-2">
                Email del Cliente
              </label>
              <input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full border-gray-300 rounded-lg shadow-sm focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div className="mb-4">
              <label htmlFor="name" className="block text-gray-700 font-medium mb-2">
                Nombre del Cliente
              </label>
              <input
                type="text"
                id="name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="w-full border-gray-300 rounded-lg shadow-sm focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div className="mb-4">
              <label htmlFor="password" className="block text-gray-700 font-medium mb-2">
                Contraseña
              </label>
              <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full border-gray-300 rounded-lg shadow-sm focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div className="flex space-x-2">
              <button
                type="submit"
                className="bg-green-500 text-white py-2 px-4 rounded hover:bg-green-600"
              >
                Guardar
              </button>
              <button
                type="button"
                onClick={() => setEditingCustomer(null)}
                className="bg-gray-500 text-white py-2 px-4 rounded hover:bg-gray-600"
              >
                Cancelar
              </button>
            </div>
          </form>
        </div>
      )}
    </div>
  );
};

export default CustomerList;
