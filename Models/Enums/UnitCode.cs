using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Unit of measure codes based on UN/ECE Recommendation 20.
///
/// <para>These codes are used to specify the unit of measure for product quantities
/// in receipts and invoices. They are required for compliance with e-invoicing
/// standards like Peppol and EN16931.</para>
///
/// <para>Common examples:</para>
/// <list type="bullet">
///   <item><b>PIECE</b> - Individual items (most common for retail)</item>
///   <item><b>KILOGRAM</b> - Weight-based products (groceries, bulk items)</item>
///   <item><b>LITER</b> - Volume-based products (liquids)</item>
///   <item><b>HOUR</b> - Time-based services (consulting, labor)</item>
/// </list>
/// </summary>
/// <seealso href="https://unece.org/trade/uncefact/cl-recommendations">UN/ECE Recommendation 20</seealso>
public enum UnitCode
{
    // ===== MOST COMMON UNITS =====

    /// <summary>
    /// One (piece, unit). Most common unit for retail products - individual items. Peppol/UN/ECE Code: C62
    /// </summary>
    ONE,

    /// <summary>
    /// Each. Alternative to ONE, commonly used in North America. Peppol/UN/ECE Code: EA
    /// </summary>
    EACH,

    // ===== WEIGHT UNITS =====

    /// <summary>
    /// Kilogram. Used for weight-based products (groceries, bulk items). Peppol/UN/ECE Code: KGM
    /// </summary>
    KILOGRAM,

    /// <summary>
    /// Gram. Used for small weight measurements. Peppol/UN/ECE Code: GRM
    /// </summary>
    GRAM,

    /// <summary>
    /// Milligram. Used for very small weight measurements. Peppol/UN/ECE Code: MGM
    /// </summary>
    MILLIGRAM,

    /// <summary>
    /// Tonne (metric ton). 1000 kilograms. Peppol/UN/ECE Code: TNE
    /// </summary>
    TONNE,

    /// <summary>
    /// Pound. Used in imperial/US systems. Peppol/UN/ECE Code: LBR
    /// </summary>
    POUND,

    /// <summary>
    /// Ounce. Used in imperial/US systems. Peppol/UN/ECE Code: ONZ
    /// </summary>
    OUNCE,

    // ===== VOLUME UNITS =====

    /// <summary>
    /// Liter. Used for volume-based liquids. Peppol/UN/ECE Code: LTR
    /// </summary>
    LITER,

    /// <summary>
    /// Milliliter. Used for small volume measurements. Peppol/UN/ECE Code: MLT
    /// </summary>
    MILLILITER,

    /// <summary>
    /// Centiliter. Used for beverage measurements. Peppol/UN/ECE Code: CLT
    /// </summary>
    CENTILITER,

    /// <summary>
    /// Hectoliter. 100 liters, used for bulk liquids. Peppol/UN/ECE Code: HLT
    /// </summary>
    HECTOLITER,

    /// <summary>
    /// Cubic meter. Used for volume measurements. Peppol/UN/ECE Code: MTQ
    /// </summary>
    CUBIC_METER,

    /// <summary>
    /// Cubic centimeter. Used for small volume measurements. Peppol/UN/ECE Code: CMQ
    /// </summary>
    CUBIC_CENTIMETER,

    /// <summary>
    /// Gallon (US). Used in US for liquids. Peppol/UN/ECE Code: GLL
    /// </summary>
    GALLON_US,

    /// <summary>
    /// Gallon (UK). Used in UK for liquids. Peppol/UN/ECE Code: GLI
    /// </summary>
    GALLON_UK,

    // ===== LENGTH UNITS =====

    /// <summary>
    /// Meter. Used for length measurements. Peppol/UN/ECE Code: MTR
    /// </summary>
    METER,

    /// <summary>
    /// Centimeter. Used for small length measurements. Peppol/UN/ECE Code: CMT
    /// </summary>
    CENTIMETER,

    /// <summary>
    /// Millimeter. Used for very small length measurements. Peppol/UN/ECE Code: MMT
    /// </summary>
    MILLIMETER,

    /// <summary>
    /// Kilometer. Used for long distances. Peppol/UN/ECE Code: KMT
    /// </summary>
    KILOMETER,

    /// <summary>
    /// Inch. Used in imperial/US systems. Peppol/UN/ECE Code: INH
    /// </summary>
    INCH,

    /// <summary>
    /// Foot. Used in imperial/US systems. Peppol/UN/ECE Code: FOT
    /// </summary>
    FOOT,

    /// <summary>
    /// Yard. Used in imperial/US systems. Peppol/UN/ECE Code: YRD
    /// </summary>
    YARD,

    /// <summary>
    /// Mile. Used for long distances in imperial/US systems. Peppol/UN/ECE Code: SMI
    /// </summary>
    MILE,

    // ===== AREA UNITS =====

    /// <summary>
    /// Square meter. Used for area measurements. Peppol/UN/ECE Code: MTK
    /// </summary>
    SQUARE_METER,

