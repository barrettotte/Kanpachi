using System;
using IBMi.Lib;

namespace IBMi.CLI
{
    class Program
    {
        static void Main(string[] args){
            SshTest test = new SshTest();
            test.Test();
        }
    }
}
