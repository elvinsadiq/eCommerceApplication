﻿using eTickets.Data;
using eTickets.Data.Services;
using eTickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace eTickets.Controllers
{
	public class ProducersController : Controller
	{
		private readonly IProducersService _service;

		public ProducersController(IProducersService service)
		{
			this._service = service;
		}

		public async Task<IActionResult> Index()
		{
			var allProducers = await _service.GetAllAsync();

			return View(allProducers);
		}

		#region Details
		//GET: producers/details/1
		public async Task<IActionResult> Details(int id)
		{
			var producerDetails = await _service.GetByIdAsync(id);
			if (producerDetails == null) return View("NotFound");

			return View(producerDetails);
		}
		#endregion

		#region Create
		//GET: producers/create/1
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create([Bind("FullName,ProfilePictureURL,Bio")] Producer producer)
		{
			if (ModelState["FullName"].ValidationState != ModelValidationState.Valid ||
				ModelState["ProfilePictureURL"].ValidationState != ModelValidationState.Valid ||
				ModelState["Bio"].ValidationState != ModelValidationState.Valid)
			{
				return View(producer);
			}

			await _service.AddAsync(producer);
			return RedirectToAction(nameof(Index));
		}
		#endregion

		#region Edit
		//GET: producers/edit/1
		public async Task<IActionResult> Edit(int id)
		{
			var producerDetails = await _service.GetByIdAsync(id);
			if (producerDetails == null) return View("NotFound");
			return View(producerDetails);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,ProfilePictureURL,Bio")] Producer producer)
		{
			if (ModelState["Id"].ValidationState != ModelValidationState.Valid ||
				ModelState["FullName"].ValidationState != ModelValidationState.Valid ||
				ModelState["ProfilePictureURL"].ValidationState != ModelValidationState.Valid ||
				ModelState["Bio"].ValidationState != ModelValidationState.Valid)
			{
				return View(producer);
			}

			if (id == producer.Id)
			{
				await _service.UpdateAsync(id, producer);
				return RedirectToAction(nameof(Index));
			}

			return View(producer);
		}
		#endregion

		#region Delete
		//Get: Actors/Create
		public async Task<IActionResult> Delete(int id)
		{
			var producerDetails = await _service.GetByIdAsync(id);
			if (producerDetails == null) return View("NotFound");

			return View(producerDetails);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var producerDetails = await _service.GetByIdAsync(id);
			if (producerDetails == null) return View("NotFound");

			await _service.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		} 
		#endregion
	}
}
