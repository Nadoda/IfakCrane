namespace IfakCrane.Client.Services
{
    public class NavMenuService
    {
        public string? SelectedCrane { get; set; }

        public event Action<string>? OnCraneSelected;
        public event Action<string>? OnFunctionSelected;
        public event Action<string>? OnStopButtonPress;

        public async Task CraneSelection(string Crane) 
        {
           await Task.Run(() => 
           { OnCraneSelected?.Invoke(Crane); 
                SelectedCrane = Crane;
           });
        }
        public async Task FunctionSelection(string Selected_Function)
        {
            await Task.Run(() =>
            {
                OnFunctionSelected?.Invoke(Selected_Function);
            });
        }
        public async Task StopButtonClicked(string data)
        {
            await Task.Run(() =>
            {
                OnStopButtonPress?.Invoke(data);
            });
        }
    }
}
