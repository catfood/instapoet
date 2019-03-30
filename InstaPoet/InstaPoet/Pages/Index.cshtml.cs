using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstaPoet.Pages
{
    public class DatamuseWord
    {
        public string word { get; set; }
        public int score { get; set; }
        public int numSyllables { get; set; }
        public string[] tags { get; set; }
    }

    public class IndexModel : PageModel
    {
        private List<DatamuseWord> words;
        private Random rnd;

        public void OnGet()
        {
            const string defaultTheme = "ringing+in+the+ears";
            string theme = Request.Query["theme"].FirstOrDefault();
            words = getWords(theme); // urlencode it tho
/*
            if (words.Count <= 0)
            {
                words = getWords();
            }
*/
            rnd = new Random();
        }

        private List<DatamuseWord> getWords(string theme = null)
        {
            var client = new WebClient();
            string url = (theme == null) ? "https://api.datamuse.com/words?md=s" : $"https://api.datamuse.com/words?ml={theme}&md=s";
            var json = client.DownloadString(url);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DatamuseWord>>(json);
        }

        public string randomPunctuation(bool end = false)
        {
            const string regularPuncs = ".,!?;";
            const string endPuncs = ".!?";
            string puncs = end ? endPuncs : regularPuncs;
            return puncs.Substring(rnd.Next() % puncs.Length, 1);
        }

        public string syllables (int totalSyllables)
        {
            int remainingSyllables = totalSyllables;
            string result = "";
            while (remainingSyllables > 0)
            {
                var candidates = words.Where(a => a.numSyllables <= remainingSyllables).ToList();
                int r = rnd.Next() % candidates.Count();
                var nextWord = candidates[r];
                remainingSyllables -= Math.Max(1, nextWord.numSyllables); 
                result += " ";
                result += nextWord.word;
            }
            return result.Substring(1,1).ToUpper() + result.Substring(2); // initial cap; drop the leading space
        }
    }
}
