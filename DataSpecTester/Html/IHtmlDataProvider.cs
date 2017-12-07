namespace DataSpecTester.Html
{
    using ProtocolEx;
    using System;

    public interface IHtmlDataProvider : IGenericProtocolDataProvider
    {
        string Html { get; }

        string[] Protocols { get; }
    }

	public interface IGenericProtocolDataProvider {

	}
}

