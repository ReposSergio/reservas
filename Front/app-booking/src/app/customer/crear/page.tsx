"use client";
import { useState, useEffect } from "react";
import axios from "axios";
import * as bcrypt from "bcryptjs";

interface Reservation {
  id: number;
  reservationDate: string;
  notes: string;
}

interface Client {
  id: number;
  name: string;
  email: string;
  reservations: Reservation[];
}

const CustomerForm = () => {
  const [clients, setClients] = useState<Client[]>([]);
  const [name, setName] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const API_BASE_URL = "http://localhost:5000/api/clients";

  // Obtener la lista de clientes
  const fetchClients = async () => {
    try {
      const response = await axios.get<{ $values: Client[] }>(API_BASE_URL);
      setClients(response.data.$values);
    } catch (error) {
      setErrorMessage("Error al cargar los clientes.");
    }
  };

  useEffect(() => {
    fetchClients();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!name.trim() || !email.trim() || !password.trim()) {
      setErrorMessage("El nombre, correo electrónico y contraseña son obligatorios.");
      return;
    }

    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(email)) {
      setErrorMessage("El correo electrónico no tiene un formato válido.");
      return;
    }

    const hashedPassword = bcrypt.hashSync(password, 10);

    setLoading(true);
    setErrorMessage(null);
    setSuccessMessage(null);

    try {
      // Realizando el POST con el nuevo cliente y un arreglo vacío de reservas
      await axios.post(API_BASE_URL, { 
        name, 
        email, 
        password: hashedPassword, // Enviar la contraseña ya hasheada
        reservations: [] 
      });
      setSuccessMessage("Usuario se ha creado con éxito.");
      setName("");
      setEmail("");
      setPassword("");
      fetchClients(); // Actualizar la lista de clientes
    } catch (error) {
      setErrorMessage("Ocurrió un error al crear el Usuario.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4 max-w-md mx-auto">
      <h1 className="text-xl font-bold mb-4 text-center">Crear Cuenta</h1>
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
            Nombre del Usuario
          </label>
          <input
            type="text"
            id="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full border-gray-300 rounded-lg shadow-sm focus:ring-blue-500 focus:border-blue-500"
            placeholder="Ejemplo: Cliente A"
          />
        </div>
        <div className="mb-4">
          <label
            htmlFor="email"
            className="block text-gray-700 font-medium mb-2"
          >
            Correo Electrónico
          </label>
          <input
            type="email"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full border-gray-300 rounded-lg shadow-sm focus:ring-blue-500 focus:border-blue-500"
            placeholder="ejemplo@correo.com"
          />
        </div>
        <div className="mb-4">
          <label
            htmlFor="password"
            className="block text-gray-700 font-medium mb-2"
          >
            Contraseña
          </label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full border-gray-300 rounded-lg shadow-sm focus:ring-blue-500 focus:border-blue-500"
            placeholder="********"
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
          {loading ? "Creando..." : "Crear Cuenta"}
        </button>
      </form>

      {/* Listado de clientes */}
      <h2 className="text-xl font-bold mt-8 mb-4 text-center">Clientes Registrados</h2>
      <ul className="space-y-4">
        {clients.map((client) => (
          <li
            key={client.id}
            className="bg-white shadow-md rounded-lg p-4 border border-gray-200"
          >
            <h3 className="text-lg font-semibold">{client.name}</h3>
            <p className="text-gray-600">Correo: {client.email}</p>
            {client.reservations.length > 0 ? (
              <ul className="mt-2 list-disc list-inside text-gray-600">
                {client.reservations.map((reservation) => (
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

export default CustomerForm;
