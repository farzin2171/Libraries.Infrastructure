namespace LibraryTools.Types
{
    /// <summary>
    /// Used to provide Startup task support
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Executes the logic
        /// </summary>
        void Execute();
    }
}
