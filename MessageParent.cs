using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Diagnostics;

namespace Napier_Bank_Message_Filtering_System
{
    [DataContract]
    public class MessageParent
    {
        [DataMember(Name = "Header", IsRequired = true)]
        public String messageHeader;
        [DataMember(Name = "Sender", IsRequired = true)]
        public String sender;
        [DataMember(Name = "Message", IsRequired = true)]
        public String messageText;

        public void TextspeakAbreviation(string Body)
        {
            var reader = new StreamReader(File.OpenRead(@"C:\textwords.csv"));
            List<string> listAbbreviations = new List<string>();
            List<string> listAbbreviationsExpanded = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                listAbbreviations.Add(values[0]);
                listAbbreviationsExpanded.Add(values[1]);
            }

            foreach (string element in listAbbreviations)
            {
                bool b = Body.Contains(element);
                if (b == true)
                {
                    int i = listAbbreviations.IndexOf(element);
                    string replacementstring = "<" + listAbbreviationsExpanded[i] + ">";
                    Body = Body.Replace(element, replacementstring);
                }
            }
            this.messageText = Body;
        }
    }
}
