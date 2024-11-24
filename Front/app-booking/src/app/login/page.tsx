"use client";

import React, { useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";  

interface LoginData {
  email: string;
  password: string;
}

interface LoginResponse {
  Name: string;
  Email: string;
  Token: string;
  Message: string;
}

const Login = () => {
  const [loginData, setLoginData] = useState<LoginData>({
    email: "",
    password: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const router = useRouter();  // Hook para redirección después del login

  // Manejo de cambios en los inputs
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setLoginData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  // Manejo de la sumisión del formulario
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null); // Limpiar cualquier error anterior

    try {
      const response = await axios.post<LoginResponse>(
        "http://localhost:5000/api/authentication/login",
        loginData
      );

      if (response.data) {
        console.log("Login exitoso:", response.data);

        // Guardar el token en el almacenamiento local
        localStorage.setItem("token", response.data.Token);

        // Redirigir a la página de inicio o dashboard
        router.push("/customer");

        console.log("Token guardado en localStorage:", localStorage.getItem("token"));
      }
    } catch (err: any) {
      setError(err.response?.data?.message || "Error al iniciar sesión. Verifica tus credenciales.");
      console.error("Error al hacer login:", err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex justify-center items-center bg-gray-100">
      <div className="w-full max-w-md p-6 bg-white shadow-md rounded-lg">
        <h2 className="text-2xl font-semibold text-center mb-4">
          Iniciar Sesión
        </h2>
        {error && <div className="text-red-500 text-center mb-4">{error}</div>}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label
              htmlFor="email"
              className="block text-sm font-medium text-gray-700"
            >
              Correo Electrónico
            </label>
            <input
              type="email"
              id="email"
              name="email"
              value={loginData.email}
              onChange={handleInputChange}
              required
              className="mt-1 block w-full px-4 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
          <div>
            <label
              htmlFor="password"
              className="block text-sm font-medium text-gray-700"
            >
              Contraseña
            </label>
            <input
              type="password"
              id="password"
              name="password"
              value={loginData.password}
              onChange={handleInputChange}
              required
              className="mt-1 block w-full px-4 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
          <button
            type="submit"
            disabled={isLoading}
            className="w-full py-2 px-4 bg-blue-500 text-white rounded-lg shadow-md hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            {isLoading ? "Cargando..." : "Iniciar Sesión"}
          </button>
          <a 
          href="http://localhost:3000/customer/crear" 
          className="block  ml-1 w-full py-2 text-center bg-blue-500 text-white rounded-lg hover:bg-blue-600  focus:ring-2 focus:ring-blue-500 ">
          Crear Cuenta
          </a>
        </form>
      </div>
    </div>
  );
};

export default Login;
