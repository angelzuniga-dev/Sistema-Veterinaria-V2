using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using SistemaVeterinaria.Dominio;

namespace SistemaVeterinaria.Persistencia;

internal sealed class RepositorioVeterinaria
{
    private const string FormatoFecha = "yyyy-MM-dd";
    private readonly string _cadenaConexion;

    public RepositorioVeterinaria(string cadenaConexion) => _cadenaConexion = cadenaConexion;

    // ---------------------------------------------------------------
    // Usuarios e inicio de sesion
    // ---------------------------------------------------------------
    public DataTable ObtenerUsuarios() => Consultar(
        "SELECT Id, Nombre, UsuarioLogin, Rol, Especialidad, NumeroColegiado FROM Usuarios ORDER BY Rol, Nombre;");

    public DataTable ObtenerVeterinarios() => Consultar(
        "SELECT Id, Nombre, Especialidad, NumeroColegiado FROM Usuarios WHERE Rol = 'Veterinario' ORDER BY Nombre;");

    public int CrearUsuario(Usuario usuario)
    {
        var veterinario = usuario as Veterinario;
        return Insertar(@"INSERT INTO Usuarios (Nombre, UsuarioLogin, PasswordHash, Rol, Especialidad, NumeroColegiado)
            VALUES ($nombre, $login, $hash, $rol, $especialidad, $colegiado);",
            ("$nombre", usuario.Nombre), ("$login", usuario.UsuarioLogin),
            ("$hash", CalcularHash(usuario.Password)), ("$rol", usuario.Rol.ToString()),
            ("$especialidad", veterinario?.Especialidad), ("$colegiado", veterinario?.NumeroColegiado));
    }

    public void EliminarUsuario(int id) => Ejecutar("DELETE FROM Usuarios WHERE Id = $id;", ("$id", id));

    public bool HayUsuarios() => Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Usuarios;")) > 0;

    /// <summary>Caso de uso: Iniciar Sesion. Devuelve "Nombre (Rol)" o null si las credenciales no son validas.</summary>
    public string? IniciarSesion(string usuarioLogin, string password)
    {
        using var conexion = AbrirConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT Nombre || ' (' || Rol || ')' FROM Usuarios WHERE UsuarioLogin = $login AND PasswordHash = $hash;";
        comando.Parameters.AddWithValue("$login", usuarioLogin);
        comando.Parameters.AddWithValue("$hash", CalcularHash(password));
        return comando.ExecuteScalar() as string;
    }

    // ---------------------------------------------------------------
    // CRUD de clientes
    // ---------------------------------------------------------------
    public DataTable ObtenerClientes() => Consultar(
        "SELECT Id, Nombre, Telefono, Direccion, Email FROM Clientes ORDER BY Nombre;");

    public int CrearCliente(Cliente cliente) => Insertar(
        "INSERT INTO Clientes (Nombre, Telefono, Direccion, Email) VALUES ($nombre, $telefono, $direccion, $email);",
        ("$nombre", cliente.Nombre), ("$telefono", cliente.Telefono),
        ("$direccion", cliente.Direccion), ("$email", cliente.Email));

    public void ActualizarCliente(int id, Cliente cliente) => EjecutarObligatorio(
        "UPDATE Clientes SET Nombre = $nombre, Telefono = $telefono, Direccion = $direccion, Email = $email WHERE Id = $id;",
        "No existe un cliente con ese Id.",
        ("$nombre", cliente.Nombre), ("$telefono", cliente.Telefono),
        ("$direccion", cliente.Direccion), ("$email", cliente.Email), ("$id", id));

    public void EliminarCliente(int id) => EjecutarObligatorio(
        "DELETE FROM Clientes WHERE Id = $id;", "No existe un cliente con ese Id.", ("$id", id));

