"use client";

import { useState, useEffect } from "react";
import axios from "axios";

interface Reservation {
  id: number;
  clientId: number;
  serviceId: number;
  reservationDate: string;
  notes: string;
  client?: object; // Representación de los datos del cliente
  service?: object; // Representación de los datos del servicio
}

const ReservationList: React.FC = () => {
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [filteredReservations, setFilteredReservations] = useState<Reservation[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editForm, setEditForm] = useState<Partial<Reservation>>({});
  const [searchDate, setSearchDate] = useState<string>(""); // Estado para la fecha de búsqueda
  const [clientId, setClientId] = useState<string | null>(null); // Estado para el clientId

  // Obtener el $id del cliente desde localStorage solo en el cliente
  useEffect(() => {
    if (typeof window !== "undefined") {
      const storedClientId = localStorage.getItem("$id");
      setClientId(storedClientId); // Guardamos el clientId en el estado
    }
  }, []);

  // Obtener reservas al cargar el componente
  useEffect(() => {
    if (!clientId) {
      setError("No se encontró el ID de cliente. Por favor inicie sesión.");
      setLoading(false);
      return;
    }

    const fetchReservations = async () => {
      try {
        const response = await axios.get(`http://localhost:5000/api/reservations/client/${clientId}`);
        const reservationsData = response.data.$values;
        setReservations(reservationsData);
        setFilteredReservations(reservationsData); // Inicializar las reservas filtradas
        setLoading(false);
      } catch (err) {
        console.error("Error fetching reservations:", err);
        setError("Failed to fetch reservations. Please try again later.");
        setLoading(false);
      }
    };

    fetchReservations();
  }, [clientId]);

  // Filtrar las reservas según la fecha de búsqueda
  const handleSearchDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedDate = e.target.value;
    setSearchDate(selectedDate);

    if (selectedDate) {
      const filtered = reservations.filter((reservation) =>
        reservation.reservationDate.startsWith(selectedDate)
      );
      setFilteredReservations(filtered);
    } else {
      setFilteredReservations(reservations); // Si no hay fecha seleccionada, mostrar todas las reservas
    }
  };

  // Manejar la eliminación de una reserva
  const handleDelete = async (id: number) => {
    try {
      await axios.delete(`http://localhost:5000/api/reservations/${id}`);
      setReservations((prev) =>
        prev.filter((reservation) => reservation.id !== id)
      );
      setSuccessMessage("Reservation deleted successfully.");
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err) {
      console.error("Error deleting reservation:", err);
      setError("Failed to delete reservation.");
    }
  };

  // Manejar la edición de una reserva
  const handleEdit = (reservation: Reservation) => {
    setEditingId(reservation.id);
    setEditForm(reservation);
  };

  // Manejar el envío del formulario de edición
  const handleEditSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!editingId) return;

    try {
      const response = await axios.get(
        `http://localhost:5000/api/reservations/${editingId}`
      );
      const fullReservation = response.data;

      const updatedReservation = {
        ...fullReservation,
        clientId: editForm.clientId ?? fullReservation.clientId,
        serviceId: editForm.serviceId ?? fullReservation.serviceId,
        reservationDate: editForm.reservationDate
          ? new Date(editForm.reservationDate).toISOString()
          : fullReservation.reservationDate,
        notes: editForm.notes ?? fullReservation.notes,
      };

      await axios.put(
        `http://localhost:5000/api/reservations/${editingId}`,
        updatedReservation
      );

      setReservations((prev) =>
        prev.map((res) =>
          res.id === editingId ? { ...res, ...updatedReservation } : res
        )
      );

      setEditingId(null);
      setEditForm({});
      setSuccessMessage("Reservation updated successfully.");
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err) {
      console.error("Error editing reservation:", err);
      setError("Failed to edit reservation.");
    }
  };

  return (
    <div className="max-w-4xl mx-auto p-4">
      <h2 className="text-2xl font-bold mb-4">Reservation List</h2>
      {error && <p className="text-red-500">{error}</p>}
      {successMessage && (
        <p className="text-green-500 mb-4">{successMessage}</p>
      )}

      {/* Campo de búsqueda por fecha */}
      <div className="mb-4">
        <label className="block text-sm font-medium">Search by Date</label>
        <input
          type="date"
          value={searchDate}
          onChange={handleSearchDateChange}
          className="mt-1 block w-full p-2 border rounded shadow-sm"
        />
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : filteredReservations.length > 0 ? (
        <table className="min-w-full border-collapse border border-gray-300">
          <thead>
            <tr>
              <th className="border border-gray-300 px-4 py-2">ID</th>
              <th className="border border-gray-300 px-4 py-2">Client ID</th>
              <th className="border border-gray-300 px-4 py-2">Service ID</th>
              <th className="border border-gray-300 px-4 py-2">
                Reservation Date
              </th>
              <th className="border border-gray-300 px-4 py-2">Notes</th>
              <th className="border border-gray-300 px-4 py-2">Actions</th>
            </tr>
          </thead>
          <tbody>
            {filteredReservations.map((reservation) => (
              <tr key={reservation.id}>
                <td className="border border-gray-300 px-4 py-2">
                  {reservation.id}
                </td>
                <td className="border border-gray-300 px-4 py-2">
                  {reservation.clientId}
                </td>
                <td className="border border-gray-300 px-4 py-2">
                  {reservation.serviceId}
                </td>
                <td className="border border-gray-300 px-4 py-2">
                  {new Date(reservation.reservationDate).toLocaleString()}
                </td>
                <td className="border border-gray-300 px-4 py-2">
                  {reservation.notes}
                </td>
                <td className="border border-gray-300 px-4 py-2 space-x-2">
                  <button
                    onClick={() => handleEdit(reservation)}
                    className="bg-yellow-500 text-white px-2 py-1 rounded hover:bg-yellow-600"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(reservation.id)}
                    className="bg-red-500 text-white px-2 py-1 rounded hover:bg-red-600"
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>No reservations found.</p>
      )}

      {editingId && (
        <form
          onSubmit={handleEditSubmit}
          className="mt-4 p-4 border rounded shadow"
        >
          <h3 className="text-lg font-bold mb-2">Edit Reservation</h3>
          {/* ... Formulario de edición */}
        </form>
      )}
    </div>
  );
};

export default ReservationList;
