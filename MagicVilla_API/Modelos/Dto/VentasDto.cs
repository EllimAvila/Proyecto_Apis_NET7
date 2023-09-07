namespace MagicVilla_API.Modelos.Dto
{
    public class VentasDto
    {
        public int Id { get; set; }
        public string DescripcionProducto { get; set; }
        public string UnidadMedida {  get; set; }
        public double Cantidad {  get; set; }
        public double PrecioUnitario {  get; set; }
        public double Descuento { get; set; }
    }
}
