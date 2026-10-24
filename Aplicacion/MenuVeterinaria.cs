using System.Data;
using SistemaVeterinaria.Dominio;
using SistemaVeterinaria.Persistencia;

namespace SistemaVeterinaria.Aplicacion;

internal sealed class MenuVeterinaria
{
    private readonly RepositorioVeterinaria _repositorio;
    private bool _continuar = true;

    public MenuVeterinaria(RepositorioVeterinaria repositorio) => _repositorio = repositorio;

    public void Ejecutar()
    {
        while (_continuar)
        {
            MostrarMenu();
            try
            {
                var entrada = Console.ReadLine();
                if (entrada is null) break;
                switch (entrada.Trim())
                {
                    case "1": CrearCliente(); break;
                    case "2": ListarClientes(); break;
                    case "3": ActualizarCliente(); break;
                    case "4": EliminarCliente(); break;
                    case "5": CrearMascota(); break;
                    case "6": ListarMascotas(); break;
                    case "7": ActualizarMascota(); break;
                    case "8": EliminarMascota(); break;
                    case "9": CrearUsuario(); break;
                    case "10": ListarUsuarios(); break;
                    case "11": IniciarSesion(); break;
                    case "12": CrearMedicamento(); break;
                    case "13": ListarMedicamentos(); break;
                    case "14": AjustarStock(); break;
                    case "15": EliminarMedicamento(); break;
                    case "16": AgendarCita(); break;
                    case "17": ListarCitas(); break;
                    case "18": ReprogramarCita(); break;
                    case "19": CancelarCita(); break;
                    case "20": AtenderConsulta(); break;
                    case "21": PrescribirTratamiento(); break;
                    case "22": ConsultarHistorial(); break;
                    case "23": GenerarFactura(); break;
                    case "24": ListarFacturas(); break;
                    case "25": CargarDatosDemostracion(); break;
                    case "0": _continuar = false; break;
                    default: Console.WriteLine("Seleccione una opcion valida."); break;
                }
            }
            catch (Exception error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error controlado con try-catch: {error.Message}");
                Console.ResetColor();
            }

            if (_continuar) Pausar();
        }
    }

    private static void MostrarMenu()
    {
        if (!Console.IsOutputRedirected) Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==========================================================");
        Console.WriteLine("        SISTEMA DE CONTROL CLINICO VETERINARIO");
        Console.WriteLine("==========================================================");
        Console.ResetColor();
        Console.WriteLine("CLIENTES:     1 Crear | 2 Listar | 3 Actualizar | 4 Eliminar");
        Console.WriteLine("MASCOTAS:     5 Crear | 6 Listar | 7 Actualizar | 8 Eliminar");
        Console.WriteLine("USUARIOS:     9 Crear | 10 Listar | 11 Iniciar sesion");
        Console.WriteLine("INVENTARIO:   12 Crear | 13 Listar | 14 Ajustar stock | 15 Eliminar");
        Console.WriteLine("CITAS:        16 Agendar | 17 Listar | 18 Reprogramar | 19 Cancelar");
        Console.WriteLine("ATENCION:     20 Atender consulta | 21 Prescribir tratamiento | 22 Historial");
        Console.WriteLine("FACTURACION:  23 Generar factura | 24 Listar facturas");
        Console.WriteLine("EXTRAS:       25 Cargar datos de demostracion | 0 Salir");
        Console.Write("Opcion: ");
    }

    // ----------------------------- Clientes -----------------------------
    private void CrearCliente()
    {
        var id = _repositorio.CrearCliente(LeerCliente());
        Console.WriteLine($"Cliente creado correctamente con Id {id}.");
    }

    private void ListarClientes() => MostrarTabla(_repositorio.ObtenerClientes(), "CLIENTES");

    private void ActualizarCliente()
    {
        ListarClientes();
        _repositorio.ActualizarCliente(LeerEntero("Id del cliente a actualizar"), LeerCliente());
        Console.WriteLine("Cliente actualizado correctamente.");
    }

    private void EliminarCliente()
    {
        ListarClientes();
        _repositorio.EliminarCliente(LeerEntero("Id del cliente a eliminar"));
        Console.WriteLine("Cliente eliminado; sus mascotas, citas e historial tambien se eliminan.");
    }

