"use client";
import { useState, useEffect } from "react";
import axios from "axios";

interface Reservation {
  id: number;
  reservationDate: string;
  notes: string;
}

interface Service {
  id: number;
  name: string;
  price: number;
  reservations: Reservation[];
}

const ServiceForm = () => {
  const [services, setServices] = useState<Service[]>([]);
  const [name, setName] = useState<string>("");
  const [price, setPrice] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const API_BASE_URL = "http://localhost:5000/api/services";

  // Obtener la lista de servicios
  const fetchServices = async () => {
    try {
      const response = await axios.get<{ $values: Service[] }>(API_BASE_URL);
      setServices(response.data.$values);
    } catch (error) {
      setErrorMessage("Error al cargar los servicios.");
    }
  };

  useEffect(() => {
    fetchServices();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!name.trim() || price <= 0) {
      setErrorMessage("El nombre y el precio son obligatorios y el precio debe ser mayor a 0.");
      return;
    }

    setLoading(true);
    setErrorMessage(null);
    setSuccessMessage(null);

    try {
      // Realizando el POST con el nuevo servicio y un arreglo vacío de reservas
      await axios.post(API_BASE_URL, { 
        name, 
        price, 
        reservations: [] 
      });
      setSuccessMessage("El servicio se ha creado con éxito.");
      setName("");
      setPrice(0);
      fetchServices(); // Actualizar la lista de servicios
    } catch (error) {
      setErrorMessage("Ocurrió un error al crear el servicio.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4 max-w-md mx-auto">
      <h1 className="text-xl font-bold mb-4 text-center">Crear Nuevo Servicio</h1>
      <form
        onSubmit={handleSubmit}
        className="bg-white shadow-md rounded-lg p-6 border border-gray-200"
      >
        {successMessage && (
          <p className="text-green-500 text-center mb-4">{successMessage}</p>
        )}
        {errorMessage && (
          <p className="text-red-500 text-center mb-4">{errorMessage}</p>
        )}
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
            placeholder="Ejemplo: Todo incluido"
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
            onChange={(e) => setPrice(Number(e.target.value))}
            className="w-full border-gray-300 rounded-lg shadow-sm focus:ring-blue-500 focus:border-blue-500"
            placeholder="Ejemplo: 3000.00"
          />
        </div>
        <button
          type="submit"
          disabled={loading}
          className={`w-full py-2 px-4 font-bold text-white rounded-lg ${
            loading
              ? "bg-gray-400 cursor-not-allowed"
              : "bg-blue-500 hover:bg-blue-600"
          }`}
        >
          {loading ? "Creando..." : "Crear Servicio"}
        </button>
      </form>

      {/* Listado de servicios */}
      <h2 className="text-xl font-bold mt-8 mb-4 text-center">Servicios Registrados</h2>
      <ul className="space-y-4">
        {services.map((service) => (
          <li
            key={service.id}
            className="bg-white shadow-md rounded-lg p-4 border border-gray-200"
          >
            <h3 className="text-lg font-semibold">{service.name}</h3>
            <p className="text-gray-600">Precio: ${service.price.toFixed(2)}</p>
            {service.reservations.length > 0 ? (
              <ul className="mt-2 list-disc list-inside text-gray-600">
                {service.reservations.map((reservation) => (
                  <li key={reservation.id}>
                    Fecha: {new Date(reservation.reservationDate).toLocaleString()} - Notas: {reservation.notes}
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-sm text-gray-500">No hay reservas asociadas.</p>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ServiceForm;
