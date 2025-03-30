using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CnSharp.IO
{
    /// <summary>
    /// File watcher class
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("FilePath = {FilePath}")]
    public sealed class FileWatcher
    {
        #region Private Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FileSystemWatcher _fsw;

        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// Gets a value indicating whether the file content changed event can be raised: <c>true</c> means the event can be raised, the default value is <c>true</c>.
        /// </summary>
        public bool CanRaiseChangedEvent { get; set; }

        /// <summary>
        /// Gets the absolute path of the monitored file.
        /// </summary>
        public string FilePath { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher" /> class.
        /// </summary>
        /// <param name="filePath">The absolute path of the file to be monitored.</param>
        /// <exception cref="System.IO.FileNotFoundException">The file to be monitored was not found!</exception>
        public FileWatcher(string filePath)
            : this()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The file to be monitored was not found!", filePath);

            FilePath = filePath;
            InitFileSystemWatcher();
            CanRaiseChangedEvent = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher" /> class.
        /// </summary>
        public FileWatcher()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="FileWatcher" /> class.
        /// </summary>
        /// <param name="filePath">The absolute path of the file to be monitored.</param>
        /// <returns>The created instance of the <see cref="FileWatcher" /> class.</returns>
        public static FileWatcher Instance(string filePath)
        {
            return new FileWatcher(filePath);
        }

        #endregion Entrance

        #region Events

        /// <summary>
        /// Occurs when the content of the original  file changes.
        /// </summary>
        public event Action OnFileChanged;

        /// <summary>
        /// Raises the OnFileChanged event.
        /// </summary>
        public void RaiseChangedEvent()
        {
            if (OnFileChanged != null && CanRaiseChangedEvent)
            {
                OnFileChanged();
            }
        }

        #endregion Events

        #region Private Methods

        private void InitFileSystemWatcher()
        {
            _fsw = new FileSystemWatcher(Path.GetDirectoryName(FilePath), Path.GetFileName(FilePath))
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite |
                               NotifyFilters.Security,
            };

            _fsw.Changed += (sender, e) =>
            {
                var work = sender as FileSystemWatcher;

                if (_locker.TryEnterWriteLock(0))
                {
                    try
                    {
                        switch (e.ChangeType)
                        {
                            case WatcherChangeTypes.Changed:
                                work.EnableRaisingEvents = false;
                                RaiseChangedEvent();
                                work.EnableRaisingEvents = true;
                                break;

                            case WatcherChangeTypes.Deleted:
                            case WatcherChangeTypes.Renamed:
                                work.EnableRaisingEvents = false;
                                break;

                            case WatcherChangeTypes.All:
                            case WatcherChangeTypes.Created:
                            default:
                                break;
                        }
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                }
            };
            _fsw.EnableRaisingEvents = true;
        }

        #endregion Private Methods
    }
}