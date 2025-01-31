using HaverDevProject.Data;
using Microsoft.AspNetCore.Mvc;
using HaverDevProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using HaverDevProject.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HaverDevProject.Controllers.APIs
{    
    public class ApiController : Controller
    {
        private readonly HaverNiagaraContext _context;

        public ApiController(HaverNiagaraContext context)
        {
            _context = context;
        }

        // GET: api/Ncrs
        [AllowAnonymous]
        [HttpGet]        
        [Route("api/Ncrs")]
        public async Task<ActionResult<IEnumerable<NcrQaApiDTO>>> GetNcrs()
        {
            var ncrsDTO = await _context.NcrQas
                .Include(n => n.Ncr)
                .Include(n => n.Supplier)
                .Include(n => n.Item)
                .Include(n => n.Defect)
                .Include(n => n.Ncr).ThenInclude(n => n.NcrProcurement)
                .Select(n => new NcrQaApiDTO
                {
                    NcrNumber = n.Ncr.NcrNumber,
                    NcrStatus = n.Ncr.NcrStatus,
                    NcrPhase = n.Ncr.NcrPhase,
                    NcrQaProcessApplicable = n.NcrQaProcessApplicable,
                    NcrQaItemMarNonConforming = n.NcrQaItemMarNonConforming,
                    NcrQacreationDate = n.NcrQacreationDate,
                    NcrQaOrderNumber = n.NcrQaOrderNumber,
                    NcrQaSalesOrder = n.NcrQaSalesOrder,
                    NcrQaQuanReceived = n.NcrQaQuanReceived,
                    NcrQaQuanDefective = n.NcrQaQuanDefective,
                    SupplierId = n.SupplierId,
                    TotalCost = n.Ncr.NcrProcurement.NcrProcRejectedValue, //Adding Cost
                    SupplierApiDTO = new SupplierApiDTO
                    {
                        SupplierId = n.Supplier.SupplierId,
                        SupplierName = n.Supplier.SupplierName,
                        SupplierCode = n.Supplier.SupplierCode,
                        SupplierStatus = n.Supplier.SupplierStatus                        
                    },
                    ItemId = n.Item.ItemId,
                    ItemApiDTO = new ItemApiDTO
                    {
                        ItemId = n.Item.ItemId,
                        ItemName = n.Item.ItemName,
                        ItemNumber = n.Item.ItemNumber
                    },
                    DefectId = n.Defect.DefectId,
                    DefectApiDTO = new DefectApiDTO
                    {
                        DefectId = n.Defect.DefectId,
                        DefectName = n.Defect.DefectName
                    }    
                })
                .ToListAsync();

            if (ncrsDTO.Count() > 0)
            {
                return ncrsDTO;
            }
            else
            {
                return NotFound(new { message = "Error: No Ncrs records in the system." });
            }
        }       

    }    
}

