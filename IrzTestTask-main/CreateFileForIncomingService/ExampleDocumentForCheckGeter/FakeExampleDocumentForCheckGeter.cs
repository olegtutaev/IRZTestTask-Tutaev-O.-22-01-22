using System;
using System.Collections.Generic;

namespace CreateFileForIncomingService
{
    public class FakeExampleDocumentForCheckGeter : IExampleDocumentForCheckGeter
    {
        public IEnumerable<Package> Get(Document document, int count)
        {
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                var id = random.Next(100000000, 999999999);
                var text = @"<?xml version=""1.0""?><root>Фальшивая XML для тестирования интегарционного теста</root>";
                yield return new Package(id, text);
            }
        }
    }
}