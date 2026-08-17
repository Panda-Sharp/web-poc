using Bogus;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Web.Poc.WorkerService.Producer.Helpers;

public static class UrlsHelper
{
    private static readonly int PageSize = 100;

    public static IEnumerable<string> GetFromFaker()
    {
        var faker = new Faker();
        var urls = Enumerable.Range(1, PageSize)
          .Select(_ => faker.Internet.UrlWithPath());

        return urls
            .ToArray();
    }

    public static IEnumerable<string> GetFromCsv(int page)
    {
        using var reader = new StreamReader("Assets/UrlLists/aams.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var urls = csv.GetRecords<UrlDto>()
            .Skip((page - 1) * PageSize)
            .Take(PageSize);

        return urls
            .Select(x => x.Url)
            .ToArray();
    }
}
