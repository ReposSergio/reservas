import Image from "next/image";

export default function Home() {
  return (
    <div>
      <div className="min-h-screen flex flex-col items-center justify-center bg-cover bg-center" style={{ backgroundImage: 'url("/path-to-your-hotel-background.jpg")' }}>
      <div className="bg-black bg-opacity-50 text-white p-8 rounded-md shadow-lg max-w-lg text-center">
        <h1 className="text-4xl font-bold mb-4">Bienvenido a Nuestro Hotel</h1>
        <div className="text-lg mb-6">
          Disfruta de una estancia de lujo con nosotros, donde el confort y la excelencia se encuentran.
        </div>
        <a
          href="http://localhost:3000/reserva"
          className="bg-indigo-600 text-white py-2 px-4 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500"
        >
          Recervaciones
        </a>
        <a
          href="http://localhost:3000/reserva"
          className="bg-indigo-600 text-white py-2 px-4 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500"
        >
          Cliente
        </a>
        <a
          href="http://localhost:3000/reserva"
          className="bg-indigo-600 text-white py-2 px-4 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500"
        >
          Servicios
        </a>
      </div>
    </div>
    </div>
  );
}
