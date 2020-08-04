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

             await cdm.UpdateQuotes(Guid.Parse("FAC8A666-74D8-4531-B3AD-DA7B95360462"));

            Console.WriteLine("finished loading quotes");

        }
    }
}
