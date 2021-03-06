using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IsCrawler
{
    public class Stemming
    {
        private const string ParentForderPath = @"d:\InformationSearch\";
        private readonly Uri uri;
        private readonly FileProvider fileProvider;

        public Stemming(string domain)
        {
            uri = new Uri(domain);
            fileProvider = new FileProvider();
            var stemmingforder = $"{ParentForderPath}/{uri.Host}/stemming/";
            if (!Directory.Exists(stemmingforder))
            {//Create forder for link
                Directory.CreateDirectory(stemmingforder);
            }

        }

        public void StartStemming() 
        {
            var index = fileProvider.GetTextFromFile($"{ParentForderPath}/{uri.Host}/index.txt");
            var files = index.Split("\n").Select(x => x.Split(" ")).Select(x => x[2]).ToList();
            foreach (var file in files)
            {
                StemmingFile(file);
            }
        }

        private void StemmingFile(string path) 
        {
            var text = fileProvider.GetTextFromFile(path);
            var words = new List<string>();
            MatchCollection collection = Regex.Matches(text, @"([\w]{1,})");
            var porter = new Porter();
            foreach (Match word in collection)
            {
                string stremmed;
                if (word.Value.Length > 4)
                {
                    stremmed = porter.Stemm(word.Value);
                }
                else 
                {
                    stremmed = word.Value;
                }
                words.Add(stremmed);
            }
            var filename = Regex.Match(path, @"([\d]*.txt)");
            fileProvider.WriteTextToFile($"{ParentForderPath}/{uri.Host}/stemming/{filename.Value}", string.Join(' ',words));
        }
    }
}
