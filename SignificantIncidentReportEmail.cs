using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Napier_Bank_Message_Filtering_System
{
    [DataContract]
    public class SignificantIncidentReportEmail : EmailMessage
    {
        [DataMember(Name = "Sort Code", IsRequired = true)]
        public String sortcode;
        [DataMember(Name = "Nature of Incident", IsRequired = true)]
        public String natureofincident;

        public override string ToString()
        {
            return this.messageHeader + " " + this.sender + " " + this.subject + this.sortcode + " " + this.natureofincident + " " + this.messageText;
        }
    }
}
