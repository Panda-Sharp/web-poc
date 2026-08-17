using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Poc.Application.Contracts;

public interface IUrl
{
    Task OnAddUrls(IEnumerable<string> urls);
}
