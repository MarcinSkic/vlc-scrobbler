using CsvHelper;
using Microsoft.EntityFrameworkCore;
using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Persistence;
using VlcTracker.Service.Persistence.Entities;

namespace VlcTracker.Service.Services;

public class ScrobblesService(TrackingContext dbContext) : IScrobblesService
{
    public async Task<IEnumerable<ScrobbleModel>> GetScrobbles()
    {
        return await dbContext
            .Scrobbles.OrderByDescending(scrobble => scrobble.Date)
            .Select(scrobble => ScrobbleModel.FromScrobble(scrobble))
            .ToListAsync();
    }

    public async Task<IEnumerable<ScrobblesGroupedResponse>> GetScrobblesByFilename()
    {
        return await dbContext
            .Scrobbles.GroupBy(scrobble => scrobble.FileName)
            .Select(grouping => new ScrobblesGroupedResponse
            {
                FileName = grouping.Key,
                Duration = grouping
                    .Where(scrobble => scrobble.VideoDuration != 0)
                    .Average(scrobble => scrobble.VideoDuration),
                TotalScrobbleDurationIncludingImported = grouping.Sum(scrobble =>
                    scrobble.ScrobbleDuration ?? scrobble.VideoDuration ?? 0
                ),
                TotalScrobbleDuration = grouping.Sum(scrobble => scrobble.ScrobbleDuration ?? 0),
                RepeatCount = grouping.Count(s => s.InRepeat),
                Count = grouping.Count(),
            })
            .OrderByDescending(scrobble => scrobble.Count)
            .ToListAsync();
    }

    public async Task<TotalScrobblingTimeResponse> GetTotalScrobblingTime()
    {
        return new TotalScrobblingTimeResponse
        {
            TotalScrobbleDurationIncludingImported =
                await dbContext.Scrobbles.SumAsync(scrobble => scrobble.ScrobbleDuration ?? scrobble.VideoDuration ?? 0),
            TotalScrobbleDuration = await dbContext.Scrobbles.SumAsync(scrobble => scrobble.ScrobbleDuration ?? 0)

        };
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
            .SelectMany(importedScrobbles => Enumerable.Range(0,importedScrobbles.Count).Select(_ => new Scrobble
            {
                Id = Guid.NewGuid(),
                FileName = importedScrobbles.FileName,
                Title = importedScrobbles.Title,
                InRepeat = false,
                VideoDuration = importedScrobbles.VideoDuration,
                ScrobbleDuration = null,
                Date = importedScrobbles.Date
            }))
            .ToList();
        
        dbContext.Scrobbles.AddRange(records);
        await dbContext.SaveChangesAsync(cancellationToken);
        return records.Count;
    }
}
