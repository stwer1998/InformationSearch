using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsCrawler
{
    public class InvertList
    {//https://up-promo.pro/seo-blog/vvedenie-v-poisk/invertirovannyij-indeks/
        //https://habr.com/ru/post/53987/

        //https://professorweb.ru/my/LINQ/base/level2/2_9.php

        private const string ParentForderPath = @"d:\InformationSearch\";
        private readonly Uri uri;
        private readonly FileProvider fileProvider;
        private readonly string StemingForder;
        private Dictionary<string, List<int>> indexedDict;
        private int docNum;
        public InvertList(string domain)
        {
            uri = new Uri(domain);
            fileProvider = new FileProvider();
            StemingForder = $"{ParentForderPath}/{uri.Host}/stemming/";            
        }

        public void Invert()
        {
            var dict = new Dictionary<string, IList<int>>();
            var index = fileProvider.GetTextFromFile($"{ParentForderPath}/{uri.Host}/index.txt");
            var files = index.Split("\n").Select(x => x.Split(" ")).Select(x => x[2]).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                var words = fileProvider.GetTextFromFile($"{StemingForder}{i}.txt").Split(' ').ToList();
                foreach (var word in words)
                {
                    if (dict.ContainsKey(word)) 
                    {
                        dict[word].Add(i);
                    }
                    else 
                    {
                        dict.Add(word, new List<int> { i });
                    }
                }
            }

            foreach (var item in dict)
            {
                dict[item.Key] = item.Value.Distinct().ToList();
            }

            var invertDict = JsonConvert.SerializeObject(dict);
            fileProvider.WriteTextToFile($"{StemingForder}invertlist.txt", invertDict);
        }

        public List<int> Search(string query) 
        {
            if (indexedDict == null) 
            {
                indexedDict = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(fileProvider.GetTextFromFile($"{StemingForder}invertlist.txt"));
            }
            var index = fileProvider.GetTextFromFile($"{ParentForderPath}/{uri.Host}/index.txt");
            var files = index.Split("\n").Select(x => x.Split(" ")).Select(x => x[2]).ToList();
            docNum = files.Count;
            var words = query.Split(' ');

            var result = new List<int>();
            if (words[0].StartsWith("!")) 
            {
                result.AddRange(NotExist(words[0]));
            }
            else 
            {
                result.AddRange(Exist(words[0]));
            }
            result.Sort();

            for (int i = 2; i < words.Length; i+=2)
            {
                if (words[i - 1] == "&") 
                {//пересечение
                    result = result.Intersect(words[i].StartsWith("!") ? NotExist(words[i]) : Exist(words[i])).Distinct().ToList();
                }
                else
                {//объединение
                    result = result.Union(words[i].StartsWith("!") ? NotExist(words[i]) : Exist(words[i])).Distinct().ToList();
                }
            }


            result.Sort();

            return result;
        }

        private List<int> Exist(string word) 
        {
            var porter = new Porter();
            word = porter.Stemm(word);
            if (indexedDict.ContainsKey(word))
            {
                return indexedDict[word];
            }
            else 
            {
                return new List<int>();
            }
        }

        private List<int> NotExist(string word)
        {
            var porter = new Porter();
            word = porter.Stemm(word.Replace("!",string.Empty));
            var list = new List<int>();
            for (int i = 0; i < docNum; i++)
            {
                list.Add(i);
            }
            if (indexedDict.ContainsKey(word))
            {//Операция Except возвращает последовательность, содержащую все элементы первой последовательности, которых нет во второй последовательности.
                list = list.Except(indexedDict[word]).ToList();
            }
            
            return list;
            
        }
    }
}
