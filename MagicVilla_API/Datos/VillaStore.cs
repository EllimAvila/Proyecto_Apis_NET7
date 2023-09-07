using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API.Datos
{
    public static class VillaStore
    {
        public static List<VillaDto> villaList = new List<VillaDto>()
        {
            new VillaDto { Id = 1, Nombre = "Villa con vista a la piscina"},
            new VillaDto { Id = 2, Nombre = "Villa con vista a la playa"}
        };

        public static List<VentasDto> detalleVenta = new List<VentasDto>()
        {
            new VentasDto { Id = 1, DescripcionProducto = "GLP", UnidadMedida = "GLL", PrecioUnitario = 7.5425, 
                Cantidad = 10.641, Descuento = 0.86},
            new VentasDto { Id = 2, DescripcionProducto = "GASOHOL PREMIUM", UnidadMedida = "GLL", PrecioUnitario = 15.754,
                Cantidad = 1.614, Descuento = 0},
            new VentasDto { Id = 3, DescripcionProducto = "DIESEL B5 S-50 UV", UnidadMedida = "GLL", 
                PrecioUnitario = 13.26271, Cantidad = 7.668, Descuento = 0.20},
            new VentasDto { Id = 4, DescripcionProducto = "INCA KOLA 750ml", UnidadMedida = "UNIDAD", PrecioUnitario = 2.50,
                Cantidad = 2, Descuento = 0}
        };
    }
}
