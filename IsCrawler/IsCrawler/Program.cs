using System;
using System.Collections.Generic;

namespace IsCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter link:");

            var d = "https://metanit.com/";

            //var crawler = new Crawler.Crawler();
            //string link = Console.ReadLine();
            //crawler.Crawl(link);

            //var stemming = new Stemming(d);
            //stemming.StartStemming();

            var inverter = new InvertList(d);
            //inverter.Invert();
            var search = inverter.Search("оформляет & случае & качестве");

            var tdidf = new TfIdf(d);
            tdidf.TF();
            tdidf.Idf();
            tdidf.TfIdfCalc();

            Console.WriteLine();



        }
    }
}
