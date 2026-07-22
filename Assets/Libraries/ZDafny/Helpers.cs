namespace ZDafny.Helpers
{
    using System.Linq;
    using Dafny;

    public class Convertor
    {
        public static string RuneSeqToString(ISequence<Rune> src)
        {
            return string.Join("", src.CloneAsArray().Select(r => r.ToString()));
        }
    }
}
