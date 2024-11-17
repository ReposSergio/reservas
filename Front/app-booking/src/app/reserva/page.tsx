"use client";

import { useState } from "react";
import axios from "axios";

const ReservationForm: React.FC = () => {
  const [clientId, setClientId] = useState<number | null>(null);
  const [serviceId, setServiceId] = useState<number | null>(null);
  const [reservationDate, setReservationDate] = useState<string>("");
  const [notes, setNotes] = useState<string>("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const response = await axios.post(
        "http://localhost:5000/api/reservations",
        {
          clientId,
          serviceId,
          reservationDate,
          notes,
        }
      );

      console.log("Reservation created successfully:", response.data);
      // Puedes manejar la respuesta exitosa aquí, como limpiar el formulario o mostrar un mensaje de éxito.
    } catch (error) {
      console.error("Error creating reservation:", error);
      // Maneja el error, muestra un mensaje de error o realiza acciones adicionales.
    }
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="max-w-md mx-auto p-4 border rounded-lg shadow-lg space-y-4"
    >
      <div>
        <label
          htmlFor="clientId"
          className="block text-sm font-medium text-gray-700"
        >
          Client ID
        </label>
        <input
          type="number"
          id="clientId"
          value={clientId || ""}
          onChange={(e) => setClientId(parseInt(e.target.value))}
          className="mt-1 block w-full p-2 border rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
          required
        />
      </div>

      <div>
        <label
          htmlFor="serviceId"
          className="block text-sm font-medium text-gray-700"
        >
          Service ID
        </label>
        <input
          type="number"
          id="serviceId"
          value={serviceId || ""}
          onChange={(e) => setServiceId(parseInt(e.target.value))}
          className="mt-1 block w-full p-2 border rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
          required
        />
      </div>

      <div>
        <label
          htmlFor="reservationDate"
          className="block text-sm font-medium text-gray-700"
        >
          Reservation Date
        </label>
        <input
          type="datetime-local"
          id="reservationDate"
          value={reservationDate}
          onChange={(e) => setReservationDate(e.target.value)}
          className="mt-1 block w-full p-2 border rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
          required
        />
      </div>

      <div>
        <label
          htmlFor="notes"
          className="block text-sm font-medium text-gray-700"
        >
          Notes
        </label>
        <textarea
          id="notes"
          value={notes}
          onChange={(e) => setNotes(e.target.value)}
          className="mt-1 block w-full p-2 border rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
          rows={3}
        ></textarea>
      </div>

      <button
        type="submit"
        className="w-full m-1 bg-indigo-600  text-white py-2 px-4 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500"
      >
        Create Reservation
      </button>
      <a
        type="submit"
        className=" block w-full bg-indigo-600 text-center text-white py-2 px-4  m-1 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500"
      >
        Edit Reservation
      </a>
    </form>
  );
};

export default ReservationForm;
