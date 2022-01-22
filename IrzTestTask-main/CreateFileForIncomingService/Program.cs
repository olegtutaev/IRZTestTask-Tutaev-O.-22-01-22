using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace CreateFileForIncomingService
{
    class Program
    {
        private static Dictionary<SubjectArea, List<Document>> subjectAreaDocumentComaration =
            new Dictionary<SubjectArea, List<Document>>()
            {
                [SubjectArea.D01] = new List<Document>()
                {
                    Document.ПриобретениеТоваровУслуг,
                    Document.РеализацияТоваровУслуг,
                    Document.ПриобретениеУслугПрочихАктивов,
                },

                [SubjectArea.Tmc] = new List<Document>()
                {
                    Document.СостоянияСомнительностиСерий,
                    Document.ПеремещениеТоваровМеждуОрганизациямиИРЗ,
                    Document.СерииНоменклатуры,
                },

                [SubjectArea.Bank] = new List<Document>()
                {
                    Document.СБДСРасчетыСКонтрагентами,
                    Document.СчетФактураПолученный,
                    Document.СчетФактураВыданныйАванс,
                },
            };

        public static string postXMLData(string destinationUrl, string xml)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(xml);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse(); // В случае 400 в консоли выдаст System.Net.WebException

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                Console.WriteLine("200 OK");
                return responseStr;
            }
            else
                return null;
        }

        static void Main()
        {
            List<string> list = new List<string>();

            foreach (var file in Directory.GetFiles(@"..//..//..//..//IrzTestTask-main\IrzTestFile\", "*.xml", SearchOption.AllDirectories))
                list.Add(file);

            for (int i=0; i < list.Count; i++)
            {
                Console.Write(list[i] + " returned: ");
                var xml = XDocument.Load(list[i]);
                postXMLData("http://localhost:63524//api/Incoming", xml.ToString());
            }

            var path = ConfigurationManager.AppSettings["PathForFilesIncomingService"];
            if(Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);

            IExampleDocumentForCheckGeter exampleDocumentForCheckGeter = new FakeExampleDocumentForCheckGeter();
            
            var subjectAreaSet = new HashSet<SubjectArea> { SubjectArea.D01, SubjectArea.Tmc, SubjectArea.Bank };
            foreach (var subjectArea in subjectAreaSet)
            {
                var subjectAreaPath = Path.Combine(path, subjectArea.ToString());
                Directory.CreateDirectory(subjectAreaPath);

                var documents = subjectAreaDocumentComaration[subjectArea];

                foreach (var document in documents)
                {
                    var documentPath = Path.Combine(subjectAreaPath, document.ToString());
                    Directory.CreateDirectory(documentPath);

                    var packages = exampleDocumentForCheckGeter.Get(document, 10);
                    foreach (var package in packages)
                    {
                        var filePath = Path.Combine(documentPath, package.Id + ".xml");
                        File.WriteAllText(filePath, package.Text);
                    }
                }
            }
        }
    }
}
