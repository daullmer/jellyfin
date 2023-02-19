using MediaBrowser.Controller.Library;

namespace MediaBrowser.XbmcMetadata.Savers;

/// <summary>
/// The live tv nfo metadata saver.
/// </summary>
public class LiveTVNfoSaver : ILiveTvMetadataSaver
{
    /// <inheritdoc />
    public string Name => "Nfo";
}
