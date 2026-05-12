using System.IO;

namespace Calculator
{
    internal static class SoundPaths
    {
        public static string InSoundsFolder(string fileName) =>
            Path.Combine(AppContext.BaseDirectory, "resources", "sounds", fileName);
    }
}
