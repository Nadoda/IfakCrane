using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using IfakCrane.Client.Services;
using IfakCrane.Shared.Models;
using Newtonsoft.Json;


namespace IfakCrane.Client.Pages
{
    public partial class CraneUI
    {
        #region : Fields For the Target Icon

        bool showTarget = false;
        double dotX = 0;
        double dotY = 0;
        string dotXpx => $"{dotX}px";
        string dotYpx => $"{dotY}px";
        private double circleX = 0;
        private double circleY = 0;
        private bool isDragging = false;
        private double offsetX = 0;
        private double offsetY = 0;
        private ElementReference circleElement;

        #endregion

        #region : Fields for the CraneUI component
        private string? selectedMasterCrane;
        private string? SelectedFunction;
        private string? TrolleyC1Style;
        private string? Crane1Style;
        private string? TrolleyC2Style = "";
        private string? Crane2Style = "";
        private string[]? PublishingDataArray;
        private Timer? timer;
        #endregion

        #region : Services Injection and Component Initialization
        [Inject]
        public SignalRService? signalR { get; set; }

        [Inject]
        public NavMenuService? navToCrane { get; set; }

        [Inject]
        public NotificationService? notificationService { get; set; }

        protected async override Task OnInitializedAsync()
        {
            signalR.OnReceivedPositionC1 += AnimateTrolleyC1;
            signalR.OnReceivedPositionC1 += AnimateCrane1;
            signalR.OnReceivedPositionC2 += AnimateTrolleyC2;
            signalR.OnReceivedPositionC2 += AnimateCrane2;
            signalR.OnAutoModeStatus += AutoModeHandler;
            signalR.OnGoToStatus += GoToStatusHandler;
            navToCrane.OnCraneSelected += SelectedCraneHandler;
            navToCrane.OnFunctionSelected += FunctionSelectHandler;
            navToCrane.OnStopButtonPress += StopButtonEventHandler;
            await signalR.StartConnection();
        }
        #endregion

        #region : Event Handler Methods for handling events of NavMenuService.cs 
        private async void SelectedCraneHandler(string newItem)
        {
            selectedMasterCrane = newItem;
            await InvokeAsync(StateHasChanged);
        }

        private async void FunctionSelectHandler(string newItem)
        {
            SelectedFunction = newItem;
            if (SelectedFunction == "goto_topic")
            {
                timer?.Dispose();
                timer = null;
            }
            else if (SelectedFunction == "cometome_topic")
            {
                timer?.Dispose();
                timer = null;
                showTarget = false;
                circleX = 0;
                circleY = 0;
                isDragging = false;
                offsetX = 0;
                offsetY = 0;
            }
            else if (SelectedFunction == "followme_topic")
            {
                showTarget = false;
                circleX = 0;
                circleY = 0;
                isDragging = false;
                offsetX = 0;
                offsetY = 0;
                await Follow_Me_Function();
            }

            await InvokeAsync(StateHasChanged);
        }

