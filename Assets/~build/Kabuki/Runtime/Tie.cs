using ArgEx = System.ArgumentException;

namespace Active.Core{
public readonly struct tie{

    public static readonly tie done = new tie(+1),
             fail = new tie(-1),
             cont = new tie( 0);
    internal readonly int ω;

    public tie(int value) => this.ω = value;

    public bool failing  => ω <= -1;
    public bool running  => ω ==  0;
    public bool complete => ω >=  1;

    public static tie operator & (tie x, tie y)
    => (x.ω != 0) ? throw new ArgEx("Unexpected Value") : x;

    public static tie operator | (tie x, tie y)
    => (x.ω == 0) ? throw new ArgEx("Unexpected Value") : x;

    public static bool operator true  (tie s) => s.ω == 0;

    public static bool operator false (tie s) => s.ω != 0;

    public static explicit operator tie(status that) => that.complete ? done : that.running ? cont : fail;

    public static implicit operator status(tie that) => that.complete ? status.done() :
                        that.running ? status.cont() : status.fail();

}}
