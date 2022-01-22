using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using IncomingService.XmlEntitiesChecker;

namespace IncomingService.Controllers
{
    public class IncomingController : ApiController
    {
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            try
            {
                var contentStream = request.Content.ReadAsStreamAsync().Result;
                
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(contentStream);

                IXmlEntitiesChecker XmlEntitiesChecker = new FakeXmlEntitiesChecker();
                var result =
                    XmlEntitiesChecker.Check(xmlDocument) ?
                    Request.CreateResponse(HttpStatusCode.OK, "XML успешно обработана") :
                    Request.CreateResponse(HttpStatusCode.BadRequest, "При обработке XML произошла ошибка");

                return result;
            }
            catch (Exception exception)
            {
                var textMessage = string.Empty;
                do
                {
                    textMessage += exception.Message + '\n';
                    exception = exception.InnerException;
                } while (exception != null);

                return Request.CreateResponse(HttpStatusCode.BadRequest, "На сервере произошла ошибка.\n" + exception.Message);
            }
        }
    }
}