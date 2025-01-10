using Microsoft.AspNetCore.Components;
using Minesweeper.Models;

namespace Minesweeper.Pages
{
    public partial class MinesweeperGame
    {
        [Parameter]
        public required Game Game { get; set; }

        public async Task UpdateAsync()
        {
            StateHasChanged();
        }
    }
}
