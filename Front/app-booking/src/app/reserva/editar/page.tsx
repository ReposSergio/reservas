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
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editForm, setEditForm] = useState<Partial<Reservation>>({});

  // Obtener reservas al cargar el componente
  useEffect(() => {
    const fetchReservations = async () => {
      try {
        const response = await axios.get("http://localhost:5000/api/reservations");
        setReservations(response.data.$values);
        setLoading(false);
      } catch (err) {
        console.error("Error fetching reservations:", err);
        setError("Failed to fetch reservations. Please try again later.");
        setLoading(false);
      }
    };

    fetchReservations();
  }, []);

  // Manejar la eliminación de una reserva
  const handleDelete = async (id: number) => {
    try {
      await axios.delete(`http://localhost:5000/api/reservations/${id}`);
      setReservations((prev) => prev.filter((reservation) => reservation.id !== id));
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
      const response = await axios.get(`http://localhost:5000/api/reservations/${editingId}`);
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

      await axios.put(`http://localhost:5000/api/reservations/${editingId}`, updatedReservation);

      setReservations((prev) =>
        prev.map((res) => (res.id === editingId ? { ...res, ...updatedReservation } : res))
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
      {successMessage && <p className="text-green-500 mb-4">{successMessage}</p>}
      {loading ? (
        <p>Loading...</p>
      ) : reservations.length > 0 ? (
        <table className="min-w-full border-collapse border border-gray-300">
          <thead>
            <tr>
              <th className="border border-gray-300 px-4 py-2">ID</th>
              <th className="border border-gray-300 px-4 py-2">Client ID</th>
              <th className="border border-gray-300 px-4 py-2">Service ID</th>
              <th className="border border-gray-300 px-4 py-2">Reservation Date</th>
              <th className="border border-gray-300 px-4 py-2">Notes</th>
              <th className="border border-gray-300 px-4 py-2">Actions</th>
            </tr>
          </thead>
          <tbody>
            {reservations.map((reservation) => (
              <tr key={reservation.id}>
                <td className="border border-gray-300 px-4 py-2">{reservation.id}</td>
                <td className="border border-gray-300 px-4 py-2">{reservation.clientId}</td>
                <td className="border border-gray-300 px-4 py-2">{reservation.serviceId}</td>
                <td className="border border-gray-300 px-4 py-2">
                  {new Date(reservation.reservationDate).toLocaleString()}
                </td>
                <td className="border border-gray-300 px-4 py-2">{reservation.notes}</td>
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
        <form onSubmit={handleEditSubmit} className="mt-4 p-4 border rounded shadow">
          <h3 className="text-lg font-bold mb-2">Edit Reservation</h3>
          <div className="mb-4">
            <label className="block text-sm font-medium">Client ID</label>
            <input
              type="number"
              value={editForm.clientId || ""}
              onChange={(e) =>
                setEditForm((prev) => ({ ...prev, clientId: parseInt(e.target.value) }))
              }
              className="mt-1 block w-full p-2 border rounded shadow-sm"
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium">Service ID</label>
            <input
              type="number"
              value={editForm.serviceId || ""}
              onChange={(e) =>
                setEditForm((prev) => ({ ...prev, serviceId: parseInt(e.target.value) }))
              }
              className="mt-1 block w-full p-2 border rounded shadow-sm"
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium">Reservation Date</label>
            <input
              type="datetime-local"
              value={editForm.reservationDate || ""}
              onChange={(e) =>
                setEditForm((prev) => ({ ...prev, reservationDate: e.target.value }))
              }
              className="mt-1 block w-full p-2 border rounded shadow-sm"
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium">Notes</label>
            <textarea
              value={editForm.notes || ""}
              onChange={(e) => setEditForm((prev) => ({ ...prev, notes: e.target.value }))}
              className="mt-1 block w-full p-2 border rounded shadow-sm"
            />
          </div>
          <button
            type="submit"
            className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
          >
            Save Changes
          </button>
          <button
            type="button"
            onClick={() => setEditingId(null)}
            className="ml-2 bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
          >
            Cancel
          </button>
        </form>
      )}
    </div>
  );
};

export default ReservationList;
