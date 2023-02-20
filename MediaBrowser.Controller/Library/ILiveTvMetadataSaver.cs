using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.LiveTv;

namespace MediaBrowser.Controller.Library;

/// <summary>
/// Interface ILiveTVMetadataSaver.
/// </summary>
public interface ILiveTvMetadataSaver
{
    /// <summary>
    /// Gets the saver name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Saves series metadata.
    /// </summary>
    /// <param name="timer">Information about the recording timer.</param>
    /// <param name="seriesPath">Path the series gets saved to.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task SaveSeriesAsync(TimerInfo timer, string seriesPath);

    /// <summary>
    /// Save video metadata.
    /// </summary>
    /// <param name="timer">Information about the recording timer.</param>
    /// <param name="recordingPath">The path the recording gets saved to.</param>
    /// <param name="item">The program information.</param>
    /// <param name="lockData">Whether to lock the data.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task SaveVideoAsync(TimerInfo timer, string recordingPath, BaseItem item, bool lockData);
}