    // ----------------------------- Mascotas -----------------------------
    private void CrearMascota()
    {
        ListarClientes();
        var clienteId = LeerEntero("Id del propietario");
        var id = _repositorio.CrearMascota(clienteId, LeerTexto("Nombre"), LeerTexto("Especie"),
            LeerTexto("Raza"), LeerEntero("Edad (anios)"), (double)LeerDecimal("Peso (kg)"));
        Console.WriteLine($"Mascota registrada con Id {id}; su historia clinica fue creada.");
    }

    private void ListarMascotas() => MostrarTabla(_repositorio.ObtenerMascotas(), "MASCOTAS");

    private void ActualizarMascota()
    {
        ListarMascotas();
        var id = LeerEntero("Id de la mascota a actualizar");
        ListarClientes();
        _repositorio.ActualizarMascota(id, LeerEntero("Id del propietario"), LeerTexto("Nombre"),
            LeerTexto("Especie"), LeerTexto("Raza"), LeerEntero("Edad (anios)"), (double)LeerDecimal("Peso (kg)"));
        Console.WriteLine("Mascota actualizada correctamente.");
    }

    private void EliminarMascota()
    {
        ListarMascotas();
        _repositorio.EliminarMascota(LeerEntero("Id de la mascota a eliminar"));
        Console.WriteLine("Mascota eliminada junto con su historial y citas.");
    }

    // ----------------------------- Usuarios -----------------------------
    private void CrearUsuario()
    {
        Console.WriteLine("Rol: 1 Administrador | 2 Veterinario | 3 Recepcionista");
        var rol = LeerEntero("Rol");
        var nombre = LeerTexto("Nombre");
        var login = LeerTexto("Usuario");
        var password = LeerTexto("Contrasenia");
        Usuario usuario = rol switch
        {
            1 => new Administrador(nombre, login, password),
            2 => new Veterinario(nombre, login, password, LeerTexto("Especialidad"), LeerTexto("Numero de colegiado")),
            3 => new Recepcionista(nombre, login, password),
            _ => throw new ArgumentException("Rol no valido.")
        };
        var id = _repositorio.CrearUsuario(usuario);
        Console.WriteLine($"Usuario creado con Id {id} ({usuario.Rol}).");
    }

    private void ListarUsuarios() => MostrarTabla(_repositorio.ObtenerUsuarios(), "USUARIOS");

    private void IniciarSesion()
    {
        var resultado = _repositorio.IniciarSesion(LeerTexto("Usuario"), LeerTexto("Contrasenia"));
        Console.WriteLine(resultado is null
            ? "Credenciales invalidas."
            : $"Credenciales validas. Bienvenido/a {resultado}");
    }

    // ---------------------------- Inventario ----------------------------
    private void CrearMedicamento()
    {
        var medicamento = new Medicamento(LeerTexto("Nombre"), LeerEntero("Stock inicial"), LeerDecimal("Precio unitario (Q)"));
        var id = _repositorio.CrearMedicamento(medicamento);
        Console.WriteLine($"Medicamento creado con Id {id}.");
    }

    private void ListarMedicamentos() => MostrarTabla(_repositorio.ObtenerMedicamentos(), "INVENTARIO DE MEDICAMENTOS");

    private void AjustarStock()
    {
        ListarMedicamentos();
        _repositorio.AjustarStock(LeerEntero("Id del medicamento"), LeerEntero("Cantidad (+ entrada / - salida)"));
        Console.WriteLine("Stock actualizado.");
    }

    private void EliminarMedicamento()
    {
        ListarMedicamentos();
        _repositorio.EliminarMedicamento(LeerEntero("Id del medicamento a eliminar"));
        Console.WriteLine("Medicamento eliminado.");
    }

    // ------------------------------ Citas -------------------------------
    private void AgendarCita()
    {
        ListarMascotas();
        var mascotaId = LeerEntero("Id de la mascota");
        MostrarTabla(_repositorio.ObtenerVeterinarios(), "VETERINARIOS");
        var veterinarioId = LeerEntero("Id del veterinario");
        var id = _repositorio.AgendarCita(mascotaId, veterinarioId, LeerFecha("Fecha (AAAA-MM-DD)"), LeerHora("Hora (HH:mm)"));
        Console.WriteLine($"Cita #{id} agendada (Pendiente).");
    }

