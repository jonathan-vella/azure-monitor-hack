﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using TTA.Core;
using TTA.Interfaces;
using TTA.Models;
using TTA.Web.Options;

namespace TTA.Web.Pages.User;

//[Authorize]
public class DashboardPageModel : PageModel
{
    private readonly ILogger<DashboardPageModel> logger;
    private readonly IProfileSettingsService profileSettingsService;
    private readonly IWorkTaskRepository workTaskRepository;
    private WebSettings webSettings;

    public DashboardPageModel(ILogger<DashboardPageModel> logger,
        IProfileSettingsService profileSettingsService, IWorkTaskRepository workTaskRepository,
        IOptions<WebSettings> webSettingsValue)
    {
        this.logger = logger;
        webSettings = webSettingsValue.Value;
        this.profileSettingsService = profileSettingsService;
        this.workTaskRepository = workTaskRepository;
    }

    public async Task OnGetAsync(int? pageNumber, string query)
    {
        int currentPageNumber = pageNumber ?? 1;
        var profileName = User.Identity.Name;
        logger.LogInformation("Loading dashboard for user {User} - starting at {DateStart}", profileName, DateTime.Now);
        var id = profileName.GetUniqueValue();
        ProfileSettings = await profileSettingsService.GetAsync(id);
        logger.LogInformation("Got profile for {UniqueSettingsId} - ended at {DateEnd}", id, DateTime.Now);

        UserTasks = await workTaskRepository.WorkTasksForUserAsync(profileName, currentPageNumber,
            webSettings.PageCount, query);
        logger.LogInformation("Loaded {UserTaskNumber} work tasks for user with {Query}", UserTasks.TotalPages, query);
    }

    [BindProperty] public TTAUserSettings ProfileSettings { get; set; }
    [BindProperty] public PaginatedList<WorkTask> UserTasks { get; set; }
}