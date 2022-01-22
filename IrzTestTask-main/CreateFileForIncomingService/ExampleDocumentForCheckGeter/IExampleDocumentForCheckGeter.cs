using System.Collections.Generic;

namespace CreateFileForIncomingService
{
    public interface IExampleDocumentForCheckGeter
    {
        IEnumerable<Package> Get(Document document, int count);
    }
}