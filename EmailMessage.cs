using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace Napier_Bank_Message_Filtering_System
{

    [DataContract]
    public class EmailMessage: MessageParent
    {
        [DataMember(Name = "subject", IsRequired = true)]
        public String subject;

        public List<String> QuarantineURLs(string messagebody, List<String> QuarantinedURLlist)
        {
            Regex regex = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            MatchCollection mactches = regex.Matches(messagebody);
            foreach (Match match2 in mactches)
            {
                QuarantinedURLlist.Add(match2.Value);
                messagebody = messagebody.Replace(match2.Value, "<URL Quarantined>");
            }

            this.messageText = messagebody;
            return QuarantinedURLlist;
        }

        public override string ToString() {
            return this.messageHeader + " " + this.sender + " " + this.subject + " " + this.messageText;
        }
    }
}
