using System;

namespace IsCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter link:");

            var crawler = new Crawler.Crawler();
            string link = Console.ReadLine();
            crawler.Crawl(link);

            Console.WriteLine();



        }
    }
}