    private void ListarCitas() => MostrarTabla(_repositorio.ObtenerCitas(), "CITAS");

    private void ReprogramarCita()
    {
        ListarCitas();
        _repositorio.ReprogramarCita(LeerEntero("Id de la cita"), LeerFecha("Nueva fecha (AAAA-MM-DD)"), LeerHora("Nueva hora (HH:mm)"));
        Console.WriteLine("Cita reprogramada.");
    }

    private void CancelarCita()
    {
        ListarCitas();
        _repositorio.CancelarCita(LeerEntero("Id de la cita a cancelar"));
        Console.WriteLine("Cita cancelada.");
    }

    // ----------------------------- Atencion -----------------------------
    private void AtenderConsulta()
    {
        ListarCitas();
        var citaId = LeerEntero("Id de la cita a atender");
        var id = _repositorio.RegistrarConsulta(citaId, DateTime.Today, LeerTexto("Motivo"), LeerTexto("Diagnostico"));
        Console.WriteLine($"Consulta #{id} registrada; la cita queda Atendida y se agrego al historial.");
    }

    private void PrescribirTratamiento()
    {
        MostrarTabla(_repositorio.ObtenerConsultas(), "CONSULTAS");
        var consultaId = LeerEntero("Id de la consulta");
        var tratamientoId = _repositorio.CrearTratamiento(consultaId, LeerTexto("Descripcion"),
            LeerFecha("Fecha inicio (AAAA-MM-DD)"), LeerFecha("Fecha fin (AAAA-MM-DD)"));
        Console.WriteLine($"Tratamiento #{tratamientoId} creado. Agregue medicamentos (Id 0 para terminar).");

        ListarMedicamentos();
        while (true)
        {
            var medicamentoId = LeerEntero("Id del medicamento (0 = terminar)");
            if (medicamentoId == 0) break;
            _repositorio.AgregarDetalleTratamiento(tratamientoId, medicamentoId, LeerTexto("Dosis"), LeerEntero("Cantidad"));
            Console.WriteLine("  -> Medicamento prescrito y descontado del inventario.");
        }
    }

    private void ConsultarHistorial()
    {
        ListarMascotas();
        MostrarTabla(_repositorio.ObtenerHistorial(LeerEntero("Id de la mascota")), "HISTORIA CLINICA");
    }

    // ---------------------------- Facturacion ---------------------------
    private void GenerarFactura()
    {
        MostrarTabla(_repositorio.ObtenerConsultas(), "CONSULTAS");
        var facturaId = _repositorio.GenerarFactura(LeerEntero("Id de la consulta a facturar"), DateTime.Today);
        Console.WriteLine($"Factura #{facturaId} generada.");
        MostrarTabla(_repositorio.ObtenerDetalleFactura(facturaId), $"DETALLE FACTURA #{facturaId}");
    }

    private void ListarFacturas() => MostrarTabla(_repositorio.ObtenerFacturas(), "FACTURAS");

