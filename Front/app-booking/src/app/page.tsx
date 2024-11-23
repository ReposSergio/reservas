import Image from "next/image";

export default function Home() {
  return (
    <div className="min-h-screen bg-gray-100">
      {/* Hero Section */}
      <div
        className="relative h-screen bg-cover bg-center"
        style={{ backgroundImage: 'url("https://i.ibb.co/ZBzxPtF/holiday-hotel-las-vegas.png")' }} 
      >
        <div className="absolute inset-0 bg-black bg-opacity-50 flex flex-col justify-center items-center text-center px-6">
          <h1 className="text-5xl font-extrabold text-white mb-4">
            Bienvenido al Bookings.com
          </h1>
          <p className="text-lg text-white mb-8">
            Descubra una experiencia de lujo y comodidad en cada detalle.
          </p>
          <div className="space-x-4">
            <a
              href="http://localhost:3000/reserva"
              className="bg-white text-black font-semibold py-2 px-6 rounded-lg hover:bg-gray-200 transition-all"
            >
              Reservaciones
            </a>
            <a
              href="http://localhost:3000/login"
              className="bg-transparent border border-white text-white font-semibold py-2 px-6 rounded-lg hover:bg-white hover:text-black transition-all"
            >
              LogIn
            </a>

            <a
              href="http://localhost:3000/customer/crear"
              className="bg-transparent border border-white text-white font-semibold py-2 px-6 rounded-lg hover:bg-white hover:text-black transition-all"
            >
              SingUp
            </a>
            <a
              href="http://localhost:3000/services"
              className="bg-transparent border border-white text-white font-semibold py-2 px-6 rounded-lg hover:bg-white hover:text-black transition-all"
            >
              Servicios
            </a>
          </div>
        </div>
      </div>

      {/* About Section */}
      <section className="py-16 bg-white">
        <div className="container mx-auto px-6">
          <h2 className="text-4xl font-bold text-center text-gray-800 mb-6">
            Acerca de Nosotros
          </h2>
          <p className="text-lg text-gray-600 text-center max-w-3xl mx-auto">
            Bookings Hotels ofrece un equilibrio perfecto entre diseño moderno y confort, brindando un servicio excepcional y una experiencia inolvidable para nuestros huéspedes.
          </p>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-6 text-center">
          <h2 className="text-4xl font-bold text-gray-800 mb-6">
            Reserve su Próxima Estadía
          </h2>
          <a
            href="http://localhost:3000/reserva"
            className="bg-indigo-600 text-white font-semibold py-3 px-8 rounded-lg hover:bg-indigo-700 transition-all"
          >
            Hacer una Reserva
          </a>
        </div>
      </section>
    </div>
  );
}
