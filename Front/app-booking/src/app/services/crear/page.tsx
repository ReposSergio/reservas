"use client";
import { useState } from "react";
import axios from "axios";

const ServiceForm = () => {
  const [name, setName] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const API_BASE_URL = "http://localhost:5000/api/Service";

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!name.trim()) {
      setErrorMessage("El nombre del servicio es obligatorio.");
      return;
    }

    setLoading(true);
    setErrorMessage(null);
    setSuccessMessage(null);

    try {
      await axios.post(API_BASE_URL, { name, description });
      setSuccessMessage("El servicio se ha creado con éxito.");
      setName("");
      setDescription("");
    } catch (error) {
      setErrorMessage(
        "Ocurrió un error al crear el servicio. Por favor, inténtalo nuevamente."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4 max-w-md mx-auto">
      <h1 className="text-xl font-bold mb-4 text-center">Crear Nuevo Servicio</h1>
      <form onSubmit={handleSubmit} className="bg-white shadow-md rounded-lg p-6 border border-gray-200">
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
            placeholder="Ejemplo: Servicio A"
          />
        </div>
        <div className="mb-4">
          <label
            htmlFor="description"
            className="block text-gray-700 font-medium mb-2"
          >
            Descripción del Servicio
          </label>
          <textarea
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="w-full border-gray-300 rounded-lg shadow-sm focus:ring-blue-500 focus:border-blue-500"
            rows={3}
            placeholder="Describe el servicio (opcional)"
          ></textarea>
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
    </div>
  );
};

export default ServiceForm;
