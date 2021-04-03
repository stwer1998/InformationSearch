using System;
using System.Collections.Generic;

namespace IsCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter link:");

            var link = "https://metanit.com/sharp/aspnet5/2.16.php";

            var crawler = new Crawler.Crawler();
            //string link = Console.ReadLine();
            crawler.Crawl(link);

            var stemming = new Stemming(link);
            stemming.StartStemming();

            var inverter = new InvertList(link);
            inverter.Invert();
            //var search = inverter.Search("оформляет & случае & качестве");

            var tdidf = new TfIdf(link);
            tdidf.TF();
            tdidf.Idf();
            tdidf.TfIdfCalc();

            var search = new Search(link);
            search.SearchWord("общеязыковая среда конструкторах");//конструкторах 25   StudioПоследнее 2

            Console.WriteLine();



        }
    }
}
