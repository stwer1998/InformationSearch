using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsCrawler
{
    //http://akorsa.ru/2017/01/vektornaya-model-i-kosinusnoe-shodstvo-cosine-similarity/

    public class Search
    {
        private const string ParentForderPath = @"d:\InformationSearch\";
        private readonly Uri uri;
        private readonly FileProvider fileProvider;
        private readonly string StemingForder;
        //private readonly string tfForder;
        private readonly string tfIdfForder;
        private readonly int CountDocument = 100;
        private readonly Dictionary<int, Dictionary<string,double>> wordMatrix;
        private Dictionary<string, List<int>> invertDict;
        public Search(string domain)
        {
            uri = new Uri(domain);
            fileProvider = new FileProvider();
            StemingForder = $"{ParentForderPath}/{uri.Host}/stemming/";
            //tfForder = $"{ParentForderPath}/{uri.Host}/tf/";
            tfIdfForder = $"{ParentForderPath}/{uri.Host}/tf-idf/";
            wordMatrix = new Dictionary<int, Dictionary<string, double>>();

            for (int i = 0; i < CountDocument; i++)
            {
                var text = fileProvider.GetTextFromFile($"{tfIdfForder}{i}.txt");
                var dict = text.Split('\n').Select(x => x.Split(":")).ToDictionary(x => x[0].Trim(), y => double.Parse(y[1]));
                wordMatrix.Add(i, dict);
            }

            if (invertDict == null)
            {
                invertDict = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(fileProvider.GetTextFromFile($"{StemingForder}invertlist.txt"));
            }
            invertDict = invertDict.ToDictionary(x => x.Key.Trim(), y => y.Value);

        }

        public void SearchWord(string query)
        {
            var countDocument = 100;
            var porter = new Porter();
            var words = query.Split(' ').Select(x => porter.Stemm(x)).ToArray();
            var length = words.Length;
            var tfs = new double[length];

            for (int i = 0; i < length; i++)
            {
                var count = words.Where(x => x == words[i]).Count();
                tfs[i] = (double)count / length;
            }

            var idfs = new double[length];
            for (int i = 0; i < length; i++)
            {
                if (invertDict.ContainsKey(words[i]))
                {
                    //var r = invertDict[words[i]].Count;
                    idfs[i] = invertDict.ContainsKey(words[i]) ? Math.Round(Math.Log10(countDocument / invertDict[words[i]].Count), 5) : 0;
                }
                else
                {
                    idfs[i] = 0;
                }
            }

            var tfIdf = new Dictionary<string, double>();
            for (int i = 0; i < length; i++)
            {
                tfIdf.Add(words[i], tfs[i] * idfs[i]);
            }

            //Рассчитаем длину запроса
            var queryLenght = Math.Sqrt(tfIdf.Select(x => x.Value).Select(x => Math.Pow(x, 2)).Sum());

            //Документы в которых имеется одно слово
            var indexDocuments = new List<int>();
            foreach (var item in words)
            {
                if (invertDict.ContainsKey(item))
                {
                    indexDocuments.AddRange(invertDict[item]);
                }
            }
            indexDocuments = indexDocuments.Distinct().ToList();

            //
            var docsLenght = new Dictionary<int, double>();

            //вычисляем длину документов
            foreach (var index in indexDocuments)
            {
                docsLenght.Add(index, Math.Sqrt(wordMatrix[index].Values.Select(x => Math.Pow(x, 2)).Sum()));
            }

            //косинусное сходство
            var result = new Dictionary<int, double>();
            foreach (var index in indexDocuments)
            {
                double fstPart = 0;
                foreach (var word in words)
                {
                    //var k = wordMatrix[index][word];
                    if (wordMatrix[index].ContainsKey(word))
                    {

                        fstPart += tfIdf[word] * wordMatrix[index][word];
                    }
                }
                var point = (fstPart / (docsLenght[index] * queryLenght));
                if (point == double.NaN)
                {//если на запросе нет слов которые определяют какой-то документ(слова имеются во всех документых)
                    point = 0;
                }
                result.Add(index, point);
            }

            foreach (var item in result.OrderByDescending(x=>x.Value).Take(10))
            {
                Console.WriteLine($"{item.Key} : {item.Value}");
            }
            Console.WriteLine();
        }
    }
}
