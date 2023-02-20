using System;
using System.IO;
using System.Threading.Tasks;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;

namespace MediaBrowser.XbmcMetadata.Savers;

/// <summary>
/// The live tv nfo metadata saver.
/// </summary>
public class LiveTvNfoSaver : ILiveTvMetadataSaver
{
    private readonly IFileSystem _fileSystem;
    private readonly IServerConfigurationManager _configurationManager;
    private readonly ILibraryManager _libraryManager;
    private readonly IUserManager _userManager;
    private readonly IUserDataManager _userDataManager;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTvNfoSaver"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="configurationManager">the server configuration manager.</param>
    /// <param name="libraryManager">The library manager.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="userDataManager">The user data manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public LiveTvNfoSaver(
        IFileSystem fileSystem,
        IServerConfigurationManager configurationManager,
        ILibraryManager libraryManager,
        IUserManager userManager,
        IUserDataManager userDataManager,
        ILoggerFactory loggerFactory)
    {
        _fileSystem = fileSystem;
        _configurationManager = configurationManager;
        _libraryManager = libraryManager;
        _userManager = userManager;
        _userDataManager = userDataManager;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public string Name => "Nfo";

    /// <inheritdoc />
    public async Task SaveSeriesAsync(TimerInfo timer, string seriesPath)
    {
        var nfoPath = Path.Combine(seriesPath, "tvshow.nfo");

        if (File.Exists(nfoPath))
        {
            return;
        }

        Series series = new Series()
        {
            ProviderIds = timer.SeriesProviderIds,
            Name = timer.Name,
            OfficialRating = timer.OfficialRating,
            Genres = timer.Genres
        };

        using var memoryStream = new MemoryStream();
        var saver = new SeriesNfoSaver(_fileSystem, _configurationManager, _libraryManager, _userManager, _userDataManager, _loggerFactory.CreateLogger<SeriesNfoSaver>());

        saver.Save(series, memoryStream, nfoPath);

        memoryStream.Position = 0;

        await saver.SaveToFileAsync(memoryStream, nfoPath).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SaveVideoAsync(TimerInfo timer, string recordingPath, BaseItem item, bool lockData)
    {
        var nfoPath = Path.ChangeExtension(recordingPath, ".nfo");
        if (File.Exists(nfoPath))
        {
            return;
        }

        using var memoryStream = new MemoryStream();
        BaseNfoSaver saver;

        if (timer.IsProgramSeries)
        {
            saver = new EpisodeNfoSaver(_fileSystem, _configurationManager, _libraryManager, _userManager, _userDataManager, _loggerFactory.CreateLogger<EpisodeNfoSaver>());
            item.Name = timer.EpisodeTitle;
            item.PremiereDate ??= !timer.IsRepeat ? DateTime.UtcNow : null;
        }
        else
        {
            saver = new MovieNfoSaver(_fileSystem, _configurationManager, _libraryManager, _userManager, _userDataManager, _loggerFactory.CreateLogger<MovieNfoSaver>());
        }

        saver.Save(item, memoryStream, nfoPath);

        memoryStream.Position = 0;

        await saver.SaveToFileAsync(memoryStream, nfoPath).ConfigureAwait(false);
    }
}