        private async void StopButtonEventHandler(string data)
        {
            if (data == "stop")
            {
                timer?.Dispose();
                timer = null;
                circleX = 0;
                circleY = 0;
                isDragging = false;
                offsetX = 0;
                offsetY = 0;
            }

            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region: Event Handler Methods for handling events of SigalRService.cs
        
        private async void AutoModeHandler(dynamic data)
        {
            Console.WriteLine(data);
            await InvokeAsync(StateHasChanged);
        }

        private async void GoToStatusHandler(dynamic status)
        {
            if (status.GetString() == "Done")
            {
                showTarget = false;
            }

            await InvokeAsync(StateHasChanged);
        }

        // For Crane1
        private async void AnimateTrolleyC1(dynamic data) // Animation on y-axis
        {
            var Data = data.GetString();
            var commaIndex = Data.IndexOf(',');
            if (commaIndex != -1)
            {
                var Positiondata = Data.Substring(commaIndex + 1).Trim();
                var position = JsonConvert.DeserializeObject<PositionData>(Positiondata);
                await Task.Run(() =>
                {
                    TrolleyC1Style = $"transform: translatey({(position.Y * -265 / 53200) + 265}px);";
                });
            }

            await InvokeAsync(StateHasChanged);
        }

        private async void AnimateCrane1(dynamic data) // Animation on x-axis
        {
            var Data = data.GetString();
            var commaIndex = Data.IndexOf(',');
            if (commaIndex != -1)
            {
                var Positiondata = Data.Substring(commaIndex + 1).Trim();
                Console.WriteLine(Positiondata + " From Crane 1");
                var position = JsonConvert.DeserializeObject<PositionData>(Positiondata);
                await Task.Run(() =>
                {
                    Crane1Style = $"transform: translateX({position.X * 800 / 159060}px);";
                });
            }

            await InvokeAsync(StateHasChanged);
        }

        // For Crane 2 ///
        private async void AnimateTrolleyC2(dynamic data) // Animation on y-axis
        {
            var Data = data.GetString();
            var commaIndex = Data.IndexOf(',');
            if (commaIndex != -1)
            {
                var Positiondata = Data.Substring(commaIndex + 1).Trim();
                var position = JsonConvert.DeserializeObject<PositionData>(Positiondata);
                await Task.Run(() =>
                {
                    TrolleyC2Style = $"transform: translatey({(position.Y * -300 / 53200) + 510}px);";
                });
            }

            await InvokeAsync(StateHasChanged);
        }

        private async void AnimateCrane2(dynamic data) // Animation on x-axis
        {
            var Data = data.GetString();
            var commaIndex = Data.IndexOf(',');
            if (commaIndex != -1)
            {
                var Positiondata = Data.Substring(commaIndex + 1).Trim();
                Console.WriteLine(Positiondata + " From Crane 2");
                var position = JsonConvert.DeserializeObject<PositionData>(Positiondata);
                await Task.Run(() =>
                {
                    Crane2Style = $"transform: translateX({((position.X * 800 / 159060))}px);";
                }); //-190
            }

            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region: Component's Methods
        
        private void GoToFunction()
        {
            //showDot = true;
            if (selectedMasterCrane == "crane1" && dotY >= 200)
            {
                //string[] dataArray = new string[]
                //{
                //    "","defaulthmi","crane1","[\"\"]","trolley1","[\"\"]",$"{(int)(dotX*163000/797) }",$"{(int)(-((dotY-265)*53200)/265)}","11400"};    // 797px approx 800px
                PublishingDataArray = new string[]
                {
                    "",
                    "defaulthmi",
                    "crane1",
                    "[\"\"]",
                    "trolley1",
                    "[\"\"]",
                    $"{(int)(dotX * 159060 / 800)}",
                    $"{(int)((-53200 * dotY / 300) + (53200 * 500 / 300))}",
                    "11400"
                };
                signalR.PublishToServer(selectedMasterCrane, "goto_topic", JsonConvert.SerializeObject(PublishingDataArray));
            }
            else if (selectedMasterCrane == "crane2" && dotY <= 300)
            {
                string[] PublishingDataArray = new string[]
                {
                    "",
                    "defaulthmi",
                    "crane2",
                    "[\"\"]",
                    "trolley1",
                    "[\"\"]",
                    $"{(int)(dotX * 163000 / 800)}",
                    $"{(int)((-dotY * 53200 / 300) + 90808)}",
                    "0"
                }; // 797px approx 800px
                signalR.PublishToServer(selectedMasterCrane, "goto_topic", JsonConvert.SerializeObject(PublishingDataArray));
            }
            else
            {
                notificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Info, Detail = $"Out of the {selectedMasterCrane} region" });
            }
        }

        private void MouseClickPosition(MouseEventArgs e)
        {
            if (SelectedFunction == "goto_topic")
            {
                dotX = e.OffsetX;
                dotY = e.OffsetY;
                showTarget = true;
                GoToFunction();
                StateHasChanged();
            }
        }