    /// <summary>
    /// Square centimeter. Used for small area measurements. Peppol/UN/ECE Code: CMK
    /// </summary>
    SQUARE_CENTIMETER,

    /// <summary>
    /// Square foot. Used in imperial/US systems. Peppol/UN/ECE Code: FTK
    /// </summary>
    SQUARE_FOOT,

    /// <summary>
    /// Square inch. Used in imperial/US systems. Peppol/UN/ECE Code: INK
    /// </summary>
    SQUARE_INCH,

    // ===== TIME UNITS =====

    /// <summary>
    /// Second. Used for very short time measurements. Peppol/UN/ECE Code: SEC
    /// </summary>
    SECOND,

    /// <summary>
    /// Minute. Used for short time-based services. Peppol/UN/ECE Code: MIN
    /// </summary>
    MINUTE,

    /// <summary>
    /// Hour. Used for time-based services (consulting, labor). Peppol/UN/ECE Code: HUR
    /// </summary>
    HOUR,

    /// <summary>
    /// Day. Used for daily rentals or services. Peppol/UN/ECE Code: DAY
    /// </summary>
    DAY,

    /// <summary>
    /// Week. Used for weekly rentals or subscriptions. Peppol/UN/ECE Code: WEE
    /// </summary>
    WEEK,

    /// <summary>
    /// Month. Used for monthly subscriptions or rentals. Peppol/UN/ECE Code: MON
    /// </summary>
    MONTH,

    /// <summary>
    /// Year. Used for annual subscriptions or services. Peppol/UN/ECE Code: ANN
    /// </summary>
    YEAR,

    // ===== PACKAGING UNITS =====

    /// <summary>
    /// Set. Used for grouped items sold as a set. Peppol/UN/ECE Code: SET
    /// </summary>
    SET,

    /// <summary>
    /// Pair. Used for items sold in pairs (e.g., shoes, gloves). Peppol/UN/ECE Code: PR
    /// </summary>
    PAIR,

    /// <summary>
    /// Dozen. 12 items. Peppol/UN/ECE Code: DZN
    /// </summary>
    DOZEN,

    /// <summary>
    /// Piece. Used for individual items in packaging context. Peppol/UN/ECE Code: NMP
    /// </summary>
    PIECE,

    /// <summary>
    /// Pack. Used for packaged items. Peppol/UN/ECE Code: XPK
    /// </summary>
    PACK,

    /// <summary>
    /// Box. Used for boxed items. Peppol/UN/ECE Code: BX
    /// </summary>
    BOX,

    /// <summary>
    /// Carton. Used for items sold by the carton. Peppol/UN/ECE Code: CT
    /// </summary>
    CARTON,

    /// <summary>
    /// Case. Used for cased items. Peppol/UN/ECE Code: CS
    /// </summary>
    CASE,

    /// <summary>
    /// Package. Used for packaged items. Peppol/UN/ECE Code: PK
    /// </summary>
    PACKAGE,

    /// <summary>
    /// Pallet. Used for palletized goods. Peppol/UN/ECE Code: PF
    /// </summary>
    PALLET,

    /// <summary>
    /// Bottle. Used for bottled products. Peppol/UN/ECE Code: BO
    /// </summary>
    BOTTLE,

    /// <summary>
    /// Can. Used for canned products. Peppol/UN/ECE Code: CA
    /// </summary>
    CAN,

    /// <summary>
    /// Bag. Used for bagged items. Peppol/UN/ECE Code: BG
    /// </summary>
    BAG,

    // ===== SERVICE/ACTIVITY UNITS =====

    /// <summary>
    /// Service unit. Used for general service charges. Peppol/UN/ECE Code: E48
    /// </summary>
    SERVICE_UNIT,

    /// <summary>
    /// Activity. Used for activity-based billing. Peppol/UN/ECE Code: ACT
    /// </summary>
    ACTIVITY,

    /// <summary>
    /// Job. Used for job-based billing. Peppol/UN/ECE Code: E51
    /// </summary>
    JOB,

    // ===== INFORMATION/DATA UNITS =====

    /// <summary>
    /// Byte. Used for data storage/transfer. Peppol/UN/ECE Code: AD
    /// </summary>
    BYTE,

    /// <summary>
    /// Kilobyte. 1000 bytes. Peppol/UN/ECE Code: 2P
    /// </summary>
    KILOBYTE,

    /// <summary>
    /// Megabyte. 1000000 bytes. Peppol/UN/ECE Code: 4L
    /// </summary>
    MEGABYTE
}

