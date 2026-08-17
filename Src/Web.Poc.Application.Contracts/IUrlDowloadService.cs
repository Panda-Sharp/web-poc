using System.Threading.Tasks;

namespace Web.Poc.Application.Contracts
{
    public interface IUrlDowloadService
    {
        Task DownloaFile(string url);
    }
}