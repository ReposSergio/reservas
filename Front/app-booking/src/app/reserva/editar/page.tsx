"use client";

import React, { useEffect, useState } from "react";
import axios from "axios";

interface Reservation {
  id: number;
  customerId: number;
  serviceId: number;
  reservationDate: string;
  notes: string;
}

const Reservation: React.FC = () => {
  const [reservation, setReservation] = useState<Reservation | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchReservation = async () => {
      try {
        const response = await axios.get<Reservation>(
          "http://localhost:3000/api/reservation/1"
        );
        setReservation(response.data);
        setError(null);
      } catch (err) {
        setError("Error fetching reservation data");
      } finally {
        setIsLoading(false);
      }
    };

    fetchReservation();
  }, []);

  const handleUpdate = async () => {
    if (!reservation) return;

    try {
      const updatedData = {
        ...reservation,
        notes: "Detalles actualizados de la reserva.",
      };

      const response = await axios.put(
        `http://localhost:3000/api/reservation/${reservation.id}`,
        updatedData
      );
      setReservation(response.data);
      alert("Reserva actualizada correctamente.");
    } catch (err) {
      setError("Error al actualizar la reserva.");
    }
  };

  if (isLoading) return <p>Cargando...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div>
      <h2>Reserva</h2>
      {reservation && (
        <div>
          <p>ID: {reservation.id}</p>
          <p>ID del Cliente: {reservation.customerId}</p>
          <p>ID del Servicio: {reservation.serviceId}</p>
          <p>
            Fecha de la Reserva:{" "}
            {new Date(reservation.reservationDate).toLocaleString()}
          </p>
          <p>Notas: {reservation.notes}</p>
          <button onClick={handleUpdate}>Actualizar Reserva</button>
        </div>
      )}
    </div>
  );
};

export default Reservation;
