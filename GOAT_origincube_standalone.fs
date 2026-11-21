FeatureScript 3132;
import(path : "onshape/std/common.fs", version : "3132.0");

// =====================================================================
// GOAT_OriginCube (self-contained / 自包含版)
// 不依赖任何外部文档导入，可在任何 OnShape 文档中编译使用。
//
// 功能 1：导出 20 个带单位的机械参考函数，可在任意尺寸/变量输入框中
//         直接调用，例如  M3_tap_hole()  ->  2.5 mm
// 功能 2：通用查表函数  GOAT_ref("M3_tap_hole")  ->  2.5 mm
// 功能 3：插入 GOAT_OriginCube feature 后，可把全部参考值导入为
//         #GOAT_... 命名变量
//
// 数据来源：OnShape 文档 "7. Mechanical Reference Guide"
// （已备份至 mechanical_reference_guide.csv）
// =====================================================================

// ------------------------------------------------------------------
// Fasteners
// ------------------------------------------------------------------

export function M3_tap_hole() returns ValueWithUnits
{
    return 2.5 * millimeter;
}

export function M3_clearance_hole() returns ValueWithUnits
{
    return 3.2 * millimeter;
}

export function M4_tap_hole() returns ValueWithUnits
{
    return 3.3 * millimeter;
}

export function M4_clearance_hole() returns ValueWithUnits
{
    return 4.2 * millimeter;
}

export function M5_tap_hole() returns ValueWithUnits
{
    return 4.1 * millimeter;
}

export function M5_clearance_hole() returns ValueWithUnits
{
    return 5.2 * millimeter;
}

export function M6_tap_hole() returns ValueWithUnits
{
    return 5.1 * millimeter;
}

export function M6_clearance_hole() returns ValueWithUnits
{
    return 6.2 * millimeter;
}

export function tap_hole_10_32() returns ValueWithUnits
{
    return 4.04 * millimeter;
}

export function clearance_hole_10_32() returns ValueWithUnits
{
    return 5 * millimeter;
}

export function Standard_bearing_1_125() returns ValueWithUnits
{
    return 28.65 * millimeter;
}

export function M5_Nut_Distance() returns ValueWithUnits
{
    return 8 * millimeter;
}

export function M5_Socket_Head() returns ValueWithUnits
{
    return 8.5 * millimeter;
}

// ------------------------------------------------------------------
// Fabrication
// ------------------------------------------------------------------

export function Plate_Chamfer() returns ValueWithUnits
{
    return 0.5 * millimeter;
}

export function Shaft_End_Chamfer() returns ValueWithUnits
{
    return 1 * millimeter;
}

export function Shaft_TapHole_Depth_M5() returns ValueWithUnits
{
    return 15 * millimeter;
}

export function Spacer_M4_OD() returns ValueWithUnits
{
    return 9 * millimeter;
}

export function Spacer_M5_OD() returns ValueWithUnits
{
    return 10 * millimeter;
}

// ------------------------------------------------------------------
// Magic Numbers
// ------------------------------------------------------------------

export function Offset_3DPrint() returns ValueWithUnits
{
    return 0.2 * millimeter;
}

export function Offset_CTC() returns ValueWithUnits
{
    return 0.381 * millimeter;
}

// ------------------------------------------------------------------
// Generic lookup: GOAT_ref("M3_tap_hole") -> 2.5 mm
// ------------------------------------------------------------------

export function GOAT_ref(name is string) returns ValueWithUnits
{
    const GOAT_REFERENCE_VALUES = {
        // Fasteners
        "M3_tap_hole" : 2.5 * millimeter,
        "M3_clearance_hole" : 3.2 * millimeter,
        "M4_tap_hole" : 3.3 * millimeter,
        "M4_clearance_hole" : 4.2 * millimeter,
        "M5_tap_hole" : 4.1 * millimeter,
        "M5_clearance_hole" : 5.2 * millimeter,
        "M6_tap_hole" : 5.1 * millimeter,
        "M6_clearance_hole" : 6.2 * millimeter,
        "tap_hole_10_32" : 4.04 * millimeter,
        "clearance_hole_10_32" : 5 * millimeter,
        "Standard_bearing_1_125" : 28.65 * millimeter,
        "M5_Nut_Distance" : 8 * millimeter,
        "M5_Socket_Head" : 8.5 * millimeter,
        // Fabrication
        "Plate_Chamfer" : 0.5 * millimeter,
        "Shaft_End_Chamfer" : 1 * millimeter,
        "Shaft_TapHole_Depth_M5" : 15 * millimeter,
        "Spacer_M4_OD" : 9 * millimeter,
        "Spacer_M5_OD" : 10 * millimeter,
        // Magic Numbers
        "Offset_3DPrint" : 0.2 * millimeter,
        "Offset_CTC" : 0.381 * millimeter
    };

    // Unknown names fail the type check and surface an error to the caller
    const value is ValueWithUnits = GOAT_REFERENCE_VALUES[name];
    return value;
}

