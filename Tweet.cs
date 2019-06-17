using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Napier_Bank_Message_Filtering_System
{
    [DataContract]
    public class Tweet: MessageParent
    {
        public List<String> GetHashtags(string messagebody, List<String> HashtagList)
        {
            var regex = new Regex(@"(?<=#)\w+");
            var hashtagmatches = regex.Matches(messagebody);
            foreach (Match hashtagmatch in hashtagmatches)
            {
                string hashtagtemp = "#" + hashtagmatch.Value;
                HashtagList.Add(hashtagtemp);
            }
            return HashtagList;
        }

        public List<String> GettwitterIDs(string messagebody, Regex twitteridRegex, List<String> TwitterIDList)
        {
            Match m = twitteridRegex.Match(messagebody);
            var matches = twitteridRegex.Matches(messagebody);
            string messagebodylesshashtagsids = messagebody;
            foreach (Match match in matches)
            {
                string twitteridtemp = "@" + match.Value;
                TwitterIDList.Add(twitteridtemp);
            }

            return TwitterIDList;
        }

        public override string ToString() {
            return this.messageHeader + " " +  this.sender + " " + this.messageText;
        }

    }
}
