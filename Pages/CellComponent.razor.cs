using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Minesweeper.Models;

namespace Minesweeper.Pages
{
    public partial class CellComponent
    {
        [Parameter]
        public required Cell Cell { get; set; }

        [Parameter]
        public required Game Game { get; set; }

        [Parameter]
        public EventCallback Update { get; set; }

        public string HiddenClass { get; set; } = "hidden";
        public string RevealedClass { get; set; } = "revealed";

        public void Click(MouseEventArgs e)
        {
            if (e.Button == 0)
            {
                Reveal();
            }
            else if (e.Button == 2)
            {
                ToggleFlag();
            }
        }

        private void ToggleFlag()
        {
            Game.FlagCell(Cell);
            StateHasChanged();
            Update.InvokeAsync();
        }

        private void Reveal()
        {
            Game.RevealCell(Cell);
            StateHasChanged();
            Update.InvokeAsync();
        }
    }
}