    // ------------------------------ Extras ------------------------------
    /// <summary>Carga un escenario de ejemplo completo (usuarios, cliente, mascota, cita, consulta y factura).</summary>
    private void CargarDatosDemostracion()
    {
        if (_repositorio.HayUsuarios())
            throw new InvalidOperationException("La base ya tiene datos; la demostracion solo se carga en una base vacia.");

        _repositorio.CrearUsuario(new Administrador("Ana Morales", "amorales", "admin123"));
        _repositorio.CrearUsuario(new Recepcionista("Luis Perez", "lperez", "recep123"));
        var vetId = _repositorio.CrearUsuario(
            new Veterinario("Carla Gomez", "cgomez", "vet123", "Medicina General", "COL-4521"));

        var clienteId = _repositorio.CrearCliente(
            new Cliente("Maria Fernandez", "5555-1234", "Zona 10, Ciudad", "maria@correo.com"));
        var mascotaId = _repositorio.CrearMascota(clienteId, "Firulais", "Perro", "Labrador", 3, 24.5);

        var antiparasitarioId = _repositorio.CrearMedicamento(new Medicamento("Antiparasitario Canino", 50, 45.0m));
        var antiinflamatorioId = _repositorio.CrearMedicamento(new Medicamento("Antiinflamatorio", 30, 60.0m));

        var citaId = _repositorio.AgendarCita(mascotaId, vetId, new DateTime(2026, 9, 22), "10:00");
        var consultaId = _repositorio.RegistrarConsulta(citaId, new DateTime(2026, 9, 22),
            "Revision general y vomito ocasional", "Gastroenteritis leve y presencia de parasitos");
        var tratamientoId = _repositorio.CrearTratamiento(consultaId, "Desparasitacion y control de inflamacion",
            new DateTime(2026, 9, 22), new DateTime(2026, 9, 29));
        _repositorio.AgregarDetalleTratamiento(tratamientoId, antiparasitarioId, "1 tableta cada 24h", 3);
        _repositorio.AgregarDetalleTratamiento(tratamientoId, antiinflamatorioId, "5 mg cada 12h", 2);
        var facturaId = _repositorio.GenerarFactura(consultaId, new DateTime(2026, 9, 22));

        var segundaCitaId = _repositorio.AgendarCita(mascotaId, vetId, new DateTime(2026, 9, 25), "15:00");
        _repositorio.ReprogramarCita(segundaCitaId, new DateTime(2026, 9, 26), "09:00");
        _repositorio.CancelarCita(segundaCitaId);

        Console.WriteLine("Datos de demostracion guardados en la base de datos.");
        MostrarTabla(_repositorio.ObtenerDetalleFactura(facturaId), $"DETALLE FACTURA #{facturaId}");
        MostrarTabla(_repositorio.ObtenerHistorial(mascotaId), "HISTORIA CLINICA DE FIRULAIS");
        MostrarTabla(_repositorio.ObtenerMedicamentos(), "INVENTARIO");
        MostrarTabla(_repositorio.ObtenerCitas(), "CITAS");
    }

    // ------------------------- Lectura de datos -------------------------
    private static Cliente LeerCliente() => new(
        LeerTexto("Nombre"), LeerTexto("Telefono"), LeerTexto("Direccion"), LeerTexto("Email"));

    private static int LeerEntero(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        if (!int.TryParse(Console.ReadLine(), out var valor)) throw new ArgumentException("Ingrese un numero entero valido.");
        return valor;
    }

    private static decimal LeerDecimal(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        var texto = Console.ReadLine()?.Replace(',', '.');
        if (!decimal.TryParse(texto, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out var valor) || valor < 0)
            throw new ArgumentException("Ingrese un numero valido (ej. 24.5).");
        return valor;
    }

    private static DateTime LeerFecha(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        if (!DateTime.TryParse(Console.ReadLine(), out var fecha)) throw new ArgumentException("Ingrese una fecha valida.");
        return fecha.Date;
    }

    private static string LeerHora(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        if (!TimeOnly.TryParse(Console.ReadLine(), out var hora)) throw new ArgumentException("Ingrese una hora valida (HH:mm).");
        return hora.ToString("HH:mm");
    }

    private static string LeerTexto(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        var texto = Console.ReadLine();
        return string.IsNullOrWhiteSpace(texto)
            ? throw new ArgumentException($"El campo '{mensaje}' es obligatorio.")
            : texto.Trim();
    }

    private static void MostrarTabla(DataTable tabla, string titulo)
    {
        Console.WriteLine($"\n=== {titulo} ===");
        if (tabla.Rows.Count == 0)
        {
            Console.WriteLine("No hay registros.");
            return;
        }

        foreach (DataColumn columna in tabla.Columns) Console.Write($"{columna.ColumnName,-22}");
        Console.WriteLine();
        foreach (DataRow fila in tabla.Rows)
        {
            foreach (var valor in fila.ItemArray) Console.Write($"{valor,-22}");
            Console.WriteLine();
        }
    }

    private static void Pausar()
    {
        if (Console.IsInputRedirected) return;
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }
}
