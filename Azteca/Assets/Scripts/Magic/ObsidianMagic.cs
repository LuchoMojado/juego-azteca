using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianMagic : IMagicType
{
    //si necesitas referencias de cosas las pedis por constructor

    public void MagicType()
    {
        throw new System.NotImplementedException();
        // aca iria lo que ejecutamos cuando el jugador hace click
    }

    //en disconnected los codigos que usan este patron son: IMoveType, NormalMove, WallGrabMove, WallRunMove y Movement que es el que los llama
}
