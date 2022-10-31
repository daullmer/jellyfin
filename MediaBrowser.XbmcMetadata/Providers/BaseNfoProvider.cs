#pragma warning disable CS1591

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.XbmcMetadata.Savers;

namespace MediaBrowser.XbmcMetadata.Providers
{
    public abstract class BaseNfoProvider<T> : ILocalMetadataProvider<T>, IHasItemChangeMonitor
        where T : BaseItem, new()
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDirectoryService _directoryService;

        protected BaseNfoProvider(IFileSystem fileSystem, IDirectoryService directoryService)
        {
            _fileSystem = fileSystem;
            _directoryService = directoryService;
        }

        /// <inheritdoc />
        public string Name => BaseNfoSaver.SaverName;

        /// <inheritdoc />
        public Task<MetadataResult<T>> GetMetadata(
            ItemInfo info,
            CancellationToken cancellationToken)
        {
            var result = new MetadataResult<T>();

            var file = GetXmlFile(info, _directoryService);

            if (file is null)
            {
                return Task.FromResult(result);
            }

            var path = file.FullName;

            try
            {
                result.Item = new T();

                Fetch(result, path, cancellationToken);
                result.HasMetadata = true;
            }
            catch (FileNotFoundException)
            {
                result.HasMetadata = false;
            }
            catch (IOException)
            {
                result.HasMetadata = false;
            }

            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public bool HasChanged(BaseItem item, IDirectoryService directoryService)
        {
            var file = GetXmlFile(new ItemInfo(item), directoryService);

            if (file is null)
            {
                return false;
            }

            return file.Exists && _fileSystem.GetLastWriteTimeUtc(file) > item.DateLastSaved;
        }

        protected abstract void Fetch(MetadataResult<T> result, string path, CancellationToken cancellationToken);

        protected abstract FileSystemMetadata? GetXmlFile(ItemInfo info, IDirectoryService directoryService);
    }
}
