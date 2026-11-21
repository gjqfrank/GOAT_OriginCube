FeatureScript 1913;
import(path : "onshape/std/geometry.fs", version : "1913.0");
icon::import(path : "6e2b0111d19a599ad1b3bb92", version : "a7e29144686ac71ac3887543");


export import(path : "25c4723618104588db858c18", version : "4859e21b6c223f659a76a785");
shapes::import(path : "371db2f4cb77050e04c9f19b", version : "5c2e41afc4f19c6d82a1bb92");

export enum OriginType
{
    annotation { "Name" : "Cube" }
    Cube,
    annotation { "Name" : "Sphere" }
    Sphere,
    // annotation { "Name" : "Pyramid"}
    // Pyramid,
    annotation { "Name" : "Cat"}
    Cat,
    annotation { "Name" : "None" }
    None
}

export enum GroundLocation
{
    annotation { "Name" : "Top" }
    Top,
    annotation { "Name" : "Middle" }
    Middle,
    annotation { "Name" : "Bottom" }
    Bottom
}

annotation { "Feature Type Name" : "Origin Cube", "Icon" : icon::BLOB_DATA, "Feature Name Template" : "#_name", 

    "Feature Type Description": "Places an Origin Cube in a part studio. Also includes optional design functions (C-C Calculators, etc), constants (bolt hole sizes, common FRC dimensions, etc) imports. Read more on https://www.frcdesign.org/. Featurescript developed by Jonathan Mi with assets used from Alex Kempen, Eliot D., and TLamp"}


