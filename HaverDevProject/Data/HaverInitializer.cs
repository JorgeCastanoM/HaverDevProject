using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using HaverDevProject.Models;
using NuGet.DependencyResolver;
using Microsoft.CodeAnalysis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HaverDevProject.Data
{
    public static class HaverInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            HaverNiagaraContext context = applicationBuilder.ApplicationServices.CreateScope()
            .ServiceProvider.GetRequiredService<HaverNiagaraContext>();

            try
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                //context.Database.Migrate();


                if (!context.Suppliers.Any())
                {
                    context.Suppliers.AddRange(

                        new Supplier
                        {
                            SupplierCode = "793254",
                            SupplierName = "INTEGRITY WOVEN WIRE",
                            SupplierEmail = "integritywire@integrity.com",
                            SupplierStatus = false
                        },
                        new Supplier
                        {
                            SupplierCode = "792356",
                            SupplierName = "FLO COMPONENTS LTD.",
                            SupplierEmail = "flocompltd@flo.com",
                            SupplierStatus = false
                        },
                        new Supplier
                        {
                            SupplierCode = "700009",
                            SupplierName = "AJAX TOCCO",
                            SupplierEmail = "ajaxtocco@ajax.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "700013",
                            SupplierName = "HINGSTON METAL FABRICATORS",
                            SupplierEmail = "hingstonmetal@hingston.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "700027",
                            SupplierName = "HOTZ ENVIRONMENTAL SERVICES",
                            SupplierEmail = "hotzservices@hotz.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "700044",
                            SupplierName = "BLACK CREEK METAL",
                            SupplierEmail = "blackcreekmetal@blackcreek.com",
                            SupplierStatus = false
                        },
                        new Supplier
                        {
                            SupplierCode = "700045",
                            SupplierName = "POLYMER EXTRUSIONS INC",
                            SupplierEmail = "lsm@polymerextrusions.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "700087",
                            SupplierName = "DON CASSELMAN & SON LTD",
                            SupplierEmail = "fastenal@casselman.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "880006",
                            SupplierName = "W S TYLER - PARTICLE & FINE",
                            SupplierEmail = "wstyler@wstyler.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "790891",
                            SupplierName = "LAWRENCE SINTERED METALS",
                            SupplierEmail = "lsm@olawrence.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "700493",
                            SupplierName = "FASTENAL COMPANY",
                            SupplierEmail = "fastenalcompany@fastenal.com"
                        },

                        new Supplier
                        {
                            SupplierCode = "880065",
                            SupplierName = "HBC ENGINEERING",
                            SupplierEmail = "hbcengineering@hbc.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "700502",
                            SupplierName = "ST CATHARINES PATTERN LTD",
                            SupplierEmail = "stcathpattern@patternltd.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "700505",
                            SupplierName = "NIAGARA PRECISION LTD",
                            SupplierEmail = "niagaraprecision@niagaraprecision.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "700508",
                            SupplierName = "BORDER CITY CASTINGS",
                            SupplierEmail = "bordercitycastings@bordercity.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "888888",
                            SupplierName = "HBC PROCUREMENT",
                            SupplierEmail = "hbcprocurement@hbc.com",
                        },
                        new Supplier
                        {
                            SupplierCode = "792679",
                            SupplierName = "IFM EFFECTOR CANADA INC.",
                            SupplierEmail = "ifmeffector@ifm.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "792565",
                            SupplierName = "PLAS-TECH DESIGN FABRICATION DISTRI",
                            SupplierEmail = "plastechdesign@plastech.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "792493",
                            SupplierName = "SGF-SUDDEUTSCHE GELENKSCHEIBENFABRI",
                            SupplierEmail = "sgfsudd@sgf.com"
                        },
                        new Supplier
                        {
                            SupplierCode = "792011",
                            SupplierName = "VANDER WEYDEN CONSTRUCTION",
                            SupplierEmail = "vanderweydenconstruction@vander.com"
                        });

                    context.SaveChanges();
                }

                if (!context.Items.Any())
                {
                    context.Items.AddRange(
                        new Item
                        {
                            ItemNumber = 10342455,
                            ItemName = "Bearing Housing"                           
                        },
                        new Item
                        {
                            ItemNumber = 10482834,
                            ItemName = "Backing Shield"},
                        new Item
                        {
                            ItemNumber = 11536261,
                            ItemName = "Side Arm"},
                        new Item
                        {
                            ItemNumber = 11854290,
                            ItemName = "Panel"},
                        new Item
                        {
                            ItemNumber = 10344215,
                            ItemName = "Jig Washer"},
                        new Item
                        {
                            ItemNumber = 10482863,
                            ItemName = "Screen"},
                        new Item
                        {
                            ItemNumber = 11536287,
                            ItemName = "Conveyor"},
                        new Item
                        {
                            ItemNumber = 11854266,
                            ItemName = "Rotary Kiln"},
                        new Item
                        {
                            ItemNumber = 10344216,
                            ItemName = "Liner"},
                        new Item
                        {
                            ItemNumber = 10482864,
                            ItemName = "Spherical Roller Bearing"},
                        new Item
                        {
                            ItemNumber = 11536288,
                            ItemName = "Cyconical Bore"},
                        new Item
                        {
                            ItemNumber = 11854267,
                            ItemName = "Spherical Roller Washer"},
                        new Item
                        {
                            ItemNumber = 20158246,
                            ItemName = "Hydraulic Excavator Hammer"
                        },
                        new Item
                        {
                            ItemNumber = 20248765,
                            ItemName = "Telescopic Boom Lift"
                        },
                        new Item
                        {
                            ItemNumber = 20395876,
                            ItemName = "Reinforced Concrete Pipe"
                        },
                        new Item
                        {
                            ItemNumber = 20485792,
                            ItemName = "Structural Steel I-Beam"
                        },
                        new Item
                        {
                            ItemNumber = 20576849,
                            ItemName = "Crawler Crane"
                        },
                        new Item
                        {
                            ItemNumber = 20687954,
                            ItemName = "Tower Crane"
                        },
                        new Item
                        {
                            ItemNumber = 20796583,
                            ItemName = "Skid-Steer Loader"
                        },
                        new Item
                        {
                            ItemNumber = 20875693,
                            ItemName = "Trencher Machine"
                        },
                        new Item
                        {
                            ItemNumber = 20986742,
                            ItemName = "Asphalt Paver"
                        },
                        new Item
                        {
                            ItemNumber = 21097851,
                            ItemName = "Vibratory Soil Compactor"
                        });

                    context.SaveChanges();
                }

                if (!context.Defects.Any())
                {
                    context.Defects.AddRange(
                        new Defect
                        {
                            DefectName = "Design Error(Drawing)"
                        },
                        new Defect
                        {
                            DefectName = "Poor Paint finish"
                        }, 
                        new Defect
                        {
                            DefectName = "Poor quality surface finish"
                        },
                        new Defect
                        {
                            DefectName = "Poor Weld quality"
                        },
                        new Defect
                        {
                            DefectName = "Missing Items"
                        },
                        new Defect
                        {
                            DefectName = "Broken / Twisted Wires"
                        },
                        new Defect
                        {
                            DefectName = "Out of Crimp"
                        },
                        new Defect
                        {
                            DefectName = "Incorrect Center Hole Punching"
                        },
                        new Defect
                        {
                            DefectName = "Incorrect hardware"
                        },
                        new Defect
                        {
                            DefectName = "Delivery quality"
                        },
                        new Defect
                        {
                            DefectName = "Incorrect specification"
                        },
                        new Defect
                        {
                            DefectName = "Incorrect dimensions"
                        });

                    context.SaveChanges();
                }                                            

                if (!context.Ncrs.Any())
                {
                    context.Ncrs.AddRange(
                        new Ncr
                        {
                            NcrNumber = "2023-137",
                            NcrLastUpdated = DateTime.Parse("2023-12-18"),   
                            NcrStatus = false,
                            NcrPhase = NcrPhase.Closed
                        },
                        new Ncr
                        {
                            NcrNumber = "2023-138",
                            NcrLastUpdated = DateTime.Parse("2023-12-19"),
                            NcrStatus = true,
                            NcrPhase = NcrPhase.ReInspection
                            
                        },
                        new Ncr
                        {
                            NcrNumber = "2023-139",
                            NcrLastUpdated = DateTime.Parse("2023-12-22"),
                            NcrStatus = false,
                            NcrPhase = NcrPhase.Closed
                        },
                        new Ncr
                        {
                            NcrNumber = "2023-140",
                            NcrLastUpdated = DateTime.Parse("2024-01-18"),
                            NcrStatus = true,
                            NcrPhase = NcrPhase.ReInspection
                        },
                        new Ncr
                        {
                            NcrNumber = "2023-141",
                            NcrLastUpdated = DateTime.Parse("2024-01-14"),
                            NcrStatus = false,
                            NcrPhase = NcrPhase.Closed
                        },
                        new Ncr
                        {
                            NcrNumber = "2024-001",
                            NcrLastUpdated = DateTime.Parse("2024-01-10"),
                            NcrStatus = true,
                            NcrPhase = NcrPhase.Procurement
                        },
                        new Ncr
                        {
                            NcrNumber = "2024-002",
                            NcrLastUpdated = DateTime.Parse("2024-01-11"),
                            NcrStatus = true,
                            NcrPhase = NcrPhase.Procurement
                        },
                        new Ncr
                        {
                            NcrNumber = "2024-003",
                            NcrLastUpdated = DateTime.Parse("2024-01-15"),
                            NcrStatus = true,
                            NcrPhase = NcrPhase.Operations
                        },
                        new Ncr
                        {
                            NcrNumber = "2024-004",
                            NcrLastUpdated = DateTime.Parse("2024-01-19"),
                            NcrStatus = true,
                            NcrPhase = NcrPhase.Operations
                        },
                        new Ncr
                        {
                            NcrNumber = "2024-005",
                            NcrLastUpdated = DateTime.Parse("2024-01-22"),
                            NcrStatus = true,
                            NcrPhase = NcrPhase.Engineer
                        },
                        new Ncr
                        {
                            NcrNumber = "2024-006",
                            NcrLastUpdated = DateTime.Parse("2024-01-23"),
                            NcrStatus = false,
                            NcrPhase = NcrPhase.Closed
                        },
                        new Ncr
                        {
                            NcrNumber = "2024-007",
                            NcrLastUpdated = DateTime.Parse("2024-01-23"),
                            NcrStatus = true,
                            NcrPhase = NcrPhase.Engineer
                        });
                    context.SaveChanges();
                }

                if (!context.NcrQas.Any())
                {
                    context.NcrQas.AddRange(
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401227",
                            NcrQacreationDate = DateTime.Parse("2018-10-07"), 
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-137").NcrId,
                            NcrQaProcessApplicable = false,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Bearing Housing").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Delivery quality").DefectId,
                            SupplierId = 1,
                            NcrQaOrderNumber = "4500695162",
                            NcrQaQuanReceived = 10,
                            NcrQaQuanDefective = 8,
                            NcrQaDescriptionOfDefect = "quality of item was not up to specifications",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401228",
                            NcrQacreationDate = DateTime.Parse("2022-11-09"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-138").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Bearing Housing").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect dimensions").DefectId,
                            SupplierId = 3,
                            NcrQaOrderNumber = "4500695429",
                            NcrQaQuanReceived = 5,
                            NcrQaQuanDefective = 5,
                            NcrQaDescriptionOfDefect = "The inner race has not the right size",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401229",
                            NcrQacreationDate = DateTime.Parse("2020-11-11"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-139").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Bearing Housing").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect dimensions").DefectId,
                            SupplierId = 1,
                            NcrQaProcessApplicable = false,
                            NcrQaOrderNumber = "4500684525",
                            NcrQaQuanReceived = 12,
                            NcrQaQuanDefective = 3,
                            NcrQaDescriptionOfDefect = "The outer race has not the right size",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401230",
                            NcrQacreationDate = DateTime.Parse("2023-12-13"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-140").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Backing Shield").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect hardware").DefectId,
                            SupplierId = 5,
                            NcrQaOrderNumber = "4500683983",
                            NcrQaQuanReceived = 28,
                            NcrQaQuanDefective = 14,
                            NcrQaDescriptionOfDefect = "Missing installation coils",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401231",
                            NcrQacreationDate = DateTime.Parse("2023-12-17"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-141").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Side Arm").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect specification").DefectId,
                            SupplierId = 4,
                            NcrQaOrderNumber = "4500694121",
                            NcrQaQuanReceived = 2,
                            NcrQaQuanDefective = 2,
                            NcrQaDescriptionOfDefect = "Item is too big, not the right size",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401232",
                            NcrQacreationDate = DateTime.Parse("2024-01-03"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-001").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Backing Shield").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect hardware").DefectId,
                            SupplierId = 3,
                            NcrQaProcessApplicable = false,
                            NcrQaOrderNumber = "4500681790",
                            NcrQaQuanReceived = 1,
                            NcrQaQuanDefective = 1,
                            NcrQaDescriptionOfDefect = "Missing necessary bolts",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401233",
                            NcrQacreationDate = DateTime.Parse("2024-01-04"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-002").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Bearing Housing").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Delivery quality").DefectId,
                            SupplierId = 1,
                            NcrQaOrderNumber = "4500671162",
                            NcrQaQuanReceived = 9,
                            NcrQaQuanDefective = 8,
                            NcrQaDescriptionOfDefect = "Quality of item is not acceptable",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401234",
                            NcrQacreationDate = DateTime.Parse("2024-02-06"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-003").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Side Arm").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect specification").DefectId,
                            SupplierId = 1,
                            NcrQaOrderNumber = "4500685546",
                            NcrQaQuanReceived = 4,
                            NcrQaQuanDefective = 1,
                            NcrQaDescriptionOfDefect = "Not the right size, too big",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401235",
                            NcrQacreationDate = DateTime.Parse("2024-02-07"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-004").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Side Arm").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect specification").DefectId,
                            SupplierId = 5,
                            NcrQaOrderNumber = "4500683210",
                            NcrQaQuanReceived = 15,
                            NcrQaQuanDefective = 10,
                            NcrQaDescriptionOfDefect = "Not the right size, too big",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401236",
                            NcrQacreationDate = DateTime.Parse("2024-03-11"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-005").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Bearing Housing").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Delivery quality").DefectId,
                            SupplierId = 4,
                            NcrQaProcessApplicable = false,
                            NcrQaOrderNumber = "4500700595",
                            NcrQaQuanReceived = 17,
                            NcrQaQuanDefective = 6,
                            NcrQaDescriptionOfDefect = "Quality of item is not acceptable",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401681",
                            NcrQacreationDate = DateTime.Parse("2024-03-14"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-006").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Side Arm").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect specification").DefectId,
                            SupplierId = 3,
                            NcrQaOrderNumber = "4500695645",
                            NcrQaQuanReceived = 12,
                            NcrQaQuanDefective = 2,
                            NcrQaDescriptionOfDefect = "Quality of item is not acceptable",
                            NcrQaEngDispositionRequired = true
                        },
                        new NcrQa
                        {
                            NcrQaItemMarNonConforming = true,
                            NcrQaSalesOrder = "10401682",
                            NcrQacreationDate = DateTime.Parse("2024-03-16"),
                            NcrQaUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-007").NcrId,
                            ItemId = context.Items.FirstOrDefault(f => f.ItemName == "Backing Shield").ItemId,
                            DefectId = context.Defects.FirstOrDefault(f => f.DefectName == "Incorrect hardware").DefectId,
                            SupplierId = 4,
                            NcrQaOrderNumber = "4500691574",
                            NcrQaQuanReceived = 24,
                            NcrQaQuanDefective = 6,
                            NcrQaDescriptionOfDefect = "Missing three bolts",
                            NcrQaEngDispositionRequired = true
                        });
                    context.SaveChanges();
                }                

                if (!context.EngDispositionTypes.Any())
                {
                    context.EngDispositionTypes.AddRange(
                        new EngDispositionType
                        {
                            EngDispositionTypeName = "Use As Is"
                        },
                        new EngDispositionType
                        {
                            EngDispositionTypeName = "Repair"
                        }, new EngDispositionType
                        {
                            EngDispositionTypeName = "Rework"
                        },
                        new EngDispositionType
                        {
                            EngDispositionTypeName = "Scrap"
                        });

                    context.SaveChanges();
                }

                if (!context.NcrEngs.Any())
                {
                    context.NcrEngs.AddRange(
                        new NcrEng
                        {
                            NcrEngCustomerNotification = false,
                            NcrEngDispositionDescription = "N/A",
                            NcrEngUserId = "2",
                            NcrEngCreationDate = DateTime.Parse("2023-12-18"),
                            NcrEngCompleteDate = DateTime.Parse("2023-12-18"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Use As Is").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-137").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = true,
                            NcrEngDispositionDescription = "Item will be repaired in shop",
                            NcrEngUserId = "2",
                            NcrEngCreationDate = DateTime.Parse("2023-12-19"),
                            NcrEngCompleteDate = DateTime.Parse("2023-12-19"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Repair").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-138").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = true,
                            NcrEngDispositionDescription = "item will be reprocessed to ensure compliance of the product with applicable specifications",
                            NcrEngUserId = "2",
                            NcrEngCreationDate = DateTime.Parse("2023-12-23"),
                            NcrEngCompleteDate = DateTime.Parse("2023-12-23"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Rework").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-139").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = true,
                            NcrEngDispositionDescription = "N/A",
                            NcrEngUserId = "2",
                            NcrEngCreationDate = DateTime.Parse("2024-01-18"),
                            NcrEngCompleteDate = DateTime.Parse("2024-01-18"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Scrap").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-140").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = false,
                            NcrEngDispositionDescription = "N/A",
                            NcrEngUserId = "2",
                            NcrEngCreationDate = DateTime.Parse("2024-01-14"),
                            NcrEngCompleteDate = DateTime.Parse("2024-01-14"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Use As Is").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-141").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = true,
                            NcrEngDispositionDescription = "N/A",
                            NcrEngUserId = "2", 
                            NcrEngCreationDate = DateTime.Parse("2024-01-11"),
                            NcrEngCompleteDate = DateTime.Parse("2024-01-11"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Scrap").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-001").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = true,
                            NcrEngDispositionDescription = "item will be reprocessed to ensure compliance of the product with applicable specifications",
                            NcrEngUserId = "2",
                            NcrEngCreationDate = DateTime.Parse("2024-01-11"),
                            NcrEngCompleteDate = DateTime.Parse("2024-01-11"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Rework").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-002").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = true,
                            NcrEngDispositionDescription = "item will be reprocessed to ensure compliance of the product with applicable specifications",
                            NcrEngUserId = "2",
                            NcrEngCreationDate = DateTime.Parse("2024-01-15"),
                            NcrEngCompleteDate = DateTime.Parse("2024-01-15"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Rework").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-003").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = false,
                            NcrEngDispositionDescription = "Item will be repaired in shop to conform to requirements",
                            NcrEngUserId = "2", 
                            NcrEngCreationDate = DateTime.Parse("2024-01-22"),
                            NcrEngCompleteDate = DateTime.Parse("2024-01-22"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Repair").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-004").NcrId
                        },
                        new NcrEng
                        {
                            NcrEngCustomerNotification = true,
                            NcrEngDispositionDescription = "N/A",
                            NcrEngUserId = "2",
                            NcrEngCreationDate = DateTime.Parse("2024-01-23"),
                            NcrEngCompleteDate = DateTime.Parse("2024-01-23"),
                            EngDispositionTypeId = context.EngDispositionTypes.FirstOrDefault(f => f.EngDispositionTypeName == "Scrap").EngDispositionTypeId,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-006").NcrId
                        }
                        );
                    context.SaveChanges();
                }

                if (!context.Drawings.Any())
                {
                    context.Drawings.AddRange(
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 1,
                            DrawingUpdatedRevNumber = 2,
                            DrawingRevDate = DateTime.Parse("2023-12-08"),
                            DrawingUserId = 1, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2023-137").NcrEngId //revisar
                        },
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 3,
                            DrawingUpdatedRevNumber = 4,
                            DrawingRevDate = DateTime.Parse("2023-12-10"),
                            DrawingUserId = 2, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2023-138").NcrEngId //THIS MUST CHANGE
                        },
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 5,
                            DrawingUpdatedRevNumber = 6,
                            DrawingRevDate = DateTime.Parse("2023-12-16"),
                            DrawingUserId = 3, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2023-139").NcrEngId //THIS MUST CHANGE
                        },
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 7,
                            DrawingUpdatedRevNumber = 8,
                            DrawingRevDate = DateTime.Parse("2023-12-16"),
                            DrawingUserId = 4, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2023-140").NcrEngId //THIS MUST CHANGE
                        },
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 1,
                            DrawingUpdatedRevNumber = 3,
                            DrawingRevDate = DateTime.Parse("2023-12-19"),
                            DrawingUserId = 1, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2023-141").NcrEngId //THIS MUST CHANGE
                        },
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 2,
                            DrawingUpdatedRevNumber = 4,
                            DrawingRevDate = DateTime.Parse("2024-01-04"),
                            DrawingUserId = 2, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2024-001").NcrEngId //THIS MUST CHANGE
                        },
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 3,
                            DrawingUpdatedRevNumber = 4,
                            DrawingRevDate = DateTime.Parse("2024-01-06"),
                            DrawingUserId = 3, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2024-002").NcrEngId //THIS MUST CHANGE
                        },
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 4,
                            DrawingUpdatedRevNumber = 7,
                            DrawingRevDate = DateTime.Parse("2024-01-07"),
                            DrawingUserId = 4, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2024-003").NcrEngId//THIS MUST CHANGE
                        },
                        new Drawing
                        {
                            DrawingOriginalRevNumber = 1,
                            DrawingUpdatedRevNumber = 3,
                            DrawingRevDate = DateTime.Parse("2024-01-08"),
                            DrawingUserId = 1, 
                            NcrEngId = context.NcrEngs.FirstOrDefault(f => f.Ncr.NcrNumber == "2024-004").NcrEngId //THIS MUST CHANGE
                        }
                        );
                    context.SaveChanges();
                }

                if (!context.FollowUpTypes.Any())
                {
                    context.FollowUpTypes.AddRange(
                        new FollowUpType
                        {
                            FollowUpTypeName = "Resolution"
                        },
                        new FollowUpType
                        {
                            FollowUpTypeName = "Clarification"
                        },
                        new FollowUpType
                        {
                            FollowUpTypeName = "Feedback"
                        },
                        new FollowUpType
                        {
                            FollowUpTypeName = "Update"
                        });

                    context.SaveChanges();
                }

                if (!context.OpDispositionTypes.Any())
                {
                    context.OpDispositionTypes.AddRange(
                        new OpDispositionType
                        {
                            OpDispositionTypeName = "Rework 'In-House'"
                        },
                        new OpDispositionType
                        {
                            OpDispositionTypeName = "Scrap in House"
                        },
                        new OpDispositionType
                        {
                            OpDispositionTypeName = "Return to Supplier for either 'rework' or 'replace'"
                        },
                        new OpDispositionType
                        {
                            OpDispositionTypeName = "Defer for HBC Engineering Review"
                        });

                    context.SaveChanges();
                }

                if (!context.NcrOperations.Any())
                {
                    context.NcrOperations.AddRange(
                        new NcrOperation
                        {
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-137").NcrId,
                            OpDispositionTypeId = context.OpDispositionTypes.FirstOrDefault(f => f.OpDispositionTypeName == "Rework 'In-House'").OpDispositionTypeId,
                            NcrPurchasingDescription = "Replacement required",
                            Car = true,
                            CarNumber = "2024-01",
                            FollowUp = true,
                            NcrOpCompleteDate = DateTime.Parse("2024-03-05"),
                            ExpectedDate = DateTime.Parse("2024-05-18"),
                            NcrOpCreationDate = DateTime.Parse("2024-02-18"),
                            FollowUpTypeId = context.FollowUpTypes.FirstOrDefault(f => f.FollowUpTypeName == "Update").FollowUpTypeId,
                            UpdateOp = DateTime.Parse("2024-01-18"),
                            NcrOperationUserId = "3"
                        },
                        new NcrOperation
                        {
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-138").NcrId,
                            OpDispositionTypeId = context.OpDispositionTypes.FirstOrDefault(f => f.OpDispositionTypeName == "Scrap in House").OpDispositionTypeId,
                            NcrPurchasingDescription = "Replacement required",
                            Car = true,
                            CarNumber = "2024-02",
                            FollowUp = true,
                            NcrOpCompleteDate = DateTime.Parse("2024-03-05"),
                            ExpectedDate = DateTime.Parse("2024-05-18"),
                            NcrOpCreationDate = DateTime.Parse("2024-02-18"),
                            FollowUpTypeId = context.FollowUpTypes.FirstOrDefault(f => f.FollowUpTypeName == "Resolution").FollowUpTypeId,
                            UpdateOp = DateTime.Parse("2024-01-19"),
                            NcrOperationUserId = "3"
                        },
                        new NcrOperation
                        {
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-139").NcrId,
                            OpDispositionTypeId = context.OpDispositionTypes.FirstOrDefault(f => f.OpDispositionTypeName == "Defer for HBC Engineering Review").OpDispositionTypeId,
                            NcrPurchasingDescription = "Replacement required",
                            Car = true,
                            CarNumber = "3456",
                            FollowUp = true,
                            NcrOpCompleteDate = DateTime.Parse("2024-03-05"),
                            ExpectedDate = DateTime.Parse("2024-05-18"),
                            NcrOpCreationDate = DateTime.Parse("2024-02-18"),
                            FollowUpTypeId = context.FollowUpTypes.FirstOrDefault(f => f.FollowUpTypeName == "Clarification").FollowUpTypeId,
                            UpdateOp = DateTime.Parse("2024-01-20"),
                            NcrOperationUserId = "3"
                        },
                        new NcrOperation
                        {
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-140").NcrId,
                            OpDispositionTypeId = context.OpDispositionTypes.FirstOrDefault(f => f.OpDispositionTypeName == "Rework 'In-House'").OpDispositionTypeId,
                            NcrPurchasingDescription = "Rework per engineering disposition",
                            Car = true,
                            CarNumber = "2024-03",
                            FollowUp = true,
                            NcrOpCompleteDate = DateTime.Parse("2024-03-05"),
                            ExpectedDate = DateTime.Parse("2024-05-18"),
                            NcrOpCreationDate = DateTime.Parse("2024-02-18"),
                            FollowUpTypeId = context.FollowUpTypes.FirstOrDefault(f => f.FollowUpTypeName == "Feedback").FollowUpTypeId,
                            UpdateOp = DateTime.Parse("2024-01-21"),
                            NcrOperationUserId = "3"
                        },
                        new NcrOperation
                        {
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-006").NcrId,
                            OpDispositionTypeId = context.OpDispositionTypes.FirstOrDefault(f => f.OpDispositionTypeName == "Defer for HBC Engineering Review").OpDispositionTypeId,
                            NcrPurchasingDescription = "Replacement required",
                            Car = true,
                            CarNumber = "2024-04",
                            FollowUp = true,
                            NcrOpCompleteDate = DateTime.Parse("2024-03-05"),
                            ExpectedDate = DateTime.Parse("2024-05-18"),
                            NcrOpCreationDate = DateTime.Parse("2024-02-18"),
                            FollowUpTypeId = context.FollowUpTypes.FirstOrDefault(f => f.FollowUpTypeName == "Clarification").FollowUpTypeId,
                            UpdateOp = DateTime.Parse("2024-01-20"),
                            NcrOperationUserId = "3"
                        },
                        new NcrOperation
                        {
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-141").NcrId,
                            OpDispositionTypeId = context.OpDispositionTypes.FirstOrDefault(f => f.OpDispositionTypeName == "Rework 'In-House'").OpDispositionTypeId,
                            NcrPurchasingDescription = "Replacement required",
                            Car = true,
                            CarNumber = "2024-05",
                            FollowUp = true,
                            NcrOpCompleteDate = DateTime.Parse("2024-01-18"),
                            ExpectedDate = DateTime.Parse("2024-05-18"),
                            NcrOpCreationDate = DateTime.Parse("2024-02-18"),
                            FollowUpTypeId = context.FollowUpTypes.FirstOrDefault(f => f.FollowUpTypeName == "Clarification").FollowUpTypeId,
                            UpdateOp = DateTime.Parse("2024-01-20"),
                            NcrOperationUserId = "3"
                        },
                        new NcrOperation
                        {
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-002").NcrId,
                            OpDispositionTypeId = context.OpDispositionTypes.FirstOrDefault(f => f.OpDispositionTypeName == "Defer for HBC Engineering Review").OpDispositionTypeId,
                            NcrPurchasingDescription = "Replacement required",
                            Car = true,
                            CarNumber = "2024-15",
                            FollowUp = true,
                            ExpectedDate = DateTime.Parse("2024-05-18"),
                            NcrOpCompleteDate = DateTime.Parse("2024-01-18"),
                            NcrOpCreationDate = DateTime.Parse("2024-02-18"),
                            FollowUpTypeId = context.FollowUpTypes.FirstOrDefault(f => f.FollowUpTypeName == "Update").FollowUpTypeId,
                            UpdateOp = DateTime.Parse("2024-01-20"),
                            NcrOperationUserId = "3"

                        },
                        new NcrOperation
                        {
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-001").NcrId,
                            OpDispositionTypeId = context.OpDispositionTypes.FirstOrDefault(f => f.OpDispositionTypeName == "Scrap in House").OpDispositionTypeId,
                            NcrPurchasingDescription = "Replacement required",
                            Car = true,
                            CarNumber = "2024-07",
                            FollowUp = true,
                            NcrOpCompleteDate = DateTime.Parse("2024-01-23"),
                            ExpectedDate = DateTime.Parse("2024-05-18"),
                            NcrOpCreationDate = DateTime.Parse("2024-02-18"),
                            FollowUpTypeId = context.FollowUpTypes.FirstOrDefault(f => f.FollowUpTypeName == "Feedback").FollowUpTypeId,
                            UpdateOp = DateTime.Parse("2024-01-20"),
                            NcrOperationUserId = "3"
                        }
                        ); ;
                    context.SaveChanges();
                }

                if (!context.NcrReInspects.Any())
                {
                    context.NcrReInspects.AddRange(
                        new NcrReInspect
                        {
                            NcrReInspectAcceptable = true,
                            NcrReInspectUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-137").NcrId,
                            NcrReInspectCreationDate = DateTime.Parse("2023-12-07")
                        },
                        new NcrReInspect
                        {
                            NcrReInspectAcceptable = true,
                            NcrReInspectUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-139").NcrId,
                            NcrReInspectCreationDate = DateTime.Parse("2023-12-11")
                        },
                        new NcrReInspect
                        {
                            NcrReInspectAcceptable = true,
                            NcrReInspectUserId = "1", 
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-006").NcrId,
                            NcrReInspectCreationDate = DateTime.Parse("2024-01-14")
                        }
                        );
                    context.SaveChanges();
                }
                if (!context.NcrProcurements.Any())
                {
                    context.NcrProcurements.AddRange(
                        new NcrProcurement
                        {
                            NcrProcSupplierReturnReq = true,
                            NcrProcExpectedDate = DateTime.Parse("2023-12-07"),
                            NcrProcDisposedAllowed = false,
                            NcrProcSAPReturnCompleted = true,
                            NcrProcCreditExpected = true,
                            NcrProcSupplierBilled = true,
                            NcrProcUserId = "4", 
                            NcrProcCreated = DateTime.Parse("2023-12-18"),
                            NcrProcCompleteDate = DateTime.Parse("2023-12-18"),
                            SupplierReturnMANum = "11345",
                            SupplierReturnName = "FEDEX",
                            SupplierReturnAccount = "12345",
                            NcrProcRejectedValue = 1500.00M,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-137").NcrId
                        },
                        new NcrProcurement
                        {
                            NcrProcSupplierReturnReq = true,
                            NcrProcExpectedDate = DateTime.Parse("2023-12-19"),
                            NcrProcDisposedAllowed = false,
                            NcrProcSAPReturnCompleted = true,
                            NcrProcCreditExpected = true,
                            NcrProcSupplierBilled = true,
                            NcrProcUserId = "4", 
                            NcrProcCreated = DateTime.Parse("2023-12-19"),
                            NcrProcCompleteDate = DateTime.Parse("2023-12-19"),
                            SupplierReturnMANum = "11345",
                            SupplierReturnName = "Purolator",
                            SupplierReturnAccount = "12345",
                            NcrProcRejectedValue = 15087.00M,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-138").NcrId
                        },
                        new NcrProcurement
                        {
                            NcrProcSupplierReturnReq = true,
                            NcrProcExpectedDate = DateTime.Parse("2023-12-23"),
                            NcrProcDisposedAllowed = false,
                            NcrProcSAPReturnCompleted = true,
                            NcrProcCreditExpected = true,
                            NcrProcSupplierBilled = true,
                            NcrProcUserId = "4", 
                            NcrProcCreated = DateTime.Parse("2023-12-23"),
                            NcrProcCompleteDate = DateTime.Parse("2023-12-23"),
                            SupplierReturnMANum = "11345",
                            SupplierReturnName = "FEDEX",
                            SupplierReturnAccount = "12345",
                            NcrProcRejectedValue = 5634.25M,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-139").NcrId
                        },
                        new NcrProcurement
                        {
                            NcrProcSupplierReturnReq = true,
                            NcrProcExpectedDate = DateTime.Parse("2024-01-18"),
                            NcrProcDisposedAllowed = false,
                            NcrProcSAPReturnCompleted = true,
                            NcrProcCreditExpected = true,
                            NcrProcSupplierBilled = true,
                            NcrProcUserId = "4", 
                            NcrProcCreated = DateTime.Parse("2024-01-18"),
                            NcrProcCompleteDate = DateTime.Parse("2024-01-18"),
                            SupplierReturnMANum = "11345",
                            SupplierReturnName = "FEDEX",
                            SupplierReturnAccount = "12345",
                            NcrProcRejectedValue = 1111.75M,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-140").NcrId
                        },
                        new NcrProcurement
                        {
                            NcrProcSupplierReturnReq = true,
                            NcrProcExpectedDate = DateTime.Parse("2024-01-10"),
                            NcrProcDisposedAllowed = false,
                            NcrProcSAPReturnCompleted = true,
                            NcrProcCreditExpected = true,
                            NcrProcSupplierBilled = true,
                            NcrProcUserId = "4", 
                            NcrProcCreated = DateTime.Parse("2024-01-14"),
                            NcrProcCompleteDate = DateTime.Parse("2024-01-14"),
                            SupplierReturnMANum = "11345",
                            SupplierReturnName = "Purolator",
                            SupplierReturnAccount = "12345",
                            NcrProcRejectedValue = 2714.50M,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2023-141").NcrId
                        },
                        new NcrProcurement
                        {
                            NcrProcSupplierReturnReq = true,
                            NcrProcExpectedDate = DateTime.Parse("2024-01-30"),
                            NcrProcDisposedAllowed = false,
                            NcrProcSAPReturnCompleted = true,
                            NcrProcCreditExpected = true,
                            NcrProcSupplierBilled = true,
                            NcrProcUserId = "4", 
                            NcrProcCreated = DateTime.Parse("2024-01-23"),
                            NcrProcCompleteDate = DateTime.Parse("2023-01-23"),
                            SupplierReturnMANum = "11345",
                            SupplierReturnName = "FEDEX",
                            SupplierReturnAccount = "12345",
                            NcrProcRejectedValue = 1000.00M,
                            NcrId = context.Ncrs.FirstOrDefault(f => f.NcrNumber == "2024-006").NcrId
                        }
                        );
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
