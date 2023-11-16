namespace IfakCrane.Client.Services
{
    public class NavToCraneUICommService
    {
        public string? SelectedCrane { get; set; }

        public event Action<string>? OnCraneSelected;
        public event Action<string>? OnFunctionSelected;

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
    }
}
