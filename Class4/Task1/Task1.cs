using NUnit.Framework.Constraints;

namespace Task1
{
    // Необходимо заменить на более подходящий тип (коллекцию), позволяющий
    // эффективно искать диапазон по заданному IP-адресу
    using IPRangesDatabase = List<Task1.IPRange>;

    public class Task1
    {
        /*
        * Объекты этого класса создаются из строки, но хранят внутри помимо строки
        * ещё и целочисленное значение соответствующего адреса. Например, для адреса
         * 127.0.0.1 должно храниться число 1 + 0 * 2^8 + 0 * 2^16 + 127 * 2^24 = 2130706433.
        */
        internal record IPv4Addr(string StrValue) : IComparable<IPv4Addr>
        {
            internal uint IntValue = Ipstr2Int(StrValue);

            private static uint Ipstr2Int(string strValue)
            {
                uint intValue = 0;
                String[] strList = strValue.Split('.').Reverse().ToArray();
                for (int i = 0; i < strList.Length; ++i)
                {
                    intValue += (uint)Int32.Parse(strList[i]) * (uint)Math.Pow(256, i);
                }

                return intValue;
            }

            // Благодаря этому методу мы можем сравнивать два значения IPv4Addr
            public int CompareTo(IPv4Addr other)
            {
                return IntValue.CompareTo(other.IntValue);
            }

            public override string ToString()
            {
                return StrValue;
            }
        }

        internal record class IPRange(IPv4Addr IpFrom, IPv4Addr IpTo)
        {
            public override string ToString()
            {
                return $"{IpFrom},{IpTo}";
            }
        }

        internal record class IPLookupArgs(string IpsFile, List<string> IprsFiles);

        internal static IPLookupArgs? ParseArgs(string[] args)
        {
            if (args.Length < 2)
            {
                return null;
            }

            string ipsFile = args[0];
            List<string> iprsFiles = new List<string>();
            for (int i = 1; i < args.Length; ++i)
                iprsFiles.Add(args[i].TrimStart().TrimEnd());
            return new IPLookupArgs(ipsFile, iprsFiles);
        }

        internal static List<IPv4Addr> LoadQuery(string filename)
        {
            List<IPv4Addr> query = new List<IPv4Addr>();
            using (StreamReader reader = new StreamReader(filename))
            {
                string? line = reader.ReadLine();
                while (line != null)
                {
                    query.Add(new IPv4Addr(line));
                    line = reader.ReadLine();
                }
            }

            return query;
        }

        internal static IPRangesDatabase LoadRanges(List<String> filenames)
        {
            IPRangesDatabase query = new List<IPRange>();
            foreach (string filename in filenames)
            {
                StreamReader reader = new StreamReader(filename);
                string? line = reader.ReadLine();
                while (line != null)
                {
                    String[] range = line.Split(',');
                    var from = new IPv4Addr(range[0]);
                    var to = new IPv4Addr(range[1]);
                    query.Add(new IPRange(from, to));
                    line = reader.ReadLine();
                }
            }

            return query;
        }

        internal static IPRange? FindRange(IPRangesDatabase ranges, IPv4Addr addr)
        {
            foreach (var range in ranges)
            {
                if (range.IpFrom.CompareTo(addr) < 0 && addr.CompareTo(range.IpTo) < 0)
                {
                    return range;
                }
            }

            return null;
        }

        public static void Main(string[] args)
        {
            var ipLookupArgs = ParseArgs(args);
            if (ipLookupArgs == null)
            {
                return;
            }

            var queries = LoadQuery(ipLookupArgs.IpsFile);
            var ranges = LoadRanges(ipLookupArgs.IprsFiles);
            using (StreamWriter writer = new StreamWriter(ipLookupArgs.IpsFile.Replace(".ips", ".out"), true))
            {
                foreach (var ip in queries)
                {
                    var findRange = FindRange(ranges, ip);
                    var result = "NO";
                    if (findRange != null) result = $"YES ({findRange.ToString()})";
                    writer.WriteLine($"{ip}: {result}");
                }
            }
        }
    }
}
