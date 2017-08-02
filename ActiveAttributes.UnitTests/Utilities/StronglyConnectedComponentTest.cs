// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using ActiveAttributes.Utilities;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Utilities
{
  [TestFixture]
  public class StronglyConnectedComponentTests
  {
    [Test]
    public void EmptyGraph ()
    {
      var graph = new List<Vertex<int>> ();
      var detector = new StronglyConnectedComponentFinder<int> ();
      var cycles = detector.DetectCycle (graph);
      Assert.AreEqual (0, cycles.Count);
    }

    // A
    [Test]
    public void SingleVertex ()
    {
      var graph = new List<Vertex<int>> ();
      graph.Add (new Vertex<int> (1));
      var detector = new StronglyConnectedComponentFinder<int> ();
      var components = detector.DetectCycle (graph);
      Assert.AreEqual (1, components.Count);
      Assert.AreEqual (1, components.IndependentComponents ().Count ());
      Assert.AreEqual (0, components.Cycles ().Count ());
    }

    // A→B
    [Test]
    public void Linear2 ()
    {
      var graph = new List<Vertex<int>> ();
      var vA = new Vertex<int> (1);
      var vB = new Vertex<int> (2);
      vA.Dependencies.Add (vB);
      graph.Add (vA);
      graph.Add (vB);
      var detector = new StronglyConnectedComponentFinder<int> ();
      var components = detector.DetectCycle (graph);
      Assert.AreEqual (2, components.Count);
      Assert.AreEqual (2, components.IndependentComponents ().Count ());
      Assert.AreEqual (0, components.Cycles ().Count ());
    }

    // A→B→C
    [Test]
    public void Linear3 ()
    {
      var graph = new List<Vertex<int>> ();
      var vA = new Vertex<int> (1);
      var vB = new Vertex<int> (2);
      var vC = new Vertex<int> (3);
      vA.Dependencies.Add (vB);
      vB.Dependencies.Add (vC);
      graph.Add (vA);
      graph.Add (vB);
      graph.Add (vC);
      var detector = new StronglyConnectedComponentFinder<int> ();
      var components = detector.DetectCycle (graph);
      Assert.AreEqual (3, components.Count);
      Assert.AreEqual (3, components.IndependentComponents ().Count ());
      Assert.AreEqual (0, components.Cycles ().Count ());
    }

    // A↔B
    [Test]
    public void Cycle2 ()
    {
      var graph = new List<Vertex<int>> ();
      var vA = new Vertex<int> (1);
      var vB = new Vertex<int> (2);
      vA.Dependencies.Add (vB);
      vB.Dependencies.Add (vA);
      graph.Add (vA);
      graph.Add (vB);
      var detector = new StronglyConnectedComponentFinder<int> ();
      var components = detector.DetectCycle (graph);
      Assert.AreEqual (1, components.Count);
      Assert.AreEqual (0, components.IndependentComponents ().Count ());
      Assert.AreEqual (1, components.Cycles ().Count ());
      Assert.AreEqual (2, components.First ().Count);
    }

    // A→B
    // ↑ ↓
    // └─C
    [Test]
    public void Cycle3 ()
    {
      var graph = new List<Vertex<int>> ();
      var vA = new Vertex<int> (1);
      var vB = new Vertex<int> (2);
      var vC = new Vertex<int> (3);
      vA.Dependencies.Add (vB);
      vB.Dependencies.Add (vC);
      vC.Dependencies.Add (vA);
      graph.Add (vA);
      graph.Add (vB);
      graph.Add (vC);
      var detector = new StronglyConnectedComponentFinder<int> ();
      var components = detector.DetectCycle (graph);
      Assert.AreEqual (1, components.Count);
      Assert.AreEqual (0, components.IndependentComponents ().Count ());
      Assert.AreEqual (1, components.Cycles ().Count ());
      Assert.AreEqual (3, components.Single ().Count);
    }

    // A→B   D→E
    // ↑ ↓   ↑ ↓
    // └─C   └─F
    [Test]
    public void TwoIsolated3Cycles ()
    {
      var graph = new List<Vertex<int>> ();
      var vA = new Vertex<int> (1);
      var vB = new Vertex<int> (2);
      var vC = new Vertex<int> (3);
      vA.Dependencies.Add (vB);
      vB.Dependencies.Add (vC);
      vC.Dependencies.Add (vA);
      graph.Add (vA);
      graph.Add (vB);
      graph.Add (vC);

      var vD = new Vertex<int> (4);
      var vE = new Vertex<int> (5);
      var vF = new Vertex<int> (6);
      vD.Dependencies.Add (vE);
      vE.Dependencies.Add (vF);
      vF.Dependencies.Add (vD);
      graph.Add (vD);
      graph.Add (vE);
      graph.Add (vF);

      var detector = new StronglyConnectedComponentFinder<int> ();
      var components = detector.DetectCycle (graph);
      Assert.AreEqual (2, components.Count);
      Assert.AreEqual (0, components.IndependentComponents ().Count ());
      Assert.AreEqual (2, components.Cycles ().Count ());
      Assert.IsTrue (components.All (c => c.Count == 3));
    }

    // A→B
    // ↑ ↓
    // └─C-→D
    [Test]
    public void Cycle3WithStub ()
    {
      var graph = new List<Vertex<int>> ();
      var vA = new Vertex<int> (1);
      var vB = new Vertex<int> (2);
      var vC = new Vertex<int> (3);
      var vD = new Vertex<int> (4);
      vA.Dependencies.Add (vB);
      vB.Dependencies.Add (vC);
      vC.Dependencies.Add (vA);
      vC.Dependencies.Add (vD);
      graph.Add (vA);
      graph.Add (vB);
      graph.Add (vC);
      graph.Add (vD);
      var detector = new StronglyConnectedComponentFinder<int> ();
      var components = detector.DetectCycle (graph);
      Assert.AreEqual (2, components.Count);
      Assert.AreEqual (1, components.IndependentComponents ().Count ());
      Assert.AreEqual (1, components.Cycles ().Count ());
      Assert.AreEqual (1, components.Count (c => c.Count == 3));
      Assert.AreEqual (1, components.Count (c => c.Count == 1));
      Assert.IsTrue (components.Single (c => c.Count == 1).Single () == vD);
    }
  }
}
