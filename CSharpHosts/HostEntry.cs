using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpHosts
{
    public class HostEntry {
        public bool Enabled { get; set; }
        public string IpAddress { get; set; }
        public string WebAddress { get; set; }
        public string Comment { get; set; }

        public override string ToString() {
            // Basic string concatenation, in host file entry form.
            return
                (!Enabled ? "#DISABLED " : "")
                + IpAddress + " " + WebAddress + " " +
                (Comment == "" ? "" : ("#" + Comment));
        }
        public HostEntry() { }

        public HostEntry(bool enabled,
            string ipAddress,
            string webAddress,
            string comment) {

            this.Enabled = enabled;
            this.IpAddress = ipAddress;
            this.WebAddress = webAddress;
            this.Comment = comment;
        }
        public static HostEntry FromLine(string line) {
            // If the string begins with "#DISABLED " then it should
            // return disabled = !enabled.
            // Alternative: 
            //      -> Regex.Matches(line, "^#DISABLED (.*?)").Count < 1;
            bool enabled = !line.StartsWith("#DISABLED ");
            // Any string before a #, a # indicates a comment. If the
            // count is more or equal to one, it has a comment.
            bool commented = Regex.Matches(line, ".+#(.*)?$").Count >= 1;

            // If it's disabled, account for the that in the regex.
            // else, have a standard regex matcher.
            string regex = !enabled ? "^#DISABLED (.*?) (.*)" : "^(.*?) (.*)";
            // If it contains a comment, then add regex to account
            // for that comment. (#(.*)) matches everything beyond the
            // # character.
            regex += commented ? " #(.*)" : "";

            // Get the only match in the string (assume `line` is one
            // line only).
            var g = Regex.Matches(line, regex)[0];

            // g.Groups[1] = the ip,
            // g.Groups[2] = the address,
            // g.Groups[3] = the comment, if there is one.
            HostEntry e = new HostEntry(enabled, g.Groups[1].Value,
                g.Groups[2].Value, commented ? g.Groups[3].Value : "");
            return e;
        }
    }
}
