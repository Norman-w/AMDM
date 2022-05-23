using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    interface IAMDM_Grid_Ex
    {
        float WidthMM { get; set; }
        float HeightMM { get; set; }
        float DepthMM { get; set; }
        int MaxLoadAbleCount { get; set; }
    }
}
