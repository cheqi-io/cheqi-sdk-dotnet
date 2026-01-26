using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Unit of measure codes based on UN/ECE Recommendation 20.
/// </summary>
[JsonConverter(typeof(UnitCodeJsonConverter))]
public enum UnitCode
{
    /// <summary>One (piece, unit). UN/ECE Code: C62</summary>
    [EnumMember(Value = "C62")] ONE,
    
    /// <summary>Each. UN/ECE Code: EA</summary>
    [EnumMember(Value = "EA")] EACH,
    
    /// <summary>Kilogram. UN/ECE Code: KGM</summary>
    [EnumMember(Value = "KGM")] KILOGRAM,
    
    /// <summary>Gram. UN/ECE Code: GRM</summary>
    [EnumMember(Value = "GRM")] GRAM,
    
    /// <summary>Milligram. UN/ECE Code: MGM</summary>
    [EnumMember(Value = "MGM")] MILLIGRAM,
    
    /// <summary>Tonne. UN/ECE Code: TNE</summary>
    [EnumMember(Value = "TNE")] TONNE,
    
    /// <summary>Pound. UN/ECE Code: LBR</summary>
    [EnumMember(Value = "LBR")] POUND,
    
    /// <summary>Ounce. UN/ECE Code: ONZ</summary>
    [EnumMember(Value = "ONZ")] OUNCE,
    
    /// <summary>Liter. UN/ECE Code: LTR</summary>
    [EnumMember(Value = "LTR")] LITER,
    
    /// <summary>Milliliter. UN/ECE Code: MLT</summary>
    [EnumMember(Value = "MLT")] MILLILITER,
    
    /// <summary>Centiliter. UN/ECE Code: CLT</summary>
    [EnumMember(Value = "CLT")] CENTILITER,
    
    /// <summary>Hectoliter. UN/ECE Code: HLT</summary>
    [EnumMember(Value = "HLT")] HECTOLITER,
    
    /// <summary>Cubic meter. UN/ECE Code: MTQ</summary>
    [EnumMember(Value = "MTQ")] CUBIC_METER,
    
    /// <summary>Cubic centimeter. UN/ECE Code: CMQ</summary>
    [EnumMember(Value = "CMQ")] CUBIC_CENTIMETER,
    
    /// <summary>Gallon (US). UN/ECE Code: GLL</summary>
    [EnumMember(Value = "GLL")] GALLON_US,
    
    /// <summary>Gallon (UK). UN/ECE Code: GLI</summary>
    [EnumMember(Value = "GLI")] GALLON_UK,
    
    /// <summary>Meter. UN/ECE Code: MTR</summary>
    [EnumMember(Value = "MTR")] METER,
    
    /// <summary>Centimeter. UN/ECE Code: CMT</summary>
    [EnumMember(Value = "CMT")] CENTIMETER,
    
    /// <summary>Millimeter. UN/ECE Code: MMT</summary>
    [EnumMember(Value = "MMT")] MILLIMETER,
    
    /// <summary>Kilometer. UN/ECE Code: KMT</summary>
    [EnumMember(Value = "KMT")] KILOMETER,
    
    /// <summary>Inch. UN/ECE Code: INH</summary>
    [EnumMember(Value = "INH")] INCH,
    
    /// <summary>Foot. UN/ECE Code: FOT</summary>
    [EnumMember(Value = "FOT")] FOOT,
    
    /// <summary>Yard. UN/ECE Code: YRD</summary>
    [EnumMember(Value = "YRD")] YARD,
    
    /// <summary>Mile. UN/ECE Code: SMI</summary>
    [EnumMember(Value = "SMI")] MILE,
    
    /// <summary>Square meter. UN/ECE Code: MTK</summary>
    [EnumMember(Value = "MTK")] SQUARE_METER,
    
    /// <summary>Square centimeter. UN/ECE Code: CMK</summary>
    [EnumMember(Value = "CMK")] SQUARE_CENTIMETER,
    
    /// <summary>Square foot. UN/ECE Code: FTK</summary>
    [EnumMember(Value = "FTK")] SQUARE_FOOT,
    
    /// <summary>Square inch. UN/ECE Code: INK</summary>
    [EnumMember(Value = "INK")] SQUARE_INCH,
    
    /// <summary>Second. UN/ECE Code: SEC</summary>
    [EnumMember(Value = "SEC")] SECOND,
    
    /// <summary>Minute. UN/ECE Code: MIN</summary>
    [EnumMember(Value = "MIN")] MINUTE,
    
