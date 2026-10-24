import java.util.ArrayList;
import java.util.List;

/**
 * Mascota
 * Paciente de la clinica veterinaria, asociada a un Cliente y a una HistoriaClinica.
 */
public class Mascota
{
    private static int contador = 1;

    private int idMascota;
    private String nombre;
    private String especie;
    private String raza;
    private int edad;
    private double peso;
    private Cliente propietario;
    private HistoriaClinica historiaClinica;
    private List<Cita> citas;

    public Mascota(String nombre, String especie, String raza, int edad, double peso, Cliente propietario)
    {
        this.idMascota = contador++;
        this.nombre = nombre;
        this.especie = especie;
        this.raza = raza;
        this.edad = edad;
        this.peso = peso;
        this.propietario = propietario;
        this.historiaClinica = new HistoriaClinica(this);
        this.citas = new ArrayList<Cita>();
        propietario.agregarMascota(this);
    }

    public void agregarCita(Cita cita)
    {
        citas.add(cita);
    }

    public int getIdMascota()
    {
        return idMascota;
    }

    public String getNombre()
    {
        return nombre;
    }

    public String getEspecie()
    {
        return especie;
    }

    public String getRaza()
    {
        return raza;
    }

    public int getEdad()
    {
        return edad;
    }

    public double getPeso()
    {
        return peso;
    }

    public Cliente getPropietario()
    {
        return propietario;
    }

    public HistoriaClinica getHistoriaClinica()
    {
        return historiaClinica;
    }

    public List<Cita> getCitas()
    {
        return citas;
    }

    @Override
    public String toString()
    {
        return "Mascota #" + idMascota + " - " + nombre + " (" + especie + ", " + raza + ", " + edad + " anios)";
    }
}
