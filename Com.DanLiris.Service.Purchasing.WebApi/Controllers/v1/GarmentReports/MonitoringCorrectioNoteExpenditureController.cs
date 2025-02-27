﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.DanLiris.Service.Purchasing.Lib.Interfaces;
using Com.DanLiris.Service.Purchasing.Lib.Services;
using Com.DanLiris.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.DanLiris.Service.Purchasing.WebApi.Controllers.v1.GarmentReports
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/garment-correction-note-expenditure/monitoring")]
	[Authorize]
	public class MonitoringCorrectionNoteExpenditureController : Controller
	{
		private string ApiVersion = "1.0.0";
		public readonly IServiceProvider serviceProvider;
		//private readonly IMapper mapper;
		private readonly IMonitoringCorrectionNoteExpenditureFacade facade;
		private readonly IdentityService identityService;

		public MonitoringCorrectionNoteExpenditureController(IServiceProvider serviceProvider, IMonitoringCorrectionNoteExpenditureFacade facade)
		{
			this.serviceProvider = serviceProvider;
			this.facade = facade;
			this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
		}
		[HttpGet]
		public IActionResult GetReport(DateTime? dateFrom, DateTime? dateTo, /**string jnsBC,**/ int page, int size, string Order = "{}")
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{

				var data = facade.GetMonitoringKeluarNKReport(dateFrom, dateTo, page, size, Order, offset/**, jnsBC**/);

				return Ok(new
				{
					apiVersion = ApiVersion,
					data = data.Item1,
					info = new { total = data.Item2 },
					message = General.OK_MESSAGE,
					statusCode = General.OK_STATUS_CODE
				});
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}
		[HttpGet("by-user")]
		public IActionResult GetReportByUser(string username, DateTime? dateFrom, DateTime? dateTo, /**string jnsBC,**/ int page, int size, string Order = "{}")
		{
			identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
			username = identityService.Username;

			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{

				var data = facade.GetMonitoringKeluarNKByUserReport(dateFrom, dateTo, page, size, Order, offset/**, jnsBC**/);

				return Ok(new
				{
					apiVersion = ApiVersion,
					data = data.Item1,
					info = new { total = data.Item2 },
					message = General.OK_MESSAGE,
					statusCode = General.OK_STATUS_CODE
				});
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}
		[HttpGet("download")]
		public IActionResult GetXls(DateTime? dateFrom, DateTime? dateTo, /**string jnsBC,**/ int page, int size, string Order = "{}")
		{
			try
			{
				byte[] xlsInBytes;

				int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
				var xls = facade.GenerateExcelMonitoringKeluarNK(dateFrom, dateTo, page, size, Order, offset/**, jnsBC**/);

				string filename = "Monitoring Pengeluaran Nota Koreksi - All";
				if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");
				if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
				filename += ".xlsx";

				xlsInBytes = xls.ToArray();
				var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
				return file;
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}

		}
		[HttpGet("by-user/download")]
		public IActionResult GetXlsByUser(string username, DateTime? dateFrom, DateTime? dateTo, /**string jnsBC,**/ int page, int size, string Order = "{}")
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				username = identityService.Username;

				byte[] xlsInBytes;

				int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
				var xls = facade.GenerateExcelMonitoringKeluarNKByUser(dateFrom, dateTo, page, size, Order, offset/**, jnsBC**/);

				string filename = "Monitoring Pengeluaran  Nota Koreksi - " + username + "All";
				if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");
				if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
				filename += ".xlsx";

				xlsInBytes = xls.ToArray();
				var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
				return file;
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}

		}
	}
}