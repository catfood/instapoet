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
            words = getWords();
            rnd = new Random();
        }

        private List<DatamuseWord> getWords()
        {
            var client = new WebClient();
            string theme = Request.Query["theme"].FirstOrDefault() ?? "ringing+in+the+ears";
            var json = client.DownloadString($"https://api.datamuse.com/words?ml={theme}&md=s");
            var words = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DatamuseWord>>(json);
            return words;
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
            return result.Substring(1); // drop the leading space
        }
    }
}
