using OnlineStore.Data.Utilities.Interfaces;

namespace OnlineStore.Data.Seeding.Interfaces
{
    public interface IXmlSeeder
    {
		public string RootName { get; }

		public IXmlHelper XmlHelper { get; }
	}
}
