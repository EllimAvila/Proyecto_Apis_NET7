using MagicVilla_API.Datos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Net.Mail;
using System.Net;
using System.Security.AccessControl;
using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Drawing.Layout;
using PdfSharp.Charting;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Humanizer;
using Humanizer.Localisation.NumberToWords;
using System.Globalization;
using QRCoder;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }

        [HttpGet("id")]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if(id == 0)
            {
                
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpGet("enviar-comprobante")]
        public IActionResult EnviarComprobante(string email, string cpe) 
        {
            try
            {
                // REGISTRAR PROVEEDOR DE CODIFICACIÓN
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                // CREAR PDF
                PdfDocument pdf = new PdfDocument();
                PdfPage page = pdf.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                //FUENTES
                XFont fontNormal = new XFont("Arial", 9);
                XFont fontBold = new XFont("Arial", 9, XFontStyle.Bold);
                XFont fontPrincipal = new XFont("Arial", 13, XFontStyle.Bold);
                XFont fontPie = new XFont("Arial", 8);

                //VARIABLES
                var logo = XImage.FromFile("Assets/logo.png");
                string nombreEmpresa = "SERVICENTRO ROBLES E.I.R.L.";

                string direccionPrincipal = "Dirección de la oficina principal de la empresa" + ".";
                string direccionSucursal = "Dirección de la sucursal donde se realizó la transacción y se " +
                    "generó el comprobante de pago electrónico" + ".";
                string direcciones = "Principal: " + direccionPrincipal + "\n" + "Sucursal: " + direccionSucursal;
                //VARIABLES VENTA
                string rucEmpresa = "R.U.C. N° " + "24857965432";
                string tipoComprobante = "Factura Electrónica";
                string numComprobante = cpe;
                string fechaEmision = "06/09/2023";
                string fechaVencimiento = "07/09/2023";
                string condicionPago = "CONTADO";
                string observaciones = "Billete falso";
                string nombreCliente = "Ellim Avila";
                string direccionCliente = "Jr. Saint Jhosep 578";
                string placaCliente = "D05-7C5";
                string guiaRemision = "Guía de remisión";
                string guiaTransportista = "Guía de transportista";
                string rucCliente = "95865412560";
                string hash = "LR2KCSZSZRZEDHQW3H098EEMHLI=";

                //CÁLCULO DINÁMICO DE ALTURA DE TÍTULO
                XTextFormatter tf = new XTextFormatter(gfx);
                tf.Alignment = XParagraphAlignment.Center;

                var altura = gfx.MeasureString(nombreEmpresa, fontPrincipal, XStringFormats.Center);
                if (altura.Width > 476)
                {
                    altura.Height = altura.Height * 3.1;
                }
                else
                {
                    if (altura.Width > 238)
                    {
                        altura.Height = altura.Height * 2.1;
                    }
                }
                XRect areaTitulo = new(149, 25, 238, altura.Height);

                //LOGO, NOMBRE DE LA EMPRESA Y DIRECCIONES
                gfx.DrawImage(logo, 30, 25, 80, 80);
                tf.DrawString(nombreEmpresa, fontPrincipal, XBrushes.Black, areaTitulo, XStringFormats.TopLeft);
                tf.DrawString(direcciones, fontNormal, XBrushes.Black, new XRect(155, areaTitulo.Bottom + 10, 214, 80),
                    XStringFormats.TopLeft);

                //INFORMACIÓN DE CPE - SUPERIOR DERECHA
                //CONTENEDORES
                gfx.DrawRectangle(XBrushes.RoyalBlue, new XRect(423, 35, 149, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(423, 35, 149, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(423, 55, 149, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(423, 75, 149, 20));
                //TEXTO
                gfx.DrawString(rucEmpresa, fontNormal, XBrushes.White, new XRect(423, 40, 149, 20),
                    XStringFormats.TopCenter);
                gfx.DrawString(tipoComprobante, fontNormal, XBrushes.Black, new XRect(423, 60, 149, 20),
                    XStringFormats.TopCenter);
                gfx.DrawString(numComprobante, fontNormal, XBrushes.Black, new XRect(423, 80, 149, 20),
                    XStringFormats.TopCenter);

                //BLOQUE DE FECHAS, CONDICIÓN Y OBSERVACIONES
                //CONTENEDORES
                gfx.DrawRectangle(XBrushes.RoyalBlue, new XRect(15, 120, 565, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 120, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(134, 120, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(253, 120, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(372, 120, 208, 20));

                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 140, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(134, 140, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(253, 140, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(372, 140, 208, 20));
                //TEXTO
                gfx.DrawString("FECHA DE EMISIÓN", fontBold, XBrushes.White, new XRect(15, 120, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString("FECHA DE VENCIMIENTO", fontBold, XBrushes.White, new XRect(134, 120, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString("CONDICIÓN DE PAGO", fontBold, XBrushes.White, new XRect(253, 120, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString("OBSERVACIONES", fontBold, XBrushes.White, new XRect(372, 120, 208, 20),
                    XStringFormats.Center);

                gfx.DrawString(fechaEmision, fontNormal, XBrushes.Black, new XRect(15, 140, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString(fechaVencimiento, fontNormal, XBrushes.Black, new XRect(134, 140, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString(condicionPago, fontNormal, XBrushes.Black, new XRect(253, 140, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString(observaciones, fontNormal, XBrushes.Black, new XRect(372, 140, 208, 20),
                    XStringFormats.Center);

                //BLOQUE DE INFORMACIÓN DEL CLIENTE
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 170, 565, 80));

                gfx.DrawString("RAZÓN SOCIAL        :", fontBold, XBrushes.Black, new XRect(20, 175, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("DIRECCIÓN               :", fontBold, XBrushes.Black, new XRect(20, 195, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("PLACA                       :", fontBold, XBrushes.Black, new XRect(20, 215, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("GUÍA DE REMISIÓN :", fontBold, XBrushes.Black, new XRect(20, 235, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("GUÍA DE TRANSPORTISTA :", fontBold, XBrushes.Black, new XRect(320, 235, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("RUC :", fontBold, XBrushes.Black, new XRect(320, 175, 20, 20),
                    XStringFormats.TopLeft);

                gfx.DrawString(nombreCliente, fontNormal, XBrushes.Black, new XRect(115, 175, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(direccionCliente, fontNormal, XBrushes.Black, new XRect(115, 195, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(placaCliente, fontNormal, XBrushes.Black, new XRect(115, 215, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(guiaRemision, fontNormal, XBrushes.Black, new XRect(115, 235, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(guiaTransportista, fontNormal, XBrushes.Black, new XRect(448, 235, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(rucCliente, fontNormal, XBrushes.Black, new XRect(350, 175, 5, 20),
                    XStringFormats.TopLeft);

                //INFORMACIÓN DE LOS PRODUCTOS ADQUIRIDOS
                //CONTENEDORES
                gfx.DrawRectangle(new XPen(XColors.DarkGray), XBrushes.RoyalBlue, new XRect(15, 260, 565, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 260, 75, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(90, 260, 85, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(175, 260, 170, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(345, 260, 85, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(430, 260, 75, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(505, 260, 75, 20));

                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 280, 75, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(90, 280, 85, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(175, 280, 170, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(345, 280, 85, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(430, 280, 75, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(505, 280, 75, 300));
                //TEXTO
                gfx.DrawString("CANTIDAD", fontBold, XBrushes.White, new XRect(15, 260, 75, 20), XStringFormats.Center);
                gfx.DrawString("UNIDAD/MEDIDA", fontBold, XBrushes.White, new XRect(90, 260, 85, 20), XStringFormats.Center);
                gfx.DrawString("DESCRIPCIÓN", fontBold, XBrushes.White, new XRect(175, 260, 170, 20), XStringFormats.Center);
                gfx.DrawString("PRECIO UNITARIO", fontBold, XBrushes.White, new XRect(345, 260, 85, 20), XStringFormats.Center);
                gfx.DrawString("DESCUENTO", fontBold, XBrushes.White, new XRect(430, 260, 75, 20), XStringFormats.Center);
                gfx.DrawString("IMPORTE", fontBold, XBrushes.White, new XRect(505, 260, 75, 20), XStringFormats.Center);

                gfx.DrawString("SON:", fontNormal, XBrushes.Black, new XRect(20, 590, 20, 20), XStringFormats.TopLeft);


                gfx.DrawString("SUB TOTAL              S/", fontBold, XBrushes.Black, new XRect(420, 590, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("DESCUENTO            S/", fontBold, XBrushes.Black, new XRect(420, 610, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("OP. GRAVADAS       S/", fontBold, XBrushes.Black, new XRect(420, 630, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("OP. INAFECTA         S/", fontBold, XBrushes.Black, new XRect(420, 650, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("OP. EXONERADA    S/", fontBold, XBrushes.Black, new XRect(420, 670, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("OP. GRATUITAS      S/", fontBold, XBrushes.Black, new XRect(420, 690, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("I.G.V.  18%                S/", fontBold, XBrushes.Black, new XRect(420, 710, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("IMPORTE TOTAL     S/", fontBold, XBrushes.Black, new XRect(420, 730, 100, 20), XStringFormats.CenterLeft);

                //CÁLCULO DE LOS MONTOS DE LA VENTA - DETALLE DE VENTA
                //VARIABLES VENTA
                double subTotal = 0, opGravadas = 0, opInafecta = 0, opExonerada = 0, opGratuitas = 0, igv = 0.18,
                    importeTotal = 0, montoDcto = 0;
                int indiceVenta = 0;
                foreach (var venta in VillaStore.detalleVenta)
                {
                    double importeVenta = (venta.Cantidad * venta.PrecioUnitario) - venta.Descuento;
                    subTotal += importeVenta;
                    montoDcto += venta.Descuento;

                    gfx.DrawString(venta.Cantidad.ToString(), fontNormal, XBrushes.Black, new XRect(15, 285 + indiceVenta * 20, 75, 15), XStringFormats.Center);
                    gfx.DrawString(venta.UnidadMedida, fontNormal, XBrushes.Black, new XRect(90, 285 + indiceVenta * 20, 85, 15), XStringFormats.Center);
                    gfx.DrawString(venta.DescripcionProducto, fontNormal, XBrushes.Black, new XRect(175, 285 + indiceVenta * 20, 170, 15), XStringFormats.Center);
                    gfx.DrawString(venta.PrecioUnitario.ToString(), fontNormal, XBrushes.Black, new XRect(345, 285 + indiceVenta * 20, 85, 15), XStringFormats.Center);
                    gfx.DrawString(venta.Descuento.ToString(), fontNormal, XBrushes.Black, new XRect(430, 285 + indiceVenta * 20, 75, 15), XStringFormats.Center);
                    gfx.DrawString(importeVenta.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(505, 285 + indiceVenta * 20, 75, 15), XStringFormats.Center);

                    indiceVenta++;
                }

                //CÁLCULO DE LOS MONTOS DE LA VENTA
                opGravadas = subTotal;
                importeTotal = subTotal + (igv * subTotal);

                gfx.DrawString(subTotal.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 590, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(montoDcto.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 610, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(opGravadas.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 630, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(opInafecta.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 650, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(opExonerada.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 670, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(opGratuitas.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 690, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString((igv * subTotal).ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 710, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(importeTotal.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 730, 100, 20), XStringFormats.CenterRight);

                if (importeTotal > 0)
                {
                    //EXPRESAR MONTO EN PALABRAS
                    double parteEntera = Math.Truncate(importeTotal);
                    double parteDecimal = importeTotal - parteEntera;
                    string textoCompletar = " con " + Math.Round(parteDecimal * 100) + "/100 soles";
                    int convEntero = (int)parteEntera;
                    gfx.DrawString(convEntero.ToWords(new CultureInfo("es")) + textoCompletar, fontNormal, XBrushes.Black, new XRect(20, 610, 20, 20),
                    XStringFormats.TopLeft);
                }

                //GENERAR QR
                string urlQr = "Información sobre el comprobante de pago electrónico";
                QRCodeGenerator generadorQr = new QRCodeGenerator();
                QRCodeData datosQr = generadorQr.CreateQrCode(urlQr, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new QRCode(datosQr);
                Bitmap imagenQr = qRCode.GetGraphic(20);

                MemoryStream qrStream = new MemoryStream();
                imagenQr.Save(qrStream, ImageFormat.Png);

                gfx.DrawImage(XImage.FromStream(qrStream), 17, 650, 100, 100);

                //TEXTO DE PÍE DE PÁGINA
                string textoPie1 = "Representación impresa de " + tipoComprobante.ToUpper() + ", disponible en www.sunat.gob.pe";
                string textoPie2 = "Autorizado mediante Resolución Nro. 018-005-0002710/SUNAT";
                string textoPie3 = "Obtenga copia de su documento en http://4-fact.com/sven/auth/consulta";
                string textoPie4 = "Código hash : " + hash;
                gfx.DrawString(textoPie1, fontPie, XBrushes.Gray, new XRect(17, 760, 100, 15), XStringFormats.CenterLeft);
                gfx.DrawString(textoPie2, fontPie, XBrushes.Gray, new XRect(17, 775, 100, 15), XStringFormats.CenterLeft);
                gfx.DrawString(textoPie3, fontPie, XBrushes.Gray, new XRect(17, 790, 100, 15), XStringFormats.CenterLeft);
                gfx.DrawString(textoPie4, fontPie, XBrushes.Gray, new XRect(17, 805, 100, 15), XStringFormats.CenterLeft);

                // ALMACENAR PDF EN MEMORYSTREAM
                MemoryStream pdfStream = new MemoryStream();
                pdf.Save(pdfStream, false);
                pdfStream.Position = 0;

                // CONFIGURACIÓN DE PARÁMETROS DE CORREO ELECTRÓNICO
                SmtpClient smtpClient = new SmtpClient("smtp.titan.email")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("test@lamtv.pe", "218562eaz/"),
                    EnableSsl = true,
                };

                // DAR FORMATO AL CORREO A ENVIAR
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("test@lamtv.pe","App Sven"),
                    Subject = "Comprobante electrónico generado",
                    Body = "Estimado cliente, se ha emitido el comprobante electrónico de su compra en nuestro" +
                    " establecimiento, gracias por su preferencia  y vuelva pronto.",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(email);

                // ADJUNTAR PDF GENERADO
                Attachment pdfAttachment = new Attachment(pdfStream, cpe+".pdf", "application/pdf");
                mailMessage.Attachments.Add(pdfAttachment);

                // ENVIAR
                smtpClient.Send(mailMessage);

                return Ok("Correo enviado con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar y enviar el PDF: {ex.StackTrace} \n {ex.Message}");
            }
        }

        [HttpGet("descargar-comprobante")]
        public IActionResult DescargarComprobante(string cpe)
        {
            try
            {
                // REGISTRAR PROVEEDOR DE CODIFICACIÓN
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                // CREAR PDF
                PdfDocument pdf = new PdfDocument();
                PdfPage page = pdf.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                    
                    //FUENTES
                XFont fontNormal = new XFont("Arial", 9);
                XFont fontBold = new XFont("Arial", 9, XFontStyle.Bold);
                XFont fontPrincipal = new XFont("Arial", 13, XFontStyle.Bold);
                XFont fontPie = new XFont("Arial", 8);
                    
                    //VARIABLES
                var logo = XImage.FromFile("Assets/logo.png");
                string nombreEmpresa = "SERVICENTRO ROBLES E.I.R.L.";

                string direccionPrincipal = "Dirección de la oficina principal de la empresa"+".";
                string direccionSucursal = "Dirección de la sucursal donde se realizó la transacción y se " +
                    "generó el comprobante de pago electrónico"+".";
                string direcciones ="Principal: "+direccionPrincipal + "\n" +"Sucursal: "+ direccionSucursal;
                    //VARIABLES VENTA
                string rucEmpresa = "R.U.C. N° "+"24857965432";
                string tipoComprobante = "Factura Electrónica";
                string numComprobante = cpe;
                string fechaEmision = "06/09/2023";
                string fechaVencimiento= "07/09/2023";
                string condicionPago = "CONTADO";
                string observaciones = "Billete falso";
                string nombreCliente = "Ellim Avila";
                string direccionCliente = "Jr. Saint Jhosep 578";
                string placaCliente = "D05-7C5";
                string guiaRemision = "Guía de remisión";
                string guiaTransportista = "Guía de transportista";
                string rucCliente = "95865412560";
                string hash = "LR2KCSZSZRZEDHQW3H098EEMHLI=";

                //CÁLCULO DINÁMICO DE ALTURA DE TÍTULO
                XTextFormatter tf = new XTextFormatter(gfx);
                tf.Alignment=XParagraphAlignment.Center;

                var altura = gfx.MeasureString(nombreEmpresa, fontPrincipal, XStringFormats.Center);
                if(altura.Width  > 476)
                {
                    altura.Height = altura.Height*3.1;
                }
                else
                {
                    if (altura.Width > 238)
                    {
                        altura.Height = altura.Height * 2.1;
                    }
                }
                XRect areaTitulo = new(149, 25, 238, altura.Height);

                    //LOGO, NOMBRE DE LA EMPRESA Y DIRECCIONES
                gfx.DrawImage(logo, 30, 25, 80, 80);
                tf.DrawString(nombreEmpresa, fontPrincipal, XBrushes.Black, areaTitulo,XStringFormats.TopLeft);
                tf.DrawString(direcciones, fontNormal, XBrushes.Black, new XRect(155, areaTitulo.Bottom+10, 214, 80),
                    XStringFormats.TopLeft);

                    //INFORMACIÓN DE CPE - SUPERIOR DERECHA
                        //CONTENEDORES
                gfx.DrawRectangle(XBrushes.RoyalBlue, new XRect(423, 35, 149, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(423, 35, 149, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(423, 55, 149, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(423, 75, 149, 20));
                        //TEXTO
                gfx.DrawString(rucEmpresa, fontNormal, XBrushes.White, new XRect(423, 40, 149, 20), 
                    XStringFormats.TopCenter);
                gfx.DrawString(tipoComprobante, fontNormal, XBrushes.Black, new XRect(423, 60, 149, 20), 
                    XStringFormats.TopCenter);
                gfx.DrawString(numComprobante, fontNormal, XBrushes.Black, new XRect(423, 80, 149, 20), 
                    XStringFormats.TopCenter);

                    //BLOQUE DE FECHAS, CONDICIÓN Y OBSERVACIONES
                        //CONTENEDORES
                gfx.DrawRectangle(XBrushes.RoyalBlue, new XRect(15, 120, 565, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 120, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(134, 120, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(253, 120, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(372, 120, 208, 20));
                        
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 140, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(134, 140, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(253, 140, 119, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(372, 140, 208, 20));
                        //TEXTO
                gfx.DrawString("FECHA DE EMISIÓN", fontBold, XBrushes.White, new XRect(15, 120, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString("FECHA DE VENCIMIENTO", fontBold, XBrushes.White, new XRect(134, 120, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString("CONDICIÓN DE PAGO", fontBold, XBrushes.White, new XRect(253, 120, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString("OBSERVACIONES", fontBold, XBrushes.White, new XRect(372, 120, 208, 20),
                    XStringFormats.Center);

                gfx.DrawString(fechaEmision, fontNormal, XBrushes.Black, new XRect(15, 140, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString(fechaVencimiento, fontNormal, XBrushes.Black, new XRect(134, 140, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString(condicionPago, fontNormal, XBrushes.Black, new XRect(253, 140, 119, 20),
                    XStringFormats.Center);
                gfx.DrawString(observaciones, fontNormal, XBrushes.Black, new XRect(372, 140, 208, 20),
                    XStringFormats.Center);

                //BLOQUE DE INFORMACIÓN DEL CLIENTE
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 170, 565, 80));

                gfx.DrawString("RAZÓN SOCIAL        :", fontBold, XBrushes.Black, new XRect(20, 175, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("DIRECCIÓN               :", fontBold, XBrushes.Black, new XRect(20, 195, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("PLACA                       :", fontBold, XBrushes.Black, new XRect(20, 215, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("GUÍA DE REMISIÓN :", fontBold, XBrushes.Black, new XRect(20, 235, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("GUÍA DE TRANSPORTISTA :", fontBold, XBrushes.Black, new XRect(320, 235, 20, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString("RUC :", fontBold, XBrushes.Black, new XRect(320, 175, 20, 20),
                    XStringFormats.TopLeft);

                gfx.DrawString(nombreCliente, fontNormal, XBrushes.Black, new XRect(115, 175, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(direccionCliente, fontNormal, XBrushes.Black, new XRect(115, 195, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(placaCliente, fontNormal, XBrushes.Black, new XRect(115, 215, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(guiaRemision, fontNormal, XBrushes.Black, new XRect(115, 235, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(guiaTransportista, fontNormal, XBrushes.Black, new XRect(448, 235, 5, 20),
                    XStringFormats.TopLeft);
                gfx.DrawString(rucCliente, fontNormal, XBrushes.Black, new XRect(350, 175, 5, 20),
                    XStringFormats.TopLeft);

                //INFORMACIÓN DE LOS PRODUCTOS ADQUIRIDOS
                    //CONTENEDORES
                gfx.DrawRectangle(new XPen(XColors.DarkGray), XBrushes.RoyalBlue, new XRect(15, 260, 565, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 260, 75, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(90, 260, 85, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(175, 260, 170, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(345, 260, 85, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(430, 260, 75, 20));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(505, 260, 75, 20));

                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(15, 280, 75, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(90, 280, 85, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(175, 280, 170, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(345, 280, 85, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(430, 280, 75, 300));
                gfx.DrawRectangle(new XPen(XColors.DarkGray), new XRect(505, 280, 75, 300));
                //TEXTO
                gfx.DrawString("CANTIDAD", fontBold, XBrushes.White, new XRect(15, 260, 75, 20),XStringFormats.Center);
                gfx.DrawString("UNIDAD/MEDIDA", fontBold, XBrushes.White, new XRect(90, 260, 85, 20), XStringFormats.Center);
                gfx.DrawString("DESCRIPCIÓN", fontBold, XBrushes.White, new XRect(175, 260, 170, 20), XStringFormats.Center);
                gfx.DrawString("PRECIO UNITARIO", fontBold, XBrushes.White, new XRect(345, 260, 85, 20), XStringFormats.Center);
                gfx.DrawString("DESCUENTO", fontBold, XBrushes.White, new XRect(430, 260, 75, 20), XStringFormats.Center);
                gfx.DrawString("IMPORTE", fontBold, XBrushes.White, new XRect(505, 260, 75, 20), XStringFormats.Center);

                gfx.DrawString("SON:", fontNormal, XBrushes.Black, new XRect(20, 590, 20, 20), XStringFormats.TopLeft);


                gfx.DrawString("SUB TOTAL              S/", fontBold, XBrushes.Black, new XRect(420, 590, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("DESCUENTO            S/", fontBold, XBrushes.Black, new XRect(420, 610, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("OP. GRAVADAS       S/", fontBold, XBrushes.Black, new XRect(420, 630, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("OP. INAFECTA         S/", fontBold, XBrushes.Black, new XRect(420, 650, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("OP. EXONERADA    S/", fontBold, XBrushes.Black, new XRect(420, 670, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("OP. GRATUITAS      S/", fontBold, XBrushes.Black, new XRect(420, 690, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("I.G.V.  18%                S/", fontBold, XBrushes.Black, new XRect(420, 710, 100, 20), XStringFormats.CenterLeft);
                gfx.DrawString("IMPORTE TOTAL     S/", fontBold, XBrushes.Black, new XRect(420, 730, 100, 20), XStringFormats.CenterLeft);

                //CÁLCULO DE LOS MONTOS DE LA VENTA - DETALLE DE VENTA
                //VARIABLES VENTA
                double subTotal=0, opGravadas=0, opInafecta = 0, opExonerada = 0, opGratuitas = 0, igv = 0.18,
                    importeTotal = 0, montoDcto = 0;
                int indiceVenta = 0;
                foreach (var venta in VillaStore.detalleVenta)
                {
                    double importeVenta = (venta.Cantidad * venta.PrecioUnitario) - venta.Descuento;
                    subTotal += importeVenta;
                    montoDcto += venta.Descuento;

                    gfx.DrawString(venta.Cantidad.ToString(), fontNormal, XBrushes.Black, new XRect(15, 285+ indiceVenta * 20, 75, 15), XStringFormats.Center);
                    gfx.DrawString(venta.UnidadMedida, fontNormal, XBrushes.Black, new XRect(90, 285 + indiceVenta * 20, 85, 15), XStringFormats.Center);
                    gfx.DrawString(venta.DescripcionProducto, fontNormal, XBrushes.Black, new XRect(175, 285 + indiceVenta * 20, 170, 15), XStringFormats.Center);
                    gfx.DrawString(venta.PrecioUnitario.ToString(), fontNormal, XBrushes.Black, new XRect(345, 285 + indiceVenta * 20, 85, 15), XStringFormats.Center);
                    gfx.DrawString(venta.Descuento.ToString(), fontNormal, XBrushes.Black, new XRect(430, 285 + indiceVenta * 20, 75, 15), XStringFormats.Center);
                    gfx.DrawString(importeVenta.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(505, 285 + indiceVenta * 20, 75, 15), XStringFormats.Center);

                    indiceVenta++;
                }

                //CÁLCULO DE LOS MONTOS DE LA VENTA
                opGravadas = subTotal;
                importeTotal = subTotal + (igv * subTotal);

                gfx.DrawString(subTotal.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 590, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(montoDcto.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 610, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(opGravadas.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 630, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(opInafecta.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 650, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(opExonerada.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 670, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(opGratuitas.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 690, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString((igv*subTotal).ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 710, 100, 20), XStringFormats.CenterRight);
                gfx.DrawString(importeTotal.ToString("0.00"), fontNormal, XBrushes.Black, new XRect(480, 730, 100, 20), XStringFormats.CenterRight);

                if (importeTotal > 0)
                {
                    //EXPRESAR MONTO EN PALABRAS
                    double parteEntera = Math.Truncate(importeTotal);
                    double parteDecimal = importeTotal - parteEntera;
                    string textoCompletar = " con " + Math.Round(parteDecimal * 100) + "/100 soles";
                    int convEntero = (int)parteEntera;
                    gfx.DrawString(convEntero.ToWords(new CultureInfo("es")) + textoCompletar, fontNormal, XBrushes.Black, new XRect(20, 610, 20, 20),
                    XStringFormats.TopLeft);
                }

                //GENERAR QR
                string urlQr = "Información sobre el comprobante de pago electrónico";
                QRCodeGenerator generadorQr = new QRCodeGenerator();
                QRCodeData datosQr = generadorQr.CreateQrCode(urlQr, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new QRCode(datosQr);
                Bitmap imagenQr = qRCode.GetGraphic(20);

                MemoryStream qrStream = new MemoryStream();
                imagenQr.Save(qrStream, ImageFormat.Png);

                gfx.DrawImage(XImage.FromStream(qrStream), 17, 650, 100, 100);

                //TEXTO DE PÍE DE PÁGINA
                string textoPie1 = "Representación impresa de "+tipoComprobante.ToUpper()+", disponible en www.sunat.gob.pe";
                string textoPie2 = "Autorizado mediante Resolución Nro. 018-005-0002710/SUNAT";
                string textoPie3 = "Obtenga copia de su documento en http://4-fact.com/sven/auth/consulta";
                string textoPie4 = "Código hash : "+hash;
                gfx.DrawString(textoPie1, fontPie, XBrushes.Gray, new XRect(17, 760, 100, 15), XStringFormats.CenterLeft);
                gfx.DrawString(textoPie2, fontPie, XBrushes.Gray, new XRect(17, 775, 100, 15), XStringFormats.CenterLeft);
                gfx.DrawString(textoPie3, fontPie, XBrushes.Gray, new XRect(17, 790, 100, 15), XStringFormats.CenterLeft);
                gfx.DrawString(textoPie4, fontPie, XBrushes.Gray, new XRect(17, 805, 100, 15), XStringFormats.CenterLeft);

                // ALMACENAR PDF EN MEMORYSTREAM
                MemoryStream pdfStream = new MemoryStream();
                pdf.Save(pdfStream, false);
                pdfStream.Position = 0;



                // DEVOLVER PDF COMO RESPUESTA DE ARCHIVO
                //DESCARGAR
                return File(pdfStream, "application/pdf", cpe + ".pdf");

                //MOSTRAR PDF
                //Response.Headers.Add("Content-Disposition", $"inline; filename={cpe}.pdf");
                //return File(pdfStream, "application/pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el PDF: {ex.StackTrace} \n {ex.Message}");
            }
        }
    }
}
