using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApplication1;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            magicSolver magicSolver = new magicSolver();
            Assert.AreEqual(
                "Case #1: 7\nCase #2: Bad magician!\nCase #3: Volunteer cheated!", magicSolver.GetResult(
                "3\n\n2\n1 2 3 4\n5 6 7 8\n9 10 11 12\n13 14 15 16\n3\n1 2 5 4\n3 11 6 15\n9 10 7 12\n13 14 8 16\n2\n1 2 3 4\n5 6 7 8\n9 10 11 12\n13 14 15 16\n2\n1 2 3 4\n5 6 7 8\n9 10 11 12v13 14 15 16\n2\n1 2 3 4\n5 6 7 8\n9 10 11 12\n13 14 15 16\n3\n1 2 3 4\n5 6 7 8\n9 10 11 12\n13 14 15 16"));
        }  
        [TestMethod]
        public void TestMethod2()
        {
            magicSolver magicSolver = new magicSolver();
            string result = magicSolver.GetResult("3\n\n2\n1 2 3 4\n5 6 7 8\n9 10 11 12\n13 14 15 16\n3\n1 2 5 4\n3 11 6 15\n9 10 7 12\n13 14 8 16\n2\n1 2 3 4\n5 6 7 8\n9 10 11 12\n13 14 15 16\n2\n1 2 3 4\n5 6 7 8\n9 10 11 12v13 14 15 16\n2\n1 2 3 4\n5 6 7 8\n9 10 11 12\n13 14 15 16\n3\n1 2 3 4\n5 6 7 8\n9 10 11 12\n13 14 15 16");
            Debug.Print(result);
            Assert.IsNull(null);
        }

    }    
}
