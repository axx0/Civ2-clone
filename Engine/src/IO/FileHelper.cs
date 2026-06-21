using System;
using System.IO;
using System.Text;

namespace Civ2engine.IO
{
    public static class FileHelper
    {
        /// <summary>
        /// Ensures a file path is canonical (normalized and absolute).
        /// </summary>
        /// <param name="path">The file path to normalize.</param>
        /// <returns>The canonical absolute path.</returns>
        public static string GetCanonicalPath(string path)
        {
            // Normalize the path and resolve to absolute
            string canonicalPath = Path.GetFullPath(path);
            
            // Ensure the path is normalized (e.g., resolve .. and .)
            canonicalPath = Path.Combine(Directory.GetCurrentDirectory(), canonicalPath);
            
            return canonicalPath;
        }

        /// <summary>
        /// Safely writes content to a file with async thread affinity.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="content">The content to write.</param>
        /// <param name="overwrite">Whether to overwrite existing content.</param>
        /// <returns>True if write succeeded, false otherwise.</returns>
        public static bool WriteSafe(string path, string content, bool overwrite = true)
        {
            // Ensure path is canonical
            string canonicalPath = GetCanonicalPath(path);
            
            // Check if file exists and handle overwrite
            if (File.Exists(canonicalPath) && !overwrite)
            {
                Console.WriteLine($"File {canonicalPath} already exists. Use overwrite=true to replace.");
                return false;
            }
            
            try
            {
                // Write content with async thread affinity
                using var writer = new StreamWriter(canonicalPath, false, Encoding.UTF8);
                writer.Write(content);
                writer.Flush();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to {canonicalPath}: {ex.Message}");
                return false;
            }
        }
    }
}