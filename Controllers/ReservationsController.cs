using DiveCenterManager.Services;
using DiveCenterManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DiveCenterManager.Controllers;

public class ReservationsController : Controller
{
    private readonly ReservationService _reservationService;
    private readonly ActivityService _activityService;
    private readonly DiveSiteService _diveSiteService;
    private readonly BoatService _boatService;
    private readonly StaffService _staffService;

    public ReservationsController(
        ReservationService reservationService,
        ActivityService activityService,
        DiveSiteService diveSiteService,
        BoatService boatService,
        StaffService staffService)
    {
        _reservationService = reservationService;
        _activityService = activityService;
        _diveSiteService = diveSiteService;
        _boatService = boatService;
        _staffService = staffService;
    }

    public async Task<IActionResult> Index()
    {
        var reservations =
            await _reservationService.GetAllAsync();

        return View(reservations);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new ReservationCreateViewModel();

        await LoadFormOptionsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ReservationCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadFormOptionsAsync(model);

            return View(model);
        }

        try
        {
            await _reservationService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadFormOptionsAsync(model);

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model =
            await _reservationService.GetByIdAsync(id);

        if (model is null)
        {
            return NotFound();
        }

        await LoadEditFormOptionsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        ReservationEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadEditFormOptionsAsync(model);

            return View(model);
        }

        try
        {
            await _reservationService.UpdateAsync(model);

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadEditFormOptionsAsync(model);

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(
        int id,
        string status)
    {
        try
        {
            await _reservationService.SetStatusAsync(
                id,
                status);

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailability(
        DateTime reservationDate,
        int participantCount)
    {
        if (participantCount < 1)
        {
            participantCount = 1;
        }

        var boats =
            await _boatService.GetAvailableAsync(
                reservationDate,
                participantCount);

        var staff =
            await _staffService.GetAvailableAsync(
                reservationDate);

        return Json(new
        {
            boats = boats.Select(boat => new
            {
                boat.Id,
                boat.Name,
                boat.Capacity
            }),

            staff = staff.Select(person => new
            {
                person.Id,
                Name = $"{person.FirstName} {person.LastName}"
            })
        });
    }
    [HttpGet]
public async Task<IActionResult> GetEditAvailability(
    int reservationId,
    DateTime reservationDate,
    int participantCount)
{
    if (participantCount < 1)
    {
        participantCount = 1;
    }

    var boats =
        await _boatService.GetAvailableForEditAsync(
            reservationDate,
            participantCount,
            reservationId);

    var staff =
        await _staffService.GetAvailableForEditAsync(
            reservationDate,
            reservationId);

    return Json(new
    {
        boats = boats.Select(boat => new
        {
            boat.Id,
            boat.Name,
            boat.Capacity
        }),

        staff = staff.Select(person => new
        {
            person.Id,
            Name = $"{person.FirstName} {person.LastName}"
        })
    });
}

    private async Task LoadFormOptionsAsync(
        ReservationCreateViewModel model)
    {
        model.AvailableActivities =
            await _activityService.GetActiveAsync();

        model.AvailableDiveSites =
            await _diveSiteService.GetActiveAsync();

        model.AvailableBoats =
            await _boatService.GetAvailableAsync(
                model.ReservationDate,
                model.ParticipantCount);

        model.AvailableStaff =
            await _staffService.GetAvailableAsync(
                model.ReservationDate);
    }

    private async Task LoadEditFormOptionsAsync(
        ReservationEditViewModel model)
    {
        model.AvailableActivities =
            await _activityService.GetActiveAsync();

        model.AvailableDiveSites =
            await _diveSiteService.GetActiveAsync();

        model.AvailableBoats =
            await _boatService.GetAvailableForEditAsync(
                model.ReservationDate,
                model.ParticipantCount,
                model.Id);

        model.AvailableStaff =
            await _staffService.GetAvailableForEditAsync(
                model.ReservationDate,
                model.Id);
    }
}