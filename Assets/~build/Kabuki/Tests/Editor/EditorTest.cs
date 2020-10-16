using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace Kabuki.Test{
public class EditorTest{

    // Assertions ---------------------------------------------------

    protected void o (bool arg) => Assert.That(arg);

    protected void o (object x, object y) => Assert.That(x, Is.EqualTo(y));

    protected void Print(object arg) => UnityEngine.Debug.Log(arg);

    // --------------------------------------------------------------

}}
