using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SectorModel.QuoteLoader
{
    class Program
    {
        

        static async Task Main(string[] args)
        {
            CloudDataManager cdm = new CloudDataManager();

             Console.WriteLine("started loading quotes");

            //htis model is the SPDR model which has all ETFs in it
             await cdm.UpdateQuotes(Guid.Parse("4237E32A-5C60-4141-8658-FA357C28EF28"));

            Console.WriteLine("finished loading quotes");

        }
    }
}
