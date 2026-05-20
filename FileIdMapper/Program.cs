using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileIdMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            string baseDir = args.Length > 0 ? Path.GetFullPath(args[0]) : Directory.GetCurrentDirectory();
            string csvPath = Path.Combine(baseDir, "listfile.csv");
            string outPath = Path.Combine(baseDir, "custom_files.txt");

            if (!File.Exists(csvPath))
            {
                Console.Error.WriteLine($"listfile.csv not found: {csvPath}");
                Environment.Exit(1);
            }

            // ------------------------------------------------------------
            // Phase 0 – Count lines to size dictionary perfectly
            // ------------------------------------------------------------
            long lineCount = CountLinesFast(csvPath);
            int dictCapacity = (int)(lineCount + (lineCount / 10));
            if (dictCapacity < 16) dictCapacity = 16;

            Console.WriteLine($"Detected ~{lineCount:N0} CSV lines. Sizing dictionary to {dictCapacity:N0} buckets.");
            var lookup = new Dictionary<string, long>(capacity: dictCapacity, comparer: StringComparer.OrdinalIgnoreCase);

            // ------------------------------------------------------------
            // Phase 1 – Parse CSV into dictionary
            // ------------------------------------------------------------
            using (var sr = new StreamReader(csvPath, Encoding.UTF8, false, bufferSize: 2_097_152))
            {
                while (sr.ReadLine() is string line)
                {
                    int semi = line.IndexOf(';');
                    if (semi <= 0) continue;

                    ReadOnlySpan<char> idSpan = line.AsSpan(0, semi);
                    if (!long.TryParse(idSpan, out long fileId)) continue;

                    string path = line.Substring(semi + 1);
                    if (path.AsSpan().IndexOf('\\') >= 0)
                        path = path.Replace('\\', '/');

                    lookup[path] = fileId;
                }
            }

            Console.WriteLine($"Loaded {lookup.Count:N0} rows in {sw.Elapsed.TotalMilliseconds:F1} ms ({GC.GetTotalMemory(false) / 1024 / 1024} MB used)");
            sw.Restart();

            // ------------------------------------------------------------
            // Phase 2 – Scan disk and match
            // ------------------------------------------------------------
            var matches = new List<(long id, string path)>(capacity: 50_000);

            foreach (string file in Directory.EnumerateFiles(baseDir, "*.*", SearchOption.AllDirectories))
            {
                if (file.Equals(outPath, StringComparison.OrdinalIgnoreCase) ||
                    file.Equals(csvPath, StringComparison.OrdinalIgnoreCase))
                    continue;

                string rel = Path.GetRelativePath(baseDir, file);
                if (rel.AsSpan().IndexOf('\\') >= 0)
                    rel = rel.Replace('\\', '/');

                if (lookup.TryGetValue(rel, out long fileId))
                    matches.Add((fileId, rel));
            }

            Console.WriteLine($"Matched {matches.Count:N0} files in {sw.Elapsed.TotalMilliseconds:F1} ms");
            sw.Restart();

            // ------------------------------------------------------------
            // Phase 3 – Sort by FileID
            // ------------------------------------------------------------
            matches.Sort((a, b) => a.id.CompareTo(b.id));

            // ------------------------------------------------------------
            // Phase 4 – Write custom_files.txt
            // ------------------------------------------------------------
            using (var writer = new StreamWriter(outPath, append: false, Encoding.UTF8, bufferSize: 2_097_152))
            {
                foreach (var (id, path) in matches)
                {
                    writer.Write(id);
                    writer.Write(';');
                    writer.Write(path);
                    writer.Write('\n');
                }
            }

            sw.Stop();
            Console.WriteLine($"Wrote custom_files.txt in {sw.Elapsed.TotalMilliseconds:F1} ms");
        }

        static long CountLinesFast(string path)
        {
            long count = 0;
            const int bufferSize = 131_072;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: bufferSize, useAsync: false);
                int read;
                while ((read = fs.Read(buffer, 0, bufferSize)) > 0)
                {
                    ReadOnlySpan<byte> span = buffer.AsSpan(0, read);
                    for (int i = 0; i < span.Length; i++)
                        if (span[i] == (byte)'\n')
                            count++;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            if (new FileInfo(path).Length > 0)
                count++;

            return count;
        }
    }
}