using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Xml.Linq;
using WebJobChollometro.Data;
using WebJobChollometro.Models;

namespace WebJobChollometro.Repositories
{
    public class RepositoryChollometro
    {
        private ChollometroContext context;

        public RepositoryChollometro(ChollometroContext context)
        {
            this.context = context;
        }

        private async Task<int> GetMaxIdCholloAsync()
        {
            if (await this.context.Chollos.CountAsync() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Chollos.MaxAsync(x => x.IdChollo) + 1;
            }
        }

        public async Task<List<Chollo>> GetChollosWebAsync()
        {
            string url = "https://www.chollometro.com/rss";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Accept = @"text/html application/xhtml+xml, *.*";
            request.Host = "www.chollometro.com";
            request.Headers.Add("Accept-language", "es-ES");
            request.Referer = "https://www.chollometro.com";
            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            string xmlData = "";

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                xmlData = await reader.ReadToEndAsync();
            }
            XDocument document = XDocument.Parse(xmlData);

            var consulta = from datos in document.Descendants("item")
                           select datos;

            int idChollo = await this.GetMaxIdCholloAsync();

            List<Chollo> chollos = new List<Chollo>();

            foreach (var tag in consulta)
            {
                Chollo c = new Chollo();

                c.IdChollo = idChollo;
                c.Titulo = tag.Element("title").Value;
                c.Link = tag.Element("link").Value;
                c.Descripcion = tag.Element("description").Value;
                c.Fecha = DateTime.Parse(tag.Element("pubDate").Value);
                idChollo++;
                chollos.Add(c);
            }

            return chollos;
        }

        public async Task PopulateChollosAzureAsync()
        {
            List<Chollo> chollosWeb = await this.GetChollosWebAsync();

            foreach (Chollo c in chollosWeb)
            {
                await this.context.Chollos.AddAsync(c);
            }

            await this.context.SaveChangesAsync();
        }
    }
}