export const myFeature = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        // Select origin shape
        annotation { "Name" : "Origin Object", "Default" : OriginType.Cube, "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
        definition.shape is OriginType;
        
        if(definition.shape != OriginType.None)
        {
            annotation { "Name" : "Origin Location", "Default" : GroundLocation.Top, "UIHint": [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
            definition.gLoc is GroundLocation;
        }
        
        // Select which functions to import
        
         annotation { "Name" : "Enable Mate Connector", "Default" : true, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
        definition.MCEnabled is boolean;
        
        annotation { "Name" : "All functions", "Default" : true, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
        definition.AllFunctions is boolean;
            
        annotation { "Group Name" : "Functions", "Collapsed By Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE], "Driving Parameter" : "AllFunctions"}
        {
            if (definition.AllFunctions != true)
            {
                annotation { "Name" : "3mm Belts ", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.BELT3 is boolean;
                
                annotation { "Name" : "5mm Belts", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.BELT5 is boolean;
                
                annotation { "Name" : "RT25 Belts", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.BELTRT25 is boolean;
                
                annotation { "Name" : "#25 Chain", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.CHAIN25 is boolean;
                
                annotation { "Name" : "#35 Chain", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.CHAIN35 is boolean;
                
                annotation { "Name" : "20DP Gears", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.GEAR20 is boolean;
                
                annotation { "Name" : "32DP Gears", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.GEAR32 is boolean;
            }
        }
        
        // Select which variables to import
        annotation { "Name" : "All variables", "Default" : true, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
        definition.AllVariables is boolean;
        
        annotation { "Group Name" : "Variables", "Collapsed By Default" : false, "Driving Parameter" : "AllVariables", "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE]}
        {
            if (definition.AllVariables != true)
            {
                annotation { "Name" : "Standard bolt fits", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.BoltVariables is boolean;
                
                annotation { "Name" : "Heatset inserts", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.HeatsetInserts is boolean;
                
                annotation { "Name" : "FRC related", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.FRCRelated is boolean;
            }
        }
        
        // Select which variables to import
        
        annotation { "Name" : "Advanced options", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
        definition.advOptions is boolean;
        
        if (definition.advOptions)
        {
            annotation { "Group Name" : "Advanced options", "Collapsed By Default" : false, "Driving Parameter" : "advOptions", "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE]}
            {
                annotation { "Name" : "Override feature lockout", "Default" : false, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                definition.locationOverride is boolean;
                
                annotation { "Name" : "Origin Shape Size", "Default" : 2.0*inch, "UIHint": [UIHint.REMEMBER_PREVIOUS_VALUE] }
                isLength(definition.shapeSize, { (inch) : [0.01, 2, 10000] } as LengthBoundSpec);
                
            }
        }
        
    }
    
    {
        if (!definition.advOptions)
        {
            definition.shapeSize = 2 * inch;
            definition.locationOverride = false;
        }
        //transform the shape
        const zTrans = {
            GroundLocation.Top : - (definition.shapeSize/2)/inch,
            GroundLocation.Middle : 0,
            GroundLocation.Bottom : (definition.shapeSize/2)/inch
        };
        
        const isPartStudioEmpty = isQueryEmpty(context, qModifiableEntityFilter(qEverything(EntityType.BODY)->qSubtraction(qDefaultBodies())));
        
        if (!isPartStudioEmpty && !definition.locationOverride)
        {
            reportFeatureWarning(context, id, "Origin Cube should be the first feature in your part studio to function correctly. Place this feature to the top of your feature tree by using the rollback bar or dragging it. If you know what you're doing, you can override this error by enabling \"Override Feature Lockout\"");
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Begin Origin Cube
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        if(definition.shape != OriginType.None) //if placing a shape at the origin
        {
            cube(context, id + "seed", {
                    "sideLength" : 0.001 * inch
            });
            var originCube = qCreatedBy(id + "seed", EntityType.BODY);
            
            //import the actual shape
            var instantiator = newInstantiator(id + "instantiate");
            var shape = addInstance(instantiator, shapes::build, {
                    "configuration" : { "ShapeType" : toString(definition.shape), "size" : definition.shapeSize},
                    "transform"     : identityTransform()
                });
            instantiate(context, instantiator);
            //strip shape of mate connectors
            opDeleteBodies(context, id + "deleteBodies1", {
                    "entities" : qBodyType(shape, BodyType.MATE_CONNECTOR)
            });
            //combine the seed components
            opBoolean(context, id + "boolean1", {
                    "tools" : qUnion([originCube, qBodyType(shape, BodyType.SOLID)]),
                    "operationType" : BooleanOperationType.UNION
            });

            opTransform(context, id + "transform1", {
                    "bodies" : originCube,
                    "transform" : transform(vector(0, 0, zTrans[definition.gLoc]) * inch)
            });
            
            //set name, color, and appearance
            setProperty(context, {
                "entities" : originCube,
                "propertyType" : PropertyType.NAME,
                "value" : "Origin " ~ toString(definition.shape)
            });
            setProperty(context, {
                "entities" : originCube,
                "propertyType" : PropertyType.APPEARANCE,
                "value" : color(0.5, 0.5, 0.5, 0.25)
            });
            // get rid of missing material error
            setProperty(context, {
                "entities" : originCube,
                "propertyType" : PropertyType.MATERIAL,
                "value" :  material("Origin Cube Material", 0.00001 * gram / centimeter ^ 3)
            });
            setProperty(context, {
                "entities" : originCube,
                "propertyType" : PropertyType.EXCLUDE_FROM_BOM,
                "value" :  true
            });
            
            if(definition.MCEnabled){
                //create mate connector at origin
                opMateConnector(context, id + "mateConnector1", {
                        "coordSystem" : WORLD_COORD_SYSTEM,
                        "owner" : originCube
                });
            }
        }       
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // End Origin Cube
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Begin Function Libraries
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        if (definition.AllFunctions || definition.BELT3)
        {
            getFunctionLibraries(context, FunctionLibraryType.BELT3, functionLibraries);
        }

        if (definition.AllFunctions || definition.BELT5)
        {
            getFunctionLibraries(context, FunctionLibraryType.BELT5, functionLibraries);
        }
        
        if (definition.AllFunctions || definition.BELTRT25)
        {
            getFunctionLibraries(context, FunctionLibraryType.BELTRT25, functionLibraries);
        }

        if (definition.AllFunctions || definition.CHAIN25)
        {
            getFunctionLibraries(context, FunctionLibraryType.CHAIN25, functionLibraries);
            setVariable(context, "SprocketOD_25_Offset", (0.25/2)*inch, "Radius Offset for #25 Sprocket OD from PD");
        }

        if (definition.AllFunctions || definition.CHAIN35)
        {
            getFunctionLibraries(context, FunctionLibraryType.CHAIN35, functionLibraries);
            setVariable(context, "SprocketOD_35_Offset", (0.375/2)*inch, "Radius Offset for #35 Sprocket OD from PD");
        }

        if (definition.AllFunctions || definition.GEAR20)
        {
            // getFunctionLibraries(context, FunctionLibraryType.GEAR20, functionLibraries);
            setVariable(context, "GearOD_20DP_Offset", (0.1/2)*inch, "Radius Offset for 20DP Gear OD from PD");
        }

        if (definition.AllFunctions || definition.GEAR32)
        {
            // getFunctionLibraries(context, FunctionLibraryType.GEAR32, functionLibraries);
            setVariable(context, "GearOD_32DP_Offset", (0.0625/2)*inch, "Radius Offset for 32DP Gear OD from PD");
            
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // End Function Libraries
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Begin Variable Import
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        ////////FRC Related
        const FRCRelated = [
            makeVariable("MK4i_Chassis2Ground_Clearance", 1.75*inch, "Ground Clearance Offset for MK4i Modules (With Standard Mounting Style)"),
            makeVariable("MK4i_TubeOffset", 4.25*inch, "Tube length offset for MK4i Modules")
        ];
        
        ////////Bolt Variables
        const BoltVariables = [
            // Imperial Tap Sizes
            makeVariable("TapSize_0_80", 0.0469*inch, "#0-80 Tap Size Hole Diameter"),
            makeVariable("TapSize_1_72", 0.0595*inch, "#1-72 Tap Size Hole Diameter"),
            makeVariable("TapSize_2_56", 0.0700*inch, "#2-56 Tap Size Hole Diameter"),
            makeVariable("TapSize_3_48", 0.0790*inch, "#3-48 Tap Size Hole Diameter"),
            makeVariable("TapSize_4_40", 0.0890*inch, "#4-40 Tap Size Hole Diameter"),
            makeVariable("TapSize_5_40", 0.102*inch, "#5-40 Tap Size Hole Diameter"),
            makeVariable("TapSize_6_32", 0.1065*inch, "#6-32 Tap Size Hole Diameter"),
            makeVariable("TapSize_8_32", 0.1360*inch, "#8-32 Tap Size Hole Diameter"),
            makeVariable("TapSize_10_24", 0.150*inch, "#10-24 Tap Size Hole Diameter"),
            makeVariable("TapSize_10_32", 0.1590*inch, "#10-32 Tap Size Hole Diameter"),
            makeVariable("TapSize_1d4_20", 0.2010*inch, "1/4\"-20 Tap Size Hole Diameter"),
            makeVariable("TapSize_1d4_28", 0.2130*inch, "1/4\"-28 Tap Size Hole Diameter"),
            makeVariable("TapSize_5d16_18", 0.2570*inch, "5/16\"-18 Tap Size Hole Diameter"),
            makeVariable("TapSize_3d8_16", 0.3125*inch, "3/8\"-16 Tap Size Hole Diameter"),
            makeVariable("TapSize_1d2_13", 0.4219*inch, "1/2\"-13 Tap Size Hole Diameter"),
            
            // Metric Tap Sizes
            makeVariable("TapSize_M1p5", 1.25*millimeter, "M1.5 Tap Size Hole Diameter"),
            makeVariable("TapSize_M1p6", 1.35*millimeter, "M1.6 Tap Size Hole Diameter"),
            makeVariable("TapSize_M1p8", 1.55*millimeter, "M1.8 Tap Size Hole Diameter"),
            makeVariable("TapSize_M2", 1.75*millimeter, "M2 Tap Size Hole Diameter"),
            makeVariable("TapSize_M2p2", 1.9*millimeter, "M2.2 Tap Size Hole Diameter"),
            makeVariable("TapSize_M2p5", 2.2*millimeter, "M2.5 Tap Size Hole Diameter"),
            makeVariable("TapSize_M3", 2.7*millimeter, "M3 Tap Size Hole Diameter"),
            makeVariable("TapSize_M3p5", 3.1*millimeter, "M3.5 Tap Size Hole Diameter"),
            makeVariable("TapSize_M4", 3.5*millimeter, "M4 Tap Size Hole Diameter"),
            makeVariable("TapSize_M4p5", 4*millimeter, "M4.5 Tap Size Hole Diameter"),
            makeVariable("TapSize_M5", 4.5*millimeter, "M5 Tap Size Hole Diameter"),
            makeVariable("TapSize_M6", 5.4*millimeter, "M6 Tap Size Hole Diameter"),
            makeVariable("TapSize_M7", 6.4*millimeter, "M7 Tap Size Hole Diameter"),
            makeVariable("TapSize_M85", 7.2*millimeter, "M8.5 Tap Size Hole Diameter"),
            makeVariable("TapSize_M9", 8.20*millimeter, "M9 Tap Size Hole Diameter"),
            makeVariable("TapSize_M10", 9*millimeter, "M10 Tap Size Hole Diameter"),
            
            // Imperial Close Fit
            makeVariable("CloseFit_0_80", 0.0635*inch, "#0-80 Close Fit Hole Diameter"),
            makeVariable("CloseFit_1_72", 0.0760*inch, "#1-72 Close Fit Hole Diameter"),
            makeVariable("CloseFit_2_56", 0.0890*inch, "#2-56 Close Fit Hole Diameter"),
            makeVariable("CloseFit_3_48", 0.1040*inch, "#3-48 Close Fit Hole Diameter"),
            makeVariable("CloseFit_4_40", 0.1160*inch, "#4-40 Close Fit Hole Diameter"),
            makeVariable("CloseFit_5_40", 0.1360*inch, "#5-40 Close Fit Hole Diameter"),
            makeVariable("CloseFit_6_32", 0.1440*inch, "#6-32 Close Fit Hole Diameter"),
            makeVariable("CloseFit_8_32", 0.1695*inch, "#8-32 Close Fit Hole Diameter"),
            makeVariable("CloseFit_10_24", 0.196*inch, "#10-24 Close Fit Hole Diameter"),
            makeVariable("CloseFit_10_32", 0.1960*inch, "#10-32 Close Fit Hole Diameter"),
            makeVariable("CloseFit_1d4_20", 0.2570*inch, "1/4\"-20 Close Fit Hole Diameter"),
            makeVariable("CloseFit_1d4_28", 0.2570*inch, "1/4\"-28 Close Fit Hole Diameter"),
            makeVariable("CloseFit_5d16_18", 0.3230*inch, "5/16\"-18 Close Fit Hole Diameter"),
            makeVariable("CloseFit_3d8_16", 0.3860*inch, "3/8\"-16 Close Fit Hole Diameter"),
            makeVariable("CloseFit_1d2_13", 0.5156*inch, "1/2\"-13 Close Fit Hole Diameter"),
            
            // Metric Close Fit
            makeVariable("CloseFit_M1p5", 1.6*millimeter, "M1.5 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M1p6", 1.70*millimeter, "M1.6 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M1p8", 1.90*millimeter, "M1.8 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M2", 2.1*millimeter, "M2 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M2p2", 2.3*millimeter, "M2.2 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M2p5", 2.65*millimeter, "M2.5 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M3", 3.15*millimeter, "M3 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M3p5", 3.7*millimeter, "M3.5 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M4", 4.2*millimeter, "M4 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M4p5", 4.75*millimeter, "M4.5 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M5", 5.25*millimeter, "M5 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M6", 6.3*millimeter, "M6 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M7", 7.4*millimeter, "M7 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M8", 8.4*millimeter, "M8 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M9", 9.5*millimeter, "M9 Close Fit Hole Diameter"),
            makeVariable("CloseFit_M10", 10.5*millimeter, "M10 Close Fit Hole Diameter"),
            
            //Imperial Clearance Fit
            makeVariable("ClearanceFit_0_80", 0.0700*inch, "#0-80 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_1_72", 0.0810*inch, "#1-72 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_2_56", 0.0960*inch, "#2-56 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_3_48", 0.1065*inch, "#3-48 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_4_40", 0.1285*inch, "#4-40 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_5_40", 0.1405*inch, "#5-40 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_6_32", 0.1495*inch, "#6-32 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_8_32", 0.1770*inch, "#8-32 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_10_24", 0.2010*inch, "#10-24 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_10_32", 0.2010*inch, "#10-32 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_1d4_20", 0.2660*inch, "1/4\"-20 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_1d4_28", 0.2660*inch, "1/4\"-28 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_5d16_18", 0.3320*inch, "5/16\"-18 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_3d8_16", 0.3970*inch, "3/8\"-16 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_1d2_13", 0.5312*inch, "1/2\"-13 Clearance Fit Hole Diameter"),
            
            // Metric Clearance Fit
            makeVariable("ClearanceFit_M1p5", 1.65*millimeter, "M1.5 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M1p6", 1.75*millimeter, "M1.6 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M1p8", 2*millimeter, "M1.8 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M2", 2.2*millimeter, "M2 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M2p2", 2.4*millimeter, "M2.2 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M2p5", 2.75*millimeter, "M2.5 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M3", 3.30*millimeter, "M3 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M3p5", 3.85*millimeter, "M3.5 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M4", 4.50*millimeter, "M4 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M4p5", 5*millimeter, "M4.5 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M5", 5.25*millimeter, "M5 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M6", 6.6*millimeter, "M6 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M7", 7.7*millimeter, "M7 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M8", 8.8*millimeter, "M8 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M9", 9.9*millimeter, "M9 Clearance Fit Hole Diameter"),
            makeVariable("ClearanceFit_M10", 11*millimeter, "M10 Clearance Fit Hole Diameter")
        ];
        
        const HeatsetInserts = [
            //imperial
            makeVariable("HSI_2", 0.1285*inch, "#2 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_4", 0.1590*inch, "#4 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_6", 0.1910*inch, "#6 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_8", 0.2210*inch, "#8 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_10", 0.2570*inch, "#10 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_1d4", 0.3160*inch, "1/4\" Heat Set Insert Hole Diameter"),
            //metric
            makeVariable("HSI_M2", 3.3*millimeter, "M2 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_M2p5", 3.9*millimeter, "M2.5 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_M3", 4*millimeter, "M3 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_M3p5", 4.9*millimeter, "M3.5 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_M4", 5.6*millimeter, "M4 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_M5", 6.5*millimeter, "M5 Heat Set Insert Hole Diameter"),
            makeVariable("HSI_M6", 8*millimeter, "M6 Heat Set Insert Hole Diameter")
        ];
                
        if (definition.AllVariables || definition.FRCRelated)
        {
            setVariables(context, FRCRelated);
        }
        
        if (definition.AllVariables || definition.BoltVariables)
        {
            setVariables(context, BoltVariables);
        }
        
        if (definition.AllVariables || definition.HeatsetInserts)
        {
            setVariables(context, HeatsetInserts);
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // End Variable Import
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        setFeatureComputedParameter(context, id, {
                    "name" : "_name",
                    "value" : "Origin Cube"
                });
    
        
    }); //End of Feature
    
    
//Helper Functions
function getFunctionLibraries(context is Context, functionLibraryDef is string, functionLibraries is array)
{
    var libraryName = functionLibraryDef as string;

    if (libraryName == "NONE")
    {
        return;
    }

    var librariesToAdd = [libraryName];

    for (var functionLibrary in functionLibraries)
    {
        if (functionLibrary.libraryName == libraryName)
        {
            librariesToAdd = concatenateArrays([librariesToAdd, functionLibrary.includedLibraries]);
            break;
        }
    }

    for (var functionLibrary in functionLibraries)
    {
        if (isIn(functionLibrary.libraryName, librariesToAdd))
        {
            for (var variableFunction in functionLibrary.libraryFunctions)
            {
                setVariable(context, variableFunction.name, variableFunction.value, variableFunction.definition);
            }
        }
    }
}

function makeVariable(name is string, value, description is string) {
    return { "name" : name, "value" : value, "description" : description };
}

function setVariables(context is Context, variableList is array) {
    mapArray(variableList, 
        function(variable is map) { 
            setVariable(context, variable.name, variable.value, variable.description); }
    );
}

