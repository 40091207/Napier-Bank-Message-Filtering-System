using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;



namespace Napier_Bank_Message_Filtering_System
{ //xmlns:local="clr-namespace:NapierBankMessagingSystem"
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        List<string> quarantinelist = new List<string>();
        List<string> hashtaglist = new List<string>();
        List<string> twitterIDlist = new List<string>();
        List<Tuple<string, string>> SIRlist = new List<Tuple<string, string>>();


        private void Add_Message_Click(object sender, RoutedEventArgs e)
        {
            

            if (string.IsNullOrWhiteSpace(MessagesBox.Text))
            {
                System.Windows.MessageBox.Show("");
                System.Windows.MessageBox.Show("Message String Empty", "Error Check Message String");
                return;
            }

            String message = MessagesBox.Text;
            Main(message);

        }

        private void SMStest_Click(object sender, RoutedEventArgs e)
        {
            MessagesBox.Text = "S123456789100154175430104This is the sms example message";
        }

        private void Emailtest_Click(object sender, RoutedEventArgs e)
        {
            MessagesBox.Text = "E123456789john.smith@example.org12345678911234567892123456789";
        }

        private void EmailSIRtest_Click(object sender, RoutedEventArgs e)
        {
            MessagesBox.Text = "E123456789john.smith@example.orgSIR dd/mm/yy99-99-99Thefthello";
        }

        private void Tweettest_Click(object sender, RoutedEventArgs e)
        {
            MessagesBox.Text = "T123456789@johnSmith hello im cool #cool @yousmell @whatsupdude #noway hahaha @wowcool";
        }

        private void Startbtn_Click(object sender, RoutedEventArgs e)
        {
            filereader();
        }

