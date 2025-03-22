using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsUi;
public class DataGetter
{
    public VssConnection connection { get; set; } = null;
    private WorkItemTrackingHttpClient witClient { get; set; }
    private readonly Config _config;

    public DataGetter(Config config)
    {
        _config = config;
        Uri orgUrl = new(config.CollectionUri);
        connection = new(orgUrl, new VssBasicCredential(string.Empty, config.PatToken));
        witClient = connection.GetClient<WorkItemTrackingHttpClient>();
    }

    public async Task ShowWorkItems(UIState uiState)
    {
        if (uiState.SelectedQuery == string.Empty) return;
        uiState.stateToTitles.Clear();

        uiState.stateToTitles = new(){
            {"New", new()},
            {"Active", new()},
            {"Ready For Test", new()},
        };
        // Get an instance of the work item tracking client

        try
        {
            // Get the specified work item
            var items = await witClient.GetQueryAsync(_config.DefaultProject, uiState.SelectedQuery, QueryExpand.All, 2);
            Wiql query = new() { Query = items.Wiql };
            var workItemIds = (await witClient.QueryByWiqlAsync(query)).WorkItems.Select(x => x.Id);

            var workItems = await witClient.GetWorkItemsAsync(workItemIds);

            foreach (var workItem in workItems)
            {
                workItem.Fields.TryGetValue("System.Title", out string title);
                workItem.Fields.TryGetValue("System.State", out string state);
                workItem.Fields.TryGetValue("System.Description", out string desc);
                workItem.Fields.TryGetValue("System.WorkItemType", out string type);
                workItem.Fields.TryGetValue("Microsoft.VSTS.TCM.ReproSteps", out string repro);

                string stateStr = state?.ToString() ?? "Unknown";
                string titleStr = title?.ToString() ?? "Untitled";

                // Add state and title to dictionary
                if (!uiState.stateToTitles.ContainsKey(stateStr))
                {
                    uiState.stateToTitles[stateStr] = new();
                }

                uiState.stateToTitles[stateStr].WorkItems.Add(new()
                {
                    Id = workItem.Id,
                    Title = $"{type} ({workItem.Id}): {title}",
                    Description = repro == null
                    ? HtmlUtilities.ConvertToPlainText(desc ?? string.Empty).Trim() :
                    HtmlUtilities.ConvertToPlainText(repro ?? string.Empty).Trim(),
                    State = state,
                    Type = type
                });
            }
        }
        catch (AggregateException aex)
        {
            if (aex.InnerException is VssServiceException vssex)
            {
                Console.WriteLine(vssex.Message);
            }
        }
    }
}
