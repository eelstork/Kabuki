using NUnit.Framework;
using Active.Core;

namespace Kabuki.Test{
public class TieTest: EditorTest{

    [Test] public void And([Values(-1, 0, 1)]int x, [Values(-1, 0, 1)]int y){
        o( new tie(x) && new tie(y), new tie(x));
    }

    [Test] public void Short([Values(-1, 1)]int x){
        var a = 0;
        var z = new tie(x) && Do(a = 1);
        o(a, 0);
    }

    [Test] public void Bridge(){
        var a = 0;
        var x = tie.cont && Do(a = 1);
        o(a, 1);
    }

    tie Do(object arg) => tie.done;

}}
