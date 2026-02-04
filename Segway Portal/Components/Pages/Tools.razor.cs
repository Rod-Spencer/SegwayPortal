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

        [Inject]
        public SegwayContext? SegDB { get; set; }

        #endregion Injections
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Properties

        public String? ErrorMessage { get; set; }
        public String? WarningMessage { get; set; }

        #endregion Private Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Properties

        public List<PortalServiceTool>? PortalTools { get; set; }

        public PortalServiceTool? PortalTool { get; set; } = null;

        public PortalServiceToolData? PortalData { get; set; } = null;

        public List<PortalToolState>? PortalToolStates { get; set; } = null;

        public List<PortalToolInstallerType>? InstallerTypes { get; set; } = null;

        public List<PortalToolType>? PortalToolTypes { get; set; } = null;

        public Portal_Tool? PortalToolComponent { get; set; }

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

            if (SegDB == null)
            {
                ErrorMessage = "SegwayContext is not initialized.";
                return;
            }

            PortalTools = SegDB?.PortalServiceTools.OrderBy(t => t.DisplayName).ToList();
            if (PortalTools == null || PortalTools.Count == 0)
            {
                WarningMessage = "No PortalTools found.";
            }

            InstallerTypes = SegDB?.PortalToolInstallerTypes.OrderBy(x => x.Description).ToList();
            if (InstallerTypes == null || InstallerTypes.Count == 0)
            {
                if (String.IsNullOrEmpty(WarningMessage) == true) WarningMessage = "";
                WarningMessage += "No Portal Tool Installer Types found.";
            }

            PortalToolStates = SegDB?.PortalToolStates.OrderBy(x => x.Description).ToList();
            if (PortalToolStates == null || PortalToolStates.Count == 0)
            {
                if (String.IsNullOrEmpty(WarningMessage) == true) WarningMessage = "";
                WarningMessage += "No Portal Tool States found.";
            }

            PortalToolTypes = SegDB?.PortalToolTypes.OrderBy(x => x.Description).ToList();
            if (PortalToolTypes == null || PortalToolTypes.Count == 0)
            {
                if (String.IsNullOrEmpty(WarningMessage) == true) WarningMessage = "";
                WarningMessage += "No Portal Tool Types found.";
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

            //// Optionally update the list (simple upsert by reference equality or other key logic).
            //// Keep minimal: if the tool is not present, add it.
            //if (PortalTool != null)
            //{
            //    if (PortalTools!.Contains(PortalTool) == false)
            //    {
            //        PortalTools.Add(PortalTool);
            //    }
            //}

            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task HandleShowPortalToolPageChanged(Boolean saved)
        {
            if (saved == true)
            {
                if (PortalData is not null)
                {
                    if (PortalData.Id == Guid.Empty)
                    {
                        PortalData.Id = Guid.NewGuid();
                        PortalTool?.FileDataId = PortalData?.Id;
                        SegDB?.PortalServiceToolData.Add(PortalData);
                    }
                    else
                    {
                        SegDB?.PortalServiceToolData.Update(PortalData);
                    }

                    SegDB?.SaveChanges();
                    StateHasChanged();
                }

                if (PortalTool is not null)
                {
                    if (PortalTool.Id == Guid.Empty)
                    {
                        PortalTool.Id = Guid.NewGuid();
                        SegDB?.PortalServiceTools.Add(PortalTool);
                    }
                    else
                    {
                        SegDB?.PortalServiceTools.Update(PortalTool);
                    }
                    SegDB?.SaveChanges();
                    StateHasChanged();
                    OnInitialized();
                }
            }
            // The child component notifies visibility changes. React as needed.
            // Minimal implementation: trigger UI refresh.
            return Task.CompletedTask;
        }

        private Task HandlerPortalDataChanged(PortalServiceToolData? portalData)
        {
            if (portalData != null)
            {
                // Update the local model with the new data
                PortalData = portalData;
            }
            return Task.CompletedTask;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Methods


        private async Task ShowPopup()
        {
            PortalTool = new PortalServiceTool { Id = Guid.Empty };
            PortalData = new PortalServiceToolData { Id = Guid.Empty };
            await Task.Yield();
            PortalToolComponent?.ShowPopup();
            StateHasChanged();
        }

        private async Task AddToolBasedOn(Guid toolId)
        {
            var tool = PortalTools?.FirstOrDefault(x => x.Id == toolId);
            PortalTool = new PortalServiceTool
            {
                Id = Guid.Empty,
                DisplayName = null,
                FileName = null,
                Folder = tool?.Folder,
                InstallerTypeId = tool?.InstallerTypeId,
                ServiceToolStateId = tool?.ServiceToolStateId,
                ServiceToolTypeId = tool?.ServiceToolTypeId,
                FileDataId = Guid.NewGuid(),
                Version = tool?.Version
            };

            PortalData = new PortalServiceToolData { Id = Guid.Empty };

            await Task.Yield();
            if (PortalTool == null) return;
            PortalToolComponent?.ShowPopup();
            StateHasChanged();
        }


        private async Task EditTool(Guid toolId)
        {
            PortalTool = PortalTools?.FirstOrDefault(x => x.Id == toolId);
            await Task.Yield();
            PortalToolComponent?.ShowPopup();
            StateHasChanged();
        }

        private async Task DeleteTool(Guid toolId)
        {

            PortalTool = PortalTools?.FirstOrDefault(x => x.Id == toolId);
            await Task.Yield();
            if (PortalTool is not null)
            {
                var pData = SegDB?.PortalServiceToolData.FirstOrDefault(x => x.Id == PortalTool.FileDataId);
                if (pData != null)
                {
                    SegDB?.PortalServiceToolData.Remove(pData);
                }

                if (PortalTool.Id != Guid.Empty)
                {
                    SegDB?.PortalServiceTools.Remove(PortalTool);
                }
                SegDB?.SaveChanges();
                StateHasChanged();
                OnInitialized();
            }
        }

        private async Task DownloadTool(Guid toolId)
        {
            PortalTool = PortalTools?.FirstOrDefault(x => x.Id == toolId);
            await Task.Yield();
            PortalToolComponent?.ShowPopup();
            StateHasChanged();
        }


        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////
    }

}
