namespace Web.Poc.Application.Contracts;

public interface IUrl
{
	Task AddUrls(IEnumerable<string> urls);
}