        private void StartDragging(MouseEventArgs e)
        {
            isDragging = true;
            offsetX = e.ClientX - circleX;
            offsetY = e.ClientY - circleY;
        }

        private async void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                circleX = e.ClientX - offsetX;
                circleY = e.ClientY - offsetY;
                // Ensure the circle stays within the bounds of the rectangle
                if (circleX < 0)
                    circleX = 0;
                if (circleX > 797)
                    circleX = 797;
                if (circleY < 0)
                    circleY = 0;
                if (circleY > 497)
                    circleY = 497;
                // Update the circle's position using JavaScript interop
                await jSRuntime.InvokeVoidAsync("updateCirclePosition", circleElement, circleX, circleY);
                await InvokeAsync(StateHasChanged);
            }
        }

        private void StopDragging()
        {
            isDragging = false;
            if (SelectedFunction == "cometome_topic")
            {
                if (selectedMasterCrane == "crane1" && circleY >= 200)
                {
                    PublishingDataArray = new string[]
                    {
                        "",
                        "defaulthmi",
                        "crane1",
                        "[\"\"]",
                        "trolley1",
                        "[\"\"]",
                        $"{(int)(circleX * 159060 / 800)}",
                        $"{(int)((-53200 * circleY / 300) + (53200 * 500 / 300))}",
                        "11400"
                    };
                    signalR.PublishToServer(selectedMasterCrane, "cometome_topic", JsonConvert.SerializeObject(PublishingDataArray));
                }
                else if (selectedMasterCrane == "crane2" && circleY <= 300)
                {
                    string[] PublishingDataArray = new string[]
                    {
                        "",
                        "defaulthmi",
                        "crane2",
                        "[\"\"]",
                        "trolley1",
                        "[\"\"]",
                        $"{(int)(circleX * 163000 / 800)}",
                        $"{(int)((-circleY * 53200 / 300) + 90808)}",
                        "0"
                    }; // 797px approx 800px
                    signalR.PublishToServer(selectedMasterCrane, "cometome_topic", JsonConvert.SerializeObject(PublishingDataArray));
                }
                else
                {
                    notificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Info, Detail = $"Out of the {selectedMasterCrane} region" });
                }
            }
        }

        private async Task Follow_Me_Function()
        {
            await Task.Run(() =>
            {
                timer = new Timer(_ =>
                {
                    Console.WriteLine($"{circleX},{circleY}");
                    if (selectedMasterCrane == "crane1" && circleY >= 200)
                    {
                        PublishingDataArray = new string[]
                        {
                            "",
                            "defaulthmi",
                            "crane1",
                            "[\"\"]",
                            "trolley1",
                            "[\"\"]",
                            $"{(int)(circleX * 159060 / 800)}",
                            $"{(int)((-53200 * circleY / 300) + (53200 * 500 / 300))}",
                            "11400"
                        };
                        signalR.PublishToServer(selectedMasterCrane, "followme_topic", JsonConvert.SerializeObject(PublishingDataArray));
                    }
                    else if (selectedMasterCrane == "crane2" && circleY <= 300)
                    {
                        string[] PublishingDataArray = new string[]
                        {
                            "",
                            "defaulthmi",
                            "crane2",
                            "[\"\"]",
                            "trolley1",
                            "[\"\"]",
                            $"{(int)(circleX * 163000 / 800)}",
                            $"{(int)((-circleY * 53200 / 300) + 90808)}",
                            "0"
                        };
                        signalR.PublishToServer(selectedMasterCrane, "followme_topic", JsonConvert.SerializeObject(PublishingDataArray));
                    }
                    else
                    {
                        //notificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Info, Detail = $"Out of the {selectedMasterCrane} region" });
                    }
                }, null, 0, 500);
            });
        }
        #endregion
    }
}