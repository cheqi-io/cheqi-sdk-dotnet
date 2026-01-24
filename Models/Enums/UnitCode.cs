using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Unit of measurement codes (UN/CEFACT).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UnitCode
{
    /// <summary>
    /// Each (unit).
    /// </summary>
    EA,

    /// <summary>
    /// Kilogram.
    /// </summary>
    KGM,

    /// <summary>
    /// Gram.
    /// </summary>
    GRM,

    /// <summary>
    /// Litre.
    /// </summary>
    LTR,

    /// <summary>
    /// Millilitre.
    /// </summary>
    MLT,

    /// <summary>
    /// Metre.
    /// </summary>
    MTR,

    /// <summary>
    /// Centimetre.
    /// </summary>
    CMT,

    /// <summary>
    /// Millimetre.
    /// </summary>
    MMT,

    /// <summary>
    /// Square metre.
    /// </summary>
    MTK,

    /// <summary>
    /// Cubic metre.
    /// </summary>
    MTQ,

    /// <summary>
    /// Piece.
    /// </summary>
    PCE,

    /// <summary>
    /// Pack.
    /// </summary>
    PA,

    /// <summary>
    /// Box.
    /// </summary>
    BX,

    /// <summary>
    /// Hour.
    /// </summary>
    HUR,

    /// <summary>
    /// Day.
    /// </summary>
    DAY,

    /// <summary>
    /// Week.
    /// </summary>
    WEE,

    /// <summary>
    /// Month.
    /// </summary>
    MON,

    /// <summary>
    /// Year.
    /// </summary>
    ANN
}
