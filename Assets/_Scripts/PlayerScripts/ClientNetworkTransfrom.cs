using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
    [DisallowMultipleComponent]
    public class ClientAuthTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }


}
