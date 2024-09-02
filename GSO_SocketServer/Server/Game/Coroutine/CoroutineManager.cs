using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class CoroutineManager
    {
        private List<Coroutine> coroutines = new List<Coroutine>();

        public void StartCoroutine(IEnumerator routine)
        {
            coroutines.Add(new Coroutine(routine));
        }
    }
}
