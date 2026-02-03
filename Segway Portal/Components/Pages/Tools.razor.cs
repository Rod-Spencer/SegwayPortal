using Microsoft.AspNetCore.Components;
using Segway.EF.SegwayCntxt;
using Segway_Portal.Components.Components;

namespace Segway_Portal.Components.Pages
{
    public partial class Tools : ComponentBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Constructors
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Parameters
        #endregion Parameters
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Injections
        #endregion Injections
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Properties

        public String? ErrorMessage { get; set; }
        public String? WarningMessage { get; set; }

        public SegwayContext? segDB { get; set; }

        // Component reference (avoids type collision with model)
        private Portal_Tool? PortalToolComponent;


        #endregion Private Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Properties

        public List<PortalServiceTool>? PortalTools { get; set; }

        public PortalServiceTool? PortalTool { get; set; } = null;

        public List<PortalToolState>? PortalToolStates { get; set; } = null;

        public List<PortalToolInstallerType>? InstallerTypes { get; set; } = null;

        public List<PortalToolType>? PortalToolTypes { get; set; } = null;

        #endregion Public Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Screen State Properties
        #endregion Screen State Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Override Methods
        #endregion Override Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Protected Methods

        protected override void OnInitialized()
        {
            ErrorMessage = null;
            WarningMessage = null;

            if (segDB == null)
            {
                segDB = new SegwayContext();
            }

            PortalTools = segDB?.PortalServiceTools.OrderBy(t => t.DisplayName).ToList();
            if (PortalTools == null || PortalTools.Count == 0)
            {
                WarningMessage = "No PortalTools found.";
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Methods

        private Task HandlePortalToolChanged(PortalServiceTool tool)
        {
            // Update local model with the value from the child component
            PortalTool = tool;

            // Optionally update the list (simple upsert by reference equality or other key logic).
            // Keep minimal: if the tool is not present, add it.
            if (tool != null)
            {
                if (!PortalTools.Contains(tool))
                {
                    PortalTools.Add(tool);
                }
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task HandleShowPortalToolPageChanged(bool show)
        {
            // The child component notifies visibility changes. React as needed.
            // Minimal implementation: trigger UI refresh.
            StateHasChanged();
            return Task.CompletedTask;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Methods

        private Task ShowPopup()
        {
            PortalTool = new PortalServiceTool() { Id = Guid.NewGuid() };
            PortalTool.ShowPopup();
            return Task.CompletedTask;
        }

        //private Task ShowPopup()
        //{
        //    // Prepare a new PortalServiceTool for the child editor/component.
        //    PortalTool = new PortalServiceTool();
        //    // If the child component exposes a public method to open itself, you could call it:
        //    // await PortalToolComponent?.OpenAsync();
        //    StateHasChanged();
        //    return Task.CompletedTask;
        //}

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////
    }

}
