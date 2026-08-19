using System;
using System.Threading.Tasks;

namespace Web.Poc.Application.Contracts
{
    public interface IUrlDowloadService
    {
        Task DownloaFile(Uri uri);
    }
}