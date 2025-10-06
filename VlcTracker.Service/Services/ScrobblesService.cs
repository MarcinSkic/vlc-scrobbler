using System.Text.RegularExpressions;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Models.Api.Core;
using VlcTracker.Service.Persistence;
using VlcTracker.Service.Persistence.Entities;

namespace VlcTracker.Service.Services;

public class ScrobblesService(TrackingContext dbContext) : IScrobblesService
{
    public async Task<IEnumerable<ScrobbleModel>> GetScrobbles(Filters filters)
    {
        return await GetFilteredQuery(filters).OrderByDescending(scrobble => scrobble.Date)
            .Select(scrobble => ScrobbleModel.FromScrobble(scrobble))
            .ToListAsync();
    }

    public async Task<IEnumerable<ScrobblesGroupedResponse>> GetScrobblesByFilename(Filters filters)
    {
        return await GetFilteredQuery(filters).GroupBy(scrobble => scrobble.FileName)
            .Select(grouping => new ScrobblesGroupedResponse
            {
                FileName = grouping.Key,
                Duration = grouping
                    .Where(scrobble => scrobble.VideoDuration != 0)
                    .Average(scrobble => scrobble.VideoDuration),
                TotalScrobbleDuration = grouping.Sum(scrobble => scrobble.ScrobbleDuration ?? scrobble.VideoDuration ?? 0),
                RepeatCount = grouping.Count(s => s.InRepeat),
                Count = grouping.Count(),
            })
            .OrderByDescending(scrobble => scrobble.Count)
            .ToListAsync();
    }

    public async Task<TotalScrobblingTimeResponse> GetTotalScrobblingTime(Filters filters)
    {
        return new TotalScrobblingTimeResponse
        {
            TotalVideoDuration = await GetFilteredQuery(filters).SumAsync(scrobble =>
                scrobble.VideoDuration ?? 0
            ),
            TotalScrobbleDuration = await GetFilteredQuery(filters).SumAsync(scrobble =>
                scrobble.ScrobbleDuration ?? scrobble.VideoDuration ?? 0
            ),
        };
    }
    
    private IQueryable<Scrobble> GetFilteredQuery(Filters filters)
    {
        IQueryable<Scrobble> query = dbContext.Scrobbles;

        if (filters.FileNameRegex is not null)
        {
            query = query.Where(scrobble => Regex.IsMatch(scrobble.FileName,filters.FileNameRegex));
        }

        if (filters.ExcludeImported ?? false)
        {
            query = query.Where(scrobble => scrobble.Date != null && scrobble.ScrobbleDuration != 0 && scrobble.ScrobbleDuration != null);
        }

        if (filters.FromDate is not null)
        {
            query = query.Where(scrobble => scrobble.Date >= filters.FromDate);
        }

        if (filters.ToDate is not null)
        {
            query = query.Where(scrobble => scrobble.Date <= filters.ToDate);
        }

        return query;
    }

    public async Task AddScrobbleAsync(Scrobble scrobble, CancellationToken cancellationToken)
    {
        await dbContext.Scrobbles.AddAsync(scrobble, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateScrobbleAsync(Scrobble scrobble, CancellationToken cancellationToken)
    {
        dbContext.Scrobbles.Update(scrobble);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ImportScrobblesFromCsv(
        CsvReader csvStream,
        CancellationToken cancellationToken
    )
    {
        var records = csvStream
            .GetRecords<ImportedScrobblesDto>()
            .SelectMany(importedScrobbles =>
                Enumerable
                    .Range(0, importedScrobbles.Count)
                    .Select(_ => new Scrobble
                    {
                        Id = Guid.NewGuid(),
                        FileName = importedScrobbles.FileName,
                        Title = importedScrobbles.Title,
                        InRepeat = false,
                        VideoDuration = importedScrobbles.VideoDuration,
                        ScrobbleDuration = null,
                        Date = importedScrobbles.Date,
                    })
            )
            .ToList();

        dbContext.Scrobbles.AddRange(records);
        await dbContext.SaveChangesAsync(cancellationToken);
        return records.Count;
    }
}