    // ---------------------------------------------------------------
    // CRUD de mascotas (cada mascota nace con su historia clinica)
    // ---------------------------------------------------------------
    public DataTable ObtenerMascotas() => Consultar(@"SELECT m.Id, m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso,
        c.Nombre AS Propietario
        FROM Mascotas m INNER JOIN Clientes c ON c.Id = m.ClienteId
        ORDER BY m.Nombre;");

    public int CrearMascota(int clienteId, string nombre, string especie, string raza, int edad, double peso)
    {
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();
        ValidarExiste(conexion, transaccion, "Clientes", clienteId, "El cliente seleccionado no existe.");

        var mascotaId = Insertar(conexion, transaccion,
            @"INSERT INTO Mascotas (ClienteId, Nombre, Especie, Raza, Edad, Peso)
              VALUES ($cliente, $nombre, $especie, $raza, $edad, $peso);",
            ("$cliente", clienteId), ("$nombre", nombre), ("$especie", especie),
            ("$raza", raza), ("$edad", edad), ("$peso", peso));
        Insertar(conexion, transaccion, "INSERT INTO HistoriasClinicas (MascotaId) VALUES ($mascota);",
            ("$mascota", mascotaId));

        transaccion.Commit();
        return mascotaId;
    }

    public void ActualizarMascota(int id, int clienteId, string nombre, string especie, string raza, int edad, double peso)
    {
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();
        ValidarExiste(conexion, transaccion, "Clientes", clienteId, "El cliente seleccionado no existe.");
        var filas = Ejecutar(conexion, transaccion,
            @"UPDATE Mascotas SET ClienteId = $cliente, Nombre = $nombre, Especie = $especie,
              Raza = $raza, Edad = $edad, Peso = $peso WHERE Id = $id;",
            ("$cliente", clienteId), ("$nombre", nombre), ("$especie", especie),
            ("$raza", raza), ("$edad", edad), ("$peso", peso), ("$id", id));
        if (filas == 0) throw new InvalidOperationException("No existe una mascota con ese Id.");
        transaccion.Commit();
    }

    public void EliminarMascota(int id) => EjecutarObligatorio(
        "DELETE FROM Mascotas WHERE Id = $id;", "No existe una mascota con ese Id.", ("$id", id));

    // ---------------------------------------------------------------
    // Inventario de medicamentos
    // ---------------------------------------------------------------
    public DataTable ObtenerMedicamentos() => Consultar(
        "SELECT Id, Nombre, Stock, PrecioUnitario FROM Medicamentos ORDER BY Nombre;");

    public int CrearMedicamento(Medicamento medicamento) => Insertar(
        "INSERT INTO Medicamentos (Nombre, Stock, PrecioUnitario) VALUES ($nombre, $stock, $precio);",
        ("$nombre", medicamento.Nombre), ("$stock", medicamento.Stock), ("$precio", medicamento.PrecioUnitario));

    public void ActualizarMedicamento(int id, Medicamento medicamento) => EjecutarObligatorio(
        "UPDATE Medicamentos SET Nombre = $nombre, Stock = $stock, PrecioUnitario = $precio WHERE Id = $id;",
        "No existe un medicamento con ese Id.",
        ("$nombre", medicamento.Nombre), ("$stock", medicamento.Stock),
        ("$precio", medicamento.PrecioUnitario), ("$id", id));

    /// <summary>Suma (o resta, si es negativa) la cantidad al stock sin permitir que quede negativo.</summary>
    public void AjustarStock(int id, int cantidad) => EjecutarObligatorio(
        "UPDATE Medicamentos SET Stock = Stock + $cantidad WHERE Id = $id AND Stock + $cantidad >= 0;",
        "El medicamento no existe o el stock quedaria negativo.",
        ("$cantidad", cantidad), ("$id", id));

    public void EliminarMedicamento(int id) => EjecutarObligatorio(
        "DELETE FROM Medicamentos WHERE Id = $id;", "No existe un medicamento con ese Id.", ("$id", id));

    // ---------------------------------------------------------------
    // Citas: agendar, reprogramar y cancelar
    // ---------------------------------------------------------------
    public DataTable ObtenerCitas() => Consultar(@"SELECT ci.Id, m.Nombre AS Mascota, u.Nombre AS Veterinario,
        ci.Fecha, ci.Hora, ci.Estado
        FROM Citas ci
        INNER JOIN Mascotas m ON m.Id = ci.MascotaId
        INNER JOIN Usuarios u ON u.Id = ci.VeterinarioId
        ORDER BY ci.Fecha DESC, ci.Hora DESC;");

    public int AgendarCita(int mascotaId, int veterinarioId, DateTime fecha, string hora)
    {
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();
        ValidarExiste(conexion, transaccion, "Mascotas", mascotaId, "La mascota seleccionada no existe.");
        ValidarVeterinario(conexion, transaccion, veterinarioId);
        ValidarHorarioLibre(conexion, transaccion, veterinarioId, fecha, hora, null);

        var id = Insertar(conexion, transaccion,
            "INSERT INTO Citas (MascotaId, VeterinarioId, Fecha, Hora) VALUES ($mascota, $vet, $fecha, $hora);",
            ("$mascota", mascotaId), ("$vet", veterinarioId),
            ("$fecha", fecha.ToString(FormatoFecha)), ("$hora", hora));
        transaccion.Commit();
        return id;
    }

    public void ReprogramarCita(int id, DateTime nuevaFecha, string nuevaHora)
    {
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();
        var veterinarioId = Escalar(conexion, transaccion,
            "SELECT VeterinarioId FROM Citas WHERE Id = $id AND Estado = 'Pendiente';", ("$id", id))
            ?? throw new InvalidOperationException("La cita no existe o ya no esta pendiente.");
        ValidarHorarioLibre(conexion, transaccion, Convert.ToInt32(veterinarioId), nuevaFecha, nuevaHora, id);

        Ejecutar(conexion, transaccion, "UPDATE Citas SET Fecha = $fecha, Hora = $hora WHERE Id = $id;",
            ("$fecha", nuevaFecha.ToString(FormatoFecha)), ("$hora", nuevaHora), ("$id", id));
        transaccion.Commit();
    }

    public void CancelarCita(int id) => EjecutarObligatorio(
        "UPDATE Citas SET Estado = 'Cancelada' WHERE Id = $id AND Estado = 'Pendiente';",
        "La cita no existe o ya no esta pendiente.", ("$id", id));

    // ---------------------------------------------------------------
    // Consultas, tratamientos e historia clinica
    // ---------------------------------------------------------------
    public DataTable ObtenerConsultas() => Consultar(@"SELECT co.Id, co.CitaId, m.Nombre AS Mascota,
        co.Fecha, co.Motivo, co.Diagnostico
        FROM Consultas co
        INNER JOIN HistoriasClinicas h ON h.Id = co.HistoriaClinicaId
        INNER JOIN Mascotas m ON m.Id = h.MascotaId
        ORDER BY co.Fecha DESC, co.Id DESC;");

    /// <summary>Atiende una cita pendiente: crea la consulta, la marca como atendida y la agrega al historial.</summary>
    public int RegistrarConsulta(int citaId, DateTime fecha, string motivo, string diagnostico)
    {
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();
        var historiaId = Escalar(conexion, transaccion, @"SELECT h.Id FROM Citas c
            INNER JOIN HistoriasClinicas h ON h.MascotaId = c.MascotaId
            WHERE c.Id = $cita AND c.Estado = 'Pendiente';", ("$cita", citaId))
            ?? throw new InvalidOperationException("La cita no existe o ya no esta pendiente.");

        var id = Insertar(conexion, transaccion,
            @"INSERT INTO Consultas (CitaId, HistoriaClinicaId, Fecha, Motivo, Diagnostico)
              VALUES ($cita, $historia, $fecha, $motivo, $diagnostico);",
            ("$cita", citaId), ("$historia", historiaId), ("$fecha", fecha.ToString(FormatoFecha)),
            ("$motivo", motivo), ("$diagnostico", diagnostico));
        Ejecutar(conexion, transaccion, "UPDATE Citas SET Estado = 'Atendida' WHERE Id = $cita;", ("$cita", citaId));
        transaccion.Commit();
        return id;
    }

    public void RegistrarDiagnostico(int consultaId, string diagnostico) => EjecutarObligatorio(
        "UPDATE Consultas SET Diagnostico = $diagnostico WHERE Id = $id;",
        "No existe una consulta con ese Id.", ("$diagnostico", diagnostico), ("$id", consultaId));

    public DataTable ObtenerTratamientos() => Consultar(@"SELECT t.Id, t.ConsultaId, t.Descripcion, t.FechaInicio, t.FechaFin,
        COALESCE(GROUP_CONCAT(me.Nombre || ' x' || d.Cantidad || ' (' || d.Dosis || ')', '; '), '') AS Medicamentos
        FROM Tratamientos t
        LEFT JOIN DetallesTratamiento d ON d.TratamientoId = t.Id
        LEFT JOIN Medicamentos me ON me.Id = d.MedicamentoId
        GROUP BY t.Id ORDER BY t.Id DESC;");

    public int CrearTratamiento(int consultaId, string descripcion, DateTime inicio, DateTime fin)
    {
        if (fin.Date < inicio.Date) throw new ArgumentException("La fecha fin no puede ser anterior a la fecha de inicio.");
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();
        ValidarExiste(conexion, transaccion, "Consultas", consultaId, "La consulta seleccionada no existe.");
        var id = Insertar(conexion, transaccion,
            "INSERT INTO Tratamientos (ConsultaId, Descripcion, FechaInicio, FechaFin) VALUES ($consulta, $desc, $inicio, $fin);",
            ("$consulta", consultaId), ("$desc", descripcion),
            ("$inicio", inicio.ToString(FormatoFecha)), ("$fin", fin.ToString(FormatoFecha)));
        transaccion.Commit();
        return id;
    }

    /// <summary>Caso de uso: Prescribir Tratamiento. Descuenta del inventario en la misma transaccion.</summary>
    public void AgregarDetalleTratamiento(int tratamientoId, int medicamentoId, string dosis, int cantidad)
    {
        if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor que cero.");
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();
        ValidarExiste(conexion, transaccion, "Tratamientos", tratamientoId, "El tratamiento seleccionado no existe.");

        var stock = Escalar(conexion, transaccion, "SELECT Stock FROM Medicamentos WHERE Id = $id;", ("$id", medicamentoId))
            ?? throw new InvalidOperationException("El medicamento seleccionado no existe.");
        if (Convert.ToInt32(stock) < cantidad)
            throw new InvalidOperationException($"Stock insuficiente: solo hay {stock} unidad(es).");

        Insertar(conexion, transaccion,
            "INSERT INTO DetallesTratamiento (TratamientoId, MedicamentoId, Dosis, Cantidad) VALUES ($trat, $med, $dosis, $cant);",
            ("$trat", tratamientoId), ("$med", medicamentoId), ("$dosis", dosis), ("$cant", cantidad));
        Ejecutar(conexion, transaccion, "UPDATE Medicamentos SET Stock = Stock - $cant WHERE Id = $med;",
            ("$cant", cantidad), ("$med", medicamentoId));
        transaccion.Commit();
    }

    /// <summary>Caso de uso: Consultar Historial Medico de una mascota.</summary>
    public DataTable ObtenerHistorial(int mascotaId)
    {
        using var conexion = AbrirConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = @"SELECT co.Id AS Consulta, co.Fecha, u.Nombre AS Veterinario, co.Motivo, co.Diagnostico,
            COALESCE((SELECT GROUP_CONCAT(t.Descripcion, '; ') FROM Tratamientos t WHERE t.ConsultaId = co.Id), '') AS Tratamientos
            FROM HistoriasClinicas h
            INNER JOIN Consultas co ON co.HistoriaClinicaId = h.Id
            INNER JOIN Citas ci ON ci.Id = co.CitaId
            INNER JOIN Usuarios u ON u.Id = ci.VeterinarioId
            WHERE h.MascotaId = $mascota
            ORDER BY co.Fecha, co.Id;";
        comando.Parameters.AddWithValue("$mascota", mascotaId);
        return CargarTabla(comando);
    }

    // ---------------------------------------------------------------
    // Facturacion
    // ---------------------------------------------------------------
    public DataTable ObtenerFacturas() => Consultar(@"SELECT f.Id, f.ConsultaId, c.Nombre AS Cliente, f.Fecha, f.Total
        FROM Facturas f INNER JOIN Clientes c ON c.Id = f.ClienteId
        ORDER BY f.Fecha DESC, f.Id DESC;");

    public DataTable ObtenerDetalleFactura(int facturaId)
    {
        using var conexion = AbrirConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = @"SELECT Concepto, Cantidad, PrecioUnitario, Subtotal
            FROM DetallesFactura WHERE FacturaId = $factura ORDER BY Id;";
        comando.Parameters.AddWithValue("$factura", facturaId);
        return CargarTabla(comando);
    }

    /// <summary>
    /// Caso de uso: Generar Factura. Agrega el costo de la consulta y cada medicamento
    /// recetado en sus tratamientos, y guarda el total. Solo se permite una factura por consulta.
    /// </summary>
    public int GenerarFactura(int consultaId, DateTime fecha)
    {
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();
        var clienteId = Escalar(conexion, transaccion, @"SELECT m.ClienteId FROM Consultas co
            INNER JOIN HistoriasClinicas h ON h.Id = co.HistoriaClinicaId
            INNER JOIN Mascotas m ON m.Id = h.MascotaId
            WHERE co.Id = $consulta;", ("$consulta", consultaId))
            ?? throw new InvalidOperationException("La consulta seleccionada no existe.");
        if (Convert.ToInt32(Escalar(conexion, transaccion,
                "SELECT COUNT(*) FROM Facturas WHERE ConsultaId = $consulta;", ("$consulta", consultaId))) > 0)
            throw new InvalidOperationException("Esa consulta ya fue facturada.");

        var facturaId = Insertar(conexion, transaccion,
            "INSERT INTO Facturas (ClienteId, ConsultaId, Fecha, Total) VALUES ($cliente, $consulta, $fecha, 0);",
            ("$cliente", clienteId), ("$consulta", consultaId), ("$fecha", fecha.ToString(FormatoFecha)));

        Insertar(conexion, transaccion,
            @"INSERT INTO DetallesFactura (FacturaId, Concepto, Cantidad, PrecioUnitario, Subtotal)
              VALUES ($factura, 'Consulta veterinaria', 1, $costo, $costo);",
            ("$factura", facturaId), ("$costo", Factura.CostoConsulta));

        Ejecutar(conexion, transaccion,
            @"INSERT INTO DetallesFactura (FacturaId, Concepto, Cantidad, PrecioUnitario, Subtotal)
              SELECT $factura, me.Nombre || ' (' || d.Dosis || ')', d.Cantidad, me.PrecioUnitario, d.Cantidad * me.PrecioUnitario
              FROM Tratamientos t
              INNER JOIN DetallesTratamiento d ON d.TratamientoId = t.Id
              INNER JOIN Medicamentos me ON me.Id = d.MedicamentoId
              WHERE t.ConsultaId = $consulta
              ORDER BY d.Id;",
            ("$factura", facturaId), ("$consulta", consultaId));

        Ejecutar(conexion, transaccion,
            "UPDATE Facturas SET Total = (SELECT SUM(Subtotal) FROM DetallesFactura WHERE FacturaId = $factura) WHERE Id = $factura;",
            ("$factura", facturaId));
        transaccion.Commit();
        return facturaId;
    }

    // ---------------------------------------------------------------
    // Reglas de negocio comunes
    // ---------------------------------------------------------------
    private static void ValidarExiste(SqliteConnection conexion, SqliteTransaction transaccion, string tabla, long id, string mensaje)
    {
        // "tabla" siempre viene de una constante de este archivo, nunca del usuario.
        if (Convert.ToInt32(Escalar(conexion, transaccion, $"SELECT COUNT(*) FROM {tabla} WHERE Id = $id;", ("$id", id))) == 0)
            throw new InvalidOperationException(mensaje);
    }

    private static void ValidarVeterinario(SqliteConnection conexion, SqliteTransaction transaccion, int veterinarioId)
    {
        var rol = Escalar(conexion, transaccion, "SELECT Rol FROM Usuarios WHERE Id = $id;", ("$id", veterinarioId)) as string;
        if (rol != RolUsuario.Veterinario.ToString())
            throw new InvalidOperationException("El usuario seleccionado no existe o no es veterinario.");
    }

    private static void ValidarHorarioLibre(SqliteConnection conexion, SqliteTransaction transaccion,
        int veterinarioId, DateTime fecha, string hora, int? citaActualId)
    {
        var ocupadas = Escalar(conexion, transaccion, @"SELECT COUNT(*) FROM Citas
            WHERE VeterinarioId = $vet AND Fecha = $fecha AND Hora = $hora AND Estado = 'Pendiente'
            AND ($citaId IS NULL OR Id <> $citaId);",
            ("$vet", veterinarioId), ("$fecha", fecha.ToString(FormatoFecha)), ("$hora", hora), ("$citaId", citaActualId));
        if (Convert.ToInt32(ocupadas) > 0)
            throw new InvalidOperationException("El veterinario ya tiene una cita pendiente en esa fecha y hora.");
    }

    private static string CalcularHash(string texto) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(texto)));

    // ---------------------------------------------------------------
    // Acceso a datos (conexion, comandos y parametros)
    // ---------------------------------------------------------------
    private DataTable Consultar(string sql)
    {
        using var conexion = AbrirConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        return CargarTabla(comando);
    }

    private static DataTable CargarTabla(SqliteCommand comando)
    {
        using var lector = comando.ExecuteReader();
        var tabla = new DataTable();
        tabla.Load(lector);
        return tabla;
    }

    private object? Escalar(string sql)
    {
        using var conexion = AbrirConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        return comando.ExecuteScalar();
    }

    private void Ejecutar(string sql, params (string Nombre, object? Valor)[] parametros)
    {
        using var conexion = AbrirConexion();
        Ejecutar(conexion, null, sql, parametros);
    }

    private void EjecutarObligatorio(string sql, string mensajeSinFilas, params (string Nombre, object? Valor)[] parametros)
    {
        using var conexion = AbrirConexion();
        if (Ejecutar(conexion, null, sql, parametros) == 0) throw new InvalidOperationException(mensajeSinFilas);
    }

    private int Insertar(string sql, params (string Nombre, object? Valor)[] parametros)
    {
        using var conexion = AbrirConexion();
        return Insertar(conexion, null, sql, parametros);
    }

    private static int Ejecutar(SqliteConnection conexion, SqliteTransaction? transaccion, string sql,
        params (string Nombre, object? Valor)[] parametros)
    {
        using var comando = CrearComando(conexion, transaccion, sql, parametros);
        return comando.ExecuteNonQuery();
    }

    private static int Insertar(SqliteConnection conexion, SqliteTransaction? transaccion, string sql,
        params (string Nombre, object? Valor)[] parametros)
    {
        using var comando = CrearComando(conexion, transaccion, sql + " SELECT last_insert_rowid();", parametros);
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    private static object? Escalar(SqliteConnection conexion, SqliteTransaction transaccion, string sql,
        params (string Nombre, object? Valor)[] parametros)
    {
        using var comando = CrearComando(conexion, transaccion, sql, parametros);
        var valor = comando.ExecuteScalar();
        return valor is DBNull ? null : valor;
    }

    private static SqliteCommand CrearComando(SqliteConnection conexion, SqliteTransaction? transaccion, string sql,
        (string Nombre, object? Valor)[] parametros)
    {
        var comando = conexion.CreateCommand();
        comando.Transaction = transaccion;
        comando.CommandText = sql;
        foreach (var (nombre, valor) in parametros) comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        return comando;
    }

    private SqliteConnection AbrirConexion()
    {
        var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();
        return conexion;
    }
}
