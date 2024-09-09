using System.Collections;
using UnityEngine;

namespace  PSGJ15_DCSA
{
    public class EnnemyHealth : HealthComponentBase
    {    
        protected override IEnumerator Death()
        {
            yield return null;
        }

        protected override IEnumerator PlayDeathSequence()
        {
            yield return null;
        }
    }
}