        public void filereader()
        {
            string path = @"C:\Users\Desktop\Documents\input.txt";
            // Read the file and display it line by line.
            string[] lines = System.IO.File.ReadAllLines(path);

            // Display the file contents by using a foreach loop.
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                Main(line);
            }
        }

        public void Main(String ASCIIstring)
        {
            String message = ASCIIstring;


            if (message[0] == 'S' | message[0] == 'E' | message[0] == 'T')
            {
                if (message[0] == 'S')
                {
                    //Takes the message header (first 10 charesters of message)
                    string messageHeader = message.Substring(0, Math.Min(message.Length, 10));
                    //finds the sms senders number
                    string smssender = message.Substring(10, message.Length - 10);
                    smssender = smssender.Substring(0, Math.Min(message.Length, 15));
                    smssender = Regex.Replace(smssender, "[A-Za-z]", "");
                    //works out the message body
                    string messageBody = message.Substring(10);
                    messageBody = messageBody.Replace(smssender, string.Empty);
                    //create the SMS message Object relating to the input
                    SMSMessage smsmessage = new SMSMessage()
                    {
                        messageHeader = messageHeader,
                        sender = smssender,
                        messageText = messageBody
                    };
                    //expands textspeak
                    smsmessage.TextspeakAbreviation(messageBody);
                    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(SMSMessage));
                    MemoryStream ms = new MemoryStream();
                    js.WriteObject(ms, smsmessage);
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    string txt = sr.ReadToEnd();
                    string JSON_directory = @"C:\Users\Desktop\Documents\" + messageHeader + ".json";
                    System.IO.File.WriteAllText(JSON_directory, txt);
                    sr.Close();
                    ms.Close();
                    listBox.Items.Add(smsmessage.ToString());
                }
                if (message[0] == 'E')
                {
                    //Takes the message header (first 10 charesters of message)
                    string messageHeader = message.Substring(0, Math.Min(message.Length, 10));
                    //finds the email sender
                    string emailsender = message.Substring(10, message.Length - 10);
                    //emailsender = emailsender.Substring(0, Math.Min(message.Length, 50));
                    Regex reg = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.+(com|org|co.uk|ac.uk)", RegexOptions.IgnoreCase);
                    Match match;
                    for (match = reg.Match(emailsender); match.Success; match = match.NextMatch())
                    {
                        emailsender = match.Value;
                    }

                    string messageBody = message.Substring(10);
                    string subject = messageBody.Replace(emailsender, string.Empty);

                    if (subject.Contains("SIR"))
                    {
                        //remove subject and sender
                        subject = subject.Substring(0, Math.Min(message.Length, 12));
                        messageBody = messageBody.Replace(subject, string.Empty);
                        messageBody = messageBody.Replace(emailsender, string.Empty);
                        //get the sortcode
                        string sortcode = messageBody.Substring(0, Math.Min(message.Length, 8));
                        messageBody = messageBody.Replace(sortcode, string.Empty);
                        //find the nature of incident
                        string Natureofincident = null;
                        string[] Natureofincidents = { "Theft", "Staff Attack", "ATM Theft", "Raid", "Customer Attack", "Staff Abuse", "Bomb Threat", "Terrorism", "Suspicious Incident", "Intelligence", "Cash Loss" };
                        foreach (string NoI in Natureofincidents)
                        {
                            if (messageBody.Contains(NoI))
                            {
                                Natureofincident = NoI;
                            }
                            messageBody = messageBody.Replace(NoI, string.Empty);
                        }
                        SIRlist.Add(new Tuple<string, string>(sortcode, Natureofincident));
                        
                        SignificantIncidentReportEmail emailmessage = new SignificantIncidentReportEmail()
                        {
                            sortcode = sortcode,
                            natureofincident = Natureofincident,
                            messageHeader = messageHeader,
                            sender = emailsender,
                            messageText = messageBody,
                            subject = subject
                        };

                        //Quarantines URLs and replaces text
                        quarantinelist = emailmessage.QuarantineURLs(emailmessage.messageText, quarantinelist);

                        DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(SignificantIncidentReportEmail));
                        MemoryStream ms = new MemoryStream();
                        js.WriteObject(ms, emailmessage);
                        ms.Position = 0;
                        StreamReader sr = new StreamReader(ms);
                        string txt = sr.ReadToEnd();
                        string JSON_directory = @"C:\Users\Desktop\Documents\" + messageHeader + ".json";
                        System.IO.File.WriteAllText(JSON_directory, txt);
                        sr.Close();
                        ms.Close();
                        listBox.Items.Add(emailmessage.ToString());

                    }
                    else
                    {
                        //finds the email subject
                        subject = subject.Substring(0, Math.Min(message.Length, 20));
                        //emailbody removeing urls
                        messageBody = messageBody.Replace(subject, string.Empty);
                        messageBody = messageBody.Replace(emailsender, string.Empty);

                        EmailMessage emailmessage = new EmailMessage()
                        {
                            messageHeader = messageHeader,
                            sender = emailsender,
                            messageText = messageBody
                        };

                        //Quarantines URLs and replaces text
                        quarantinelist = emailmessage.QuarantineURLs(emailmessage.messageText, quarantinelist);

                        DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(EmailMessage));
                        MemoryStream ms = new MemoryStream();
                        js.WriteObject(ms, emailmessage);
                        ms.Position = 0;
                        StreamReader sr = new StreamReader(ms);
                        string txt = sr.ReadToEnd();
                        string JSON_directory = @"C:\Users\Desktop\Documents\" + messageHeader + ".json";
                        System.IO.File.WriteAllText(JSON_directory, txt);
                        sr.Close();
                        ms.Close();
                        listBox.Items.Add(emailmessage.ToString());
                    }

                    //create object  

                }
                if (message[0] == 'T')
                {
                    //Takes the message header (first 10 charesters of message)
                    string messageHeader = message.Substring(0, Math.Min(message.Length, 10));
                    string messageBody = message.Replace(messageHeader, string.Empty);

                    //finds the twitter sender and also each mention using regex, writed hashtags to list
                    var Twitteridregex = new Regex(@"(?<=@)\w+");
                    Match m = Twitteridregex.Match(messageBody);
                    string tweetsender = "@" + m.Value;
                    messageBody = messageBody.Replace(tweetsender, string.Empty);

                    //creating the TwitterMessage object
                    Tweet tweetmessage = new Tweet()
                    {
                        messageHeader = messageHeader,
                        sender = tweetsender,
                        messageText = messageBody
                    };

                    //getting and adding the hashtags and IDs to the appropriate list then expand textspeak abreviations
                    hashtaglist = tweetmessage.GetHashtags(tweetmessage.messageText, hashtaglist);
                    twitterIDlist = tweetmessage.GettwitterIDs(tweetmessage.messageText, Twitteridregex, twitterIDlist);
                    tweetmessage.TextspeakAbreviation(messageBody);

                    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(Tweet));
                    MemoryStream ms = new MemoryStream();
                    js.WriteObject(ms, tweetmessage);
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    string txt = sr.ReadToEnd();
                    string JSON_directory = @"C:\Users\Desktop\Documents\" + messageHeader + ".json";
                    System.IO.File.WriteAllText(JSON_directory, txt);
                    sr.Close();
                    ms.Close();
                    listBox.Items.Add(tweetmessage.ToString());
                }
            }
            else { System.Windows.MessageBox.Show("MessageHeader; MessageID must start with either S, E or T"); return; }
        }

        private void Mentions_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            foreach (string i in twitterIDlist)
            { listBox.Items.Add(i); }
        }

        private void Hashtags_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();

            List<string> hashtagtrendinglist = new List<string>();
            var q = from x in hashtaglist
                    group x by x into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };
            foreach (var x in q)
            {
                hashtagtrendinglist.Add(x.Value + " Count: " + x.Count);
            }
            foreach(string s in hashtagtrendinglist)
            {
                listBox.Items.Add(s);
            }
        }

        private void SIR_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            foreach (Tuple<string, string> tuple in SIRlist){ 
                listBox.Items.Add(tuple);}
        }
    }
}
