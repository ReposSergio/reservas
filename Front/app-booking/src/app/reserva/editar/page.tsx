"use client";
import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface Booking {
  id: number;
  customerId: number;
  serviceId: number;
  reservationDate: string;
  notes: string;
}

const BookingsTable: React.FC = () => {
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [editBookingId, setEditBookingId] = useState<number | null>(null);
  const [editedBooking, setEditedBooking] = useState<Booking | null>(null);

  // Obtener todas las reservas (GET)
  useEffect(() => {
    const fetchBookings = async () => {
      try {
        const response = await axios.get<Booking[]>('http://localhost:5000/api/bookings/');
        setBookings(response.data);
        setError(null);
      } catch (err) {
        setError('Error al obtener las reservas.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchBookings();
  }, []);

  // Eliminar una reserva (DELETE)
  const handleDelete = async (id: number) => {
    try {
      await axios.delete(`http://localhost:5000/api/bookings/${id}`);
      setBookings(bookings.filter((booking) => booking.id !== id));
    } catch (err) {
      setError('Error al eliminar la reserva.');
    }
  };

  // Activar el modo de edición
  const handleEditClick = (booking: Booking) => {
    setEditBookingId(booking.id);
    setEditedBooking({ ...booking });
  };

  // Manejar cambios en los inputs de edición
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>, field: keyof Booking) => {
    if (editedBooking) {
      setEditedBooking({ ...editedBooking, [field]: e.target.value });
    }
  };

  // Guardar los cambios (PUT)
  const handleSave = async (id: number) => {
    if (!editedBooking) return;
    try {
      // Convertir la fecha a UTC y formatearla adecuadamente
      const formattedDate = new Date(editedBooking.reservationDate).toISOString(); // Convertir a UTC
      const updatedBooking = { ...editedBooking, reservationDate: formattedDate };

      // Enviar la solicitud PUT
      const response = await axios.put(`http://localhost:5000/api/bookings/${id}`, updatedBooking);

      // Actualizar la lista de reservas localmente
      setBookings(
        bookings.map((booking) => (booking.id === id ? { ...updatedBooking } : booking))
      );
      setEditBookingId(null);
      setEditedBooking(null);
    } catch (err) {
      setError('Error al actualizar la reserva.');
    }
  };

  if (isLoading) return <p className="text-center text-lg">Cargando...</p>;
  if (error) return <p className="text-center text-red-500">{error}</p>;

  return (
    <div className="max-w-4xl mx-auto p-4 bg-white rounded-lg shadow-lg">
      <h2 className="text-2xl font-bold text-center mb-4">Lista de Reservas</h2>
      <table className="min-w-full table-auto border-collapse">
        <thead>
          <tr className="bg-gray-100">
            <th className="py-2 px-4 text-left">ID</th>
            <th className="py-2 px-4 text-left">Cliente</th>
            <th className="py-2 px-4 text-left">Servicio</th>
            <th className="py-2 px-4 text-left">Fecha</th>
            <th className="py-2 px-4 text-left">Notas</th>
            <th className="py-2 px-4 text-left">Acciones</th>
          </tr>
        </thead>
        <tbody>
          {bookings.map((booking) => (
            <tr key={booking.id} className="border-t">
              <td className="py-2 px-4">{booking.id}</td>
              <td className="py-2 px-4">
                {editBookingId === booking.id ? (
                  <input
                    type="number"
                    value={editedBooking?.customerId || ''}
                    onChange={(e) => handleInputChange(e, 'customerId')}
                    className="block w-full p-1 border rounded"
                  />
                ) : (
                  booking.customerId
                )}
              </td>
              <td className="py-2 px-4">
                {editBookingId === booking.id ? (
                  <input
                    type="number"
                    value={editedBooking?.serviceId || ''}
                    onChange={(e) => handleInputChange(e, 'serviceId')}
                    className="block w-full p-1 border rounded"
                  />
                ) : (
                  booking.serviceId
                )}
              </td>
              <td className="py-2 px-4">
                {editBookingId === booking.id ? (
                  <input
                    type="datetime-local"
                    value={editedBooking?.reservationDate.slice(0, 16) || ''}
                    onChange={(e) => handleInputChange(e, 'reservationDate')}
                    className="block w-full p-1 border rounded"
                  />
                ) : (
                  new Date(booking.reservationDate).toLocaleString()
                )}
              </td>
              <td className="py-2 px-4">
                {editBookingId === booking.id ? (
                  <textarea
                    value={editedBooking?.notes || ''}
                    onChange={(e) => handleInputChange(e, 'notes')}
                    className="block w-full p-1 border rounded"
                  />
                ) : (
                  booking.notes
                )}
              </td>
              <td className="py-2 px-4 flex space-x-2">
                {editBookingId === booking.id ? (
                  <button
                    className="bg-green-500 text-white py-1 px-3 rounded hover:bg-green-600"
                    onClick={() => handleSave(booking.id)}
                  >
                    Guardar
                  </button>
                ) : (
                  <button
                    className="bg-yellow-500 text-white py-1 px-3 rounded hover:bg-yellow-600"
                    onClick={() => handleEditClick(booking)}
                  >
                    Editar
                  </button>
                )}
                <button
                  className="bg-red-500 text-white py-1 px-3 rounded hover:bg-red-600"
                  onClick={() => handleDelete(booking.id)}
                >
                  Eliminar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default BookingsTable;