// ------------------------------------------------------------------
// Feature：将参考值导入为命名变量（#GOAT_...）
// ------------------------------------------------------------------

annotation { "Feature Type Name" : "GOAT_OriginCube", "Icon" : Icon.ALONG_X, "Feature Name Template" : "#_name",
    "Feature Type Description" : "GOAT mechanical reference. Exports 20 unit-bearing functions callable in any dimension or variable field (e.g. M3_tap_hole(), GOAT_ref with a name string). Optionally imports all values as #GOAT_ named variables. Data source: 7. Mechanical Reference Guide." }
export const goatOriginCube = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Import GOAT reference variables", "Default" : true, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
        definition.importVariables is boolean;
    }
    {
        if (definition.importVariables)
        {
        const GOATReferenceVariables = [
            // Fasteners
            makeVariable("GOAT_M3_tap_hole", M3_tap_hole(), "M3 tap drill hole diameter (GOAT reference)"),
            makeVariable("GOAT_M3_clearance_hole", M3_clearance_hole(), "M3 clearance hole diameter (GOAT reference)"),
            makeVariable("GOAT_M4_tap_hole", M4_tap_hole(), "M4 tap drill hole diameter (GOAT reference)"),
            makeVariable("GOAT_M4_clearance_hole", M4_clearance_hole(), "M4 clearance hole diameter (GOAT reference)"),
            makeVariable("GOAT_M5_tap_hole", M5_tap_hole(), "M5 tap drill hole diameter (GOAT reference)"),
            makeVariable("GOAT_M5_clearance_hole", M5_clearance_hole(), "M5 clearance hole diameter (GOAT reference)"),
            makeVariable("GOAT_M6_tap_hole", M6_tap_hole(), "M6 tap drill hole diameter (GOAT reference)"),
            makeVariable("GOAT_M6_clearance_hole", M6_clearance_hole(), "M6 clearance hole diameter (GOAT reference)"),
            makeVariable("GOAT_tap_hole_10_32", tap_hole_10_32(), "#10-32 tap drill hole diameter (GOAT reference)"),
            makeVariable("GOAT_clearance_hole_10_32", clearance_hole_10_32(), "#10-32 clearance hole diameter (GOAT reference)"),
            makeVariable("GOAT_Standard_bearing_1_125", Standard_bearing_1_125(), "Standard 1.125 inch bearing dimension (GOAT reference)"),
            makeVariable("GOAT_M5_Nut_Distance", M5_Nut_Distance(), "M5 nut distance (GOAT reference)"),
            makeVariable("GOAT_M5_Socket_Head", M5_Socket_Head(), "M5 socket head cap screw diameter (GOAT reference)"),
            // Fabrication
            makeVariable("GOAT_Plate_Chamfer", Plate_Chamfer(), "Plate edge chamfer size (GOAT reference)"),
            makeVariable("GOAT_Shaft_End_Chamfer", Shaft_End_Chamfer(), "Shaft end chamfer size (GOAT reference)"),
            makeVariable("GOAT_Shaft_TapHole_Depth_M5", Shaft_TapHole_Depth_M5(), "M5 tap hole depth in shaft (GOAT reference)"),
            makeVariable("GOAT_Spacer_M4_OD", Spacer_M4_OD(), "M4 spacer outer diameter (GOAT reference)"),
            makeVariable("GOAT_Spacer_M5_OD", Spacer_M5_OD(), "M5 spacer outer diameter (GOAT reference)"),
            // Magic Numbers
            makeVariable("GOAT_Offset_3DPrint", Offset_3DPrint(), "3D printing tolerance offset (GOAT reference)"),
            makeVariable("GOAT_Offset_CTC", Offset_CTC(), "Belt center-to-center offset (GOAT reference)")
        ];
            setVariables(context, GOATReferenceVariables);
        }
        
        setFeatureComputedParameter(context, id, {
                    "name" : "_name",
                    "value" : "GOAT_OriginCube"
                });
    }, { "importVariables" : true });

//Helper Functions
function makeVariable(name is string, value, description is string) {
    return { "name" : name, "value" : value, "description" : description };
}

function setVariables(context is Context, variableList is array) {
    mapArray(variableList, 
        function(variable is map) { 
            setVariable(context, variable.name, variable.value, variable.description); }
    );
}
