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
      setBookings(bookings.filter((booking) => booking.id !== id)); // Actualiza el estado después de eliminar
    } catch (err) {
      setError('Error al eliminar la reserva.');
    }
  };

  // Editar una reserva (PUT)
  const handleEdit = async (id: number, updatedBooking: Booking) => {
    try {
      const response = await axios.put(`http://localhost:5000/api/bookings/${id}`, updatedBooking);
      setBookings(
        bookings.map((booking) =>
          booking.id === id ? { ...booking, ...response.data } : booking
        )
      );
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
              <td className="py-2 px-4">{booking.customerId}</td>
              <td className="py-2 px-4">{booking.serviceId}</td>
              <td className="py-2 px-4">{new Date(booking.reservationDate).toLocaleString()}</td>
              <td className="py-2 px-4">{booking.notes}</td>
              <td className="py-2 px-4 flex space-x-2">
                {/* Botón de Editar */}
                <button
                  className="bg-yellow-500 text-white py-1 px-3 rounded hover:bg-yellow-600"
                  onClick={() => handleEdit(booking.id, { ...booking, notes: 'Notas actualizadas' })}
                >
                  Editar
                </button>
                {/* Botón de Eliminar */}
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

