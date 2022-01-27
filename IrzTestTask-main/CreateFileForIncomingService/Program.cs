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

        static void Main()
        {
            var path = ConfigurationManager.AppSettings["PathForFilesIncomingService"];
            if (!Directory.Exists(path))
            {
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

            string[] xmlPaths = Directory.GetFiles(@"..//..//..//..//IrzTestTask-main\IrzTestFile\", "*.xml", SearchOption.AllDirectories);

            foreach (var item in xmlPaths)
            {
                string fileName = Path.GetFileName(item);
                Console.Write(fileName + " ");
                var xml = XDocument.Load(item);
                PostXMLData("http://localhost:63524/api/Incoming", xml.ToString());
            }
        }

        private static void PostXMLData(string destinationUrl, string xml)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.UTF32.GetBytes(xml);
            Stream requestStream = GetRequestStream(request, bytes);
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            
            try 
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("200 OK");
                Console.ResetColor();
            }
            catch (WebException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("400 ERROR");
                Console.ResetColor(); 
            }
        }

        private static Stream GetRequestStream(HttpWebRequest request, byte[] bytes)
        {
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            return requestStream;
        }
    }
}
