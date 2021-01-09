using MPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProjectMPI
{
    class Program
    {
        static void Main(string[] args)
        {
            MPI.Environment.Run(ref args, communicator => {
                if (Communicator.world.Rank == 0)
                { 
                }
                else
                { //child process
                  
                }
            });
        }
    }
}
