using Microsoft.AspNetCore.Components;
using IfakCrane.Client.Services;


namespace IfakCrane.Client.Shared
{
    public partial class NavMenu
    {
        #region : Defined Fields and Properties 
        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        string MasterCrane = "crane1";
        string SlaveCrane = "None";
        IEnumerable<string> MasterCranes { get; set; } = new List<string>
        {
            "crane1",
            "crane2"
        };
        IEnumerable<string> SlaveCranes { get; set; }

        string MasterTrolley = "trolley1";
        IEnumerable<string> Trolleys { get; set; } = new List<string>
        {
            "trolley1"
        };
        string SlaveTrolley = "None";
        IEnumerable<string> SlaveTrolleys { get; set; } = new List<string>
        {
            "None"
        };

        string SelectedFunction = "goto_topic";

        List<Functions> functionNames = new List<Functions>
        {
            new Functions
            {
                Name = "goto_topic",
                DisplayingName = "Go-To"
            },
            new Functions
            {
                Name = "cometome_topic",
                DisplayingName = "Come-To"
            },
            new Functions
            {
                Name = "followme_topic",
                DisplayingName = "Follow"
            }
        };
        #endregion

        #region: Services Injection and Component Initialization
        [Inject]
        public NavMenuService? navToCraneUI { get; set; }

        [Inject]
        public SignalRService? signalR { get; set; }
        protected async override Task OnInitializedAsync()
        {
            navToCraneUI.CraneSelection(MasterCrane);
            navToCraneUI.FunctionSelection(SelectedFunction);
            if (MasterCrane == "crane1")
            {
                SlaveCranes = new List<string>
                {
                    "crane2",
                    "None"
                };
            }

            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region : Component's Methods
        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private async Task OnCraneSelected(string newValue)
        {
            MasterCrane = newValue;
            await navToCraneUI.CraneSelection(newValue);
            if (MasterCrane == "crane1")
            {
                SlaveCranes = new List<string>
                {
                    "crane2",
                    "None"
                };
            }
            else if (MasterCrane == "crane2")
            {
                SlaveCranes = new List<string>
                {
                    "crane1",
                    "None"
                };
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task OnFunctionSelected(string newValue)
        {
            SelectedFunction = newValue;
            await navToCraneUI.FunctionSelection(newValue);
            await InvokeAsync(StateHasChanged);
        }

        private async void StopButtonClick()
        {
            await Task.Run(() => { signalR.PublishToServer(MasterCrane,"button_stop_topic", Newtonsoft.Json.JsonConvert.SerializeObject(new[] { "true" })); });
            await navToCraneUI.StopButtonClicked("stop");
        }
        #endregion
        private class Functions
        {
            public string? Name { get; set; }

            public string? DisplayingName { get; set; }
        }
    }
}