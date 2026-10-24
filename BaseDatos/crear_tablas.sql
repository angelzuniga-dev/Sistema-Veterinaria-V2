PRAGMA foreign_keys = ON;

-- Usuarios del sistema (Administrador, Veterinario, Recepcionista).
-- Especialidad y NumeroColegiado solo aplican al rol Veterinario.
CREATE TABLE IF NOT EXISTS Usuarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    UsuarioLogin TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Rol TEXT NOT NULL CHECK (Rol IN ('Administrador', 'Veterinario', 'Recepcionista')),
    Especialidad TEXT NULL,
    NumeroColegiado TEXT NULL UNIQUE,
    CHECK (Rol <> 'Veterinario' OR (Especialidad IS NOT NULL AND NumeroColegiado IS NOT NULL))
);

-- Duenios de las mascotas
CREATE TABLE IF NOT EXISTS Clientes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Telefono TEXT NOT NULL,
    Direccion TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE
);

-- Pacientes de la clinica; cada mascota pertenece a un cliente
CREATE TABLE IF NOT EXISTS Mascotas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ClienteId INTEGER NOT NULL,
    Nombre TEXT NOT NULL,
    Especie TEXT NOT NULL,
    Raza TEXT NOT NULL,
    Edad INTEGER NOT NULL CHECK (Edad >= 0),
    Peso REAL NOT NULL CHECK (Peso > 0),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE
);

-- Relacion 1 a 1 con Mascotas: se crea junto con la mascota
CREATE TABLE IF NOT EXISTS HistoriasClinicas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MascotaId INTEGER NOT NULL UNIQUE,
    Antecedentes TEXT NOT NULL DEFAULT '',
    Observaciones TEXT NOT NULL DEFAULT '',
    FOREIGN KEY (MascotaId) REFERENCES Mascotas(Id) ON DELETE CASCADE
);

-- Inventario de medicamentos
CREATE TABLE IF NOT EXISTS Medicamentos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL UNIQUE,
    Stock INTEGER NOT NULL CHECK (Stock >= 0),
    PrecioUnitario REAL NOT NULL CHECK (PrecioUnitario >= 0)
);

-- Agenda: una mascota con un veterinario en fecha y hora
CREATE TABLE IF NOT EXISTS Citas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MascotaId INTEGER NOT NULL,
    VeterinarioId INTEGER NOT NULL,
    Fecha TEXT NOT NULL,
    Hora TEXT NOT NULL,
    Estado TEXT NOT NULL DEFAULT 'Pendiente' CHECK (Estado IN ('Pendiente', 'Cancelada', 'Atendida')),
    FOREIGN KEY (MascotaId) REFERENCES Mascotas(Id) ON DELETE CASCADE,
    FOREIGN KEY (VeterinarioId) REFERENCES Usuarios(Id) ON DELETE RESTRICT
);

-- Atencion medica derivada de una cita (una consulta por cita)
CREATE TABLE IF NOT EXISTS Consultas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CitaId INTEGER NOT NULL UNIQUE,
    HistoriaClinicaId INTEGER NOT NULL,
    Fecha TEXT NOT NULL,
    Motivo TEXT NOT NULL,
    Diagnostico TEXT NOT NULL DEFAULT '',
    FOREIGN KEY (CitaId) REFERENCES Citas(Id) ON DELETE CASCADE,
    FOREIGN KEY (HistoriaClinicaId) REFERENCES HistoriasClinicas(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Tratamientos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ConsultaId INTEGER NOT NULL,
    Descripcion TEXT NOT NULL,
    FechaInicio TEXT NOT NULL,
    FechaFin TEXT NOT NULL,
    CHECK (date(FechaFin) >= date(FechaInicio)),
    FOREIGN KEY (ConsultaId) REFERENCES Consultas(Id) ON DELETE CASCADE
);

-- Medicamentos prescritos dentro de un tratamiento
CREATE TABLE IF NOT EXISTS DetallesTratamiento (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TratamientoId INTEGER NOT NULL,
    MedicamentoId INTEGER NOT NULL,
    Dosis TEXT NOT NULL,
    Cantidad INTEGER NOT NULL CHECK (Cantidad > 0),
    FOREIGN KEY (TratamientoId) REFERENCES Tratamientos(Id) ON DELETE CASCADE,
    FOREIGN KEY (MedicamentoId) REFERENCES Medicamentos(Id) ON DELETE RESTRICT
);

-- Una factura por consulta
CREATE TABLE IF NOT EXISTS Facturas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ClienteId INTEGER NOT NULL,
    ConsultaId INTEGER NOT NULL UNIQUE,
    Fecha TEXT NOT NULL,
    Total REAL NOT NULL CHECK (Total >= 0),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE,
    FOREIGN KEY (ConsultaId) REFERENCES Consultas(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS DetallesFactura (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FacturaId INTEGER NOT NULL,
    Concepto TEXT NOT NULL,
    Cantidad INTEGER NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario REAL NOT NULL CHECK (PrecioUnitario >= 0),
    Subtotal REAL NOT NULL CHECK (Subtotal >= 0),
    FOREIGN KEY (FacturaId) REFERENCES Facturas(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_Mascotas_ClienteId ON Mascotas(ClienteId);
CREATE INDEX IF NOT EXISTS IX_Citas_MascotaId ON Citas(MascotaId);
CREATE INDEX IF NOT EXISTS IX_Citas_VeterinarioId ON Citas(VeterinarioId);
CREATE INDEX IF NOT EXISTS IX_Tratamientos_ConsultaId ON Tratamientos(ConsultaId);
CREATE INDEX IF NOT EXISTS IX_DetallesTratamiento_TratamientoId ON DetallesTratamiento(TratamientoId);
CREATE INDEX IF NOT EXISTS IX_DetallesFactura_FacturaId ON DetallesFactura(FacturaId);
