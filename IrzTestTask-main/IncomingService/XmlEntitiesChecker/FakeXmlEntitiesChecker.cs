using System;
using System.Xml;

namespace IncomingService.XmlEntitiesChecker
{
    public class FakeXmlEntitiesChecker : IXmlEntitiesChecker
    {
        private Random random = new Random();

        public bool Check(XmlDocument xmlDocument) =>
            random.NextDouble() > 0.25;
    }
}