/**
 * Simulador
 * Clase ejecutable (void main) que recorre, paso a paso, los flujos del
 * Sistema de Control Clinico Veterinario:
 *   1) Inicio de sesion
 *   2) Registro de cliente y mascota
 *   3) Agendar cita
 *   4) Atencion de consulta (diagnostico + tratamiento)
 *   5) Facturacion
 *   6) Consulta de historial e inventario
 *
 * En BlueJ: clic derecho sobre Simulador -> void main(String[] args).
 * En consola: javac *.java  &&  java Simulador
 */
public class Simulador
{
    public static void main(String[] args)
    {
        System.out.println("==========================================================");
        System.out.println(" SIMULADOR - SISTEMA DE CONTROL CLINICO VETERINARIO");
        System.out.println("==========================================================\n");

        // ---------- Usuarios del sistema ----------
        Administrador admin = new Administrador("Ana Morales", "amorales", "admin123");
        Recepcionista recep = new Recepcionista("Luis Perez", "lperez", "recep123");
        Veterinario vet = new Veterinario("Carla Gomez", "cgomez", "vet123", "Medicina General", "COL-4521");

        // ---------- 1) Caso de uso: Iniciar Sesion ----------
        System.out.println("[1] INICIO DE SESION");
        recep.iniciarSesion("lperez", "recep123");
        vet.iniciarSesion("cgomez", "clave-incorrecta");
        vet.iniciarSesion("cgomez", "vet123");
        System.out.println();

        // ---------- 2) Caso de uso: Registrar Cliente y Mascota ----------
        System.out.println("[2] REGISTRO DE CLIENTE Y MASCOTA");
        Cliente cliente = new Cliente("Maria Fernandez", "5555-1234", "Zona 10, Ciudad", "maria@correo.com");
        System.out.println("  -> Cliente registrado: " + cliente);
        Mascota mascota = new Mascota("Firulais", "Perro", "Labrador", 3, 24.5, cliente);
        System.out.println("  -> Mascota registrada: " + mascota);
        System.out.println();

        // ---------- Inventario inicial de medicamentos ----------
        Medicamento antiparasitario = new Medicamento("Antiparasitario Canino", 50, 45.0);
        Medicamento antiinflamatorio = new Medicamento("Antiinflamatorio", 30, 60.0);

        // ---------- 3) Caso de uso: Agendar Cita ----------
        System.out.println("[3] AGENDAR CITA");
        Cita cita = new Cita("2026-09-22", "10:00", mascota, vet);
        System.out.println("  -> Cita creada: " + cita);
        System.out.println();

        // ---------- 4) Caso de uso: Registrar Consulta + Historia Clinica + Tratamiento ----------
        System.out.println("[4] ATENCION DE CONSULTA");
        Consulta consulta = new Consulta("2026-09-22", "Revision general y vomito ocasional", cita);
        consulta.registrarDiagnostico("Gastroenteritis leve y presencia de parasitos");

        Tratamiento tratamiento = new Tratamiento(
                "Desparasitacion y control de inflamacion", "2026-09-22", "2026-09-29", consulta);
        tratamiento.agregarDetalle(antiparasitario, "1 tableta cada 24h", 3);
        tratamiento.agregarDetalle(antiinflamatorio, "5 mg cada 12h", 2);

        System.out.println("  -> Estado de la cita: " + cita.getEstado());
        System.out.println();

        // ---------- 5) Caso de uso: Generar Factura ----------
        System.out.println("[5] FACTURACION");
        Factura factura = new Factura("2026-09-22", cliente, consulta);
        factura.imprimirFactura();
        System.out.println();

        // ---------- 6) Caso de uso: Consultar Historial Medico ----------
        System.out.println("[6] CONSULTAR HISTORIAL MEDICO");
        mascota.getHistoriaClinica().imprimirHistorial();
        System.out.println();

        // ---------- 7) Caso de uso: Gestionar Inventario ----------
        System.out.println("[7] ESTADO DE INVENTARIO");
        System.out.println("  " + antiparasitario);
        System.out.println("  " + antiinflamatorio);
        System.out.println();

        // ---------- 8) Caso de uso: Cancelar/Reprogramar Cita ----------
        System.out.println("[8] AGENDAR Y CANCELAR OTRA CITA (ejemplo adicional)");
        Cita segundaCita = new Cita("2026-09-25", "15:00", mascota, vet);
        segundaCita.reprogramarCita("2026-09-26", "09:00");
        segundaCita.cancelarCita();
        System.out.println();

        System.out.println("==========================================================");
        System.out.println(" FIN DE LA SIMULACION");
        System.out.println("==========================================================");
    }
}
