using System;
namespace AMDM_Domain
{
    interface IAMDM_Medicine
    {
        string Barcode { get; set; }
        float BoxHeightMM { get; set; }
        float BoxLongMM { get; set; }
        float BoxWidthMM { get; set; }
        string Company { get; set; }
        int? CTOLIA { get; set; }
        string IdOfHIS { get; set; }
        string Name { get; set; }
        float? PTOLIA { get; set; }
        /// <summary>
        /// CanLoadMinExprationDay该药品需要满足至少多少天的有效期才可以装入药槽
        /// </summary>
        int? CLMED { get; set; }
        /// <summary>
        /// SuggestLoadMinExpirationDay建议装载到药槽中的药品有效期最少多少天
        /// </summary>
        int? SLMED { get; set; }
        /// <summary>
        /// days threshhold of expiration alert 有效期到达多少的阈值以后进行提醒
        /// </summary>
        int? DTOEA { get; set; }
    }
}
