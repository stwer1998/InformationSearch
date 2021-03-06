using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsCrawler
{
    public class TfIdf
    {//https://ru.wikipedia.org/wiki/TF-IDF
        private const string ParentForderPath = @"d:\InformationSearch\";
        private readonly Uri uri;
        private readonly FileProvider fileProvider;
        private readonly string StemingForder;
        private readonly string tfForder;
        private readonly string tfIdfForder;

        private int docNum;

        public TfIdf(string domain)
        {
            uri = new Uri(domain);
            fileProvider = new FileProvider();
            StemingForder = $"{ParentForderPath}/{uri.Host}/stemming/";
            tfForder = $"{ParentForderPath}/{uri.Host}/tf/";
            tfIdfForder = $"{ParentForderPath}/{uri.Host}/tf-idf/";

            if (!Directory.Exists(tfForder))
            {//Create forder for link
                Directory.CreateDirectory(tfForder);
            }

            if (!Directory.Exists(tfIdfForder))
            {//Create forder for link
                Directory.CreateDirectory(tfIdfForder);
            }
            var index = fileProvider.GetTextFromFile($"{ParentForderPath}/{uri.Host}/index.txt");
            var files = index.Split("\n").Select(x => x.Split(" ")).Select(x => x[2]).ToList();
            docNum = files.Count;
        }

        public void TF() 
        {            
            for (int i = 0; i < docNum; i++)
            {
                var dict = new Dictionary<string, int>();
                var text = fileProvider.GetTextFromFile($"{StemingForder}{i}.txt");
                var words = fileProvider.GetTextFromFile($"{StemingForder}{i}.txt").Split(' ').ToList();
                foreach (var word in words)
                {
                    if (dict.ContainsKey(word))
                    {
                        dict[word]++;
                    }
                    else
                    {
                        dict.Add(word, 1);
                    }
                }
                var allWords = dict.Select(x => x.Value).Sum();
                var result = string.Join('\n', dict.Select(x => $"{x.Key} : {Math.Round((double)x.Value / allWords,5).ToString("0.00000")}").ToList());
                fileProvider.WriteTextToFile($"{tfForder}{i}.txt", result);
            }
        }

        public void Idf() 
        {
            var dict = new Dictionary<string, int>();

            for (int i = 0; i < docNum; i++)
            {
                var text = fileProvider.GetTextFromFile($"{StemingForder}{i}.txt");
                var words = fileProvider.GetTextFromFile($"{StemingForder}{i}.txt").Split(' ').ToList();
                foreach (var word in words.Distinct())
                {
                    if (dict.ContainsKey(word))
                    {
                        dict[word]++;
                    }
                    else
                    {
                        dict.Add(word, 1);
                    }
                }
            }

            var result = string.Join('\n', dict.Select(x => $"{x.Key} : {Math.Round(Math.Log10((double)docNum / x.Value),5).ToString("0.00000")}"));
            fileProvider.WriteTextToFile($"{tfForder}idf.txt", result);
        }

        public void TfIdfCalc() 
        {
            var idf = fileProvider.GetTextFromFile($"{tfForder}idf.txt").Split('\n').Select(x=>x.Split(':')).ToDictionary(x=>x[0].Trim(),z=>double.Parse(z[1].Trim()));
            for (int i = 0; i < docNum; i++)
            {
                var tf = fileProvider.GetTextFromFile($"{tfForder}{i}.txt").Split('\n').Select(x => x.Split(":")).ToDictionary(x => x[0].Trim(), z =>double.Parse(z[1].Trim()));
                var text = string.Join('\n', tf.Select(x => $"{x.Key} : {Math.Round((double)(x.Value * idf[x.Key]),5).ToString("0.00000")}"));
                fileProvider.WriteTextToFile($"{tfIdfForder}{i}.txt", text);
            }

        }
    }
}
