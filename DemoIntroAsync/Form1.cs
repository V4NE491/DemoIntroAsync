using System.Diagnostics;
using System.Drawing.Text;
using System.Security.Policy;

namespace DemoIntroAsync
{
    public partial class Form1 : Form
    {

        HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            //variables finales, severo enrredo con ese video

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes/resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes/resultado-Paralelo");
            PrepararEjecucion(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("inicio");
            List<Image> Imagenes = ObtenerImagenes();

            //parte secuencial

            var sw = new Stopwatch();
            sw.Start();

            foreach (var image in Imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, image);
            }

            Console.WriteLine("Secuencial - duracion en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();
            sw.Start();

            var tareasNumerable = Imagenes.Select(async async =>
            {
                await ProcesarImagen(destinoBaseParalelo, Imagenes);
            });

            Console.WriteLine("Paralelo - duracion en segundos: {0}", 
                sw.ElapsedMilliseconds / 1000.0);

            sw.Stop();
            pictureBox1.Visible = false;
        }

        private async Task ProcesarImagen(string directorio, Image imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsStringAsync();

            Bitmap bitmap;
            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            string destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);   
        }

        //de aqui en adelante viene del ejemplo final
        private static List<Image> ObtenerImagenes()
        {
            var Imagenes = new List<Imagenes>();
            for (int i = 0; i < 7; i++)
            {
                Imagenes.Add(
                    new Imagenes()
                    {
                        Nombre = $"Vacas Peludas{i}.jpg",
                        URL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fco.pinterest.com%2Fmarbus8%2Fvacas-peludas%2F&psig=AOvVaw0_qPKY-MNMPs_w8un8wwBT&ust=1721466442598000&source=images&cd=vfe&opi=89978449&ved=0CA8QjRxqFwoTCICP3q_gsocDFQAAAAAdAAAAABAE"
                    });
                Imagenes.Add(
                    new Imagenes()
                    {
                        Nombre = $"Happy Travel {i}.jpg",
                        URL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.facebook.com%2Fhappytravel.vacaciones%2Fposts%2Festas-vacas-peludas-de-las-tierras-altas-est%25C3%25A1n-deseando-daros-la-bienvenida-a-es%2F2477303722312886%2F&psig=AOvVaw2XbVj5Yc13B-1pLZIUURFs&ust=1721467240013000&source=images&cd=vfe&opi=89978449&ved=0CA8QjRxqFwoTCLDs_qvjsocDFQAAAAAdAAAAABAS"
                    });
                Imagenes.Add(
                    new Imagenes()
                    {
                        Nombre = $"Peluditas de Escocia {i}.jpg",
                        URL = "https://agroecuadortv.com/wp-content/uploads/2023/07/agroganaderia_escocia-vacas-peludas_003.jpg"
                    });
                Imagenes.Add(
                    new Imagenes()
                    {
                        Nombre = $"Vaquita comiendo {i}.jpg",
                        URL = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRVmNEOftzc-8qpVbMp8iyuPr5lD-ucuHYgC5EMuUGEBQiSah2QyGnunybaFpOeAUAcSW0&usqp=CAU"
                    });
                Imagenes.Add(
                    new Imagenes()
                    {
                        Nombre = $"Vaca Montañesa{i}.jpg",
                        URL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fes.pngtree.com%2Ffreebackground%2Fhighlander-cows-in-the-dunes-of-wassenaar-hairy-cow-outdoor-photo_14295688.html&psig=AOvVaw2XbVj5Yc13B-1pLZIUURFs&ust=1721467240013000&source=images&cd=vfe&opi=89978449&ved=0CA8QjRxqFwoTCLDs_qvjsocDFQAAAAAdAAAAABA2"
                    });
                Imagenes.Add(
                    new Imagenes()
                    {
                        Nombre = $"Vaquitas peluditas {i}.jpg",
                        URL = "https://archive.org/download/vaca-peluda-escocia/vaca-peluda-escocia_002.jpg"
                    });
                Imagenes.Add(
                    new Imagenes()
                    {
                        Nombre = $" 3 Vaquitas bonitas{i}.jpg",
                        URL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.freepik.es%2Ffotos-premium%2Ftres-vacas-peludas-paradas-campo-nevado-cuernos-levantados-ai-generativo_74030074.htm&psig=AOvVaw2XbVj5Yc13B-1pLZIUURFs&ust=1721467240013000&source=images&cd=vfe&opi=89978449&ved=0CA8QjRxqFwoTCLDs_qvjsocDFQAAAAAdAAAAABA_"
                    });
                
            }
            return Imagenes; 
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoBaseParalelo, string destinoBaseSecuencial)
        { 
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }

            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }

            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBaseParalelo);
        }
        //aqui es lo anterior de el ejemplo

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay (1000);
            Console.WriteLine("Proceso A finalizado con exito");
        }

        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso B finalizado con exito");
        }

        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso C finalizado con exito");
        }
    }
}
