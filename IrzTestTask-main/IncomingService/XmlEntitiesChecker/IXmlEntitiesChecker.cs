using System.Xml;

namespace IncomingService.XmlEntitiesChecker
{
    public interface IXmlEntitiesChecker
    {
        bool Check(XmlDocument xmlDocument);
    }
}