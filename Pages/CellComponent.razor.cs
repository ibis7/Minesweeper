using Microsoft.AspNetCore.Components;
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
        public EventCallback<Cell> RevealCell { get; set; }

        [Parameter]
        public EventCallback<Cell> FlagCell { get; set; }

        public void ToggleFlag()
        {
            FlagCell.InvokeAsync(Cell);
        }

        public void Reveal()
        {
            RevealCell.InvokeAsync(Cell);
        }
    }
}
