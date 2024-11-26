# Proyecto Prueba Reservas Hoteleras

## Apis

## POST /api/bookings -> Crear una nueva reserva
## GET /api/bookings/{id} -> Obtener una reserva por su ID
## PUT /api/bookings/{id} -> Actualizar una reserva existente
## DELETE /api/bookings/{id} -> Eliminar una reserva

#Código	Descripción	Ejemplo de Respuesta
200 OK	Autenticación exitosa. Devuelve un token JWT válido.	json { "token": "eyJhbGciOiJIUzI1NiIsInR5cCI..." }
400 Bad Request	La solicitud contiene datos inválidos o faltan campos obligatorios.	json { "message": "Email y contraseña son requeridos." }
401 Unauthorized	Las credenciales proporcionadas no son válidas.	json { "message": "Invalid credentials" }
500 Internal Server Error	Ocurrió un error inesperado en el servidor al procesar la solicitud.	json { "message": "An unexpected error occurred. Please try again later.", "details": "Descripción del error." }

