"use client";
import { useEffect, useState } from "react";
import axios from "axios";

// Interfaces para los datos
interface Reservation {
  id: number;
  clientId: number;
  serviceId: number;
  reservationDate: string;
  notes: string;
  client: {
    id: number;
    name: string;
    email: string;
  };
}

interface Service {
  id: number;
  name: string;
  price: number;
  reservations: Reservation[];
}

const ServicesList = () => {
  const [services, setServices] = useState<Service[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // Estados para edición
  const [editingService, setEditingService] = useState<Service | null>(null);
  const [name, setName] = useState<string>("");
  const [price, setPrice] = useState<number>(0);

  const API_BASE_URL = "http://localhost:5000/api/services";

  // Función para obtener los servicios desde el back-end
  const fetchServices = async () => {
    setLoading(true);
    try {
      const response = await axios.get(API_BASE_URL);

      // Transformar los datos recibidos para ajustarse a la estructura esperada
      const transformedData = response.data.$values.map((service: any) => ({
        id: service.id,
        name: service.name,
        price: service.price,
        reservations: service.reservations.$values.map((reservation: any) => ({
          id: reservation.id,
          clientId: reservation.clientId,
          serviceId: reservation.serviceId,
          reservationDate: reservation.reservationDate,
          notes: reservation.notes,
          client: {
            id: reservation.client.id,
            name: reservation.client.name,
            email: reservation.client.email,
          },
        })),
      }));

      setServices(transformedData);
    } catch (err) {
      setError("Error al cargar los servicios. Inténtalo de nuevo.");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchServices();
  }, []);

  // Manejo de eliminación
  const handleDelete = async (id: number) => {
    try {
      await axios.delete(`${API_BASE_URL}/${id}`);
      setServices(services.filter((service) => service.id !== id));
    } catch (err) {
      setError("Error al eliminar el servicio. Inténtalo de nuevo.");
    }
  };

  // Manejo de edición
  const handleEdit = (service: Service) => {
    setEditingService(service);
    setName(service.name);
    setPrice(service.price);
  };

  // Manejo de actualización
  const handleUpdate = async () => {
    if (!editingService) return;

    try {
      const updatedService = { ...editingService, name, price };
      await axios.put(`${API_BASE_URL}/${editingService.id}`, updatedService);

      setServices(
        services.map((service) =>
          service.id === editingService.id ? updatedService : service
        )
      );

      // Resetear el formulario de edición
      setEditingService(null);
      setName("");
      setPrice(0);
    } catch (err) {
      setError("Error al actualizar el servicio. Inténtalo de nuevo.");
    }
  };

  // Renderizado del componente
  if (loading) return <p className="text-center text-gray-500">Cargando servicios...</p>;
  if (error) return <p className="text-center text-red-500">{error}</p>;

  return (
    <div className="p-4">
      <h1 className="text-xl font-bold mb-4 text-center">Servicios Disponibles</h1>
      {services.length > 0 ? (
        <ul className="space-y-4">
          {services.map((service) => (
            <li
              key={service.id}
              className="bg-white shadow-md rounded-lg p-4 border border-gray-200"
            >
              <h2 className="text-lg font-semibold">{service.name}</h2>
              <p className="text-gray-600">Precio: ${service.price}</p>

              {/* Mostrar las reservas relacionadas */}
              <div className="mt-4">
                <h3 className="text-sm font-semibold text-gray-800">Reservas:</h3>
                {service.reservations.length > 0 ? (
                  <ul className="list-disc list-inside text-gray-600">
                    {service.reservations.map((reservation) => (
                      <li key={reservation.id}>
                        Fecha: {new Date(reservation.reservationDate).toLocaleString()} - Cliente:{" "}
                        {reservation.client.name} ({reservation.client.email})
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p className="text-sm text-gray-500">No hay reservas para este servicio.</p>
                )}
              </div>

              <div className="mt-4 flex space-x-2">
                <button
                  onClick={() => handleEdit(service)}
                  className="bg-blue-500 text-white py-1 px-4 rounded hover:bg-blue-600"
                >
                  Editar
                </button>
                <button
                  onClick={() => handleDelete(service.id)}
                  className="bg-red-500 text-white py-1 px-4 rounded hover:bg-red-600"
                >
                  Eliminar
                </button>
              </div>
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-center text-gray-500">No hay servicios disponibles.</p>
      )}

      {editingService && (
        <div className="mt-8 p-4 bg-gray-100 rounded-lg">
          <h2 className="text-lg font-semibold mb-4">Editar Servicio</h2>
          <form
            onSubmit={(e) => {
              e.preventDefault();
              handleUpdate();
            }}
          >
            <div className="mb-4">
              <label
                htmlFor="name"
                className="block text-gray-700 font-medium mb-2"
              >
                Nombre del Servicio
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
              <label
                htmlFor="price"
                className="block text-gray-700 font-medium mb-2"
              >
                Precio del Servicio
              </label>
              <input
                type="number"
                id="price"
                value={price}
                onChange={(e) => setPrice(parseFloat(e.target.value))}
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
                onClick={() => setEditingService(null)}
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

export default ServicesList;
