using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.Multiplayer.Common;
using Photon.Pun;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
    class MinimapSynchronizer : TransformSynchronizer
    {
        private void Awake()
        {

        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            base.OnPhotonSerializeView(stream, info);
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