/// <summary>
/// Extension methods for UnitCode enum.
/// </summary>
public static class UnitCodeExtensions
{
    private static readonly Dictionary<UnitCode, (string Code, string Description)> _unitCodeMap = new()
    {
        // Most common
        { UnitCode.ONE, ("C62", "One") },
        { UnitCode.EACH, ("EA", "Each") },
        
        // Weight
        { UnitCode.KILOGRAM, ("KGM", "Kilogram") },
        { UnitCode.GRAM, ("GRM", "Gram") },
        { UnitCode.MILLIGRAM, ("MGM", "Milligram") },
        { UnitCode.TONNE, ("TNE", "Tonne") },
        { UnitCode.POUND, ("LBR", "Pound") },
        { UnitCode.OUNCE, ("ONZ", "Ounce") },
        
        // Volume
        { UnitCode.LITER, ("LTR", "Liter") },
        { UnitCode.MILLILITER, ("MLT", "Milliliter") },
        { UnitCode.CENTILITER, ("CLT", "Centiliter") },
        { UnitCode.HECTOLITER, ("HLT", "Hectoliter") },
        { UnitCode.CUBIC_METER, ("MTQ", "Cubic meter") },
        { UnitCode.CUBIC_CENTIMETER, ("CMQ", "Cubic centimeter") },
        { UnitCode.GALLON_US, ("GLL", "Gallon (US)") },
        { UnitCode.GALLON_UK, ("GLI", "Gallon (UK)") },
        
        // Length
        { UnitCode.METER, ("MTR", "Meter") },
        { UnitCode.CENTIMETER, ("CMT", "Centimeter") },
        { UnitCode.MILLIMETER, ("MMT", "Millimeter") },
        { UnitCode.KILOMETER, ("KMT", "Kilometer") },
        { UnitCode.INCH, ("INH", "Inch") },
        { UnitCode.FOOT, ("FOT", "Foot") },
        { UnitCode.YARD, ("YRD", "Yard") },
        { UnitCode.MILE, ("SMI", "Mile") },
        
        // Area
        { UnitCode.SQUARE_METER, ("MTK", "Square meter") },
        { UnitCode.SQUARE_CENTIMETER, ("CMK", "Square centimeter") },
        { UnitCode.SQUARE_FOOT, ("FTK", "Square foot") },
        { UnitCode.SQUARE_INCH, ("INK", "Square inch") },
        
        // Time
        { UnitCode.SECOND, ("SEC", "Second") },
        { UnitCode.MINUTE, ("MIN", "Minute") },
        { UnitCode.HOUR, ("HUR", "Hour") },
        { UnitCode.DAY, ("DAY", "Day") },
        { UnitCode.WEEK, ("WEE", "Week") },
        { UnitCode.MONTH, ("MON", "Month") },
        { UnitCode.YEAR, ("ANN", "Year") },
        
        // Packaging
        { UnitCode.SET, ("SET", "Set") },
        { UnitCode.PAIR, ("PR", "Pair") },
        { UnitCode.DOZEN, ("DZN", "Dozen") },
        { UnitCode.PIECE, ("NMP", "Piece") },
        { UnitCode.PACK, ("XPK", "Pack") },
        { UnitCode.BOX, ("BX", "Box") },
        { UnitCode.CARTON, ("CT", "Carton") },
        { UnitCode.CASE, ("CS", "Case") },
        { UnitCode.PACKAGE, ("PK", "Package") },
        { UnitCode.PALLET, ("PF", "Pallet") },
        { UnitCode.BOTTLE, ("BO", "Bottle") },
        { UnitCode.CAN, ("CA", "Can") },
        { UnitCode.BAG, ("BG", "Bag") },
        
        // Service/Activity
        { UnitCode.SERVICE_UNIT, ("E48", "Service unit") },
        { UnitCode.ACTIVITY, ("ACT", "Activity") },
        { UnitCode.JOB, ("E51", "Job") },
        
        // Information/Data
        { UnitCode.BYTE, ("AD", "Byte") },
        { UnitCode.KILOBYTE, ("2P", "Kilobyte") },
        { UnitCode.MEGABYTE, ("4L", "Megabyte") }
    };

    /// <summary>
    /// Gets the UN/ECE code for this unit.
    /// This is the value that will be serialized to JSON.
    /// </summary>
    public static string GetCode(this UnitCode unit)
    {
        return _unitCodeMap.TryGetValue(unit, out var value) ? value.Code : unit.ToString();
    }

    /// <summary>
    /// Gets the human-readable description of this unit.
    /// </summary>
    public static string GetDescription(this UnitCode unit)
    {
        return _unitCodeMap.TryGetValue(unit, out var value) ? value.Description : unit.ToString();
    }

    /// <summary>
    /// Finds a UnitCode by its UN/ECE code.
    /// Used for JSON deserialization.
    /// </summary>
    /// <param name="code">The UN/ECE code to look up</param>
    /// <returns>The matching UnitCode</returns>
    /// <exception cref="ArgumentException">if the code is not recognized</exception>
    public static UnitCode FromCode(string code)
    {
        if (code == null)
            throw new ArgumentException("Unit code cannot be null");

        foreach (var kvp in _unitCodeMap)
        {
            if (kvp.Value.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                return kvp.Key;
        }

        throw new ArgumentException($"Unknown unit code: {code}. See UN/ECE Recommendation 20 for valid codes.");
    }

    /// <summary>
    /// Gets a string representation with code and description.
    /// </summary>
    public static string ToDetailedString(this UnitCode unit)
    {
        return $"{unit.GetCode()} ({unit.GetDescription()})";
    }
}
