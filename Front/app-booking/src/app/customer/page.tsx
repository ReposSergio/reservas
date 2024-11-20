"use client";
import { useEffect, useState } from "react";
import axios from "axios";

interface Customer {
  id: number;
  name: string;
}

const CustomerTable = () => {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // Estado para edición
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editedName, setEditedName] = useState<string>("");

  const API_BASE_URL = "http://localhost:5000/api/customer";

  // Cargar los clientes desde la API
  const fetchCustomers = async () => {
    setLoading(true);
    try {
      const response = await axios.get<Customer[]>(API_BASE_URL);
      setCustomers(response.data);
    } catch (err) {
      setError("Error al cargar los clientes. Inténtalo de nuevo.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCustomers();
  }, []);

  // Eliminar un cliente
  const handleDelete = async (id: number) => {
    try {
      await axios.delete(`${API_BASE_URL}/${id}`);
      setCustomers(customers.filter((customer) => customer.id !== id));
    } catch (err) {
      setError("Error al eliminar el cliente. Inténtalo de nuevo.");
    }
  };

  // Iniciar la edición de un cliente
  const handleEdit = (customer: Customer) => {
    setEditingId(customer.id);
    setEditedName(customer.name);
  };

  // Guardar cambios después de editar
  const handleUpdate = async () => {
    if (editingId === null) return;

    try {
      const updatedCustomer = { id: editingId, name: editedName };
      await axios.put(`${API_BASE_URL}/${editingId}`, updatedCustomer);

      setCustomers(
        customers.map((customer) =>
          customer.id === editingId ? updatedCustomer : customer
        )
      );

      setEditingId(null);  // Desactivar el modo de edición
      setEditedName("");  // Resetear los valores
    } catch (err) {
      setError("Error al actualizar el cliente. Inténtalo de nuevo.");
    }
  };

  if (loading) return <p className="text-center text-gray-500">Cargando clientes...</p>;
  if (error) return <p className="text-center text-red-500">{error}</p>;

  return (
    <div className="p-4">
      <h1 className="text-xl font-bold mb-4 text-center">Clientes</h1>
      <div className="overflow-x-auto">
        <table className="min-w-full bg-white border border-gray-200 rounded-lg">
          <thead className="bg-gray-100">
            <tr>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700">ID</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700">Nombre</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {customers.map((customer) => (
              <tr key={customer.id} className="border-b border-gray-200">
                <td className="px-4 py-2 text-sm text-gray-700">{customer.id}</td>
                <td className="px-4 py-2 text-sm text-gray-700">
                  {editingId === customer.id ? (
                    <input
                      type="text"
                      value={editedName}
                      onChange={(e) => setEditedName(e.target.value)}
                      className="w-full px-2 py-1 border border-gray-300 rounded-md"
                      autoFocus
                    />
                  ) : (
                    customer.name
                  )}
                </td>
                <td className="px-4 py-2 text-sm text-gray-700 flex space-x-2">
                  {editingId === customer.id ? (
                    <>
                      <button
                        onClick={handleUpdate}
                        className="px-4 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600"
                      >
                        Guardar
                      </button>
                      <button
                        onClick={() => setEditingId(null)}
                        className="px-4 py-2 bg-gray-500 text-white rounded-lg hover:bg-gray-600"
                      >
                        Cancelar
                      </button>
                    </>
                  ) : (
                    <>
                      <button
                        onClick={() => handleEdit(customer)}
                        className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
                      >
                        Editar
                      </button>
                      <button
                        onClick={() => handleDelete(customer.id)}
                        className="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600"
                      >
                        Eliminar
                      </button>
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default CustomerTable;
