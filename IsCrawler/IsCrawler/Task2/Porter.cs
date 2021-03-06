using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IsCrawler
{
    public class Porter
    {//https://medium.com/@eigenein/%D1%81%D1%82%D0%B5%D0%BC%D0%BC%D0%B5%D1%80-%D0%BF%D0%BE%D1%80%D1%82%D0%B5%D1%80%D0%B0-%D0%B4%D0%BB%D1%8F-%D1%80%D1%83%D1%81%D1%81%D0%BA%D0%BE%D0%B3%D0%BE-%D1%8F%D0%B7%D1%8B%D0%BA%D0%B0-d41c38b2d340#:~:text=%D0%A1%D1%82%D0%B5%D0%BC%D0%BC%D0%B5%D1%80%20%D0%9F%D0%BE%D1%80%D1%82%D0%B5%D1%80%D0%B0%20%E2%80%94%20%D0%B0%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC%20%D1%81%D1%82%D0%B5%D0%BC%D0%BC%D0%B8%D0%BD%D0%B3%D0%B0%20(%D0%BD%D0%B0%D1%85%D0%BE%D0%B6%D0%B4%D0%B5%D0%BD%D0%B8%D1%8F,%D0%BF%D1%80%D0%B0%D0%B2%D0%B8%D0%BB%20%D0%BE%D1%82%D1%81%D0%B5%D1%87%D0%B5%D0%BD%D0%B8%D1%8F%20%D0%BE%D0%BA%D0%BE%D0%BD%D1%87%D0%B0%D0%BD%D0%B8%D0%B9%20%D0%B8%20%D1%81%D1%83%D1%84%D1%84%D0%B8%D0%BA%D1%81%D0%BE%D0%B2.&text=%D0%9E%D0%BD%D0%B0%20%D0%BC%D0%BE%D0%B6%D0%B5%D1%82%20%D0%B1%D1%8B%D1%82%D1%8C%20%D0%BF%D1%83%D1%81%D1%82%D0%BE%D0%B9%2C%20%D0%B5%D1%81%D0%BB%D0%B8%20%D0%B3%D0%BB%D0%B0%D1%81%D0%BD%D1%8B%D0%B5%20%D0%B2%20%D1%81%D0%BB%D0%BE%D0%B2%D0%B5%20%D0%BE%D1%82%D1%81%D1%83%D1%82%D1%81%D1%82%D0%B2%D1%83%D1%8E%D1%82
        
        private const string VOWEL = "аеиоуыэюя";

        private const string PERFECTIVEGROUND = "((ив|ивши|ившись|ыв|ывши|ывшись)|((?<=[ая])(в|вши|вшись)))$";

        private const string REFLEXIVE = "(с[яь])$";

        private const string ADJECTIVE = "(ее|ие|ые|ое|ими|ыми|ей|ий|ый|ой|ем|им|ым|ом|его|ого|еых|ую|юю|ая|яя|ою|ею)$";

        private const string PARTICIPLE = "((ивш|ывш|ующ)|((?<=[ая])(ем|нн|вш|ющ|щ)))$";

        private const string VERB = "((ила|ыла|ена|ейте|уйте|ите|или|ыли|ей|уй|ил|ыл|им|ым|ены|ить|ыть|ишь|ую|ю)|((?<=[ая])(ла|на|ете|йте|ли|й|л|ем|н|ло|но|ет|ют|ны|ть|ешь|нно)))$";

        private const string NOUN = "(а|ев|ов|ие|ье|е|иями|ями|ами|еи|ии|и|ией|ей|ой|ий|й|и|ы|ь|ию|ью|ю|ия|ья|я)$";

        private const string RVRE = "^(.*?[аеиоуыэюя])(.*)$";

        private const string DERIVATIONAL = "[^аеиоуыэюя][аеиоуыэюя]+[^аеиоуыэюя]+[аеиоуыэюя].*(?<=о)сть?$";

        private const string SUPERLATIVE = "(ейше|ейш)?";


        public string Stemm(string word)
        {
            word = word.ToLower();
            word = word.Replace("ё", "е");

            if (IsMatch(word, RVRE))
            {

                if (!Replace(ref word, PERFECTIVEGROUND, ""))
                {
                    Replace(ref word, REFLEXIVE, "");
                    if (Replace(ref word, ADJECTIVE, ""))
                    {
                        Replace(ref word, PARTICIPLE, "");
                    }
                    else
                    {
                        if (!Replace(ref word, VERB, ""))
                        {
                            Replace(ref word, NOUN, "");
                        }

                    }

                }


                Replace(ref word, "и$", "");

                if (IsMatch(word, DERIVATIONAL))
                {
                    Replace(ref word, "ость?$", "");
                }


                if (!Replace(ref word, "ь$", ""))
                {
                    Replace(ref word, SUPERLATIVE, "");
                    Replace(ref word, "нн$", "н");
                }

            }

            if (VOWEL.Contains(word[word.Length - 1])) 
            {
                word = word.Substring(0, word.Length - 1);
            }

            return word;
        }

        private bool IsMatch(string word, string matchingPattern)
        {
            return new Regex(matchingPattern).IsMatch(word);
        }

        private bool Replace(ref string replace, string cleaningPattern, string by)
        {
            string original = replace;
            replace = new Regex(cleaningPattern,
                        RegexOptions.ExplicitCapture |
                        RegexOptions.Singleline
                        ).Replace(replace, by);
            return original != replace;
        }

    }

}
