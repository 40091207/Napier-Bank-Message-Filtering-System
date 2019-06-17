using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Napier_Bank_Message_Filtering_System
{
    [DataContract]
    public class SMSMessage: MessageParent
    {
        public override string ToString()
        {
            return this.messageHeader + " " + this.sender + " " + this.messageText;
        }
    }
}