    /// <summary>Hour. UN/ECE Code: HUR</summary>
    [EnumMember(Value = "HUR")] HOUR,
    
    /// <summary>Day. UN/ECE Code: DAY</summary>
    [EnumMember(Value = "DAY")] DAY,
    
    /// <summary>Week. UN/ECE Code: WEE</summary>
    [EnumMember(Value = "WEE")] WEEK,
    
    /// <summary>Month. UN/ECE Code: MON</summary>
    [EnumMember(Value = "MON")] MONTH,
    
    /// <summary>Year. UN/ECE Code: ANN</summary>
    [EnumMember(Value = "ANN")] YEAR,
    
    /// <summary>Set. UN/ECE Code: SET</summary>
    [EnumMember(Value = "SET")] SET,
    
    /// <summary>Pair. UN/ECE Code: PR</summary>
    [EnumMember(Value = "PR")] PAIR,
    
    /// <summary>Dozen. UN/ECE Code: DZN</summary>
    [EnumMember(Value = "DZN")] DOZEN,
    
    /// <summary>Piece. UN/ECE Code: NMP</summary>
    [EnumMember(Value = "NMP")] PIECE,
    
    /// <summary>Pack. UN/ECE Code: XPK</summary>
    [EnumMember(Value = "XPK")] PACK,
    
    /// <summary>Box. UN/ECE Code: BX</summary>
    [EnumMember(Value = "BX")] BOX,
    
    /// <summary>Carton. UN/ECE Code: CT</summary>
    [EnumMember(Value = "CT")] CARTON,
    
    /// <summary>Case. UN/ECE Code: CS</summary>
    [EnumMember(Value = "CS")] CASE,
    
    /// <summary>Package. UN/ECE Code: PK</summary>
    [EnumMember(Value = "PK")] PACKAGE,
    
    /// <summary>Pallet. UN/ECE Code: PF</summary>
    [EnumMember(Value = "PF")] PALLET,
    
    /// <summary>Bottle. UN/ECE Code: BO</summary>
    [EnumMember(Value = "BO")] BOTTLE,
    
    /// <summary>Can. UN/ECE Code: CA</summary>
    [EnumMember(Value = "CA")] CAN,
    
    /// <summary>Bag. UN/ECE Code: BG</summary>
    [EnumMember(Value = "BG")] BAG,
    
    /// <summary>Service unit. UN/ECE Code: E48</summary>
    [EnumMember(Value = "E48")] SERVICE_UNIT,
    
    /// <summary>Activity. UN/ECE Code: ACT</summary>
    [EnumMember(Value = "ACT")] ACTIVITY,
    
    /// <summary>Job. UN/ECE Code: E51</summary>
    [EnumMember(Value = "E51")] JOB,
    
    /// <summary>Byte. UN/ECE Code: AD</summary>
    [EnumMember(Value = "AD")] BYTE,
    
    /// <summary>Kilobyte. UN/ECE Code: 2P</summary>
    [EnumMember(Value = "2P")] KILOBYTE,
    
    /// <summary>Megabyte. UN/ECE Code: 4L</summary>
    [EnumMember(Value = "4L")] MEGABYTE
}

/// <summary>
/// Extension methods for UnitCode enum.
/// </summary>
public static class UnitCodeExtensions
{
    /// <summary>
    /// Parses a UN/ECE code string to a UnitCode enum value.
    /// </summary>
    public static UnitCode FromCode(string code)
    {
        if (code == null)
            throw new ArgumentException("Unit code cannot be null");

        foreach (UnitCode unitCode in Enum.GetValues(typeof(UnitCode)))
        {
            var memberInfo = typeof(UnitCode).GetField(unitCode.ToString());
            var enumMemberAttr = memberInfo?.GetCustomAttribute<System.Runtime.Serialization.EnumMemberAttribute>();
            
            if (enumMemberAttr?.Value?.Equals(code, StringComparison.OrdinalIgnoreCase) == true)
            {
                return unitCode;
            }
        }

        throw new ArgumentException($"Unknown unit code: {code}. See UN/ECE Recommendation 20 for valid codes.");
    }

    /// <summary>
    /// Gets the UN/ECE code string for a UnitCode enum value.
    /// </summary>
    public static string GetCode(this UnitCode unitCode)
    {
        var memberInfo = typeof(UnitCode).GetField(unitCode.ToString());
        var enumMemberAttr = memberInfo?.GetCustomAttribute<System.Runtime.Serialization.EnumMemberAttribute>();
        return enumMemberAttr?.Value ?? unitCode.ToString();
    }
}
