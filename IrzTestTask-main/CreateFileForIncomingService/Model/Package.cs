namespace CreateFileForIncomingService
{
    public class Package
    {
        public int Id { get; }
        public string Text { get; }

        public Package(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}