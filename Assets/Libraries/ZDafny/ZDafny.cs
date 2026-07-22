// Dafny program the_program compiled into C#
// To recompile, you will need the libraries
//     System.Runtime.Numerics.dll System.Collections.Immutable.dll
// but the 'dotnet' tool in .NET should pick those up automatically.
// Optionally, you may want to include compiler switches like
//     /debug /nowarn:162,164,168,183,219,436,1717,1718

using System;
using System.Numerics;
using System.Collections;
[assembly: DafnyAssembly.DafnySourceAttribute(@"// dafny 4.11.0.0
// Command-line arguments: translate cs --library libs\UnityEngine.doo --library libs\Helpers.doo --no-verify -o generated\ZDafny c:\Users\Kigh_VM\Programming\Dafny\ZDafny\ZDafny.dfy
// the_program


module ZDafny {

  import UnityEngine
  class Test1 extends UnityEngine.MonoBehaviour {
    method Awake()
    {
      UnityEngine.Debug.Log(Helpers.Convertor.RuneSeqToString(""Hello""));
    }
  }

  class Test2 extends UnityEngine.MonoBehaviour {
    method Start()
    {
      UnityEngine.Debug.Log(Helpers.Convertor.RuneSeqToString(""Dafny!""));
    }
  }

  }

")]

namespace Dafny
{
  internal class ArrayHelpers
  {
    public static T[] InitNewArray1<T>(T z, BigInteger size0)
    {
      int s0 = (int)size0;
      T[] a = new T[s0];
      for (int i0 = 0; i0 < s0; i0++)
      {
        a[i0] = z;
      }
      return a;
    }
  }
} // end of namespace Dafny
internal static class FuncExtensions
{
  public static Func<UResult> DowncastClone<TResult, UResult>(this Func<TResult> F, Func<TResult, UResult> ResConv)
  {
    return () => ResConv(F());
  }
  public static Func<U, UResult> DowncastClone<T, TResult, U, UResult>(this Func<T, TResult> F, Func<U, T> ArgConv, Func<TResult, UResult> ResConv)
  {
    return arg => ResConv(F(ArgConv(arg)));
  }
}
// end of class FuncExtensions
namespace ZDafny
{


  public partial class Test1 : UnityEngine.MonoBehaviour
  {
    public Test1()
    {
    }
    public void Awake()
    {
      UnityEngine.Debug.Log(ZDafny.Helpers.Convertor.RuneSeqToString(Dafny.Sequence<Dafny.Rune>.UnicodeFromString("Hello")));
    }
  }

  public partial class Test2 : UnityEngine.MonoBehaviour
  {
    public Test2()
    {
    }
    public void Start()
    {
      UnityEngine.Debug.Log(ZDafny.Helpers.Convertor.RuneSeqToString(Dafny.Sequence<Dafny.Rune>.UnicodeFromString("Dafny!")));
    }
  }
} // end of namespace ZDafny
namespace _module
{

} // end of namespace _module
