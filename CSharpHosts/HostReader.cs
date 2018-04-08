using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpHosts {
    public class HostsReader {
        // The default windows location for the hosts file.
        private const string sysHosts =
            @"C:\windows\system32\drivers\etc\Hosts";
        public string FilePath { get; set; }
        public List<string> Structure { get; set; }
        public HostsReader() {
            FilePath = sysHosts;
        }

        public HostsReader(string filePath) {
            FilePath = filePath;
        }

        public bool Load() {
            // Load the file to a list, for modification.
            List<string> structure = new List<string>(
                File.ReadAllLines(FilePath));

            // Initialise the Structure list.
            Structure = new List<string>();

            foreach (var e in structure) {
                // If e is a useless line, then add it as a useless line,
                // else we should ensure the line is a perfect entry,
                // and then add it to the Structure.
                string entry = isUseless(e.Trim()) ? e.Trim() :
                    HostEntry.FromLine(e).ToString().Trim();

                Structure.Add(entry);
            }

            return true;
        }

        public bool Save() {
            try {
                File.WriteAllLines(FilePath, Structure);
            } catch { return false; }

            return true;
        }

        // int -> Reference to position in Structure.
        // HostEntry -> Entry in file.
        public Dictionary<int, HostEntry> GetEntries() {
            var dict = new Dictionary<int, HostEntry>();

            // Loop through each entry in the structure, determining if
            // each item is a HostEntry or not.
            for (int i = 0; i < Structure.Count; i++) {
                string line = Structure[i];
                // If not a useless line, add it to the result.
                if (!isUseless(line))
                    dict.Add(i, HostEntry.FromLine(line));
            }

            return dict;
        }

        public void Add(HostEntry entry) {
            // Add the entry to the Structure.
            Structure.Add(entry.ToString().Trim());
        }
        public void Remove(HostEntry entry) {
            // Remove the entry to the Structure. Start from the end,
            // so that any removals won't affect the loop + iteration.
            for (int i = Structure.Count - 1; i > -1; i--) {
                string decl = entry.ToString().Trim();
                if (Structure[i].Trim() == decl)
                    Structure.RemoveAt(i);
            }
        }

        public KeyValuePair<int, HostEntry> this[int index] {
            get {
                // Get the first result of keys that match in
                // GetEntries(), with the specified index.
                return
                    GetEntries().SingleOrDefault(p => p.Key == index);
            }
            set {
                // Set the appropriate value in the Structure list,
                // with the key specified in the Dictionary key.
                Structure[value.Key] = value.Value.ToString().Trim();
            }
        }

        /*
         * Static methods
         */

        private static bool isUseless(string line) {
            // If it's empty, then it's useless and should be
            // disregarded as an entry.
            if (string.IsNullOrWhiteSpace(line)) return true;
            // If it doesn't start with a # (comment denotation)
            // then it should be considered an entry. Not accounting
            // for errors.
            if (!line.StartsWith("#")) return false;
            // If it starts with DISABLED, and there are >= 3 parts,
            // then let's assume it's a disabled entry.
            if (line.StartsWith("#DISABLED") &&
                line.Split(' ').Count() >= 3) return false;

            // Everything else, we assume is useless.
            return true;
        }
    }
}
