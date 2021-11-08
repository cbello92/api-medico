namespace AgendaApiMedico.Medico.Data
{
    public class ImagenMedicoDTO
    {
        public string ptdRut { get; set; }
        public string nombreImagen { get; set; }
        public int anchoOriginal { get; set; }
        public int altoOriginal { get; set; }
        public string fechaCreacion { get; set; }
        public string formato { get; set; }
    }
}
